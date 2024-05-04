// using System;
// using UnityEngine;
//
// public class PlayerCamera : MonoBehaviour
// {
//     [SerializeField] private bool cameraFixed;
//     [Header("Transforms")]
//     [SerializeField] private Transform targetTransform;
//     [SerializeField] private Transform pivotTransform;
//     [SerializeField] private Transform cameraTransform;
//
//     [Header("Camera Speed")] 
//     [SerializeField] private float lookSpeed = 0.1f;
//     [SerializeField] private float followSpeed = 0.1f;
//     [SerializeField] private float pivotSpeed = 0.03f;
//
//     [Space]
//     [SerializeField] private float targetPosition;
//     [SerializeField] private float defaultPosition;
//     [SerializeField] private float lookAngle;
//     [SerializeField] private float pivotAngle;
//     [SerializeField] private float minPivot = -35;
//     [SerializeField] private float maxPivot = 35;
//
//     [Header("Camera Collision")] 
//     [SerializeField] private float cameraSphereRadius = 0.2f;
//     [SerializeField] private float cameraCollisionOffset = 0.2f;
//     [SerializeField] private float minCollisionOffset = 0.2f;
//
//     [Header("Camera Zoom")] 
//     [SerializeField] private float stepSize = 2;
//     [SerializeField] private float zoomDamp = 7.5f;
//     [SerializeField] private float minZoom = 2;
//     [SerializeField] private float maxZoom = 12;
//     [SerializeField] private float zoomSpeed = 2;
//     [SerializeField] private float zoomHeight;
//
//     private Vector3 cameraTransformPosition;
//     private Vector3 cameraFollowVelocity = Vector3.zero;
//     private LayerMask ignoreLayers;
//     
//     public static PlayerCamera Instance { get; private set; }
//     public Transform TargetTransform => targetTransform;
//     public Transform CameraTransform => cameraTransform;
//
//     private void Awake()
//     {
//         if (Instance == null)
//         {
//             Instance = this;
//             DontDestroyOnLoad(gameObject);
//         }
//         else
//         {
//             Destroy(gameObject);
//         }
//
//         ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
//         defaultPosition = cameraTransform.localPosition.z;
//         zoomHeight = cameraTransform.localPosition.y;
//         zoomHeight = 2.3f;
//     }
//
//     public void FollowTarget(float delta)
//     {
//         Vector3 targetPos = Vector3.SmoothDamp(transform.position, targetTransform.position,
//             ref cameraFollowVelocity, delta / followSpeed);
//         
//         transform.position = cameraFixed ? targetTransform.position : targetPos;
//         
//         HandleCameraCollision(delta);
//     }
//
//     private void HandleCameraCollision(float delta)
//     {
//         targetPosition = defaultPosition;
//         Vector3 direction = cameraTransform.position - pivotTransform.position;
//         direction.Normalize();
//
//         if (Physics.SphereCast(pivotTransform.position, cameraSphereRadius, direction, out RaycastHit hit,
//                 Mathf.Abs(targetPosition), ignoreLayers))
//         {
//             float dis = Vector3.Distance(pivotTransform.position, hit.point);
//             targetPosition = -(dis - cameraCollisionOffset);
//         }
//
//         if (Mathf.Abs(targetPosition) < minCollisionOffset)
//         {
//             targetPosition = -minCollisionOffset;
//         }
//
//         cameraTransformPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, delta / 0.2f);
//         cameraTransform.localPosition = cameraTransformPosition;
//     }
//     
//     public void Zooming(int value)
//     {
//         zoomHeight = pivotTransform.localPosition.y + value * stepSize;
//         
//         if (zoomHeight < minZoom)
//         {
//             zoomHeight = minZoom;
//             minPivot = -35;
//         }
//         else if (zoomHeight > maxZoom)
//         {
//             zoomHeight = maxZoom;
//             minPivot = 35;
//         }
//         else
//         {
//             minPivot = 15;
//         }
//     }
//
//     public void UpdateCameraPosition(float delta)
//     {
//         Vector3 zoomTarget =
//             new Vector3(pivotTransform.localPosition.x, zoomHeight, pivotTransform.localPosition.z);
//         zoomTarget -= zoomSpeed * (zoomHeight - pivotTransform.localPosition.y) * Vector3.forward;
//
//         pivotTransform.localPosition = Vector3.Lerp(pivotTransform.localPosition, zoomTarget, delta * zoomDamp);
//         
//         pivotTransform.LookAt(transform);
//     }
// }