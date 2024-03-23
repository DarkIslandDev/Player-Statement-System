using UnityEngine;

public class RoomNode : Node
{
    [SerializeField] protected bool spawnNode;

    public bool SpawnNode
    {
        get => spawnNode;
        set => spawnNode = value;
    }

    public RoomNode() :base(null) { } 

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