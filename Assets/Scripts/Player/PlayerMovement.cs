using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Components")] [SerializeField]
    private new Rigidbody rigidbody;

    [SerializeField] private Player player;
    [SerializeField] private PlayerAnimator playerAnimator;
    [SerializeField] private PlayerInputHandler playerInputHandler;
    [Header("Speed Stats")] private float speed;
    [SerializeField] private float movementSpeed = 5;
    [SerializeField] private float sprintSpeed = 20;
    [SerializeField] private float walkSpeed = 1;
    [SerializeField] private float rotationSpeed = 10;
    [SerializeField] private float fallingSpeed = 80;

    private Vector3 moveDirection;
    private Transform cameraTransform;
    private Vector3 normalVector;
    private Vector3 targetPosition;

    public Rigidbody Rigidbody => rigidbody;

    private void Start()
    {
        rigidbody ??= GetComponent<Rigidbody>();

        cameraTransform = player.CameraHandler.Camera.transform;
    }

    public void Init(PlayerAnimator playerAnimator, PlayerInputHandler playerInputHandler)
    {
        this.playerAnimator = playerAnimator;
        this.playerInputHandler = playerInputHandler;
    }

    public void Movement(float delta)
    {
        if (player.IsInteracting)
        {
            return;
        }

        if (player.RollFlag)
        {
            return;
        }

        moveDirection = cameraTransform.forward * playerInputHandler.Vertical;
        moveDirection += cameraTransform.right * playerInputHandler.Horizontal;
        moveDirection.Normalize();
        moveDirection.y = 0;

        speed = movementSpeed;

        if (playerInputHandler.SprintInput && playerInputHandler.MoveAmount > 0.5f)
        {
            speed = sprintSpeed;
            player.IsSprinting = true;
            moveDirection *= speed;
        }
        else
        {
            if (playerInputHandler.MoveAmount < 0.5f)
            {
                moveDirection *= walkSpeed;
            }
            else
            {
                moveDirection *= speed;
            }

            player.IsSprinting = false;
        }

        Vector3 projectiveVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
        rigidbody.velocity = projectiveVelocity;

        playerAnimator.UpdateAnimatorValues(
            playerInputHandler.MoveAmount,
            0,
            player.IsSprinting);

        if (player.CanRotate)
        {
            HandleRotation(delta);
        }
    }

    private void HandleRotation(float delta)
    {
        Vector3 targetDir = Vector3.zero;
        float moveOverride = playerInputHandler.MoveAmount;

        targetDir = cameraTransform.forward * playerInputHandler.Vertical;
        targetDir += cameraTransform.right * playerInputHandler.Horizontal;
        targetDir.Normalize();
        targetDir.y = 0;

        if (targetDir == Vector3.zero)
        {
            targetDir = transform.forward;
        }

        float rs = rotationSpeed;

        Quaternion tr = Quaternion.LookRotation(targetDir);
        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, rs * delta);

        transform.rotation = targetRotation;
    }

    public void HandleRollingAndFalling()
    {
        if (playerAnimator.Animator.GetBool("IsInteracting"))
        {
            return;
        }

        if (playerInputHandler.RollFlag)
        {
            moveDirection = cameraTransform.forward * playerInputHandler.Vertical;
            moveDirection += cameraTransform.right * playerInputHandler.Horizontal;

            if (playerInputHandler.MoveAmount > 0)
            {
                playerAnimator.PlayTargetAnimation("Rolling", true);
                moveDirection.y = 0;
                Quaternion rollRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = rollRotation;
            }
        }
    }
}