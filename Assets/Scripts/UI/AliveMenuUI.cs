using UnityEngine;
using UnityEngine.UI;
using static PlayerSystem;
using static Enums;
using static GameSystem;
using TMPro;

public class AliveMenuUI : UITemplate {

    // Health bar
    [SerializeField]
    Image healthBar;

    [SerializeField]
    GameObject damageBase;

    [SerializeField]
    RectTransform damageRoot;

    // Equipment
    [SerializeField]
    Image[] equipmentBG;

    [SerializeField]
    Image[] equipmentItems;

    float EQhide;
    bool acquireEQconfig;

    // Item held
    [SerializeField]
    Image itemHeldImage;

    [SerializeField]
    TMP_Text itemHeldText;

    // Crosshair
    [SerializeField]
    RectTransform[] Crosshairs;

    // Grenades and bandages
    [SerializeField]
    Image Grenades_Image;

    [SerializeField]
    TMP_Text Grenades_Text;

    [SerializeField]
    Image Bandages_Image;

    [SerializeField]
    TMP_Text Bandages_Text;

    int prevGrenades = -1, prevBandages = -1;

    public override void ClearUp() { Cleared = true; }

    public override void SetUp(int addition) { }

    public override void UIUpdate() {

        if (!Player)
            return;

        // Health bar
        healthBar.fillAmount = Health / 100f;

        UISystem.LockCursor(Time.deltaTime * 4f);

        // Clean up damage indicators
        for (int cu = damageRoot.childCount - 1; cu > 0; cu--) {
            Image quickImage = damageRoot.GetChild(cu).GetComponent<Image>();

            if (quickImage.color.a > 0)
                quickImage.color = new (1f, 1f, 1f, quickImage.color.a - (Time.deltaTime / 3f));
            else
                Destroy(quickImage.gameObject);
        }

        // Equipment
        EquipmentFunction();

        // Grenades and bandages
        if (prevGrenades != equipment.Grenades) {
            Grenades_Image.color = equipment.Grenades > 0 ? Color.white : Color.clear;
            Grenades_Text.text = equipment.Grenades > 0 ? "x " + equipment.Grenades.ToString() : "";
            prevGrenades = equipment.Grenades;
        }

        if (prevBandages != equipment.Bandages) {
            Bandages_Image.color = equipment.Bandages > 0 ? Color.white : Color.clear;
            Bandages_Text.text = equipment.Bandages > 0 ? "x " + equipment.Bandages.ToString() : "";
            prevBandages = equipment.Bandages;
        }

    }

    public void ItemSwitch (float factor) {
        EQhide = factor;
    }

    public void DamageIndicator (Vector3 pos) {
        RectTransform newIndicator = Instantiate (damageBase).GetComponent<RectTransform>();
        newIndicator.SetParent(damageRoot);
        newIndicator.localScale = Vector3.one;
        newIndicator.anchoredPosition = Vector2.zero;

        newIndicator.GetComponent<Image>().color = Color.white;
        float yAngle = -Vector3.SignedAngle(Player.forward, pos - Player.position, Vector3.up);
        newIndicator.eulerAngles = Vector3.forward * yAngle;
    }

    void EquipmentFunction () {

        // Set up icons
        if (!acquireEQconfig) {
            for (int se = 0; se < equipment.itemData.Length; se++)
                if (equipmentItems[se])
                    equipmentItems[se].sprite = equipment.itemData[se].ItemIcon;

            acquireEQconfig = true;
        }

        // Update icons
        if ((EQhide -= Time.deltaTime) > 0f) {
            equipmentBG[0].transform.parent.localScale = Vector3.one;

            for (int se = 0; se < 5; se++) {
                bool yes = false;

                if (se < equipment.Equipment.Length) {
                    EquipmentComponent.Item getItem = equipment.Equipment[se];
                    yes = getItem != null && getItem.Acquired;
                }

                if (equipment.CurrentItem == se) {
                    equipmentBG[se].color = new (1f, 1f, 1f, Mathf.Clamp01(EQhide));
                    equipmentItems[se].color = yes ? 
                        new Color(1f, 1f, 1f, Mathf.Clamp01(EQhide)) : new Color(0f,0f,0f,0f);
                } else {
                    equipmentBG[se].color = new (.2f, .2f, .2f, Mathf.Clamp01(EQhide));
                    equipmentItems[se].color = yes ? 
                        new Color(.2f, .2f, .2f, Mathf.Clamp01(EQhide)) : new Color(0f,0f,0f,0f);
                }
            }
        } else
            equipmentBG[0].transform.parent.localScale = Vector3.zero;

        // Item held
        EquipmentComponent.Item getItemA;
        ItemConfig getItemB = null;

        itemHeldImage.color = new Color(1f, 1f, 1f, 0f);
        itemHeldText.text = "";

        if (equipment.CurrentItem >= 0) {
            getItemA = equipment.Equipment[equipment.CurrentItem];
            getItemB = equipment.itemData[equipment.CurrentItem];

            if (getItemA != null) {
                itemHeldImage.sprite = getItemB.ItemIcon;
                itemHeldImage.color = new Color(1f, 1f, 1f, 1f);

                itemHeldText.text = GetString(getItemB.EnglishName, getItemB.PolishName);
                itemHeldText.text += "\n" + getItemB.TypeOfItem switch {
                    ItemType.Gun => getItemA.Ammo + " / " + getItemA.SpareAmmo,
                    _ => "",
                };
            }
        }

        // Crosshairs
        float accuracy = 0f;

        if (getItemB != null && getItemB.TypeOfItem == ItemType.Gun) {
            GunConfig cunConfig = (GunConfig)getItemB;
            accuracy = cunConfig.Accuracy.Evaluate( equipment.Recoil );
        }

        for (int ch = 0; ch < 4; ch++) {
            Crosshairs[ch].anchoredPosition = new Vector2(
                accuracy * (1 - ch / 2), 
                accuracy * (ch / 2)
            ) * (ch % 2 == 1 ? -10 : 10);
        }
    }

}
