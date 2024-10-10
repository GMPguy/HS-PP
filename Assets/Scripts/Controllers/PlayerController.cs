using UnityEngine;
using static Enums;

public class PlayerController : MonoBehaviour {

    // Main variables
    PlayerState playerState = PlayerState.Alive;
    int prevState = -1;
    
    // References
    MovementComponent move;

    void Start () {

        // Set up references
        move = GetComponent<MovementComponent>();

    }

    void Update () {

        // If state was changed, trigger the state change function
        if (prevState != (int)playerState) 
            StateChange();

        // Trigger different functions, depending on the player's state
        switch (playerState) {

            case PlayerState.Alive:
                break;

            default:
                break;

        }

    }

    void StateChange() {

    }

}
