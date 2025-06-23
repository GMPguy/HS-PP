using UnityEngine;
using static Enums;
using Unity.Mathematics;

public static class CameraSystem  {

    // Controlling variables
    static public float2 ZoomIn;
    static CameraLogic cameraType = CameraLogic.FPP;
    static public float turnX, turnY;

    // Camera switching variables
    static float cameraSwitch;
    static Vector3 prevCameraPosition;
    static Quaternion prevCameraRotation;

    // Camera references
    public static Camera MainCamera;
    public static Transform CameraTransform;

    public static Transform ItemSlimend;
    static Transform itemHeldModel;

    public static ItemHeldComponent ItemHeld;
    static Animator itemHeldAnim;

    static AudioSource itemheldSounds;
    static SoundBankConfig itemHeldSoundBank;

    // Main variables
    static Transform camTarget;
    static Vector3 camPosition;
    static Quaternion camRotation;

    // Shift and shake values
    static float4 camShake = new float4(0f, 1f, 1f, 1f);
    static float4 camShift = new float4(0f, 1f, 1f, 1f);

    public static void CustomLateUpdate () {

        if (MainCamera != null) {

            // Shifts and shakes
            camShift.x = Mathf.MoveTowards(camShift.x, 0f, Time.deltaTime);
            camShake.x = Mathf.MoveTowards(camShake.x, 0f, Time.deltaTime);

            // Triger camera logic functions
            switch (cameraType) {
                case CameraLogic.FPP:
                    FPPcamera(Time.deltaTime);
                    break;
                case CameraLogic.Dead:
                    CameraDead(Time.deltaTime);
                    break;
                default:
                    StaticCamera();
                    break;
            }

            // Reset arm animations
            if (itemHeldAnim.GetCurrentAnimatorStateInfo(0).IsName("RestartAnimation")){
                if (PlayerSystem.equipment.CurrentItem >= 0) {
                    FPPanimation(PlayerSystem.equipment.itemData[PlayerSystem.equipment.CurrentItem].Animation_Pullout);
                    FPPmodelSet(PlayerSystem.equipment.itemData[PlayerSystem.equipment.CurrentItem].EnglishName);
                } else {
                    FPPanimation("Template");
                    FPPmodelSet("");
                }
            }


        }

    }

    /// <summary>
    /// This function is triggered by scene's camera - it set's up the references
    /// </summary>
    public static void RecallCamera (GameObject newCamera) {

        MainCamera = newCamera.GetComponent<Camera>();
        CameraTransform = newCamera.transform;

        // Spawn arm model
        GameObject newItem = Object.Instantiate(Resources.Load<GameObject>("Prefabs/ItemHeldModel"));

        itemHeldAnim = newItem.GetComponent<Animator>();
        ItemHeld = newItem.GetComponent<ItemHeldComponent>();

        itemheldSounds = newItem.GetComponent<AudioSource>();
        itemHeldModel = newItem.transform;
        itemHeldModel.SetParent(CameraTransform);
        itemHeldModel.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        itemHeldSoundBank = Resources.Load<SoundBankConfig>("Configs/ItemHeldSounds");

    }

    /// <summary>
    /// use this function to change camera types, or, to set a new target
    /// </summary>
    public static void ChangeCamera (CameraLogic newType, Transform newTarget) {

        cameraType = newType;
        camTarget = newTarget;

        cameraSwitch = 0f;

        prevCameraPosition = CameraTransform.position;
        prevCameraRotation = CameraTransform.rotation;

        // RemoveItemHeldModel
        itemHeldModel.localScale = newType == CameraLogic.FPP ? Vector3.one : Vector3.zero;

    }

    /// <summary>
    /// If FPP camera is in use, this function will play ItemHeldModel's animation and sound
    /// </summary>
    public static void FPPanimation (string animationName, string animationSound = default) {

        itemHeldAnim.Play(animationName, 0, 0f);

        if (animationSound != default) {
            itemheldSounds.clip = itemHeldSoundBank.GetSound(animationSound);
            itemheldSounds.Play();
        }
    }

    /// <summary>
    /// If player is alive, and they change their item, we change the ItemHeldModel item model
    /// </summary>
    public static void FPPmodelSet (string modelName) {

        // Find desired model, and set it's activity to either true or false
        for (int fm = 0; fm < itemHeldModel.GetChild(0).childCount; fm++) {
            GameObject checkModel = itemHeldModel.GetChild(0).GetChild(fm).gameObject;
            checkModel.SetActive(checkModel.name == modelName);

            if (checkModel.transform.childCount > 0)
                ItemSlimend = checkModel.transform.GetChild(0);
            else
                ItemSlimend = checkModel.transform;
        }
    }

    /// <summary>
    /// This function sets new shift values. Camera shift, changes camera rotation by a few degrees,
    /// and returns to basic forward direction overtime
    /// </summary>
    public static void ShiftCamera (float3 newShift) => camShift = new float4 (
        newShift.x,
        newShift.x,
        newShift.y,
        newShift.z
    ); 

    /// <summary>
    /// This function sets new shake values. Camera shake, shakes camera's position overtime
    /// </summary>
    public static void ShakeCamera (float3 newShake) => camShake = new float4 (
        newShake.x,
        newShake.x,
        newShake.y,
        newShake.z
    ); 

    /// <summary>
    /// This function is used, when player is active
    /// </summary>
    static void FPPcamera (float delta) {

        // Look around
        turnY += InputSystem.GetMouseX() * delta * SettingsSystem.CameraSensitivity;
        turnX = Mathf.Clamp(
            turnX + InputSystem.GetMosueY() * delta * SettingsSystem.InvertedAxisY * SettingsSystem.CameraSensitivity,
             -80f, 80f 
        );

        // Field of view
        MainCamera.fieldOfView = Mathf.Lerp(
            SettingsSystem.FOV,
            ZoomIn.x,
            ZoomIn.y
        );

        // Shake camera
        Vector3 shake = new Vector3(
            Mathf.Sin(Time.timeSinceLevelLoad * camShake.w),
            Mathf.Sin(Time.timeSinceLevelLoad * camShake.w * 2f),
            Mathf.Sin(Time.timeSinceLevelLoad * camShake.w / 2f)
        ) * camShake.z * (camShake.x / camShake.y);

        // Set transforms
        CameraTransform.SetPositionAndRotation(
            camTarget.position + (Vector3.up * 1.7f) + shake, 
            Quaternion.Euler(
                turnX + camShift.z * (camShift.x / camShift.y), 
                turnY + camShift.w * (camShift.x / camShift.y),
                0f
            )
        );
    }

    /// <summary>
    /// This function is used, when player died
    /// </summary>
    static void CameraDead (float delta) {
        if (!camTarget)
            return;

        cameraSwitch += delta;

        if (cameraSwitch < 1f)
            CameraTransform.position = Vector3.Lerp(
                prevCameraPosition,
                camTarget.position + (Vector3.up / 5f),
                Mathf.Pow(cameraSwitch, 2f)
            );
        else if (cameraSwitch < 2f)
            CameraTransform.rotation = Quaternion.Lerp(
                Quaternion.Euler(prevCameraRotation.eulerAngles + Vector3.forward * 60f),
                prevCameraRotation,
                1f - Mathf.Pow(cameraSwitch - 1f, 2f)
            );
    }

    /// <summary>
    /// This function is used, when camera has nothing to work with
    /// </summary>
    static void StaticCamera () =>
        CameraTransform.SetPositionAndRotation(camPosition, camRotation);

}
