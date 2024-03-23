using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private new Rigidbody rigidbody;
    [SerializeField] private Player player;
    [SerializeField] private PlayerInputHandler playerInputHandler;
    
    private Vector3 moveDirection;
    private Transform cameraTransform;
    private Vector3 normalVector;
    private Vector3 targetPosition;
    
    public Rigidbody Rigidbody => rigidbody;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        playerInputHandler = PlayerInputHandler.Instance;
        
        cameraTransform = player.CameraHandler.Camera.transform;
    }

    public void Movement(float speed)
    {
        moveDirection = cameraTransform.forward * playerInputHandler.Vertical;
        moveDirection += cameraTransform.right * playerInputHandler.Horizontal;
        moveDirection.Normalize();
        moveDirection.y = 0;

        moveDirection *= speed;

        Vector3 projectiveVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
        rigidbody.velocity = projectiveVelocity;
    }
}