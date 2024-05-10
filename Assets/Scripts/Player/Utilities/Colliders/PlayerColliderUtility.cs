using UnityEngine;

public class PlayerColliderUtility : ColliderUtility
{
    [field: SerializeField] public PlayerTriggerColliderData TriggerColliderData { get; private set; }

    protected override void OnInit()
    {
        base.OnInit();

        TriggerColliderData.Init();
    }
}