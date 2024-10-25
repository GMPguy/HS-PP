using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : UITemplate {

    // References
    [SerializeField]
    RectTransform MenuButtons;

    public override void ClearUp() {Cleared = true;}

    public override void SetUp(int addition) {

        

    }

    public override void UIUpdate() {}

    public override void EventTrigger(Enums.UIevent what, int bonus) {}
    
}
