using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerInputHandler playerInputHandler;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerCamera playerCamera;
    
    public PlayerMovement PlayerMovement => playerMovement;
    public PlayerCamera PlayerCamera => playerCamera;

    private void Start()
    {
        playerInputHandler = PlayerInputHandler.Instance;
        playerCamera = PlayerCamera.Instance;
        
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        playerInputHandler.TickInput();
        
        playerMovement.Movement(5);
    }

    private void LateUpdate()
    {                
        float delta = Time.deltaTime;
        
        playerCamera.FollowTarget(delta);
    }
}