using System.Collections.Generic;
using UnityEngine;

public abstract class Node : MonoBehaviour
{
    [SerializeField] protected bool visited;
    [SerializeField] protected Box nodeBounds;
    
    private List<Node> childrenNodeList;

    public Box NodeBounds { get => nodeBounds; set => nodeBounds = value; }
    public List<Node> ChildrenNodeList => childrenNodeList;
    public int Width => (TopRightAreaCorner.x - BottomLeftAreaCorner.x);
    public int Length => (TopRightAreaCorner.y - BottomLeftAreaCorner.y);
    public Vector2Int BottomLeftAreaCorner { get; set; }
    public Vector2Int BottomRightAreaCorner { get; set; }
    public Vector2Int TopRightAreaCorner { get; set; }
    public Vector2Int TopLeftAreaCorner { get; set; }
    public Node Parent { get; set; }
    public int TreeLayerIndex { get; protected set; }
    public bool Visited { get => visited; set => visited = value; }
    
    protected Node(Node parentNode)
    {
        childrenNodeList = new List<Node>();
        Parent = parentNode;
        parentNode?.AddChild(this);
    }

    public void AddChild(Node node) => childrenNodeList.Add(node);

    public void RemoveChild(Node node) => childrenNodeList.Remove(node);
}