using UnityEngine;

[System.Serializable]
public class PlayerJumpData
{
    [field: SerializeField] public PlayerRotationData RotationData { get; private set; }
    [field: SerializeField] [field: Range(0, 5)] public float JumpToGroundRayDistance { get; private set; }
    [field: SerializeField] public AnimationCurve JumpForceModifierOnSlopeUpwards { get; private set; }
    [field: SerializeField] public AnimationCurve JumpForceModifierOnSlopeDownwards { get; private set; }
    [field: SerializeField] public Vector3 StationaryForce { get; private set; }
    [field: SerializeField] public Vector3 WeakForce { get; private set; }
    [field: SerializeField] public Vector3 MediumForce { get; private set; }
    [field: SerializeField] public Vector3 StrongForce { get; private set; }
    [field: SerializeField] [field: Range(0, 10)]public float DecelerationForce { get; private set; }
}