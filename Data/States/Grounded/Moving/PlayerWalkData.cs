using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerWalkData
{
    [field: SerializeField] [field: Range(0, 1)] public float SpeedModifier { get; private set; } = 0.225f;
    [field: SerializeField] public List<PlayerCameraRecenteringData> BackwardsCameraRecenteringData { get; private set; }
}