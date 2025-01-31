using UnityEngine;
using Unity.Mathematics;
using static Enums;

public abstract class NPCtemplate : MonoBehaviour, HitInterface {

    // Main variables
    public AIstate CurrentState;
    public float2 Health;
    public bool IsFriendly;

    // AI thinking
    public AIthink DefaultMode;
    public AIthink CurrentThought;

    protected Vector3 movePosition, lookPosition, targetPosition;
    protected GameObject aiTarget;
    protected float focus;

    public abstract void CustomUpdate (float delta);
    public abstract void Think ();
    public abstract void HitEffect (float damage, Vector3 position);
    public abstract void Dead ();

    // Hit interface code implementation
    public void ListHI() => WorldSystem.HitInterfaces.Add(this);
    public void UnlistHI() => WorldSystem.HitInterfaces.Remove(this);
    public GameObject GetObject() => gameObject;

    public void Hit(float damage, Vector3 position) {
        if (CurrentState != AIstate.Alive)
            return;

        if ((Health.x -= damage) <= 0)
            Dead();

        HitEffect(damage, position);
    }
}
