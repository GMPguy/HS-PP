using UnityEngine;
using static Enums;

public static class CameraSystem  {

    // Controlling variables
    static CameraLogic cameraType = CameraLogic.FPP;
    static float TurnX, TurnY;

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
        TurnY += Input.GetAxis("Mouse X");
        TurnX = Mathf.Clamp(TurnX + Input.GetAxis("Mouse Y"), -80f, 80f );

        // Set transforms
        cameraTransform.SetPositionAndRotation(
            camTarget.position + Vector3.up * 1.75f, 
            Quaternion.Euler(TurnX, TurnY, 0f)
        );
    }

    /// <summary>
    /// This function is used, when camera has nothing to work with
    /// </summary>
    static void StaticCamera () =>
        cameraTransform.SetPositionAndRotation(camPosition, camRotation);

}
