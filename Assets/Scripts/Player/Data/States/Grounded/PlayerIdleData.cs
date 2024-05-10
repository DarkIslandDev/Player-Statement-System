using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerIdleData
{
    [field: SerializeField] public List<PlayerCameraRecenteringData> BackwardsCameraRecenteringData { get; private set; }
}