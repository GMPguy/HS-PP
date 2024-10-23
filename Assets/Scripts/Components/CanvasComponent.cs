using UnityEngine;

public class CanvasComponent : MonoBehaviour {

    [SerializeField]
    Enums.UImode defaultMode;

    void Start() => UISystem.RecallCanvas(transform, defaultMode);

}
