using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Characters/Player", fileName = "Player", order = 0)]
public class PlayerSO : ScriptableObject
{
    [field: SerializeField]  public PlayerGroundedData GroundedData { get; private set; }
    [field: SerializeField]  public PlayerAirborneData AirborneData { get; private set; }
}