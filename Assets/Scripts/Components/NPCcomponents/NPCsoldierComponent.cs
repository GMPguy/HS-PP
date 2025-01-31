using Unity.Mathematics;
using UnityEngine;
using static Enums;
using Random=UnityEngine.Random;

public class NPCsoldierComponent : NPCtemplate {

    // Main variables
    public float2 MoveSpeeds;
    public float2 LookSpeeds;
    bool isMoving;

    // References
    public GameObject BloodSplat;
    public HumanoidComponent Humanoid;

    public override void CustomUpdate(float delta) {

        // Movement
        if (transform.position != movePosition) {
            lookPosition = movePosition;
        }

        // Looking
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            Quaternion.LookRotation(new Vector3(lookPosition.x, transform.position.y, lookPosition.z) - transform.position),
            LookSpeeds.x
        );

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
    }

    public override void Think () {
        
    }

    void Start () {
        ListHI();
        NPCSystem.NPCList.Add(this);
    }

    void OnDestroy () {
        UnlistHI();
    }
    
}
