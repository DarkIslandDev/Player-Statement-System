using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerInteraction : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject uiButton;
    
    [Header("Interaction Variables")]
    [SerializeField] private Transform interactionPoint;
    [SerializeField] private float interactionPointRadius = 2f;
    [SerializeField] private LayerMask interactableLayer;
    
    private Player player;

    private readonly Collider[] colliders = new Collider[5];
    private int numFound;

    public GameObject UIButton
    {
        get => uiButton;
        set => uiButton = value;
    }

    public void Init(Player player, GameObject uiManagerInteractionButton)
    {
        this.player = player;
    }

    public void CheckForInteractionWithObject()
    {
        numFound = Physics.OverlapSphereNonAlloc(
            interactionPoint.position,
            interactionPointRadius,
            colliders,
            interactableLayer);

        if (numFound > 0)
        {
            if (uiButton != null)
            {
                uiButton.SetActive(true);
            }

            IInteraction interactable = colliders[0].GetComponent<IInteraction>();

            if (interactable != null && player.Input.PlayerActions.Interaction.WasPressedThisFrame())
            {
                interactable.Interact(player, this);
            }
        }
        else
        {
            if (uiButton != null)
            {
                uiButton.SetActive(false);
            }
        }
    }
}