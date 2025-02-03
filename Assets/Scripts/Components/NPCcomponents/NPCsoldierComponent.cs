using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using static Enums;
using Random=UnityEngine.Random;

public class NPCsoldierComponent : NPCtemplate {

    // Main variables
    public float2 MoveSpeeds;
    public float2 LookSpeeds;
    public float3 DetectionSpeeds;
    public float AimSpeed;
    public Transform[] PatrolPoints;
    public string AnimationBank;
    public Vector3 GuardPoint;
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
    float scram;
    bool hasAimed;
    bool isReloading;
    float spotTime;

    public override void CustomUpdate(float delta) {

        if (CantMove <= 0f) {
            int momentum = (focus > 0f && CurrentThought == AIthink.Fight) ? 1 : 0;

            // Movement
            Vector3 parallerPosition = movePosition;
            movePosition.y = rig.position.y;

            if (Vector3.Distance(rig.position, parallerPosition) > .1f) {
                lookPosition = parallerPosition;
                rig.MovePosition(Vector3.MoveTowards(rig.position, parallerPosition, MoveSpeeds[momentum] * delta));
                movingPace = Mathf.Lerp(movingPace, .5f + (momentum / 2f), delta * 4f);
            } else
                movingPace = Mathf.Lerp(movingPace, 0f, delta * 4f);
            
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

        // Scram to avoid damage
        if (Health.x > 0f && Random.Range(0f, 1f) < .2f) {
            scram = 1f;
            targetPosition = transform.position + new Vector3 (
                Random.Range(-10f, 10f), 0f, Random.Range(-10f, 10f)
            );
            if (hasAimed) {
                hasAimed = false;
                Humanoid.PlayAnim(AnimationBank + "_AtEase");
            }
        }
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
        if (Drops[pickDrop] == null)
            return;
        
        Transform newDrop = Instantiate(Drops[pickDrop]).transform;
        newDrop.position = transform.position - Vector3.up / 2f;
    }

    public override void AIAlert (AiAlertType type, Vector3 targetPos = default, GameObject targetObj = null) {

        switch (type) {
            case AiAlertType.EnemySpotted: case AiAlertType.EnemySpotted_Aid:
                if (!targetObj || CurrentThought == AIthink.Fight)
                    return;

                focus = 10f;
                CurrentThought = AIthink.Fight;
                aiTarget = targetObj;

                // Scram if alerted by others
                if (type == AiAlertType.EnemySpotted_Aid) {
                    scram = Random.Range(1f, 3f);
                    targetPosition = transform.position;
                    return;
                }

                // Alert others if initial
                for (int ff = 0; ff < NPCSystem.NPCList.Count; ff++)
                    if (NPCSystem.NPCList[ff].SquadID == SquadID) {
                        NPCSystem.NPCList[ff].AIAlert(AiAlertType.EnemySpotted_Aid, targetPos, targetObj);
                    }
                break;
        }

    }

    public override void Think () {

        float delta = Time.timeSinceLevelLoad - prevThinkTime;
        prevThinkTime = Time.timeSinceLevelLoad;

        switch(CurrentThought) {
            case AIthink.Patrol:
                AI_Patrol(delta);
                AI_LookForFoe(delta);
                break;
            case AIthink.Fight:
                AI_Combat(delta);
                break;
            case AIthink.StareAtPlayer:
                if (PlayerSystem.Player)
                    lookPosition = PlayerSystem.Player.position;
                break;
            case AIthink.BreakDance:
                lookPosition = transform.position + transform.forward + transform.right;
                break;
        }
        
    }

    /// <summary>
    /// Behaviour, when npc patrols the area
    /// </summary>
    void AI_Patrol (float delta) {

        if (currentWaypoint == -1) {
            currentWaypoint = 0;
            movePosition = PatrolPoints[0].position;
            focus = 1f;
        }

        targetPosition = PatrolPoints[currentWaypoint].position;
        Vector3 parallerPos = targetPosition;
        parallerPos.y = transform.position.y;
        movePosition = PatrolPoints[currentWaypoint].position;

        if (Vector3.Distance(rig.position, parallerPos) < 1f) {
            movePosition = transform.position;
            if ((focus -= delta) <= 0f) {
                focus = Random.Range(1f, 10f);
                currentWaypoint = (currentWaypoint + 1) % PatrolPoints.Length;
            }
        }   

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

        // Scram first
        if ((scram -= delta) > 0f) {
            Vector3 scramPos = targetPosition;
            scramPos.y = transform.position.y;
            movePosition = scramPos;

            if (Vector3.Distance(transform.position, scramPos) < 1f) {
                targetPosition = transform.position + new Vector3(
                    Random.Range(-10f, 10f), 0f, Random.Range(-10f, 10f)
                );
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
                    float spread = MainGun.Accuracy.Evaluate(Random.Range(0f, 1f));
                    Vector3 spreadDir = (transform.right * Random.Range(-1f, 1f) + transform.up * Random.Range(-1f, 1f)) * spread;

                    WorldSystem.RaycastGunFire(
                        transform.position + Vector3.up, 
                        aiTarget.transform.position - transform.position + Vector3.up + spreadDir,
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
            }
        }

    }

    /// <summary>
    /// This function is used to detect enemies
    /// </summary>
    void AI_LookForFoe (float delta) {

        if (!IsFriendly) {
            // Currently, only search for main player
            if (PlayerSystem.Player == null) {
                spotTime = 0f;
                return;
            }
            
            if (CheckTarget(PlayerSystem.Player.gameObject, PlayerSystem.Player.position + Vector3.up * 1.5f, DetectionSpeeds.x)) {
                Vector3 checkPos = PlayerSystem.Player.position;
                checkPos.y = transform.position.y;
                if (Vector3.Angle(transform.forward, checkPos - transform.position) < DetectionSpeeds.y)
                    if ((spotTime += delta) > DetectionSpeeds.z)
                        AIAlert(AiAlertType.EnemySpotted, PlayerSystem.Player.position, PlayerSystem.Player.gameObject);
            } else
                spotTime = 0f;
        }

    }

    void Start () {
        ListHI();
        NPCSystem.NPCList.Add(this);
        rig = GetComponent<Rigidbody>();
        movePosition = lookPosition = transform.position;

        currentAmmo = MaxAmmo;
        lookPosition = GuardPoint;

        Humanoid.ChangeState(AnimationBank);
    }

    void OnDestroy () {
        UnlistHI();
    }
    
}
