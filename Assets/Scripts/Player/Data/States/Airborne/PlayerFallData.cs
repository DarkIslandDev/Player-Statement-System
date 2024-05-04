using UnityEngine;

[System.Serializable]
public class PlayerFallData
{
    [field: SerializeField]
    [field: Range(1, 15)]
    public float FallSpeedLimit { get; private set; } = 15;
    [field: SerializeField]
    [field: Range(1, 100)]
    public float MinimumDistanceToBeConsideredHardFall { get; private set; } = 3;
}