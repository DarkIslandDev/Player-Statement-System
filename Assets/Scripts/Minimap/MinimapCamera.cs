using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    private float offsetY = 30;

    public void FollowPlayer(Transform player)
    {
        if (player != null)
        {
            transform.position = new Vector3(
                player.position.x, 
                player.position.y + offsetY,
                player.position.z
            );
            
            transform.rotation = Quaternion.Euler(90, player.eulerAngles.y, 0);
        }
    }
}