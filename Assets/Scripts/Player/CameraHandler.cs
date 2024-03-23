using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private bool cameraFixed;
    [SerializeField] private new Camera camera;
    [SerializeField] private Transform target;
    
    [Header("Values")]
    [SerializeField] private float followSpeed = 0.1f;
    [SerializeField] private float zoomScale = 5;
    [SerializeField] private float zoomMin = 0.5f;
    [SerializeField] private float zoomMax = 100;

    private Vector3 cameraFollowVelocity = Vector3.zero;

    public static CameraHandler Instance { get; private set; }
    public Camera Camera => camera;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }
    
    public void FollowTarget(float delta)
    {
        Vector3 targetPos = Vector3.SmoothDamp(camera.transform.position, target.position,
            ref cameraFollowVelocity, delta / followSpeed);
        
        camera.transform.position = cameraFixed ? target.position : targetPos;
    }

    public void Zoom(float zoomDiff)
    {
        if (zoomDiff != 0)
        {
            camera.orthographicSize = Mathf.Clamp(camera.orthographicSize - zoomDiff * zoomScale, zoomMin, zoomMax);
        }
    }
}