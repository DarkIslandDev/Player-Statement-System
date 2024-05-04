using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider), typeof(InputActions))]
public class Player : MonoBehaviour
{
    private PlayerMovementStateMachine movementStateMachine;

    [field: Header("References")] 
    [field: SerializeField] public PlayerSO Data { get; private set; }
    [field: Header("Collisions")]
    [field: SerializeField] public PlayerCapsuleColliderUtility ColliderUtility { get; private set; }
    [field: SerializeField] public PlayerLayerData PlayerLayerData { get; private set; }

    [field: Header("Cameras")] 
    [field: SerializeField] public PlayerCameraUtility CameraUtility { get; private set; }
    [field: Header("Animations")] 
    [field: SerializeField] public PlayerAnimationsData AnimationData { get; private set; }
    [field: Header("Components")] 
    public Rigidbody Rigidbody { get; private set; }
    public Animator Animator { get; private set; }
    public Transform MainCameraTransform { get; private set; }
    [field: SerializeField] public Transform PlayerAnimatorHandler { get; private set; }
    public PlayerInput Input { get; private set; }

    private void Awake()
    {
        Input = GetComponent<PlayerInput>();
        Rigidbody = GetComponent<Rigidbody>();
        Animator = PlayerAnimatorHandler.GetComponent<Animator>();

        ColliderUtility.Init(transform.gameObject);
        ColliderUtility.CalculateCapsuleColliderDimensions();
        
        CameraUtility.Init();
        
        AnimationData.Init();

        if (Camera.main != null)
        {
            MainCameraTransform = Camera.main.transform;
        }

        movementStateMachine = new PlayerMovementStateMachine(this);
    }

    private void Start()
    {
        movementStateMachine.ChangeState(movementStateMachine.IdlingState);
    }

    private void Update()
    {
        movementStateMachine.HandleInput();
        movementStateMachine.Update();
    }

    private void FixedUpdate()
    {
        movementStateMachine.PhysicsUpdate();
    }

    private void OnTriggerEnter(Collider collider) => movementStateMachine.OnTriggerEnter(collider);

    private void OnTriggerExit(Collider collider) => movementStateMachine.OnTriggerExit(collider);

    public void OnMovementStateAnimationEnterEvent() => movementStateMachine.OnAnimationEnterEvent();

    public void OnMovementStateAnimationExitEvent() => movementStateMachine.OnAnimationExitEvent();

    public void OnMovementStateAnimationTransitionEvent() => movementStateMachine.OnAnimationTransitionEvent();
}