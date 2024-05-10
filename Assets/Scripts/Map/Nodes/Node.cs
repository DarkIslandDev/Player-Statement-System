﻿using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Node : MonoBehaviour
{
    [SerializeField] protected bool visited;
    [SerializeField] protected Box nodeBounds;
    [Space] [SerializeField] protected bool doorTopSide;
    [SerializeField] protected bool doorBottomSide;
    [SerializeField] protected bool doorLeftSide;
    [SerializeField] protected bool doorRightSide;
    [Space] protected GameObject roomWalls;
    protected GameObject topWalls;
    protected GameObject bottomWalls;
    protected GameObject leftWalls;
    protected GameObject rightWalls;
    protected List<GameObject> walls;
    protected List<GameObject> topWallsArray;
    protected List<GameObject> bottomWallsArray;
    protected List<GameObject> leftWallsArray;
    protected List<GameObject> rightWallsArray;
    protected Mesh mesh;

    private List<Node> childrenNodeList;
    private Node parent;
    private int treeLayerIndex;

    public Box NodeBounds
    {
        get => nodeBounds;
        set => nodeBounds = value;
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

    public List<GameObject> TopWallsArray
    {
        get => topWallsArray;
        set => topWallsArray = value;
    }

    public List<GameObject> BottomWallsArray
    {
        get => bottomWallsArray;
        set => bottomWallsArray = value;
    }

    public List<GameObject> LeftWallsArray
    {
        get => leftWallsArray;
        set => leftWallsArray = value;
    }

    public List<GameObject> RightWallsArray
    {
        get => rightWallsArray;
        set => rightWallsArray = value;
    }

    public Vector2Int BottomLeftAreaCorner { get; set; }
    public Vector2Int BottomRightAreaCorner { get; set; }
    public Vector2Int TopRightAreaCorner { get; set; }
    public Vector2Int TopLeftAreaCorner { get; set; }
    public Vector2Int TopCenterArea { get; set; }
    public Vector2Int BottomCenterArea { get; set; }
    public Vector2Int LeftCenterArea { get; set; }
    public Vector2Int RightCenterArea { get; set; }
    public int Width => TopRightAreaCorner.x - BottomLeftAreaCorner.x;
    public int Length => TopRightAreaCorner.y - BottomLeftAreaCorner.y;
    public BoxCollider BoxCollider { get; set; }

    public Mesh Mesh
    {
        get => mesh;
        set => mesh = value;
    }

    public Node Parent
    {
        get => parent;
        set => parent = value;
    }

    public int TreeLayerIndex
    {
        get => treeLayerIndex;
        protected set => treeLayerIndex = value;
    }

    public bool Visited
    {
        get => visited;
        set => visited = value;
    }

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

    protected void Init(Node parentNode)
    {
        childrenNodeList = new List<Node>();
        Parent = parentNode;
        parentNode?.AddChild(this);
    }

    public void AddChild(Node node)
    {
        childrenNodeList.Add(node);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!visited && other.GetComponent<Player>())
        {
            visited = true;
            gameObject.layer = 11;
        }
    }
}