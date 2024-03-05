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

    [Header("Components")]
    [SerializeField] private Player player;

    private InputAction moveAction;
    
    public Vector2 MoveInput { get; private set; }
    public static PlayerInputHandler Instance { get; private set; }
    public float Horizontal { get; private set; }
    public float Vertical { get; private set; }
    public float MoveAmount { get; private set; }
    public float MouseX { get; private set; }
    public float MouseY { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        moveAction = playerControls.FindActionMap(actionMapName).FindAction(move);

        RegisterInputActions();
    }

    private void RegisterInputActions()
    {
        moveAction.performed += context => MoveInput = context.ReadValue<Vector2>();
        moveAction.canceled += context => MoveInput = Vector2.zero;
    }

    private void OnEnable()
    {
        moveAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
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
    }
}
