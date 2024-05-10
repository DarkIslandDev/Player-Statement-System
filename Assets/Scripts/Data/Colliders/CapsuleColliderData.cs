using UnityEngine;

[System.Serializable]
public class CapsuleColliderData
{
    [field: SerializeField] public CapsuleCollider Collider { get; private set; }
    [field: SerializeField] public Vector3 ColliderCenterInLocalSpace { get; private set; }
    [field: SerializeField] public Vector3 ColliderVerticalExtents { get; private set; }

    public void Init(GameObject gameObject)
    {
        Collider = gameObject.GetComponent<CapsuleCollider>();
        
        UpdateColliderData();
    }

    public void UpdateColliderData()
    {
        ColliderCenterInLocalSpace = Collider.center;

        ColliderVerticalExtents = new Vector3(0, Collider.bounds.extents.y, 0);
    }
}