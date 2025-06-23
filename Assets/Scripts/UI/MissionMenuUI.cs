using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameSystem;

public class MissionMenuUI : UITemplate {

    // References
    public TMP_Text LevelTitle, LevelDesc;

    public Image[] LevelImages;
    public Animator WindowAnimator;

    public TMP_Text BackButton, PlayButton;
    public RectTransform PlayButtonRect;

    // Variables
    public int SelectedLevel = -1;
    float TextShow = 0f;

    public override void ClearUp() {Cleared = true;}

    public override void SetUp(int addition) {

        BackButton.text = GetString("Back", "Wróć");
        PlayButton.text = GetString("Play", "Graj");

        PlayButtonRect.localScale = Vector3.zero;

    }

    public override void UIUpdate() {
        if (TextShow > 1f)
            return;
        
        TextShow += Time.deltaTime;
        LevelTitle.color = LevelDesc.color = new Color(1f, 1f, 1f, TextShow);

    }

    // Change zakladka
    public void LevelButton (int ButtonID) {
        // Change tab
        if (SelectedLevel == ButtonID)
            ButtonID = -1;

        if (ButtonID == -1) {
            if (SelectedLevel != 1)
                WindowAnimator.Play("ShowButtons");
            else
                WindowAnimator.Play("CingCiong");
        } else {
            if (SelectedLevel == -1)
                WindowAnimator.Play("HideButtons");
            else
                WindowAnimator.Play("CingCiong");
        }

        SelectedLevel = ButtonID;

        for (int i = 0; i < LevelImages.Length; i++)
            if (i == ButtonID)
                LevelImages[i].color = Color.yellow;
            else
                LevelImages[i].color = Color.white;
        
        LevelTitle.text = LevelDesc.text = "";

        PlayButtonRect.localScale = Vector3.zero;
    }

    // Update text from animation
    public void UpdateLevelText () {
        TextShow = 0f;
        LevelTitle.color = LevelDesc.color = Color.clear;

        switch (SelectedLevel) {
            case 0:
                LevelTitle.text = GetString("Level 1 - Invasion of Normandy", "Poziom 1 - Inwazja na Normandię");
                LevelDesc.text = GetString(
                    "It is June 1944, you are corporal Dick Tingeler, of the renowned 502nd airborne division. The airplane you were in was shot down, and now, you are the only survivor. You have hid in a ditch somewhere, to wait for a daybreak. At 9:00 you wake up, and arrive at some kind of a farm. Your objective, is to make it to Carentan, and regroup with some other squad that hasn’t been wiped out yet. You have accidentally lost all of your gear, so you have to salvage for it.",
                    "Jest czerwiec 1944, jesteś kapralem Dickem Tingelerem, ze sławnego 502 dywizjonu spadochroniarzy. Twój samolot wybuchnął, i teraz jesteś jedynym ocalałym. Chowałeś się w jakimś rowie, oczekując na świt. O 9:00 się budzisz, i pojawiasz się na jakiejś farmie. Twoim zadaniem, jest dotarcie do miasta Carentan, i dołączenie do jakiegoś oddziału, którego jeszcze Niemcy nie wytępili w pień. Przez przypadek zgubiłeś cały sprzęt, musisz znaleźć sobie jakąś broń."
                );

                PlayButtonRect.localScale = Vector3.one;
                break;
            default:
                LevelTitle.text = GetString("Not yet", "Jeszcze nie");
                LevelDesc.text = GetString("Finish previous levels to unlock this one.", "Przejdź poprzednie poziomy, żeby ten odblokować.");
                
                PlayButtonRect.localScale = Vector3.zero;
                break;
        }
    }

    public void BackButtonPress () =>
        UISystem.ChangeMode(Enums.UImode.MainMenu);

    public void PlayButtonPress () =>
        SceneManagmentSystem.LoadScene("MainScene");
}
