using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public class CommentsMenuUI : UITemplate {

    [SerializeField]
    TMP_Text[] Comments;

    [SerializeField]
    Image FlashImage;

    [SerializeField]
    RectTransform BaseComment;

    float2 flashTime;
    int flashImportance;

    public override void ClearUp() { Cleared = false; }

    public override void SetUp(int addition) {

        flashTime = new (2f, 1f);

        // Copy comments
        for (int c = 1; c < Comments.Length; c++) {
            RectTransform newComment = GameObject.Instantiate(BaseComment);
            newComment.SetParent(BaseComment.parent);
            newComment.localScale = Vector3.one;
            newComment.anchoredPosition = new (0f, c * -48f);
            Comments[c] = newComment.GetComponent<TMP_Text>();
        }

    }

    public override void UIUpdate() {

        // Display textes
        for (int c = Comments.Length - 1; c >= 0; c--) {
            if (c < UISystem.Comments.Count) {
                Comments[c].text = UISystem.Comments[c].Text;
                Comments[c].color = UISystem.Comments[c].TextColor;

                if ((UISystem.Comments[c].TextTime -= Time.unscaledDeltaTime) <= 0f)
                    UISystem.Comments.RemoveAt(c);
            } else
                Comments[c].text = "";
        }

        // Display flash
        if ((flashTime.x -= Time.deltaTime) < 0) {
            flashImportance = 0;
            FlashImage.color = Color.clear;
        } else {
            flashTime.x = Mathf.Max(flashTime.x - Time.deltaTime, 0f);

            Color flashColor = FlashImage.color;
            flashColor.a = flashTime.x / flashTime.y;
            FlashImage.color = flashColor;
        }

        Debug.Log("AAAA");

    }

    /// <summary>
    /// This function flashesh a blank image
    /// </summary>
    public void FlashTheImage (Color color, float2 time, int importance = 0) {
        if (importance < flashImportance)
            return;

        FlashImage.color = color;
        flashTime = new (time.x, time.y);
        flashImportance = importance;
    }
}
