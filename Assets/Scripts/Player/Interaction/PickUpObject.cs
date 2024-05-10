public class PickUpObject : Interaction
{
    public override bool Interact(Player player, PlayerInteraction playerInteraction)
    {
        DestroyObject();
        
        return base.Interact(player, playerInteraction);
    }

    private void DestroyObject()
    {
        Destroy(gameObject);
    }
}