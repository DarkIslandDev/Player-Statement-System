using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class Materials
{
    public Material floorMaterial;
    public Material wallMaterial;
    public Material doorMaterial;
    public Material pillarMaterial;
    public Material torchMaterial;
}

[Serializable]
public class Prefabs
{
    public GameObject wallHorizontal;
    public GameObject wallVertical;
    public GameObject door;
    public GameObject pillar;
    public GameObject torch;
}

[Serializable]
public class Components
{
    public BinarySpacePartitioner binarySpacePartitioner;
    public RoomGenerator roomGenerator;
    public CorridorGenerator corridorGenerator;
}

public class DungeonGenerator : MonoBehaviour
{
    [Space] [SerializeField] private List<RoomNode> rooms = new List<RoomNode>();
    [Space] [SerializeField] private List<CorridorNode> corridors = new List<CorridorNode>();

    private List<Node> allRoomsCollection = new List<Node>();
    private List<Node> nodes;

    private List<Vector3> possibleDoorVerticalPosition;
    private List<Vector3> possibleDoorHorizontalPosition;

    private List<Vector3> possibleWallVerticalPosition;
    private List<Vector3> possibleWallHorizontalPosition;

    private GameObject roomsParent;
    private GameObject corridorsParent;
    private GameObject wallsParent;

    [SerializeField] private RoomNode safeRoomNode;

    [Space] public DungeonGeneratorSO dungeonData;
    [Space] public Materials materials;
    [Space] public Prefabs prefabs;
    [Space] public Components components;

    public Player Player;

    public List<RoomNode> Rooms
    {
        get => rooms;
        set => rooms = value;
    }

    public List<CorridorNode> Corridors
    {
        get => corridors;
        set => corridors = value;
    }

    public GameObject RoomsParent
    {
        get => roomsParent;
        set => roomsParent = value;
    }

    public GameObject CorridorsParent
    {
        get => corridorsParent;
        set => corridorsParent = value;
    }

    private void Awake()
    {
        components.roomGenerator ??= GetComponent<RoomGenerator>();
        components.corridorGenerator ??= GetComponent<CorridorGenerator>();
        components.binarySpacePartitioner ??= GetComponent<BinarySpacePartitioner>();

        CreateDungeon();
    }

    #region Creation Methods

    public void CreateDungeon()
    {
        ResetDungeon();

        CreateParentObjects();

        allRoomsCollection = new List<Node>();
        rooms = new List<RoomNode>();
        corridors = new List<CorridorNode>();

        CreateBaseDungeon();
        CreateWalls();
        CreateDecorations();

        SpawnPlayerInSafeRoom();
    }

    private void CreateBaseDungeon()
    {
        components.binarySpacePartitioner.Init(
            dungeonData.dungeonWidth,
            dungeonData.dungeonLength
        );
        
        components.corridorGenerator.Init(
            possibleWallHorizontalPosition,
            possibleDoorHorizontalPosition,
            possibleWallVerticalPosition,
            possibleDoorVerticalPosition);
        
        components.roomGenerator.Init(
            possibleWallHorizontalPosition,
            possibleDoorHorizontalPosition,
            possibleWallVerticalPosition,
            possibleDoorVerticalPosition);

        allRoomsCollection.AddRange(
            components.binarySpacePartitioner.PrepareRoomNodesCollection(
                dungeonData.maxIterations,
                dungeonData.roomWidthMin,
                dungeonData.roomLengthMin
            ));

        components.roomGenerator.CalculateRooms(dungeonData.roomBottomCornerModifier,
            dungeonData.roomTopCornerModifier,
            dungeonData.roomOffset);

        components.corridorGenerator.CalculateCorridor(
            allRoomsCollection,
            prefabs.door,
            materials.doorMaterial,
            dungeonData.corridorWidth);
    }

    private void CreateWalls()
    {
        nodes = new List<Node>();
        nodes.AddRange(rooms);
        nodes.AddRange(corridors);

        ObjectCreator.CreateWalls(
            nodes,
            prefabs.wallHorizontal,
            prefabs.wallVertical,
            materials.wallMaterial,
            possibleWallHorizontalPosition,
            possibleWallVerticalPosition);

        for (int i = 0; i < rooms.Count; i++)
        {
            CheckPassages(rooms[i]);
        }
    }

    private void CreateDecorations()
    {
        ObjectCreator.CreatePillars(rooms, prefabs.pillar, materials.pillarMaterial);
        ObjectCreator.CreateTorches(rooms, prefabs.torch, materials.torchMaterial);
    }

    private void SpawnPlayerInSafeRoom()
    {
        if (Player)
        {
            Player.transform.position =
                new Vector3(
                    safeRoomNode.BoxCollider.center.x,
                    0,
                    safeRoomNode.BoxCollider.center.z
                );
        }
    }

    #endregion

    #region AdditionalCode

    private void CheckPassages(RoomNode node)
    {
        if (HasPassage(node.TopWalls)) node.DoorTopSide = true;
        if (HasPassage(node.BottomWalls)) node.DoorBottomSide = true;
        if (HasPassage(node.RightWalls)) node.DoorRightSide = true;
        if (HasPassage(node.LeftWalls)) node.DoorLeftSide = true;
    }

    private bool HasPassage(GameObject wallsParent)
    {
        return wallsParent.transform.Cast<Transform>().Any(child => child.CompareTag("Untagged"));
    }

    private void CreateParentObjects()
    {
        roomsParent = new GameObject("Rooms");
        roomsParent.transform.parent = transform;

        corridorsParent = new GameObject("Corridors");
        corridorsParent.transform.parent = transform;
    }

    public void ResetDungeon()
    {
        allRoomsCollection.Clear();
        rooms.Clear();
        corridors.Clear();
        components.roomGenerator.RoomList.Clear();

        nodes = new List<Node>();
        possibleDoorVerticalPosition = new List<Vector3>();
        possibleDoorHorizontalPosition = new List<Vector3>();
        possibleWallVerticalPosition = new List<Vector3>();
        possibleWallHorizontalPosition = new List<Vector3>();

        while (transform.childCount != 0)
        {
            foreach (Transform item in transform)
            {
                DestroyImmediate(item.gameObject);
            }
        }
    }

    #endregion
}