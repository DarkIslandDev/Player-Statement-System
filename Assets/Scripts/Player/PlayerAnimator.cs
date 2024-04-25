using System;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Player player;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerInputHandler playerInputHandler;

    private int ver;
    private int hor;
    private float v = 0;
    private float h = 0;

    public bool canRotate { get; set; }

    public Animator Animator
    {
        get => animator;
        set => animator = value;
    }

    private void Awake()
    {
        animator ??= GetComponent<Animator>();

        ver = Animator.StringToHash("Vertical");
        hor = Animator.StringToHash("Horizontal");
    }

    public void Init(PlayerMovement playerMovement, PlayerInputHandler playerInputHandler)
    {
        this.playerMovement = playerMovement;
        this.playerInputHandler = playerInputHandler;
    }

    public void UpdateAnimatorValues(float verMovement, float horMovement, bool isSprinting)
    {
        switch (verMovement)
        {
            case > 0 and < 0.55f:
                v = 0.5f;
                break;
            case > 0.55f:
                v = 1f;
                break;
            case < 0 and > -0.55f:
                v = -0.5f;
                break;
            case < -0.55f:
                v = -1f;
                break;
            default:
                v = 0f;
                break;
        }
        switch (horMovement)
        {
            case > 0 and < 0.55f:
                h = 0.5f;
                break;
            case > 0.55f:
                h = 1f;
                break;
            case < 0 and > -0.55f:
                h = -0.5f;
                break;
            case < -0.55f:
                h = -1f;
                break;
            default:
                h = 0f;
                break;
        }

        if (isSprinting)
        {
            v = 2;
            h = horMovement;
        }
        
        animator.SetFloat(ver, v, 0.1f, Time.deltaTime);
        animator.SetFloat(hor, h, 0.1f, Time.deltaTime);
    }

    public void CanRotate() => canRotate = true;
    
    public void CanNotRotate() => canRotate = false;

    public bool GetBool(string targetString) => animator.GetBool(targetString);
    
    public void SetBool(string targetString, bool targetBool) => animator.SetBool(targetString, targetBool);

    public void PlayTargetAnimation(string targetAnim, bool isInteracting)
    {
        animator.applyRootMotion = isInteracting;
        
        animator.SetBool("IsInteracting", isInteracting);
        animator.CrossFade(targetAnim, 0.2f);
    }

    private void OnAnimatorMove()
    {
        float delta = Time.deltaTime;
        playerMovement.Rigidbody.drag = 0;
        Vector3 deltaPosition = animator.deltaPosition;
        deltaPosition.y = 0;
        Vector3 velocity = deltaPosition / delta;
        playerMovement.Rigidbody.velocity = velocity;
    }
}