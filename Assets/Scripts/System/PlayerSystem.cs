using UnityEngine;
using static Enums;

public static class PlayerSystem {

    // Main variables
    public static Transform Player;

    static PlayerState playerState = PlayerState.Alive;
    static int prevState = -1;

    // References
    public static MovementComponent move;
    public static EquipmentComponent equipment;

    /// <summary>
    /// Use this function to set Player references pased on scene object
    /// </summary>
    public static void RecallPlayer (Transform newPlayer) {

        if (Player)
            DisposePlayer();

        Player = newPlayer;

        move = Player.GetComponent<MovementComponent>();
        equipment = Player.GetComponent<EquipmentComponent>();

    }

    /// <summary>
    /// Use this function to get rid of referenced Player, and their references
    /// </summary>
    public static void DisposePlayer () {

        if (!Player)
            return;

        GameObject.Destroy(Player);
        move = null;
        equipment = null;

    }

    public static void CustomUpdate (float delta) {

        if (!Player)
            return;

        // If state was changed, trigger the state change function
        if (prevState != (int)playerState) 
            StateChange(playerState);

        // Trigger different functions, depending on the player's state
        switch (playerState) {

            case PlayerState.Alive:
                if (Time.timeScale > 0f) {
                    Movement(delta);
                    EquipmentControl();
                }
                break;

            default:
                break;

        }

    }

    /// <summary>
    /// If player's state is changed, do some code
    /// </summary>
    static void StateChange(PlayerState newState) {
        playerState = newState;
        prevState = (int)playerState;

        switch (newState) {
            case PlayerState.Alive:
                CameraSystem.ChangeCamera(CameraLogic.FPP, Player);
                break;
        }
    }

    /// <summary>
    /// This function uses Input system, to control the movement
    /// </summary>
    static void Movement(float delta) {

        Player.rotation = Quaternion.Euler(0f, CameraSystem.turnY, 0f);

        switch (move.CurrentMovementType) {
            case MovementType.Normal:
                move.Slide( new Vector2(
                    InputSystem.GetVertical(),
                    InputSystem.GetHorizontal()
                ), delta);

                if (InputSystem.GetJump())
                    move.Jump();

                break;
        }

    }

    /// <summary>
    /// This function allows us to use weapons
    /// </summary>
    static void EquipmentControl() {
        equipment.CustomUpdate();

        for (int pc = 0; pc < 5; pc++)
            if (InputSystem.GetItem(pc))
                equipment.ChangeItem(pc, 0);

        if (InputSystem.GetMouseScroll() > .25f)
            equipment.ChangeItem((int)InputSystem.GetMouseScroll(), 1);
        
        if (InputSystem.GetHolster())
            equipment.ChangeItem(0, -1);

        // Item functions
        equipment.Fire();
        equipment.AltFire();
    }

}
