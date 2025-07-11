public static class Enums {

    public enum PlayerState {
        Suspended,
        Alive,
        Dead
    }

    public enum MovementType {
        Disabled,
        Normal
    }

    public enum CameraLogic {
        Static,
        FPP,
        Dead
    }

    public enum ItemType {
        Regular,
        Gun,
        Knife
    }

    public enum GunFireMode {
        BoltAction,
        SemiAuto,
        Automatic
    }

    public enum UImode {
        None,
        MainMenu,
        PausedMenu,
        AliveMenu,
        DeadMenu,
        LoadMenu,
        MissionMenu
    }

    public enum UIevent {
        ItemSwitch,
        DamageFrom
    }

    public enum Language {
        English,
        Polski
    }

    public enum AIthink {
        None,
        Patrol,
        Fight,
        Hunt,
        StareAtPlayer,
        BreakDance
    }

    public enum AiAlertType {
        EnemySpotted,
        EnemySpotted_Aid
    }

    public enum AIstate {
        None,
        Alive,
        Dead
    }

}
