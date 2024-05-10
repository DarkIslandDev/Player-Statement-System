using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider), typeof(InputActions))]
public class Player : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerSO data;

    [Header("Collisions")]
    [SerializeField] private PlayerLayerData layerData;

    [Header("Camera")] 
    [SerializeField] private PlayerCameraRecenteringUtility cameraRecenteringUtility;

    [Header("Animations")]
    [SerializeField] private PlayerAnimationData animationData;

    [Header("Components")] 
    [SerializeField] private Transform animatorParent;
    [SerializeField] private Transform followPointParent;
    [FormerlySerializedAs("playerColliderUtillity")] [FormerlySerializedAs("playerResizableCapsuleCollider")] [SerializeField] private PlayerColliderUtility playerColliderUtility;
    
    private new Rigidbody rigidbody;
    private Animator animator;
    private PlayerInput input;
    private PlayerInteraction playerInteraction;
    private Transform mainCameraTransform;
    private PlayerMovementStateMachine movementStateMachine;

    public PlayerSO Data
    {
        get => data;
        private set => data = value;
    }

    public PlayerLayerData LayerData
    {
        get => layerData;
        private set => layerData = value;
    }

    public PlayerCameraRecenteringUtility CameraRecenteringUtility
    {
        get => cameraRecenteringUtility;
        private set => cameraRecenteringUtility = value;
    }

    public PlayerAnimationData AnimationData
    {
        get => animationData;
        private set => animationData = value;
    }

    public Rigidbody Rigidbody
    {
        get => rigidbody;
        private set => rigidbody = value;
    }

    public Animator Animator
    {
        get => animator;
        private set => animator = value;
    }

    public PlayerInput Input
    {
        get => input;
        private set => input = value;
    }

    public PlayerColliderUtility ColliderUtility
    {
        get => playerColliderUtility;
        private set => playerColliderUtility = value;
    }

    public Transform MainCameraTransform
    {
        get => mainCameraTransform;
        private set => mainCameraTransform = value;
    }

    public PlayerInteraction PlayerInteraction
    {
        get => playerInteraction;
        set => playerInteraction = value;
    }

    private void Awake()
    {
        CameraRecenteringUtility.Init(followPointParent);
        AnimationData.Init();

        Rigidbody = GetComponent<Rigidbody>();
        Animator = animatorParent.GetComponent<Animator>();

        Input = GetComponent<PlayerInput>();
        ColliderUtility = GetComponent<PlayerColliderUtility>();

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

    private void OnTriggerEnter(Collider collider)
    {
        movementStateMachine.OnTriggerEnter(collider);
    }

    private void OnTriggerExit(Collider collider)
    {
        movementStateMachine.OnTriggerExit(collider);
    }

    public void OnMovementStateAnimationEnterEvent()
    {
        movementStateMachine.OnAnimationEnterEvent();
    }

    public void OnMovementStateAnimationExitEvent()
    {
        movementStateMachine.OnAnimationExitEvent();
    }

    public void OnMovementStateAnimationTransitionEvent()
    {
        movementStateMachine.OnAnimationTransitionEvent();
    }

    public void Init(CinemachineVirtualCamera virtualCamera, GameObject interactionButton)
    {
        cameraRecenteringUtility.VirtualCamera = virtualCamera;

        playerInteraction.Init(this, interactionButton);
    }
}