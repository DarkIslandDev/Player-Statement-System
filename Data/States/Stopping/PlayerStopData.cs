using UnityEngine;

[System.Serializable]
public class PlayerStopData
{
    [field: SerializeField]
    [field: Range(0f, 15f)]
    public float LigtDeceleration { get; private set; } = 5f;

    [field: SerializeField]
    [field: Range(0f, 15f)]
    public float MediumDeceleration { get; private set; } = 6.5f;

    [field: SerializeField]
    [field: Range(0f, 15f)]
    public float HardDeceleration { get; private set; } = 5f;
}