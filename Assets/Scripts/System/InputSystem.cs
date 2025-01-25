using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Mathematics;

public static class InputSystem {

    // References
    static InputActionAsset InputManager;

    public static void SetUp () {
        InputManager = Resources.Load<InputActionAsset> ("Configs/InputManager");
        InputManager.Enable();
    }

    // Read input actions
    public static float GetHorizontal () => FetchFloat(new (0, 0));
    public static float GetVertical () => FetchFloat(new (0, 1));
    public static bool GetJump () => WasPressed(new (0, 2));
    public static bool GetFire () => WasPressed(new (0, 3));
    public static bool HoldFire () => IsPressed(new (0, 3));
    public static bool GetAltFire () => WasPressed(new (0, 4));
    public static bool GetHolster () => WasPressed(new (0, 5));
    public static bool GetReload () => WasPressed(new (0, 6));
    public static bool GetGrenade () => WasPressed(new (0, 7));
    public static bool GetBandage () => WasPressed(new (0, 8));

    public static float GetMouseX () => FetchFloat(new (1, 0));
    public static float GetMosueY () => FetchFloat(new (1, 1));
    public static float GetMouseScroll () => FetchFloat(new (1, 2));

    public static bool GetItem (int itemID) => WasPressed(new (2, itemID));

    static float FetchFloat (int2 id) =>
        InputManager.actionMaps[id.x].actions[id.y].ReadValue<float>();
    
    static bool IsPressed (int2 id) =>
        InputManager.actionMaps[id.x].actions[id.y].IsPressed();

    static bool WasPressed (int2 id) =>
        InputManager.actionMaps[id.x].actions[id.y].WasPressedThisFrame();

}
