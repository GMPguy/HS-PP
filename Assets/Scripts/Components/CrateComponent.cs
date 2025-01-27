using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class CrateComponent : MonoBehaviour, HitInterface {

    [SerializeField]
    float Health = 10f;

    // References
    [SerializeField]
    Collider MainCollider;

    [SerializeField]
    Rigidbody Rig;

    [SerializeField]
    GameObject[] Debris;

    [SerializeField]
    GameObject WoodHit;

    void Start () => ListHI();
    void OnDestroy () => UnlistHI();

    // Hit interface code implementation
    public void ListHI() => WorldSystem.HitInterfaces.Add(this);
    public void UnlistHI() => WorldSystem.HitInterfaces.Remove(this);
    public GameObject GetObject() => gameObject;

    public void Hit(float Damage, Vector3 position) {
        Transform newHit = GameObject.Instantiate(WoodHit).transform;
        newHit.position = position;
        newHit.Rotate(Vector3.one * Random.Range(0f, 360f));

        if ((Health -= Damage) <= 0f) {
            MainCollider.enabled = false;
            Destroy(Rig);

            // Turn into debris
            for (int d = 0; d < 6; d++) {
                Rigidbody debrisRig = Debris[d].AddComponent<Rigidbody>();
                debrisRig.AddForce(Vector3.one * Random.Range(-10f, 10f), ForceMode.VelocityChange);
                debrisRig.AddTorque(Vector3.one * Random.Range(-10f, 10f), ForceMode.VelocityChange);
                Debris[d].AddComponent<BoxCollider>();
            }

            UnlistHI();
        }
    }
}
