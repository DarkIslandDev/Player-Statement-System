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
    [SerializeField]
    private string actionMapName = "Player";

    [Header("Action Map Name References")]
    [SerializeField] private string move = "Move";
    [SerializeField] private string zoom = "Zoom";
    [SerializeField] private string action = "Action";
    [SerializeField] private string menu = "Menu";
    [SerializeField] private string sprint = "Sprint";

    [Header("Components")]
    [SerializeField] private Player player;
    private InputAction moveAction;
    private InputAction zoomAction;
    private InputAction actionAction;
    private InputAction menuAction;
    private InputAction sprintAction;

    private float rollInputTimer = 0;

    // public static PlayerInputHandler Instance { get; private set; }
    public Vector2 MoveInput { get; private set; }
    public Vector2 ZoomInput { get; private set; }
    public bool ActionInput { get; private set; }
    public bool MenuInput { get; private set; }
    public bool SprintInput { get; set; }
    public float Horizontal { get; private set; }
    public float Vertical { get; private set; }
    public float MoveAmount { get; private set; }
    public float MouseX { get; private set; }
    public float MouseY { get; private set; }
    public float MouseScrollWheel { get; private set; }

    public bool RollFlag { get; set; }
    public bool SprintFlag { get; set; }

    private void Awake()
    {
        // if (Instance == null)
        // {
        //     Instance = this;
        //     DontDestroyOnLoad(gameObject);
        // }
        // else
        // {
        //     DestroyImmediate(gameObject);
        // }

        moveAction = playerControls.FindActionMap(actionMapName).FindAction(move);
        zoomAction = playerControls.FindActionMap(actionMapName).FindAction(zoom);
        actionAction = playerControls.FindActionMap(actionMapName).FindAction(action);
        menuAction = playerControls.FindActionMap(actionMapName).FindAction(menu);
        sprintAction = playerControls.FindActionMap(actionMapName).FindAction(sprint);

        RegisterInputActions();
    }

    private void RegisterInputActions()
    {
        moveAction.performed += context => MoveInput = context.ReadValue<Vector2>();
        moveAction.canceled += context => MoveInput = Vector2.zero;

        zoomAction.performed += context => ZoomInput = context.ReadValue<Vector2>();
        zoomAction.canceled += context => ZoomInput = Vector2.zero;

        actionAction.performed += context => ActionInput = context.ReadValueAsButton();
        actionAction.canceled += context => ActionInput = context.ReadValueAsButton();

        menuAction.performed += context => MenuInput = !MenuInput;

        sprintAction.performed += context => SprintInput = context.ReadValueAsButton();
        sprintAction.canceled += context => SprintInput = context.ReadValueAsButton();
    }

    private void OnEnable()
    {
        moveAction.Enable();
        zoomAction.Enable();
        actionAction.Enable();
        menuAction.Enable();
        sprintAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        zoomAction.Disable();
        actionAction.Disable();
        menuAction.Disable();
        sprintAction.Disable();
    }

    public void TickInput(float delta)
    {
        HandleMovementInput();
        HandleSprintAndRollInput(delta);
        HandleMenuInput();
    }

    public void HandleMovementInput()
    {
        Horizontal = MoveInput.x;
        Vertical = MoveInput.y;
        MoveAmount = Mathf.Clamp01(Mathf.Abs(Horizontal) + Mathf.Abs(Vertical));

        MouseScrollWheel = -ZoomInput.y;
    }

    public void HandleSprintAndRollInput(float delta)
    {
        SprintFlag = SprintInput;

        if (SprintInput)
        {
            rollInputTimer += delta;
        }
        else
        {
            if (rollInputTimer > 0 && rollInputTimer < 0.5f)
            {
                SprintFlag = false;
                RollFlag = true;
            }

            rollInputTimer = 0;
        }
    }

    public void HandleMenuInput()
    {
        player.PlayerUI.ChangeMenuVisibleState(MenuInput);
    }
}