using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraComponent : MonoBehaviour {
    void Start() => CameraSystem.RecallCamera(gameObject);
}
