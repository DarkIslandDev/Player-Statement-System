using UnityEngine;

[RequireComponent(typeof(InputActions))]
public class Player : MonoBehaviour
{
    private PlayerMovementStateMachine movementStateMachine;
 
    [field: Header("References")]
    [field: SerializeField] public PlayerSO Data { get; private set; }
    public Rigidbody Rigidbody { get; private set; }
    public Transform MainCameraTransform { get; private set; }
    public PlayerInput Input { get; private set; }
    
    private void Awake()
    {
        movementStateMachine = new PlayerMovementStateMachine(this);
    }

    private void Start()
    {
        Rigidbody = GetComponent<Rigidbody>();
        Input = GetComponent<PlayerInput>();

        if (Camera.main != null)
        {
            MainCameraTransform = Camera.main.transform;
        }

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
}