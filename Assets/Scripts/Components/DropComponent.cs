using UnityEngine;
using static PlayerSystem;

public class DropComponent : MonoBehaviour {
    
    // Main variables
    [SerializeField] DropType TypeOfDrop;
    [SerializeField] ItemConfig item;
    [SerializeField] GameObject model;
    [SerializeField] float modelScale = 1f;
    [SerializeField] int ammo;

    // Set up a few things
    void Start () {
        Transform dropModel = GameObject.Instantiate(model).transform;
        dropModel.SetParent(transform);
        dropModel.localScale = Vector3.one * modelScale;
        dropModel.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    // Acquire drop
    void OnTriggerEnter (Collider col) {
        if (col.CompareTag("Player")) {

            switch (TypeOfDrop) {
                case DropType.Item:

                    for (int fi = 0; fi < equipment.Equipment.Length; fi++) {
                        if (equipment.Equipment[fi].ConfigRef == item) {
                            if (equipment.Equipment[fi].Acquired) {
                                equipment.Equipment[fi].SpareAmmo += ammo;

                                UISystem.AddComment(
                                    GameSystem.GetString(item.EnglishName + " ammo +" + ammo, item.PolishName + " ammunicja +" + ammo),
                                    5f,
                                    Color.white
                                );
                            } else {
                                equipment.Equipment[fi].Ammo = ammo;
                                equipment.Equipment[fi].SpareAmmo = ammo * 2;
                                equipment.Equipment[fi].Acquired = true;
                                equipment.ChangeItem(fi, 0);

                                UISystem.AddComment(
                                    GameSystem.GetString("Picked up " + item.EnglishName, "Podniesiono " + item.PolishName),
                                    5f,
                                    Color.white
                                );
                            }
                        }
                    }

                    break;
                case DropType.Grenades:
                    equipment.Grenades += ammo;

                    UISystem.AddComment(
                        GameSystem.GetString("Grenades +" + ammo, "Granaty +" + ammo),
                        5f,
                        Color.white
                    );
                    break;
                case DropType.Bandages:
                    equipment.Bandages += ammo;

                    UISystem.AddComment(
                        GameSystem.GetString("Bandages +" + ammo, "Bandaże +" + ammo),
                        5f,
                        Color.white
                    );
                    break;
                default:
                    Debug.LogError("No drop code for type: " + TypeOfDrop);
                    break;
            }

            Destroy(gameObject);
        }
    }

    enum DropType {
        Item,
        Ammo,
        Grenades,
        Bandages,
        Health
    }

}
