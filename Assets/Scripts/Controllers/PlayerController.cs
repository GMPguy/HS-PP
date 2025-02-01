using UnityEngine;
using static Enums;

public class PlayerController : MonoBehaviour, HitInterface {

    [SerializeField]
    bool recall = false;

    [SerializeField]
    PlayerState DefaultState;

    void Start () {
        if (recall) {
            PlayerSystem.RecallPlayer(transform, DefaultState);
            ListHI();
        }
    }

    void OnDestroy () => UnlistHI();

    // Hit interface code implementation
    public void ListHI() => WorldSystem.HitInterfaces.Add(this);
    public void UnlistHI() => WorldSystem.HitInterfaces.Remove(this);
    public GameObject GetObject() => gameObject;
    public void Hit(float Damage, Vector3 position, GameObject killer = null) => PlayerSystem.DamagePlayer(Damage, position);

}
