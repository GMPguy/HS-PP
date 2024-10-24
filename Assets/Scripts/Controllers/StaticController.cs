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
        }

    }

    void Update () {
        PlayerSystem.CustomUpdate();
        UISystem.CustomUpdate();
    }

    void LateUpdate () =>
        CameraSystem.CustomLateUpdate();

}
