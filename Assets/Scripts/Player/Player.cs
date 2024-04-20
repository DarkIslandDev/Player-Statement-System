using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerInputHandler playerInputHandler;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private CameraHandler cameraHandler;
    [SerializeField] private Minimap minimap;
    
    public PlayerMovement PlayerMovement => playerMovement;
    public CameraHandler CameraHandler => cameraHandler;

    private void Start()
    {
        playerInputHandler = PlayerInputHandler.Instance;
        cameraHandler = CameraHandler.Instance;
        
        playerMovement ??= GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        playerInputHandler.TickInput();
        
        playerMovement.Movement(5);
        
        minimap.FollowPlayer();
    }

    private void FixedUpdate()
    {                
        float delta = Time.fixedDeltaTime;
        
        cameraHandler.FollowTarget(delta);
        if (!EventSystem.current.IsPointerOverGameObject()) cameraHandler.Zoom(-playerInputHandler.MouseScrollWheel);
    }
}