using System;
using UnityEngine;
using static Enums;

public class PlayerController : MonoBehaviour {

    // Main variables
    PlayerState playerState = PlayerState.Alive;
    int prevState = -1;
    
    // References
    MovementComponent move;
    EquipmentComponent equipment;

    void Start () {

        // Set up references
        move = GetComponent<MovementComponent>();
        equipment = GetComponent<EquipmentComponent>();

    }

    void Update () {

        // If state was changed, trigger the state change function
        if (prevState != (int)playerState) 
            StateChange(playerState);

        // Trigger different functions, depending on the player's state
        switch (playerState) {

            case PlayerState.Alive:
                Movement();
                EquipmentControl();
                break;

            default:
                break;

        }

    }

    /// <summary>
    /// If player's state is changed, do some code
    /// </summary>
    void StateChange(PlayerState newState) {
        playerState = newState;
        prevState = (int)playerState;

        switch (newState) {
            case PlayerState.Alive:
                CameraSystem.ChangeCamera(CameraLogic.FPP, transform);
                break;
        }
    }

    /// <summary>
    /// This function uses Input system, to control the movement
    /// </summary>
    void Movement() {

        transform.rotation = Quaternion.Euler(0f, CameraSystem.turnY, 0f);

        switch (move.CurrentMovementType) {
            case MovementType.Normal:
                move.Slide( new Vector2(
                    Input.GetAxis("Vertical"),
                    Input.GetAxis("Horizontal")
                ));

                if (Input.GetButton("Jump"))
                    move.Jump();

                break;
        }

    }

    /// <summary>
    /// This function allows us to use weapons
    /// </summary>
    void EquipmentControl() {
        equipment.CustomUpdate();

        // Change item
        if (Input.GetKeyDown(KeyCode.Alpha1))
            equipment.ChangeItem(0, 0);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            equipment.ChangeItem(1, 0);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            equipment.ChangeItem(2, 0);
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            equipment.ChangeItem(3, 0);
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            equipment.ChangeItem(4, 0);
        else if (Input.mouseScrollDelta.y != 0f)
            equipment.ChangeItem((int)Input.mouseScrollDelta.y, 1);
        else if (Input.GetKeyDown(KeyCode.H))
            equipment.ChangeItem(0, -1);

        // Item functions
        equipment.Fire();
        equipment.AltFire();
    }

}
