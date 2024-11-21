using UnityEngine;

/// <summary>
/// Since static classes can't use MonoBehaviour functions, this object will allow
/// us to controll static classes at runtime
/// </summary>
public class StaticController : MonoBehaviour {

    void Start() {
        
        if (FindObjectsOfType<StaticController>().Length > 1)
            Destroy(gameObject);
        else {
            DontDestroyOnLoad(gameObject);
            CameraSystem.CustomLateUpdate();
            InputSystem.SetUp();
        }

    }

    void Update () {
        WorldSystem.CustomUpdate();
        PlayerSystem.CustomUpdate(Time.deltaTime);
        UISystem.CustomUpdate();
    }

    void LateUpdate () =>
        CameraSystem.CustomLateUpdate();

}
