using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeadMenuUI : UITemplate {

    // References
    [SerializeField]
    TMP_Text GameOverText;

    [SerializeField]
    Image GameOverBG;

    [SerializeField]
    Button restartLevel;

    [SerializeField]
    TMP_Text restartLevel_Text;

    [SerializeField]
    Button quitToMenu;

    [SerializeField]
    TMP_Text quitToMenu_Text;

    // Variables
    [SerializeField]
    string[] gameOverString;

    float Progress;

    public override void ClearUp() {}

    public override void EventTrigger(Enums.UIevent what, int[] bonus) {}

    public override void SetUp(int addition) {
        GameOverText.text = GameSystem.GetString(
            "crpl. Dick Tingeler\n1922 - 1944",
            "krpl. Dick Tingeler\n1922 - 1944"
        );

        restartLevel.enabled = false;
        restartLevel.transform.localScale = Vector3.zero;
        restartLevel_Text.text = GameSystem.GetString("Try again", "Spróbuj ponownie");

        quitToMenu.enabled = false;
        quitToMenu.transform.localScale = Vector3.zero; 
        quitToMenu_Text.text = GameSystem.GetString("Quit to menu", "Wyjdź do menu");
    }

    public override void UIUpdate() {

        if (Progress >= 3f) {
            if (restartLevel.enabled == false) {
                restartLevel.enabled = true;
                restartLevel.transform.localScale = Vector3.one;

                quitToMenu.enabled = true;
                quitToMenu.transform.localScale = Vector3.one;
            }

            return;
        }

        Progress += Time.unscaledDeltaTime;

        GameOverBG.color = new (0f, 0f, 0f, Mathf.Clamp01(Progress / 2f));
        GameOverText.color = new (1f, 1f, 1f, Mathf.Clamp01(Progress - 2f));

    }
}
