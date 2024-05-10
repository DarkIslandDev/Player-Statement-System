using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFallRollingState : PlayerLandingState
{
    public PlayerFallRollingState(PlayerMovementStateMachine playerMovementStateMachine) : base(
        playerMovementStateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.ReusableData.MovementSpeedModifier = groundedData.RollData.SpeedModifier;

        base.Enter();

        StartAnimation(stateMachine.Player.AnimationData.FallRollingParameterHash);

        stateMachine.ReusableData.ShouldSprint = false;
    }

    public override void Exit()
    {
        base.Exit();

        StopAnimation(stateMachine.Player.AnimationData.FallRollingParameterHash);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (stateMachine.ReusableData.MovementInput != Vector2.zero)
        {
            return;
        }

        RotateTowardsTargetRotation();
    }

    public override void OnAnimationTransitionEvent()
    {
        if (stateMachine.ReusableData.MovementInput == Vector2.zero)
        {
            stateMachine.ChangeState(stateMachine.MediumStoppingState);

            return;
        }

        OnMove();
    }

    protected override void OnJumpStarted(InputAction.CallbackContext context)
    {
    }
}