using UnityEngine;
using static Enums;
using Unity.Mathematics;

public static class CameraSystem  {

    // Controlling variables
    static public float2 ZoomIn;
    static CameraLogic cameraType = CameraLogic.FPP;
    static public float turnX, turnY;

    // Camera references
    static Camera mainCamera;
    static Transform cameraTransform;

    // Main variables
    static Transform camTarget;
    static Vector3 camPosition;
    static Quaternion camRotation;

    public static void CustomLateUpdate (float delta) {

        if (mainCamera == null) {

            GameObject newCamera = Object.Instantiate(Resources.Load<GameObject>("Prefabs/MainCamera"));
            mainCamera = newCamera.GetComponent<Camera>();
            cameraTransform = newCamera.transform;

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
