using Unity.Mathematics;
using UnityEngine;
using static Enums;
using Random = UnityEngine.Random;

public class EquipmentComponent : MonoBehaviour {

    /// <summary>
    /// This class contains ItemConfig data, that needs a local copy,
    /// or needs to be called more easily, for the equipment, f.e.: ammo
    /// </summary>
    public class Item {
        public ItemConfig ConfigRef;
        public bool Acquired;
        public int Ammo, SpareAmmo;
    }

    // Main variables
    public Item[] Equipment;
    public int CurrentItem = -1;

    float pulloutCooldown;
    float cooldown;
    float2 delay;

    // Gun variables
    public float Recoil;
    float recoilPattern;

    public ItemConfig[] itemData;

    public void CustomUpdate () {
        cooldown -= cooldown > 0 ? Time.deltaTime : 0;
        pulloutCooldown -= pulloutCooldown > 0 ? Time.deltaTime : 0;

        if (cooldown <= 0f)
            Recoil = Mathf.Max(Recoil - Time.deltaTime, 0f);

        delay.x = Mathf.MoveTowards(delay.x, delay.y, Time.deltaTime);
    }

    /// <summary>
    /// Use this function to check which item is held, and use it's main function
    /// </summary>
    public void Fire () {
        if (pulloutCooldown > 0f || CurrentItem < 0)
            return;

        Item item = Equipment[CurrentItem];

        switch (item.ConfigRef.TypeOfItem) {
            case ItemType.Gun:
                Fire_Gun(item);
                break;
            case ItemType.Knife:
                Fire_Knife();
                break;
            default:
                Debug.LogError("No fire code for TypeOfItem " + item.ConfigRef.TypeOfItem);
                break;
        }
    }

    /// <summary>
    /// Use this function to check which item is held, and use it's main function
    /// </summary>
    public void AltFire () {
        if (pulloutCooldown > 0f || CurrentItem < 0)
            return;
    }

    /// <summary>
    /// Use this function to change item
    /// </summary>
    /// <param name="what">0 sets the ID, 1 adds to current ID, and uses modulo</param>
    public void ChangeItem (int itemID, int what) {
        if (cooldown > 0f)
            return;

        // Holster weapon
        if (what == -1) {
            pulloutCooldown = cooldown = 0f;
            CurrentItem = -1;
            CameraSystem.FPPanimation("Template");
            CameraSystem.FPPmodelSet("");
            return;
        }

        // Pull out weapon
        int newID = what switch {
            1 => CurrentItem + itemID,
            _ => itemID
        };

        newID = (Equipment.Length + newID) % Equipment.Length;

        // If item is not acquired, prevent player from changing to it
        if (!Equipment[newID].Acquired) {
            if (what == 0)
                return;
            else {
                for (int cfa = newID + 1; cfa != newID; cfa = (Equipment.Length + cfa + 1) % Equipment.Length)
                    if (Equipment[cfa].Acquired) {
                        newID = cfa;
                        break;
                    }
            }
        }

        // Don't pull out the current weapon
        if (newID == CurrentItem)
            return;
        
        CurrentItem = newID;
        pulloutCooldown = itemData[newID].PulloutTime;

        cooldown = Recoil = recoilPattern = 0f;
        delay = float2.zero;

        CameraSystem.FPPanimation(itemData[newID].Animation_Pullout);
        CameraSystem.FPPmodelSet(itemData[newID].EnglishName);

        UISystem.UIEventCall(UIevent.ItemSwitch, 3);
    }

    void Start () {
        Equipment = new Item[itemData.Length];

        // Copy data from item configs into Item array
        for (int gi = 0; gi < itemData.Length; gi++) {
            ItemConfig config = itemData[gi];

            Equipment[gi] = new Item {
                Acquired = true,
                ConfigRef = config,
                Ammo = config.MaxAmmo
            };
        }

        ChangeItem(0, -1);
    }

    /// <summary>
    /// This function is used for items, that are guns. You can shoot with them
    /// </summary>
    void Fire_Gun(Item item) {
        if (cooldown > 0f)
            return;

        GunConfig config = (GunConfig)item.ConfigRef;

        bool firePermission = 
        ((int)config.FireMode <= 1 && Input.GetButtonDown("Fire")) || 
        ((int)config.FireMode > 1 && Input.GetButton("Fire"));

        if (firePermission && item.Ammo > 0) {
            cooldown = config.Cooldown;
            Debug.Log("Bang! " + Recoil);
            Recoil = Mathf.Clamp01(Recoil + config.RecoilTime);

            item.Ammo--;

            float yRecoil = Mathf.Lerp (
                config.RecoilPatterns[(int)recoilPattern].Evaluate(Recoil),
                config.RecoilPatterns[(int)(recoilPattern + 1f) % 1].Evaluate(Recoil),
                recoilPattern % 1f
            );

            CameraSystem.FPPanimation(config.Animation_Fire);
            CameraSystem.ShiftCamera(new float3(
                1.5f,
                -config.Recoil * Recoil,
                -config.Recoil * yRecoil
            ));

            recoilPattern = (recoilPattern + config.RecoilTime) % config.RecoilPatterns.Length;
        }
    }

    /// <summary>
    /// Use this when using a knife
    /// </summary>
    void Fire_Knife () {
        if (delay.x >= .5f) {
            Debug.Log("Chop");
            delay = float2.zero;
        }

        if (cooldown > 0f)
            return;
        
        if (Input.GetButtonDown("Fire")) {
            int chooseAnimation  = (int)Random.Range(1f, 3.9f);
            CameraSystem.FPPanimation("Knife_Attack" + chooseAnimation);
            cooldown = 1f;
            delay = new float2(0f, .5f);
        }
    }

}
