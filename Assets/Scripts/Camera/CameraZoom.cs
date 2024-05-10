using Cinemachine;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    [SerializeField] [Range(0, 10)] private float defaultDistance = 5;
    [SerializeField] [Range(0, 10)] private float minimumDistance = 1;
    [SerializeField] [Range(0, 10)] private float maximumDistance = 5;

    private float minimumHeight = 0.88f;
    private float maximumHeight = 1.65f;
    
    [SerializeField] [Range(0, 10)] private float smoothing = 4;
    [SerializeField] [Range(0, 10)] private float zoomSensitivity = 1;

    private CinemachineFramingTransposer framingTransposer;
    private CinemachineInputProvider inputProvider;
    private CinemachineVirtualCamera virtualCamera;
    private Transform lookAtTransform;

    private float currentTargetDistance;

    private void Awake()
    {
        framingTransposer ??= GetComponent<CinemachineVirtualCamera>()
            .GetCinemachineComponent<CinemachineFramingTransposer>();

        inputProvider ??= GetComponent<CinemachineInputProvider>();
        virtualCamera ??= GetComponent<CinemachineVirtualCamera>();

        currentTargetDistance = defaultDistance;
        lookAtTransform = virtualCamera.LookAt;
    }

    private void Update()
    {
        Zoom();
    }

    private void Zoom()
    {
        float zoomValue = inputProvider.GetAxisValue(2) * zoomSensitivity;

        currentTargetDistance = Mathf.Clamp(
            currentTargetDistance + zoomValue,
            minimumDistance,
            maximumDistance);
        
        float currentDistance = virtualCamera.m_Lens.OrthographicSize;
        
        if (currentDistance == currentTargetDistance)
        {
            return;
        }

        float lerpedZoomValue = Mathf.Lerp(
            currentDistance, 
            currentTargetDistance, 
            smoothing * Time.deltaTime);
        
        float targetHeight = Mathf.Lerp(
            minimumHeight, 
            maximumHeight, 
            (lerpedZoomValue - minimumDistance) / (maximumDistance - minimumDistance));
    
        virtualCamera.m_Lens.OrthographicSize = lerpedZoomValue;
        
        lookAtTransform.position = new Vector3(
            lookAtTransform.position.x, 
            targetHeight, 
            lookAtTransform.position.z);
    }
}