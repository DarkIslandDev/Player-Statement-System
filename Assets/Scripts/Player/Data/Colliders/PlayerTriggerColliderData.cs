using UnityEngine;

[System.Serializable]
public class PlayerTriggerColliderData
{
    [field: SerializeField] public BoxCollider GroundCheckCollider { get; private set; }
    
    public Vector3 GroundCheckColliderExtents { get; private set; }

    public void Init()
    {
        GroundCheckColliderExtents = GroundCheckCollider.bounds.extents;
    }
}