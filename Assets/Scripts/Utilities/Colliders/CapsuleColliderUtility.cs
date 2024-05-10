using UnityEngine;

[System.Serializable]
public class CapsuleColliderUtility
{
    [field: SerializeField] public CapsuleColliderData CapsuleColliderData { get; private set; }
    [field: SerializeField] public DefaultColliderData DefaultColliderData { get; private set; }
    [field: SerializeField] public SlopeData SlopeData { get; private set; }

    public void Init(GameObject gameObject)
    {
        CapsuleColliderData = new CapsuleColliderData();

        CapsuleColliderData.Init(gameObject);
        
        OnInit();
    }

    protected virtual void OnInit()
    {
    }
    
    public void CalculateCapsuleColliderDimensions()
    {
        SetCapsuleColliderRadius(DefaultColliderData.Radius);
        SetCapsuleColliderHeight(DefaultColliderData.Height * (1 - SlopeData.StepHeightPercentage));

        RecalculateCapsuleColliderCenter();
        RecalculateColliderRadius();

        CapsuleColliderData.UpdateColliderData();
    }

    public void SetCapsuleColliderRadius(float radius) => CapsuleColliderData.Collider.radius = radius;

    public void SetCapsuleColliderHeight(float height) => CapsuleColliderData.Collider.height = height;

    public void RecalculateCapsuleColliderCenter()
    {
        float colliderHeightDifference =
            DefaultColliderData.Height - CapsuleColliderData.Collider.height;

        Vector3 newColliderCenter = new Vector3(
            0,
            DefaultColliderData.CenterY + (colliderHeightDifference / 2),
            0);

        CapsuleColliderData.Collider.center = newColliderCenter;
    }
    
    public void RecalculateColliderRadius()
    {
        float halfColliderHeight = CapsuleColliderData.Collider.height / 2;

        if (halfColliderHeight < CapsuleColliderData.Collider.radius)
        {
            SetCapsuleColliderRadius(halfColliderHeight);
        }
    }
}