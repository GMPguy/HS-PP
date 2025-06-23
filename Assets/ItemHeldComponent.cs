using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHeldComponent : MonoBehaviour {
    public GameObject Grenade;

    public void Pinout () {
        Transform newNade = Instantiate(Grenade).transform;
        newNade.position = CameraSystem.CameraTransform.position + CameraSystem.CameraTransform.forward / 2f;
        newNade.forward = CameraSystem.CameraTransform.forward;
    }
}
