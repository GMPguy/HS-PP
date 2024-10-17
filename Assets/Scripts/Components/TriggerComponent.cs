using UnityEngine;

public class TriggerComponent : MonoBehaviour {
    
    public bool Grounded = false;

    public int Colliders = 0;

    void OnTriggerEnter () {
        Colliders++;
        Grounded = Colliders > 0;
    }

    void OnTriggerExit () {
        Colliders--;
        Grounded = Colliders > 0;
    }

}
