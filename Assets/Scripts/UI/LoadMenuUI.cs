using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadMenuUI : UITemplate {

    // References
    [SerializeField]
    TMP_Text LoadingTitle;

    [SerializeField]
    TMP_Text LoadingHint;

    public override void ClearUp() {Cleared = true;}

    public override void SetUp(int addition) {

        LoadingTitle.text = GameSystem.GetString("LODAING", "WCZYTYWANIE");

        LoadingHint.text = (int)Random.Range(0f, 3.9f) switch {
            3 => GameSystem.GetString(
                "Conserve your ammo, grenades, and bandages",
                "Oszczędzaj swoją amunicję, granaty, i bandaże"
            ),
            2 => GameSystem.GetString(
                "You can get items from destroying boxes",
                "Możesz otrzymywać przedmioty ze zniszczonych pudeł"
            ),
            1 => GameSystem.GetString(
                "Losing health will cause you to lose",
                "Utrata zdrowia skończy się porażką"
            ),
            _ => GameSystem.GetString(
                "Switching to pistol if faster than reloading",
                "Przełączanie na pistolet jest szybsze od przeładowania"
            )
        };

    }

    public override void UIUpdate() {}
}
