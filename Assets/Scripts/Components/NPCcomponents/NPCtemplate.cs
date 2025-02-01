using UnityEngine;
using Unity.Mathematics;
using static Enums;

public abstract class NPCtemplate : MonoBehaviour, HitInterface {

    // Main variables
    public AIstate CurrentState;
    public float2 Health;
    public bool IsFriendly;

    public float AttackSpeed, ReloadSpeed;
    public int MaxAmmo;

    // AI thinking
    public AIthink DefaultMode;
    public AIthink CurrentThought;
    public float CantMove;

    protected Vector3 movePosition, lookPosition, targetPosition;
    protected GameObject aiTarget;

    protected float focus, attackCooldown;
    protected int currentAmmo;

    public abstract void CustomUpdate (float delta);
    public abstract void Think ();
    public abstract void HitEffect (float damage, Vector3 position);
    public abstract void Dead ();
    public abstract void AIAlert (AiAlertType type, Vector3 targetPos = default, GameObject targetObj = null);

    // Hit interface code implementation
    public void ListHI() => WorldSystem.HitInterfaces.Add(this);
    public void UnlistHI() => WorldSystem.HitInterfaces.Remove(this);
    public GameObject GetObject() => gameObject;

    public void Hit(float damage, Vector3 position, GameObject killer = null) {
        if (CurrentState != AIstate.Alive)
            return;

        if ((Health.x -= damage) <= 0)
            Dead();

        HitEffect(damage, position);

        // Alert
        if (!killer)
            return;

        AIAlert(AiAlertType.EnemySpotted, killer.transform.position, killer);
    }

    /// <summary>
    /// A simplified function, that raycasts towards a target
    /// </summary>
    protected bool CheckTarget (GameObject target, Vector3 pos, float distance) {
        if (!target)
            return false;
        
        Vector3 view = transform.position + Vector3.up / 2f;

        Debug.DrawRay(view, (pos - view) * distance);

        if (Physics.Raycast(view, pos - view, out RaycastHit hit, distance) && hit.collider.gameObject == target)
            return true;
        else 
            return false;
    }
}
