using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CommentsMenuUI : UITemplate {

    [SerializeField]
    TMP_Text[] Comments;

    [SerializeField]
    RectTransform BaseComment;

    public override void ClearUp() { Cleared = false; }

    public override void EventTrigger(Enums.UIevent what, int[] bonus) {}

    public override void SetUp(int addition) {

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

    }
}
