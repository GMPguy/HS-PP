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

    // Side items
    public int Bandages;
    public int Grenades;

    // Main variables
    public Item[] Equipment;
    public int CurrentItem = -1;

    float pulloutCooldown;
    float cooldown;
    float2 delay;

    // Gun variables
    public float Recoil;
    float recoilPattern;
    bool isReloading;

    public ItemConfig[] itemData;
    [SerializeField] GameObject grenade;

    // References
    [SerializeField]
    SoundBankComponent EquipmentSounds;

    [SerializeField]
    AudioClip Audio_Reload;

    [SerializeField]
    AudioSource Audio_SwitchItem;

    public void CustomUpdate () {
        cooldown -= cooldown > 0 ? Time.deltaTime : 0;
        pulloutCooldown -= pulloutCooldown > 0 ? Time.deltaTime : 0;

        if (cooldown <= 0f)
            Recoil = Mathf.Max(Recoil - Time.deltaTime, 0f);

        delay.x = Mathf.MoveTowards(delay.x, delay.y, Time.deltaTime);

        // Check for current gun
        if (CurrentItem >= 0 && CurrentItem < Equipment.Length){
            Item item = Equipment[CurrentItem];
            ItemConfig config = itemData[CurrentItem];

            if (item.Acquired && config.TypeOfItem == ItemType.Gun)
                ReloadGun();
        }

    }

    /// <summary>
    /// This function checks if it is possible to throw grenade right now,
    /// and does so if it is
    /// </summary>
    public void GrenadeThrow () {
        if (pulloutCooldown > 0f)
            return;

        if (Grenades <= 0) {
            UISystem.AddComment(GameSystem.GetString("No grenades!", "Brak granatów!"), 3f, Color.red);
            return;
        }

        Grenades--;

        CameraSystem.FPPanimation("GrenadeThrow");
        pulloutCooldown = 1f;
    }

    /// <summary>
    /// This function checks if it is possible to use a bandage right now,
    /// and does so if it is
    /// </summary>
    public void BandageUse () {
        if (Bandages <= 0) {
            UISystem.AddComment(GameSystem.GetString("No bandages!", "Brak bandaży!"), 3f, Color.red);
            return;
        }

        if (PlayerSystem.Health >= 100f)
            return;

         Bandages--;
         PlayerSystem.Health = 100f;
         CameraSystem.FPPanimation("BandageUse");

         UISystem.FlashImage(Color.green, new (.5f, .5f));
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
        if (newID < 0 || newID >= Equipment.Length || !Equipment[newID].Acquired) {
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

        isReloading = false;

        CameraSystem.FPPanimation(itemData[newID].Animation_Pullout);
        CameraSystem.FPPmodelSet(itemData[newID].EnglishName);

        Audio_SwitchItem.clip = itemData[newID].Audio_Pullout;
        Audio_SwitchItem.pitch = Random.Range(.8f, 1f);
        Audio_SwitchItem.Play();

        if (UISystem.TryGetAliveMenu(out AliveMenuUI ui))
            ui.ItemSwitch(3f);

    }

    void Start () {
        Equipment = new Item[itemData.Length];

        // Copy data from item configs into Item array
        for (int gi = 0; gi < itemData.Length; gi++) {
            ItemConfig config = itemData[gi];

            Equipment[gi] = new Item {
                Acquired = false,
                ConfigRef = config,
                Ammo = config.MaxAmmo,
                SpareAmmo = config.MaxAmmo * 3
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
        ((int)config.FireMode <= 1 && InputSystem.GetFire()) || 
        ((int)config.FireMode > 1 && InputSystem.HoldFire());

        if (firePermission && item.Ammo > 0) {
            cooldown = config.Cooldown;
            Recoil = Mathf.Clamp01(Recoil + config.RecoilTime);

            item.Ammo--;

            float yRecoil = Mathf.Lerp (
                config.RecoilPatterns[(int)recoilPattern].Evaluate(Recoil),
                config.RecoilPatterns[(int)(recoilPattern + 1) % config.RecoilPatterns.Length].Evaluate(Recoil),
                recoilPattern % 1f
            );

            CameraSystem.FPPanimation(config.Animation_Fire);
            CameraSystem.ShiftCamera(new float3(
                1.5f,
                -config.Recoil * Recoil,
                -config.Recoil * yRecoil
            ));

            recoilPattern = (recoilPattern + config.RecoilTime) % config.RecoilPatterns.Length;

            float2 spread = (new float2(
                Random.Range(-1f, 1f), 
                Random.Range(-1f, 1f)
            ) * config.Accuracy.Evaluate(Recoil)) / 90f;

            WorldSystem.RaycastGunFire(
                CameraSystem.MainCamera.transform.position, 
                CameraSystem.MainCamera.transform.forward
                    + CameraSystem.MainCamera.transform.right * spread.x
                    + CameraSystem.MainCamera.transform.up * spread.y,
                gameObject,
                CameraSystem.ItemSlimend,
                config
            );
        }
    }

    /// <summary>
    /// Use this when using a knife
    /// </summary>
    void Fire_Knife () {
        if (delay.x >= .5f) {
            delay = float2.zero;

            WorldSystem.RaycastAttack(
                CameraSystem.MainCamera.transform.position, 
                CameraSystem.MainCamera.transform.forward,
                10f,
                2f,
                gameObject
            );
        }

        if (cooldown > 0f)
            return;
        
        if (InputSystem.GetFire()) {
            int chooseAnimation  = (int)Random.Range(1f, 3.9f);
            CameraSystem.FPPanimation("Knife_Attack" + chooseAnimation);
            cooldown = 1f;
            delay = new float2(0f, .5f);
        }
    }

    /// <summary>
    /// When gun is held, this function checks if it can be reloaded
    /// </summary>
    void ReloadGun () {

        GunConfig currentGun = (GunConfig) itemData[CurrentItem];

        if (!isReloading) {
            // The gun is not being reloaded - check if it is possible to do so
            if (InputSystem.GetReload() && Equipment[CurrentItem].SpareAmmo > 0 && Equipment[CurrentItem].Ammo < itemData[CurrentItem].MaxAmmo) {
                isReloading = true;

                delay = new float2(0f, currentGun.ReloadTime);
                cooldown = currentGun.ReloadTime;

                CameraSystem.FPPanimation(currentGun.Animation_Reload);
                EquipmentSounds.PlayAudio(Audio_Reload, .5f);
            }
        } else {
            // The gun is being reloaded - check if it's finished
            if (delay.x >= delay.y) {
                delay = float2.zero;
                int ammoGet = Mathf.Min(itemData[CurrentItem].MaxAmmo - Equipment[CurrentItem].Ammo, Equipment[CurrentItem].SpareAmmo);

                Equipment[CurrentItem].Ammo += ammoGet;
                Equipment[CurrentItem].SpareAmmo -= ammoGet;

                isReloading = false;
            }
        }

    }

}
