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
        MainMenu,
        PausedMenu,
        AliveMenu,
        DeadMenu,
        LoadMenu
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
    }

    public enum AiAlertType {
        EnemySpotted
    }

    public enum AIstate {
        None,
        Alive,
        Dead
    }

}
