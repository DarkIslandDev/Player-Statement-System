using UnityEngine;

[System.Serializable]
public class PlayerRollData
{
    [field: SerializeField] [field: Range(0, 3)] public float SpeedModifier { get; private set; } 
}