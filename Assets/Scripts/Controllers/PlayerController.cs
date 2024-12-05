using UnityEngine;

public class PlayerController : MonoBehaviour {

    [SerializeField]
    bool recall = false;

    void Start () {
        if (recall)
            PlayerSystem.RecallPlayer(transform);
    }

}
