using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PuzzleComponent : MonoBehaviour {

    public VictoryController VictoryReference;
    bool Exploded = false;
    
    void OnCollisionEnter (Collision col) {
        if (Exploded)
            return;

        if (col.collider.GetComponent<GrenadeComponent>()) {
            Destroy(col.collider.gameObject);
            GetComponent<BoxCollider>().enabled = false;

            VictoryReference.Won = true;

            // Make debris
            for (int gc = 0; gc < transform.childCount; gc++) {
                Rigidbody rig = transform.GetChild(gc).AddComponent<Rigidbody>();
                rig.AddForce(Vector3.one * Random.Range(-10f, 10f), ForceMode.VelocityChange);
                rig.AddForce(Vector3.up * Random.Range(5f, 20f), ForceMode.VelocityChange);
                rig.AddTorque(Vector3.one * Random.Range(-10f, 10f), ForceMode.VelocityChange);
            }
        }
    }

}
