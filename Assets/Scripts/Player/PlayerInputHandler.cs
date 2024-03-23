using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("Input Action Asset")] 
    [SerializeField] private InputActionAsset playerControls;

    [Header("Action Map Name References")] 
    [SerializeField] private string actionMapName = "Player";

    [Header("Action Map Name References")] 
    [SerializeField] private string move = "Move";
    [SerializeField] private string zoom = "Zoom";

    [Header("Components")]
    [SerializeField] private Player player;

    private InputAction moveAction;
    private InputAction zoomAction;
    
    public static PlayerInputHandler Instance { get; private set; }
    public Vector2 MoveInput { get; private set; }
    public Vector2 ZoomInput { get; private set; }
    public float Horizontal { get; private set; }
    public float Vertical { get; private set; }
    public float MoveAmount { get; private set; }
    public float MouseX { get; private set; }
    public float MouseY { get; private set; }
    public float MouseScrollWheel { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
        }

        moveAction = playerControls.FindActionMap(actionMapName).FindAction(move);
        zoomAction = playerControls.FindActionMap(actionMapName).FindAction(zoom);

        RegisterInputActions();
    }

    private void RegisterInputActions()
    {
        moveAction.performed += context => MoveInput = context.ReadValue<Vector2>();
        moveAction.canceled += context => MoveInput = Vector2.zero;

        zoomAction.performed += context => ZoomInput = context.ReadValue<Vector2>();
        zoomAction.canceled += context => ZoomInput = Vector2.zero;
    }

    private void OnEnable()
    {
        moveAction.Enable();
        zoomAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        zoomAction.Disable();
    }

    public void TickInput()
    {
        HandleMovementInput();
    }

    public void HandleMovementInput()
    {
        Horizontal = MoveInput.x;
        Vertical = MoveInput.y;
        MoveAmount = Mathf.Clamp01(Mathf.Abs(Horizontal) + Mathf.Abs(Vertical));

        MouseScrollWheel = -ZoomInput.y;
    }
}
