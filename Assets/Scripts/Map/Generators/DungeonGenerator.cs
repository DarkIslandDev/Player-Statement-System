using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [Header("References")] [SerializeField]
    private DungeonGeneratorSO dungeonData;

    [Header("Components")] [SerializeField]
    private BinarySpacePartitioner binarySpacePartitioner;

    [SerializeField] private RoomGenerator roomGenerator;
    [SerializeField] private CorridorGenerator corridorGenerator;

    [Header("Room Lists")] [SerializeField]
    private List<RoomNode> rooms = new List<RoomNode>();

    [SerializeField] private List<CorridorNode> corridors = new List<CorridorNode>();

    [Header("Spawn Room")] [SerializeField]
    private RoomNode safeRoomNode;

    private List<Node> allRoomsCollection = new List<Node>();
    private List<Node> nodes;

    private List<Vector3> possibleDoorVerticalPosition;
    private List<Vector3> possibleDoorHorizontalPosition;

    private List<Vector3> possibleWallVerticalPosition;
    private List<Vector3> possibleWallHorizontalPosition;

    private GameObject roomsParent;
    private GameObject corridorsParent;

    public DungeonGeneratorSO DungeonData
    {
        get => dungeonData;
        private set => dungeonData = value;
    }


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

    public List<Vector3> PossibleWallHorizontalPosition
    {
        get => possibleWallHorizontalPosition;
        set => possibleWallHorizontalPosition = value;
    }

    public List<Vector3> PossibleWallVerticalPosition
    {
        get => possibleWallVerticalPosition;
        set => possibleWallVerticalPosition = value;
    }

    public List<Vector3> PossibleDoorHorizontalPosition
    {
        get => possibleDoorHorizontalPosition;
        set => possibleDoorHorizontalPosition = value;
    }

    public List<Vector3> PossibleDoorVerticalPosition
    {
        get => possibleDoorVerticalPosition;
        set => possibleDoorVerticalPosition = value;
    }


    private void Awake()
    {
        roomGenerator ??= GetComponent<RoomGenerator>();
        corridorGenerator ??= GetComponent<CorridorGenerator>();
        binarySpacePartitioner ??= GetComponent<BinarySpacePartitioner>();

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
        binarySpacePartitioner.Init(
            DungeonData.dungeonWidth,
            DungeonData.dungeonLength
        );

        corridorGenerator.Init(
            this,
            possibleWallHorizontalPosition,
            possibleDoorHorizontalPosition,
            possibleWallVerticalPosition,
            possibleDoorVerticalPosition);

        roomGenerator.Init(
            this,
            possibleWallHorizontalPosition,
            possibleDoorHorizontalPosition,
            possibleWallVerticalPosition,
            possibleDoorVerticalPosition);

        allRoomsCollection.AddRange(
            binarySpacePartitioner.PrepareRoomNodesCollection(
                DungeonData.maxIterations,
                DungeonData.roomWidthMin,
                DungeonData.roomLengthMin
            ));

        roomGenerator.CalculateRooms(
            DungeonData.roomBottomCornerModifier,
            DungeonData.roomTopCornerModifier,
            DungeonData.roomOffset);

        safeRoomNode = roomGenerator.GetSafeRoom();

        corridorGenerator.CalculateCorridor(
            allRoomsCollection,
            DungeonData.corridorDoor,
            DungeonData.corridorWidth);
    }

    private void CreateWalls()
    {
        nodes = new List<Node>();
        nodes.AddRange(rooms);
        nodes.AddRange(corridors);

        ObjectCreator.CreateWalls(
            nodes,
            DungeonData.horizontalWall,
            DungeonData.verticalWall,
            possibleWallHorizontalPosition,
            possibleWallVerticalPosition);

        for (int i = 0; i < rooms.Count; i++)
        {
            CheckPassages(rooms[i]);
        }
    }

    private void CreateDecorations()
    {
        roomGenerator.GetRoomsByCategory();
        roomGenerator.SpawnDecorationInRooms();

        ObjectCreator.CreatePillars(rooms, DungeonData.pillars);
        ObjectCreator.CreateTorches(rooms, DungeonData.torch);
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
    
    public Vector3 GetSafeRoomPosition()
    {
        float x = safeRoomNode.BoxCollider.center.x;
        float z = safeRoomNode.BoxCollider.center.z;

        return new Vector3(x, 0.2f, z);
    }

    public void ResetDungeon()
    {
        allRoomsCollection.Clear();
        rooms.Clear();
        corridors.Clear();
        roomGenerator.ResetRooms();
        corridorGenerator.ResetCorridors();

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