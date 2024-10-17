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
    CapsuleCollider collider;
    PhysicMaterial physMat;

    void Start () {
        rig = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();
        physMat = collider.material;
    }

    /// <summary>
    /// This function changes player's velocity on XZ axis
    /// </summary>
    public void Slide (Vector2 inputAxis) {

        physMat.bounciness = GroundDetector.Grounded ? 0f : 1f;

        inputAxis = Vector2.ClampMagnitude(inputAxis, 1f);
        
        Vector3 newVelocity =
            ((inputAxis.x * transform.forward) +
            (inputAxis.y * transform.right)
            ) * MaxSpeed;

        newVelocity.y = rig.velocity.y;

        rig.velocity = Vector3.Lerp(
            rig.velocity, newVelocity,
            GroundDetector.Grounded ? Time.deltaTime * 10f : Time.deltaTime
        );

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

            physMat.bounciness = 1f;
        }
    }

}
