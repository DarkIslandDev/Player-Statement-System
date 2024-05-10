using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraRotation : MonoBehaviour
{
    [SerializeField] private InputActionReference XYAxis;
    
    private CinemachineInputProvider inputProvider;
    
    [field: SerializeField] public bool CanRotate { get; private set; }
    
    private void Awake()
    {
        inputProvider ??= GetComponent<CinemachineInputProvider>();
    }

    private void Update()
    {
        CheckRotation();
    }

    private void CheckRotation()
    {
        inputProvider.XYAxis = CanRotate ? XYAxis : null;
    }
}