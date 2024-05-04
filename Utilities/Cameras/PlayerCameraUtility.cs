using Cinemachine;
using UnityEngine;

[System.Serializable]
public class PlayerCameraUtility
{
    private CinemachinePOV cinemachinePov;
   
    [field: SerializeField] public CinemachineVirtualCamera VirtualCamera { get; private set; }
    [field: SerializeField] public float DefaultHorizontalWaitTime { get; private set; } = 0f;
    [field: SerializeField] public float DefaultHorizontalRecenteringTime { get; private set; } = 4f;

    public void Init()
    {
        cinemachinePov = VirtualCamera.GetCinemachineComponent<CinemachinePOV>();
    }

    public void EnableRecentering(float waitTime = -1f, float recenteringTime = -1f, float baseSpeed = 1f, float speed = 1f)
    {
        cinemachinePov.m_HorizontalRecentering.m_enabled = true;
        
        cinemachinePov.m_HorizontalRecentering.CancelRecentering();

        if (waitTime == -1f)
        {
            waitTime = DefaultHorizontalWaitTime;
        }
        
        if (recenteringTime == -1f)
        {
            recenteringTime = DefaultHorizontalRecenteringTime;
        }

        recenteringTime = recenteringTime * baseSpeed / speed;

        cinemachinePov.m_HorizontalRecentering.m_WaitTime = waitTime;
        cinemachinePov.m_HorizontalRecentering.m_RecenteringTime = recenteringTime;
    }
    
    public void DisableRecentering(float waitTime = -1f, float recenteringTime = -1f)
    {
        cinemachinePov.m_HorizontalRecentering.m_enabled = false;
    }
    
}