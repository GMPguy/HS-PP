using System.Collections.Generic;
using UnityEngine;

public class TriggerComponent : MonoBehaviour {
    
    public bool Grounded = false;

    public List<Collider> Colliders;

    void OnTriggerEnter (Collider col) {
        Colliders.Add(col);
        CheckGround();
    }

    void OnTriggerExit (Collider col) {
        Colliders.Remove(col);
        CheckGround();
    }

    void CheckGround () {
        for (int c = Colliders.Count - 1; c >= 0; c--)
            if (Colliders[c] == null)
                Colliders.RemoveAt(c);

        Grounded = Colliders.Count > 0;
    }

}
