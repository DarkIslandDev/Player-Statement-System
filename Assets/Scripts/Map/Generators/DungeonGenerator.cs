using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    [Space][SerializeField] private List<RoomNode> rooms = new List<RoomNode>();
    [Space][SerializeField] private List<CorridorNode> corridors = new List<CorridorNode>();

    private List<Node> allRoomsCollection = new List<Node>();
    private List<Node> nodes;

    private List<Vector3> possibleDoorVerticalPosition;
    private List<Vector3> possibleDoorHorizontalPosition;

    private List<Vector3> possibleWallVerticalPosition;
    private List<Vector3> possibleWallHorizontalPosition;

    private GameObject roomsParent;
    private GameObject corridorsParent;

    [SerializeField] private RoomNode safeRoomNode;

    [Space] public DungeonGeneratorSO dungeonData;
    [Space] public Prefabs prefabs;
    [Space] public Components components;

    public List<RoomNode> Rooms { get => rooms; set => rooms = value; }

    public List<CorridorNode> Corridors { get => corridors; set => corridors = value; }

    public GameObject RoomsParent { get => roomsParent; set => roomsParent = value; }

    public GameObject CorridorsParent { get => corridorsParent; set => corridorsParent = value; }

    public List<Vector3> PossibleWallHorizontalPosition { get => possibleWallHorizontalPosition; set => possibleWallHorizontalPosition = value; }
    public List<Vector3> PossibleWallVerticalPosition { get => possibleWallVerticalPosition; set => possibleWallVerticalPosition = value; }
    public List<Vector3> PossibleDoorHorizontalPosition { get => possibleDoorHorizontalPosition; set => possibleDoorHorizontalPosition = value; }
    public List<Vector3> PossibleDoorVerticalPosition { get => possibleDoorVerticalPosition; set => possibleDoorVerticalPosition = value; }


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

        components.roomGenerator.CalculateRooms(
            dungeonData.roomBottomCornerModifier,
            dungeonData.roomTopCornerModifier,
            dungeonData.roomOffset);

        safeRoomNode = components.roomGenerator.GetSafeRoom();

        components.corridorGenerator.CalculateCorridor(
            allRoomsCollection,
            prefabs.door,
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
            possibleWallHorizontalPosition,
            possibleWallVerticalPosition);

        for (int i = 0; i < rooms.Count; i++)
        {
            CheckPassages(rooms[i]);
        }

    }

    private void CreateDecorations()
    {
        components.roomGenerator.GetRoomsByCategory();
        components.roomGenerator.SpawnDecorationInRooms();

        ObjectCreator.CreatePillars(rooms, dungeonData.pillars);
        ObjectCreator.CreateTorches(rooms, prefabs.torch);
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

    public Vector3 GetSafeRoomPosition()
    {
        float x = safeRoomNode.TopRightAreaCorner.x / 2;
        float z = safeRoomNode.TopRightAreaCorner.x / 2;

        return new Vector3(x, 0, z);
    }

    #endregion
}