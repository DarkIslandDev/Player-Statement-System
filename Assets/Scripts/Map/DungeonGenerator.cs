using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class Values
{
    public int dungeonWidth;
    public int dungeonLength;
    public int roomWidthMin;
    public int roomLengthMin;
    public int maxIterations;
    public int corridorWidth;
}

[Serializable]
public class Modifiers
{
    [Range(0.0f, 0.3f)] public float roomBottomCornerModifier;
    [Range(0.7f, 1.0f)] public float roomTopCornerModifier;
    [Range(0, 2)] public int roomOffset;
}

[Serializable]
public class Materials
{
    public Material floorMaterial;
    public Material wallMaterial;
    public Material doorMaterial;
    public Material pillarMaterial;
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
    public Generator generator;
    public RoomGenerator roomGenerator;
    public CorridorGenerator corridorGenerator;
}

public class DungeonGenerator : MonoBehaviour
{
    private List<Node> allRoomsCollection = new List<Node>();
    [Space] [SerializeField] private List<RoomNode> rooms = new List<RoomNode>();
    [Space] [SerializeField] private List<CorridorNode> corridors = new List<CorridorNode>();

    [Space] public Values values;
    [Space] public Modifiers modifiers;
    [Space] public Materials materials;
    [Space] public Prefabs prefabs;
    [Space] public Components components;

    public static DungeonGenerator Instance;
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

    [SerializeField] private List<Node> nodes;

    private List<Vector3> possibleDoorVerticalPosition;
    private List<Vector3> possibleDoorHorizontalPosition;

    private List<Vector3> possibleWallVerticalPosition;
    private List<Vector3> possibleWallHorizontalPosition;

    private GameObject roomsParent;
    private GameObject corridorsParent;
    private GameObject wallsParent;

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

    // public List<Node> AllRoomsCollection
    // {
    //     get => allRoomsCollection;
    //     set => allRoomsCollection = value;
    // }

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
        }

        components.generator ??= GetComponent<Generator>();
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
        CreateAdditionalObjectsInDungeon();
    }

    private void CreateBaseDungeon()
    {
        components.binarySpacePartitioner.Init(
            values.dungeonWidth,
            values.dungeonLength
        );

        allRoomsCollection.AddRange(
            components.binarySpacePartitioner.PrepareRoomNodesCollection(
                values.maxIterations,
                values.roomWidthMin,
                values.roomLengthMin
            ));

        components.roomGenerator.CalculateRooms(
            possibleWallHorizontalPosition,
            possibleDoorHorizontalPosition,
            possibleWallVerticalPosition,
            possibleDoorVerticalPosition,
            modifiers.roomBottomCornerModifier,
            modifiers.roomTopCornerModifier,
            modifiers.roomOffset);

        components.corridorGenerator.CalculateCorridor(
            possibleWallHorizontalPosition,
            possibleDoorHorizontalPosition,
            possibleWallVerticalPosition,
            possibleDoorVerticalPosition,
            allRoomsCollection,
            prefabs.door);
    }
    
    private void CreateAdditionalObjectsInDungeon()
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
            CheckPassages(nodes[i]);

            SetRoomType(i);
        }

        ObjectCreator.CreatePillars(rooms, prefabs.pillar);
        ObjectCreator.CreateTorches(rooms, prefabs.torch);

    }

    private void SetRoomType(int i)
    {
        if (i == 0)
        {
            rooms[i].RoomType = RoomType.SafeRoom;
        }
        else if(i == rooms.Count - 1)
        {
            rooms[i].RoomType = RoomType.BossRoom;
        }
        else
        {
            int randomValue = Random.Range(0, 100);

            switch (randomValue)
            {
                // 60% вероятности для MonsterRoom
                case < 60:
                    rooms[i].RoomType = RoomType.MonsterRoom;
                    break;
                // 30% вероятности для TreasureRoom
                case < 90:
                    rooms[i].RoomType = RoomType.TreasureRoom;
                    break;
                // 10% вероятности для других типов комнат
                default:
                    rooms[i].RoomType = (RoomType)Random.Range((int)RoomType.MerchantsRoom, (int)RoomType.TreasureRoom);
                    break;
            }
        }
    }

    #endregion
    
    #region AdditionalCode

    private void CheckPassages(Node node)
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