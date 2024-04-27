using Unity.VisualScripting;
using UnityEngine;

public class PlayerIdlingState : PlayerGroundedState
{
    public PlayerIdlingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
        
    }

    #region IState Methods

    public override void Enter()
    {
        base.Enter();

        stateMachine.ReusableData.MovementSpeedModifier = 0;
        
        ResetVelocity();
    }

    public override void Update()
    {
        base.Update();

        if (movementInput == Vector2.zero)
        {
            return;
        }

        OnMove();
    }

    #endregion
}