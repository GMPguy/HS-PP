using UnityEngine;

public class GrenadeComponent : MonoBehaviour {

    [SerializeField] Rigidbody rig;

    void Start () {
        rig.AddForce(transform.forward * 10f, ForceMode.VelocityChange);
        rig.AddTorque(Vector3.one * Random.Range(-10f, 10f));
        Destroy(gameObject, Random.Range(3f, 5f));
    }

    void OnDestroy () {
        WorldSystem.Explode(transform.position, 100f, 4f);
    }

}
