using UnityEngine;

/// <summary>
/// Since static classes can't use MonoBehaviour functions, this object will allow
/// us to controll static classes at runtime
/// </summary>
public class StaticController : MonoBehaviour {

    void Start() {
        
        if (FindObjectOfType<StaticController>())
            Destroy(gameObject);
        else
            DontDestroyOnLoad(gameObject);

    }

    void CustomLateUpdate () =>
        CameraSystem.CustomLateUpdate(Time.deltaTime);

}
