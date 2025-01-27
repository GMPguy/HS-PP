using Unity.Mathematics;
using UnityEngine;
using static Enums;
using Random=UnityEngine.Random;

public static class PlayerSystem {

    // Main variables
    public static Transform Player;

    static PlayerState playerState = PlayerState.Alive;
    static int prevState = -1;

    public static float Health = 100f;

    // References
    public static MovementComponent move;
    public static EquipmentComponent equipment;
    public static SoundBankComponent sounds;

    /// <summary>
    /// Use this function to set Player references pased on scene object
    /// </summary>
    public static void RecallPlayer (Transform newPlayer, PlayerState defaultState) {

        if (Player)
            DisposePlayer();

        Player = newPlayer;

        move = Player.GetComponent<MovementComponent>();
        equipment = Player.GetComponent<EquipmentComponent>();

        StateChange(defaultState);
        Health = 100f;

    }

    /// <summary>
    /// Use this function to get rid of referenced Player, and their references
    /// </summary>
    public static void DisposePlayer () {

        if (!Player)
            return;

        Object.Destroy(Player);
        move = null;
        equipment = null;

    }

    /// <summary>
    /// This function reduces health of the player, and creates effects based of it
    /// </summary>
    public static void DamagePlayer (float2 damage, Vector3 damagePos) {
        
        Health -= Random.Range(damage[0], damage[1]);

        if (UISystem.TryGetAliveMenu(out AliveMenuUI ui))
            ui.DamageIndicator(damagePos);

        if (Health <= 0f)
            KillPlayer();

    }

    public static void KillPlayer () {

        if (playerState != PlayerState.Alive)
            return;
        
        StateChange(PlayerState.Dead);
        UISystem.ChangeMode(UImode.DeadMenu);

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
            case PlayerState.Dead:
                CameraSystem.ChangeCamera(CameraLogic.Dead, Player);
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

        // Grenades
        if (InputSystem.GetGrenade())
            equipment.GrenadeThrow();

        // Bandages
        if (InputSystem.GetBandage())
            equipment.BandageUse();
    }

}
