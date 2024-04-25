using System.Collections.Generic;
using UnityEngine;

public class RoomNode : Node
{
    [SerializeField] protected DungeonEnums.RoomType roomType;
    
    protected GameObject torchParent;
    protected List<Torch> torches;

    public DungeonEnums.RoomType RoomType
    {
        get => roomType;
        set => roomType = value;
    }

    public GameObject TorchParent
    {
        get => torchParent;
        set => torchParent = value;
    }

    public List<Torch> Torches
    {
        get => torches;
        set => torches = value;
    }

    public RoomNode(Vector2Int bottomLeftAreaCorner, Vector2Int topRightAreaCorner, Node parentNode, int index) :
        base(parentNode)
    {
        BottomLeftAreaCorner = bottomLeftAreaCorner;
        TopRightAreaCorner = topRightAreaCorner;
        BottomRightAreaCorner = new Vector2Int(topRightAreaCorner.x, bottomLeftAreaCorner.y);
        TopLeftAreaCorner = new Vector2Int(bottomLeftAreaCorner.x, topRightAreaCorner.y);
        TreeLayerIndex = index;
    }
}