using System.Collections.Generic;
using UnityEngine;

public static class WorldSystem {

    // Main variables
    static bool setUp;

    // References
    public static List<HitInterface> HitInterfaces = new ();
    static ObjectsConfig WorldEffects;
    static GameObject explosion;

    public static void CustomUpdate () {

        if (!setUp) {

            WorldEffects = Resources.Load<ObjectsConfig> ("Configs/WorldEffects");
            explosion = Resources.Load<GameObject> ("Prefabs/Explosion");
            setUp = true;

        }

    }

    /// <summary>
    /// This function spawns visual effects that exist in the 3D world
    /// </summary>
    public static void WorldEffect (string effectName, Vector3 effectPosition, Vector3 effectRotation) {

        GameObject newEffect = GameObject.Instantiate( WorldEffects.Fetch(effectName) );
        newEffect.transform.position = effectPosition;
        newEffect.transform.forward = effectRotation;

    }

    /// <summary>
    /// For any kind of firearm attacks, use this function
    /// </summary>
    public static void RaycastGunFire (Vector3 org, Vector3 dir, GameObject killer, Transform slimend, GunConfig gun) {

        RaycastAttack(org, dir, Random.Range(gun.Damage.x, gun.Damage.y), Mathf.Infinity, killer);

        GameObject newFire = GameObject.Instantiate( gun.GunFire );
        newFire.transform.SetPositionAndRotation(slimend.position, slimend.rotation);

    }

    /// <summary>
    /// For any kind of raycast attacks, use this function
    /// </summary>
    public static void RaycastAttack (Vector3 org, Vector3 dir, float Damage, float distance, GameObject Killer) {
        
        if (Physics.Raycast(org, dir, out RaycastHit hit, distance))
            if (hit.collider.GetComponent<HitInterface>() != null && hit.collider.gameObject != Killer)
                // Hit something with hit interface
                hit.collider.GetComponent<HitInterface>().Hit(Damage, hit.point);
            else
                // Hit the dirt
                WorldEffect("GroundHit", hit.point, hit.normal);

    }

    /// <summary>
    /// This function damages anything nearby, and creates an explosion object
    /// </summary>
    public static void Explode (Vector3 org, float Damage, float Radius) {
        Transform boom = Object.Instantiate(explosion).transform;
        boom.position = org + Vector3.up * (Radius / 2f);
        boom.localScale = Vector3.one * Radius;

        for (int hc = 0; hc < HitInterfaces.Count; hc++) {
            Vector3 t = HitInterfaces[hc].GetObject().transform.position;
            if (Vector3.Distance(org, t) < Radius)
                HitInterfaces[hc].Hit(
                    Mathf.Max((1f - (Vector3.Distance(org, t) / Radius)) * Damage, 0f),
                    org
                );
        }
    }

}
