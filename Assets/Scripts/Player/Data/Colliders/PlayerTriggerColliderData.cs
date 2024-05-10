using UnityEngine;

[System.Serializable]
public class PlayerTriggerColliderData
{
    [field: SerializeField] public BoxCollider GroundCheckCollider { get; private set; }

    public Vector3 GroundCheckColliderVerticalExtents { get; private set; }

    public void Init()
    {
        GroundCheckColliderVerticalExtents = GroundCheckCollider.bounds.extents;
    }
}