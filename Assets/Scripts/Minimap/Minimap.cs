using UnityEngine;

public class Minimap : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform minimapCameraTransform;

    private float offsetY = 30;

    public void FollowPlayer()
    {
        if (playerTransform != null)
        {
            minimapCameraTransform.position = new Vector3(
                playerTransform.position.x, 
                playerTransform.position.y + offsetY,
                playerTransform.position.z
            );
            
            minimapCameraTransform.rotation = Quaternion.Euler(90, playerTransform.eulerAngles.y, 0);
        }
    }
}