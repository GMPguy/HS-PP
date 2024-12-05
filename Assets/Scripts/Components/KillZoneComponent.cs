using Unity.Mathematics;
using UnityEngine;

public class DamageComponent : MonoBehaviour {
    [SerializeField]
    float2 damage;

    [SerializeField]
    bool continuous;

    void OnCollisionEnter (Collision col) {
        if (continuous)
            return;

        Damage(col);
    }

    void OnCollisionStay (Collision col) {
        if (!continuous)
            return;
            
        Damage(col);
    }

    void Damage (Collision col) {
        if (col.collider.CompareTag("Player"))
            PlayerSystem.DamagePlayer(damage, transform.position);
    }
}
