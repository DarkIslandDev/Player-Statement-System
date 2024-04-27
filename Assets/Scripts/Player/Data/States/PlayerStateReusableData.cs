using UnityEngine;

public class PlayerStateReusableData
{
    private Vector3 currentTargetRotation;
    private Vector3 timeToReachTargetRotation;
    private Vector3 dampedTargetRotationCurrentVelocity;
    private Vector3 dampedTargetRotationPassedTime;
    
    public Vector2 MovementInput { get; set; }
    public float MovementSpeedModifier { get; set; } = 1;
    
    public bool ShouldWalk { get; set; }

    public ref Vector3 CurrentTargetRotation => ref currentTargetRotation;
    public ref Vector3 TimeToReachTargetRotation => ref timeToReachTargetRotation;
    public ref Vector3 DampedTargetRotationCurrentVelocity => ref dampedTargetRotationCurrentVelocity;
    public ref Vector3 DampedTargetRotationPassedTime => ref dampedTargetRotationPassedTime;
}