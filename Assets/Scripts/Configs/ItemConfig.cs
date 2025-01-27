using UnityEngine;
using static Enums;

[CreateAssetMenu(fileName = "New Item", menuName = "Configs/Item")]
public class ItemConfig : ScriptableObject {

    public Sprite ItemIcon;

    public string EnglishName, PolishName;
    public ItemType TypeOfItem = ItemType.Regular;

    public int MaxAmmo;
    public float Cooldown;
    public float PulloutTime;

    public string Animation_Pullout;
    public AudioClip Audio_Pullout;

}
