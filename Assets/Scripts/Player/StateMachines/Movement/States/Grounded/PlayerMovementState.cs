﻿using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementState : IState
{
    protected PlayerMovementStateMachine stateMachine;
    protected Vector2 movementInput;

    protected PlayerGroundedData movementData;

    public PlayerMovementState(PlayerMovementStateMachine playerMovementStateMachine)
    {
        stateMachine = playerMovementStateMachine;

        movementData = stateMachine.Player.Data.GroundedData;
        
        InitializeData();
    }

    private void InitializeData()
    {
        stateMachine.ReusableData.TimeToReachTargetRotation = movementData.BaseRotationData.TargetRotationReachTime;
    }

    #region IState Methods

    public virtual void Enter()
    {
        AddInputActionsCallbacks();
    }


    public virtual void Exit()
    {
        RemoveInputActionsCallbacks();
    }

    public virtual void HandleInput()
    {
        ReadMovementInput();
    }

    public virtual void Update()
    {
    }

    public virtual void PhysicsUpdate()
    {
        Move();
    }

    #endregion

    #region Main Methods

    private void ReadMovementInput()
    {
        movementInput = stateMachine.Player.Input.PlayerActions.Movement.ReadValue<Vector2>();
    }

    private void Move()
    {
        if (movementInput == Vector2.zero || stateMachine.ReusableData.MovementSpeedModifier == 0)
        {
            return;
        }

        Vector3 movementDirection = GetMovementInputDirection();

        float targetRotationYAngle = Rotate(movementDirection);

        Vector3 targetRotationDirection = GetTargetRotationDirection(targetRotationYAngle);

        float movementSpeed = GetMovementSpeed();

        Vector3 currentPlayerHorizontalVelocity = GetPlayerHorizontalVelocity();

        stateMachine.Player.Rigidbody.AddForce(
            targetRotationDirection * movementSpeed - currentPlayerHorizontalVelocity,
            ForceMode.VelocityChange);
    }

    private float Rotate(Vector3 direction)
    {
        float directionAngle = UpdateTargetRotation(direction);

        RotateTowardsTargetRotation();

        return directionAngle;
    }

    #endregion

    #region Reusable Methods

    private Vector3 GetMovementInputDirection()
    {
        return new Vector3(movementInput.x, 0, movementInput.y);
    }

    private float GetMovementSpeed() => movementData.BaseSpeed * stateMachine.ReusableData.MovementSpeedModifier;

    private Vector3 GetPlayerHorizontalVelocity()
    {
        Vector3 playerHorizontalVelocity = stateMachine.Player.Rigidbody.velocity;

        playerHorizontalVelocity.y = 0;

        return playerHorizontalVelocity;
    }

    private float AddCameraRotationToAngle(float angle)
    {
        angle += stateMachine.Player.MainCameraTransform.eulerAngles.y;

        if (angle > 360)
        {
            angle -= 360;
        }

        return angle;
    }

    private float GetDirectionAngle(Vector3 direction)
    {
        float directionAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;

        if (directionAngle < 0)
        {
            directionAngle += 360;
        }

        return directionAngle;
    }

    private void RotateTowardsTargetRotation()
    {
        float currentYAngle = stateMachine.Player.Rigidbody.rotation.eulerAngles.y;

        if (currentYAngle == stateMachine.ReusableData.CurrentTargetRotation.y)
        {
            return;
        }

        float smoothedYAngle = Mathf.SmoothDampAngle(
            currentYAngle,
            stateMachine.ReusableData.CurrentTargetRotation.y,
            ref 
            stateMachine.ReusableData.DampedTargetRotationCurrentVelocity.y, 
            stateMachine.ReusableData.TimeToReachTargetRotation.y -
            stateMachine.ReusableData.DampedTargetRotationPassedTime.y);

        stateMachine.ReusableData.DampedTargetRotationPassedTime.y += Time.deltaTime;

        Quaternion targetRotation = Quaternion.Euler(0, smoothedYAngle, 0);

        stateMachine.Player.Rigidbody.MoveRotation(targetRotation);
    }

    private void UpdateTargetRotationData(float targetAngle)
    {
        stateMachine.ReusableData.CurrentTargetRotation.y = targetAngle;

        stateMachine.ReusableData.DampedTargetRotationPassedTime.y = 0;
    }

    protected float UpdateTargetRotation(Vector3 direction, bool shouldConsiderCameraRotation = true)
    {
        float directionAngle = GetDirectionAngle(direction);

        if (shouldConsiderCameraRotation)
        {
            directionAngle = AddCameraRotationToAngle(directionAngle);
        }

        if (directionAngle != stateMachine.ReusableData.CurrentTargetRotation.y)
        {
            UpdateTargetRotationData(directionAngle);
        }

        return directionAngle;
    }

    protected Vector3 GetTargetRotationDirection(float angle)
    {
        return Quaternion.Euler(0, angle, 0) * Vector3.forward;
    }

    protected void ResetVelocity()
    {
        stateMachine.Player.Rigidbody.velocity = Vector3.zero;
    }

    protected virtual void AddInputActionsCallbacks()
    {
        stateMachine.Player.Input.PlayerActions.WalkToggle.started += OnWalkToggleStarted;
    }

    protected virtual void RemoveInputActionsCallbacks()
    {
        stateMachine.Player.Input.PlayerActions.WalkToggle.started -= OnWalkToggleStarted;
    }

    #endregion

    #region Input Methods

    protected virtual void OnWalkToggleStarted(InputAction.CallbackContext context)
    {
        stateMachine.ReusableData.ShouldWalk = !stateMachine.ReusableData.ShouldWalk;
    }

    #endregion
}