using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Node : MonoBehaviour
{
    [SerializeField] protected bool visited;
    [SerializeField] protected Box nodeBounds;
    [SerializeField] protected GameObject roomWalls;
    [SerializeField] protected GameObject topWalls;
    [SerializeField] protected GameObject bottomWalls;
    [SerializeField] protected GameObject leftWalls;
    [SerializeField] protected GameObject rightWalls;
    [SerializeField] protected List<GameObject> walls;
    [SerializeField] protected bool doorTopSide;
    [SerializeField] protected bool doorBottomSide;
    [SerializeField] protected bool doorLeftSide;
    [SerializeField] protected bool doorRightSide;
    
    private List<Node> childrenNodeList;

    public Box NodeBounds { get => nodeBounds; set => nodeBounds = value; }
    public List<Node> ChildrenNodeList => childrenNodeList;
    public GameObject RoomWalls
    {
        get => roomWalls;
        set => roomWalls = value;
    }

    public List<GameObject> Walls
    {
        get => walls;
        set => walls = value;
    }

    public bool DoorTopSide
    {
        get => doorTopSide;
        set => doorTopSide = value;
    }

    public bool DoorBottomSide
    {
        get => doorBottomSide;
        set => doorBottomSide = value;
    }

    public bool DoorLeftSide
    {
        get => doorLeftSide;
        set => doorLeftSide = value;
    }

    public bool DoorRightSide
    {
        get => doorRightSide;
        set => doorRightSide = value;
    }

    public Vector2Int BottomLeftAreaCorner { get; set; }
    public Vector2Int BottomRightAreaCorner { get; set; }
    public Vector2Int TopRightAreaCorner { get; set; }
    public Vector2Int TopLeftAreaCorner { get; set; }
    public Vector2Int TopCenterArea { get; set; }
    public Vector2Int BottomCenterArea { get; set; }
    public Vector2Int LeftCenterArea { get; set; }
    public Vector2Int RightCenterArea { get; set; }
    public int Width => (TopRightAreaCorner.x - BottomLeftAreaCorner.x);
    public int Length => (TopRightAreaCorner.y - BottomLeftAreaCorner.y);
    public Node Parent { get; set; }
    public int TreeLayerIndex { get; protected set; }
    public bool Visited { get => visited; set => visited = value; }
    public MeshCollider MeshCollider { get; set; }
    
    public GameObject TopWalls
    {
        get => topWalls;
        set => topWalls = value;
    }

    public GameObject BottomWalls
    {
        get => bottomWalls;
        set => bottomWalls = value;
    }

    public GameObject LeftWalls
    {
        get => leftWalls;
        set => leftWalls = value;
    }

    public GameObject RightWalls
    {
        get => rightWalls;
        set => rightWalls = value;
    }

    protected Node(Node parentNode)
    {
        childrenNodeList = new List<Node>();
        Parent = parentNode;
        parentNode?.AddChild(this);
    }

    public void AddChild(Node node) => childrenNodeList.Add(node);

    public void RemoveChild(Node node) => childrenNodeList.Remove(node);
}