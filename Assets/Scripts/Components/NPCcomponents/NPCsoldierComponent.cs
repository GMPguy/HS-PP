using Unity.Mathematics;
using UnityEngine;
using static Enums;
using Random=UnityEngine.Random;

public class NPCsoldierComponent : NPCtemplate {

    // Main variables
    public float2 MoveSpeeds;
    public float2 LookSpeeds;
    public float AimSpeed;
    public Transform[] PatrolPoints;
    public string AnimationBank;
    int currentWaypoint;
    float movingPace;

    // References
    public GameObject BloodSplat;
    public HumanoidComponent Humanoid;
    public GunConfig MainGun;
    public float AttackDistance;
    public GameObject[] Drops;
    Rigidbody rig;

    // Some ai stuff
    float prevThinkTime;
    bool hasAimed;
    bool isReloading;

    public override void CustomUpdate(float delta) {

        if (CantMove <= 0f) {
            int momentum = focus > 0f && CurrentThought == AIthink.Fight ? 1 : 0;

            // Movement
            Vector3 parallerPosition = movePosition;
            movePosition.y = rig.position.y;

            if (Vector3.Distance(rig.position, parallerPosition) > .1f) {
                lookPosition = parallerPosition;
                rig.MovePosition(Vector3.MoveTowards(rig.position, parallerPosition, MoveSpeeds[momentum] * delta));
                movingPace = Mathf.Lerp(movingPace, 1f + momentum, delta);
            } else
                movingPace = Mathf.Lerp(movingPace, 0f, delta);
            
            Humanoid.ChangePace(movingPace);

            // Looking
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                Quaternion.LookRotation(new Vector3(lookPosition.x, transform.position.y, lookPosition.z) - transform.position),
                LookSpeeds[momentum] * delta
            );
        } else
            CantMove -= delta;

    }

    public override void HitEffect(float damage, Vector3 position) {
        Transform splash = GameObject.Instantiate(BloodSplat).transform;
        splash.position = position;
        splash.Rotate(Vector3.one * Random.Range(0f, 360f));
    }

    public override void Dead() {
        CurrentState = AIstate.Dead;
        Humanoid.PlayAnim("Dead");

        CapsuleCollider col = GetComponent<CapsuleCollider>();
        col.height = col.radius = .1f;
        col.center = Vector3.down * .8f;

        NPCSystem.NPCList.Remove(this);

        // Drop
        int pickDrop = (int)Random.Range(0f, Drops.Length - .1f);
        Transform newDrop = Instantiate(Drops[pickDrop]).transform;
        newDrop.position = transform.position - Vector3.up / 2f;
    }

    public override void AIAlert (AiAlertType type, Vector3 targetPos = default, GameObject targetObj = null) {

        switch (type) {
            case AiAlertType.EnemySpotted:
                if (!targetObj)
                    return;

                focus = 10f;
                CurrentThought = AIthink.Fight;
                aiTarget = targetObj;
                break;
        }

    }

    public override void Think () {

        float delta = Time.timeSinceLevelLoad - prevThinkTime;
        prevThinkTime = Time.timeSinceLevelLoad;

        switch(CurrentThought) {
            case AIthink.Patrol:
                AI_Patrol(delta);
                break;
            case AIthink.Fight:
                AI_Combat(delta);
                break;
        }
        
    }

    /// <summary>
    /// Behaviour, when npc patrols the area
    /// </summary>
    void AI_Patrol (float delta) {

        if (Vector3.Distance(rig.position, movePosition) < .2f) {
            if ((focus -= delta) <= 0f) {
                focus = Random.Range(1f, 5f);
                currentWaypoint = (currentWaypoint + 1) % PatrolPoints.Length;
            }
        } else
            movePosition = PatrolPoints[currentWaypoint].position;

    }

    /// <summary>
    /// Behaviour, when npc gets alerted
    /// </summary>
    void AI_Combat (float delta) {

        // No target - rest
        if (!aiTarget)
            focus = 0f;

        // Reload gun first
        if (currentAmmo <= 0 && attackCooldown <= 0f) {
            attackCooldown = ReloadSpeed;
            CantMove = ReloadSpeed;
            Humanoid.PlayAnim(AnimationBank + "_Reload");
            isReloading = true;
            return;
        } else if (isReloading) {
            if ((attackCooldown -= delta) <= 0f) {
                isReloading = false;
                currentAmmo = MaxAmmo;
            }
            return;
        }

        float closeUpDist = hasAimed ? AttackDistance * 1.25f : AttackDistance;

        if (CheckTarget(aiTarget, aiTarget.transform.position + Vector3.up * 1.5f, closeUpDist)) {
            // Enemy spotted
            targetPosition = aiTarget.transform.position + (aiTarget.transform.position - transform.position).normalized;
            movePosition = transform.position;
            lookPosition = targetPosition;
            focus = 5f;

            // Take aim
            Vector3 checkPos = aiTarget.transform.position;
            checkPos.y = transform.position.y;

            if (!hasAimed) {
                hasAimed = true;
                attackCooldown = AimSpeed;
                Humanoid.PlayAnim(AnimationBank + "_TakeAim");
            }

            // Fire
            if (Vector3.Angle(transform.forward, checkPos - transform.position) < 10f) {

                if ((attackCooldown -= delta) <= 0f && currentAmmo > 0) {
                    WorldSystem.RaycastGunFire(
                        transform.position + Vector3.up, 
                        aiTarget.transform.position - transform.position + Vector3.up, 
                        gameObject, 
                        Humanoid.HandRoot, 
                        MainGun
                    );
                    currentAmmo--;
                    attackCooldown = AttackSpeed;
                    Humanoid.PlayAnim(AnimationBank + "_Shoot");
                }
            }
        } else if (CheckTarget(aiTarget, aiTarget.transform.position + Vector3.up * 1.5f, 1000f)) {
            // Enemy spotted, but not close enough
            targetPosition = aiTarget.transform.position + (aiTarget.transform.position - transform.position).normalized;
            movePosition = targetPosition;
            lookPosition = targetPosition;

            if (hasAimed) {
                hasAimed = false;
                Humanoid.PlayAnim(AnimationBank + "_AtEase");
                Debug.Log("Lost the target");
            }
        } else {
            // Enemy lost, find them
            movePosition = targetPosition;

            if ((focus -= delta) <= 0f)
                CurrentThought = DefaultMode;
            
            if (hasAimed) {
                hasAimed = false;
                Humanoid.PlayAnim(AnimationBank + "_AtEase");
                Debug.Log("Lost the target");
            }
        }

    }

    void Start () {
        ListHI();
        NPCSystem.NPCList.Add(this);
        rig = GetComponent<Rigidbody>();
        movePosition = lookPosition = transform.position;
    }

    void OnDestroy () {
        UnlistHI();
    }
    
}
