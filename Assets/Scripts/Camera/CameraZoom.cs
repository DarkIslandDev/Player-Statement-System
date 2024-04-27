using System;
using Cinemachine;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    [SerializeField] [Range(0, 10)] private float defaultDistance = 6;
    [SerializeField] [Range(0, 10)] private float minimumDistance = 1;
    [SerializeField] [Range(0, 10)] private float maximumDistance = 6;
    
    [SerializeField] [Range(0, 10)] private float smoothing = 4;
    [SerializeField] [Range(0, 10)] private float zoomSensitivity = 1;

    private CinemachineFramingTransposer framingTransposer;
    private CinemachineInputProvider inputProvider;

    private float currentTargetDistance;

    private void Awake()
    {
        framingTransposer ??= GetComponent<CinemachineVirtualCamera>()
            .GetCinemachineComponent<CinemachineFramingTransposer>();

        inputProvider ??= GetComponent<CinemachineInputProvider>();

        currentTargetDistance = defaultDistance;
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
        
        float currentDistance = framingTransposer.m_CameraDistance;

        if (currentDistance == currentTargetDistance)
        {
            return;
        }

        float lerpedZoomValue = Mathf.Lerp(
            currentDistance, 
            currentTargetDistance, 
            smoothing * Time.deltaTime);

        framingTransposer.m_CameraDistance = lerpedZoomValue;
    }
}