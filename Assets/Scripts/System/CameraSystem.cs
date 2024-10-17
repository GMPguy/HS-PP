using UnityEngine;
using static Enums;
using Unity.Mathematics;
using UnityEngine.Assertions;

public static class CameraSystem  {

    // Controlling variables
    static public float2 ZoomIn;
    static CameraLogic cameraType = CameraLogic.FPP;
    static public float turnX, turnY;

    // Camera references
    static Camera mainCamera;
    static Transform cameraTransform;

    static Transform itemHeldModel;
    static Animator itemHeldAnim;
    static AudioSource itemheldSounds;
    static SoundBankConfig itemHeldSoundBank;

    // Main variables
    static Transform camTarget;
    static Vector3 camPosition;
    static Quaternion camRotation;

    public static void CustomLateUpdate (float delta) {

        if (mainCamera == null) {

            // Spawn camera
            GameObject newCamera = Object.Instantiate(Resources.Load<GameObject>("Prefabs/MainCamera"));
            mainCamera = newCamera.GetComponent<Camera>();
            cameraTransform = newCamera.transform;

            // Spawn arm model
            GameObject newItem = Object.Instantiate(Resources.Load<GameObject>("Prefabs/ItemHeldModel"));
            itemHeldAnim = newItem.GetComponent<Animator>();
            itemheldSounds = newItem.GetComponent<AudioSource>();
            itemHeldModel = newItem.transform;
            itemHeldModel.SetParent(cameraTransform);
            itemHeldModel.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            itemHeldSoundBank = Resources.Load<SoundBankConfig>("Configs/ItemHeldSounds");

        } else {

            switch (cameraType) {
                case CameraLogic.FPP:
                    FPPcamera(delta);
                    break;
                default:
                    StaticCamera();
                    break;
            }

        }

    }

    /// <summary>
    /// use this function to change camera types, or, to set a new target
    /// </summary>
    public static void ChangeCamera (CameraLogic newType, Transform newTarget) {

        cameraType = newType;
        camTarget = newTarget;

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
        //Assert.IsNull(itemHeldModel, "Tried to set model to a non-existing ItemHeldModel");

        // Find desired model, and set it's activity to either true or false
        for (int fm = 0; fm < itemHeldModel.GetChild(0).childCount; fm++) {
            GameObject checkModel = itemHeldModel.GetChild(0).GetChild(fm).gameObject;
            checkModel.SetActive(checkModel.name == modelName);
        }
    }

    /// <summary>
    /// This function is used, when player is active
    /// </summary>
    static void FPPcamera (float delta) {

        if (!camTarget)
            return;

        // Look around
        turnY += Input.GetAxis("Mouse X") * SettingsSystem.CameraSensitivity;
        turnX = Mathf.Clamp(
            turnX + Input.GetAxis("Mouse Y") * SettingsSystem.InvertedAxisY * SettingsSystem.CameraSensitivity,
             -80f, 80f 
        );

        // Field of view
        mainCamera.fieldOfView = Mathf.Lerp(
            SettingsSystem.FOV,
            ZoomIn.x,
            ZoomIn.y
        );

        // Set transforms
        cameraTransform.SetPositionAndRotation(
            camTarget.position + Vector3.up * 1.75f, 
            Quaternion.Euler(turnX, turnY, 0f)
        );
    }

    /// <summary>
    /// This function is used, when camera has nothing to work with
    /// </summary>
    static void StaticCamera () =>
        cameraTransform.SetPositionAndRotation(camPosition, camRotation);

}
