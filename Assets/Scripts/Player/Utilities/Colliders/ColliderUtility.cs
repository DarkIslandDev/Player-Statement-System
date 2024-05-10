using UnityEngine;

public class ColliderUtility : MonoBehaviour
{
    public CapsuleColliderData CapsuleColliderData { get; private set; }
    [field: SerializeField] public DefaultColliderData DefaultColliderData { get; private set; }
    [field: SerializeField] public SlopeData SlopeData { get; private set; }

    private void Awake()
    {
        Resize();
    }

    public void Resize()
    {
        Init(gameObject);

        CalculateCapsuleColliderDimensions();
    }

    public void Init(GameObject gameObject)
    {
        if (CapsuleColliderData != null)
        {
            return;
        }

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

        SetCapsuleColliderHeight(DefaultColliderData.Height * (1f - SlopeData.StepHeightPercentage));

        RecalculateCapsuleColliderCenter();

        RecalculateColliderRadius();

        CapsuleColliderData.UpdateColliderData();
    }

    public void SetCapsuleColliderRadius(float radius)
    {
        CapsuleColliderData.Collider.radius = radius;
    }

    public void SetCapsuleColliderHeight(float height)
    {
        CapsuleColliderData.Collider.height = height;
    }

    public void RecalculateCapsuleColliderCenter()
    {
        float colliderHeightDifference = DefaultColliderData.Height - CapsuleColliderData.Collider.height;

        Vector3 newColliderCenter = new Vector3(0f, DefaultColliderData.CenterY + (colliderHeightDifference / 2f), 0f);

        CapsuleColliderData.Collider.center = newColliderCenter;
    }

    public void RecalculateColliderRadius()
    {
        float halfColliderHeight = CapsuleColliderData.Collider.height / 2f;

        if (halfColliderHeight >= CapsuleColliderData.Collider.radius)
        {
            return;
        }

        SetCapsuleColliderRadius(halfColliderHeight);
    }
}