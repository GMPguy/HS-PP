using Unity.Mathematics;
using UnityEngine;
using Random=UnityEngine.Random;

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
        if (col.collider.TryGetComponent<HitInterface>(out HitInterface hit))
            hit.Hit(Random.Range(damage.x, damage.y), transform.position);
            //PlayerSystem.DamagePlayer(damage, transform.position);
    }
}
