using UnityEngine;

public class Interaction : MonoBehaviour, IInteraction
{
    public virtual bool Interact(Player player, PlayerInteraction playerInteraction)
    {
        return true;
    }
}