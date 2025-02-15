using UnityEngine;
using static Enums;

public class MovementComponent : MonoBehaviour {

    // Main variables
    public MovementType CurrentMovementType = MovementType.Normal;

    [SerializeField]
    int MaxSpeed = 10, JumpHeight = 10;

    // Misc
    [SerializeField]
    TriggerComponent GroundDetector;

    Rigidbody rig;
    CapsuleCollider col;

    [SerializeField]
    LayerMask predictIgnore;

    // Sounds
    [SerializeField]
    SoundBankComponent PlayerSounds;

    [SerializeField]
    AudioClip Audio_Jump;

    [SerializeField]
    AudioSource Audio_Footstep;

    [SerializeField]
    AudioClip[] Audio_Footsteps;

    // Footsteps
    Vector3 prevPos;
    int currentFootstep;

    void Start () {
        rig = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
    }

    /// <summary>
    /// This function changes player's velocity on XZ axis
    /// </summary>
    public void Slide (Vector2 inputAxis, float delta) {

        inputAxis = Vector2.ClampMagnitude(inputAxis, 1f);

        float predictObby = Physics.CheckSphere(rig.position + Vector3.up + (rig.velocity * delta), .4f, predictIgnore) ? 0f : 1f;
        
        Vector3 newVelocity =
            ((inputAxis.x * transform.forward) +
            (inputAxis.y * transform.right)
            ) * MaxSpeed * predictObby;

        newVelocity.y = rig.velocity.y;

        rig.velocity = Vector3.Lerp(
            rig.velocity, newVelocity,
            GroundDetector.Grounded ? Time.deltaTime * 10f : Time.deltaTime
        );

        // Footsteps
        if (GroundDetector.Grounded && Vector3.Distance(prevPos, transform.position) > 1f && !Audio_Footstep.isPlaying) {
            prevPos = transform.position;
            Audio_Footstep.clip = Audio_Footsteps[currentFootstep];
            Audio_Footstep.Play();
            currentFootstep = (currentFootstep + 1) % Audio_Footsteps.Length;
        }
    }

    /// <summary>
    /// This function changes Y of player's velocity to that of JumpHeight, if they're grounded
    /// </summary>
    public void Jump () {
        if (GroundDetector.Grounded) {
            GroundDetector.Grounded = false;
            transform.position += Vector3.up * .25f;

            Vector3 prevVelocity = rig.velocity;
            prevVelocity.y = JumpHeight; 
            rig.velocity = prevVelocity;

            PlayerSounds.PlayAudio(Audio_Jump);
        }
    }

}
