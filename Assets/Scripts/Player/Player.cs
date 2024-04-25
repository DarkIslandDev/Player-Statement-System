using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private PlayerInputHandler playerInputHandler;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerAnimator playerAnimator;
    [SerializeField] private PlayerUI playerUI;
    [SerializeField] private CameraHandler cameraHandler;
    [SerializeField] private MinimapCamera minimap;
    [Header("Flags")]
    [SerializeField] private bool isInteracting;
    [SerializeField] private bool canRotate = true;
    [SerializeField] private bool isSprinting;
    [SerializeField] private bool rollFlag;

    public PlayerMovement PlayerMovement => playerMovement;
    public PlayerAnimator PlayerAnimator => playerAnimator;
    public PlayerUI PlayerUI => playerUI;
    public CameraHandler CameraHandler => cameraHandler;
    public MinimapCamera MinimapCamera { get => minimap; set => minimap = value; }

    public bool IsInteracting
    {
        get => isInteracting;
        set => isInteracting = value;
    }

    public bool CanRotate
    {
        get => canRotate;
        set => canRotate = value;
    }

    public bool RollFlag
    {
        get => rollFlag;
        set => rollFlag = value;
    }

    public bool IsSprinting
    {
        get => isSprinting;
        set => isSprinting = value;
    }

    public void Init(CameraHandler cameraHandler, MinimapCamera minimap, GameObject guiMenu)
    {
        this.cameraHandler = cameraHandler;
        this.minimap = minimap;

        playerUI.GUIMenu = guiMenu;
    }

    private void Awake()
    {
        playerInputHandler ??= GetComponent<PlayerInputHandler>(); 
        playerMovement ??= GetComponent<PlayerMovement>();
        playerAnimator ??= GetComponentInChildren<PlayerAnimator>();
        playerUI ??= GetComponent<PlayerUI>();

        playerAnimator.Init(playerMovement, playerInputHandler);
        playerMovement.Init(playerAnimator, playerInputHandler);
    }

    private void Update()
    {
        float delta = Time.deltaTime;

        isInteracting = playerAnimator.GetBool("IsInteracting");
        canRotate = playerAnimator.GetBool("CanRotate");

        playerInputHandler.TickInput(delta);

        playerMovement.Movement(delta);
        playerMovement.HandleRollingAndFalling();

        minimap.FollowPlayer(transform);
    }

    private void LateUpdate()
    {
        float delta = Time.deltaTime;

        playerInputHandler.RollFlag = false;
        playerInputHandler.SprintFlag = false;

        if (cameraHandler != null)
        {
            cameraHandler.FollowTarget(delta);
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                cameraHandler.Zoom(-playerInputHandler.MouseScrollWheel);
            }
        }
    }
}