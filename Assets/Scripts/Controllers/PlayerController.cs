using UnityEngine;
using static Enums;

public class PlayerController : MonoBehaviour, HitInterface {

    [SerializeField]
    bool recall = false;

    [SerializeField]
    PlayerState DefaultState;

    public Camera MinimapCamera;

    void Start () {
        if (recall) {
            PlayerSystem.RecallPlayer(transform, this, DefaultState);
            ListHI();
        }
    }

    void OnDestroy () => UnlistHI();

    // Hit interface code implementation
    public void ListHI() => WorldSystem.HitInterfaces.Add(this);
    public void UnlistHI() => WorldSystem.HitInterfaces.Remove(this);
    public GameObject GetObject() => gameObject;
    public void Hit(float Damage, Vector3 position, GameObject killer = null) {
        Vector3 from = killer ? killer.transform.position : position;
        PlayerSystem.DamagePlayer(Damage, from);
    }

}
