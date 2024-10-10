using UnityEngine;
using static Enums;

public class MovementComponent : MonoBehaviour {

    // Main variables
    [SerializeField]
    MovementType CurrentMovementType = MovementType.Normal;

    [SerializeField]
    int MaxSpeed = 10, JumpHeight = 10;

}
