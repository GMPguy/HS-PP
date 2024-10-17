using UnityEngine;
using static Enums;

public class EquipmentComponent : MonoBehaviour {

    /// <summary>
    /// This class contains ItemConfig data, that needs a local copy,
    /// , or needs to be called more easily, for the equipment, f.e.: ammo
    /// </summary>
    public class Item {
        public ItemConfig ConfigRef;
        public bool Acquired;
        public int Ammo;
        public float PulloutTime;
    }

    // Main variables
    public Item[] Equipment;
    public int CurrentItem;

    public float pulloutCooldown;
    public float cooldown;

    // Gun variables
    float recoil;

    [SerializeField]
    ItemConfig[] itemData;

    public void CustomUpdate () {
        cooldown -= cooldown > 0 ? Time.deltaTime : 0;
        pulloutCooldown -= pulloutCooldown > 0 ? Time.deltaTime : 0;

        recoil -= cooldown > 0 ? Time.deltaTime : 0;
    }

    /// <summary>
    /// Use this function to check which item is held, and use it's main function
    /// </summary>
    public void Fire () {
        if (cooldown > 0f || pulloutCooldown > 0f)
            return;

        Item item = Equipment[CurrentItem];

        switch (item.ConfigRef.TypeOfItem) {
            case ItemType.Gun:
                GunConfig config = (GunConfig)item.ConfigRef;
                cooldown = config.Cooldown;
                Debug.Log("Bang! " + recoil);
                recoil = Mathf.Clamp01(recoil + config.RecoilTime);
                break;
            default:
                Debug.Log("Dunno what's this, but it's called " + item.ConfigRef.EnglishName);
                cooldown = item.ConfigRef.Cooldown;
                break;
        }
    }

    /// <summary>
    /// Use this function to check which item is held, and use it's main function
    /// </summary>
    public void AltFire () {
        if (cooldown > 0f || pulloutCooldown > 0f)
            return;
    }

    /// <summary>
    /// Use this function to change item
    /// </summary>
    /// <param name="what">0 sets the ID, 1 adds to current ID, and uses modulo</param>
    public void ChangeItem (int itemID, int what) {
        if (cooldown > 0f)
            return;

        int newID = what switch {
            1 => CurrentItem + itemID,
            _ => itemID
        };

        newID = (Equipment.Length + newID) % Equipment.Length;
        
        CurrentItem = newID;
        pulloutCooldown = Equipment[newID].PulloutTime;
        cooldown = 0f;
    }

    void Awake () {
        Equipment = new Item[itemData.Length];

        // Copy data from item configs into Item array
        for (int gi = 0; gi < itemData.Length; gi++) {
            ItemConfig config = itemData[gi];

            Equipment[gi] = new Item {
                ConfigRef = config,
                Ammo = config.MaxAmmo,
                PulloutTime = config.PulloutTime
            };
        }

    }

}
