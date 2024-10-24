using System.Collections.Generic;
using UnityEngine;

public static class WorldSystem {

    // Main variables
    static List<ExpirationComponent> expires;
    static bool setUp;

    // References
    static ObjectsConfig WorldEffects;

    public static void CustomUpdate () {

        if (!setUp) {

            WorldEffects = Resources.Load<ObjectsConfig> ("Configs/WorldEffects");
            expires = new ();
            setUp = true;

        } else {

            // Clean objects with expiration component
            if (expires.Count > 0) {
                for (int k = 0; k < expires.Count; k++) {
                    ExpirationComponent clean = expires[k];

                    if (clean.Expire(Time.deltaTime)){
                        GameObject begone = clean.gameObject;
                        expires.RemoveAt(k);
                        GameObject.Destroy(begone);

                        k--;
                    }
                }
                
                expires.TrimExcess();
            }

        }

    }

    /// <summary>
    /// This function spawns visual effects that exist in the 3D world
    /// </summary>
    public static void WorldEffect (string effectName, Vector3 effectPosition, Vector3 effectRotation) {

        GameObject newEffect = GameObject.Instantiate( WorldEffects.Fetch(effectName) );
        newEffect.transform.position = effectPosition;
        newEffect.transform.forward = effectRotation;
        expires.Add(newEffect.GetComponent<ExpirationComponent>());

    }

    /// <summary>
    /// For any kind of firearm attacks, use this function
    /// </summary>
    public static void RaycastGunFire (Vector3 org, Vector3 dir, GameObject killer, Transform slimend, GunConfig gun) {

        RaycastAttack(org, dir, Random.Range(gun.Damage.x, gun.Damage.y), Mathf.Infinity, killer);

        GameObject newFire = GameObject.Instantiate( gun.GunFire );
        newFire.transform.SetPositionAndRotation(slimend.position, slimend.rotation);
        expires.Add(newFire.GetComponent<ExpirationComponent>());

    }

    /// <summary>
    /// For any kind of raycast attacks, use this function
    /// </summary>
    public static void RaycastAttack (Vector3 org, Vector3 dir, float Damage, float distance, GameObject Killer) {
        
        if (Physics.Raycast(org, dir, out RaycastHit hit, distance))
            if (hit.collider.GetComponent<HitInterface>() != null && hit.collider.gameObject != Killer)
                // Hit something with hit interface
                hit.collider.GetComponent<HitInterface>().Hit();
            else
                // Hit the dirt
                WorldEffect("GroundHit", hit.point, hit.normal);

    }

}
