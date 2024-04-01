using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
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

    private void Awake()
    {
        if (Instance == null)
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

        rooms = new List<RoomNode>();
        corridors = new List<CorridorNode>();

        components.generator.Init(
            components.roomGenerator,
            components.corridorGenerator,
            components.binarySpacePartitioner,
            values.dungeonWidth,
            values.dungeonLength);

        nodes = components.generator.CalculateDungeon(
            values.maxIterations,
            values.roomWidthMin,
            values.roomLengthMin,
            values.corridorWidth,
            modifiers.roomBottomCornerModifier,
            modifiers.roomTopCornerModifier,
            modifiers.roomOffset);

        CreateRoomAndCorridors();
        CreateWalls();
        CreateDoorInCorridor();

        // CreatePillars();
        CreateTorches();
    }

    private void CreateRoomAndCorridors()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            CreateMesh(nodes[i].BottomLeftAreaCorner, nodes[i].TopRightAreaCorner);
        }
    }

    private void CreateWalls()
    {
        nodes = new List<Node>();
        nodes.AddRange(rooms);
        nodes.AddRange(corridors);

        Dictionary<Vector3, GameObject> wallPositionToGameObject = new Dictionary<Vector3, GameObject>();

        foreach (Node node in nodes)
        {
            foreach (GameObject wall in node.Walls)
            {
                Vector3 wallPosition = wall.transform.position;
                wallPositionToGameObject[wallPosition] = wall;
            }
        }

        CreateWallsOfType(prefabs.wallHorizontal, possibleWallHorizontalPosition, wallPositionToGameObject);
        CreateWallsOfType(prefabs.wallVertical, possibleWallVerticalPosition, wallPositionToGameObject);
    }

    private void CreateWallsOfType(GameObject wallPrefab, List<Vector3> possibleWallPositions,
        Dictionary<Vector3, GameObject> wallPositionToGameObject)
    {
        foreach (Vector3 wallPosition in possibleWallPositions)
        {
            if (wallPositionToGameObject.TryGetValue(wallPosition, out GameObject value))
            {
                CreateWall(wallPrefab, value, wallPosition);
            }
        }
    }

    private GameObject CreateWall(GameObject wallPrefab, GameObject wallParent, Vector3 wallPosition)
    {
        GameObject wall = Instantiate(wallPrefab, wallPosition, wallPrefab.transform.rotation, wallParent.transform);
     
        return wall;
    }

    private void CreateDoorInCorridor()
    {
        foreach (CorridorNode corridor in corridors)
        {
            GameObject door = CreateObject(
                prefabs.door,
                corridor.gameObject,
                corridor.NodeBounds.Position,
                prefabs.door.transform.localRotation = corridor.Orientation == Orientation.Vertical
                    ? Quaternion.Euler(0, 90, 0)
                    : Quaternion.identity);
        }
    }

    private void CreatePillars()
    {
        foreach (RoomNode room in rooms)
        {
            if (room.NodeBounds.Size is { x: >= 12, z: >= 12 })
            {
                Vector3 leftBottomCorner =
                    new Vector3(room.BottomLeftAreaCorner.x + 1, 0, room.BottomLeftAreaCorner.y + 1);
                Vector3 rightBottomCorner =
                    new Vector3(room.BottomRightAreaCorner.x - 1, 0, room.BottomRightAreaCorner.y + 1);
                Vector3 leftTopCorner =
                    new Vector3(room.TopLeftAreaCorner.x + 1, 0, room.TopLeftAreaCorner.y - 1);
                Vector3 rightTopCorner =
                    new Vector3(room.TopRightAreaCorner.x - 1, 0, room.TopRightAreaCorner.y - 1);

                List<Vector3> positions = new List<Vector3>()
                {
                    leftBottomCorner,
                    rightBottomCorner,
                    leftTopCorner,
                    rightTopCorner
                };

                for (int i = 0; i < positions.Count; i++)
                {
                    GameObject pillar = CreateObject(
                        prefabs.pillar,
                        room.gameObject,
                        positions[i],
                        Quaternion.identity);
                }
            }
        }
    }

    private void CreateTorches()
    {
        foreach (RoomNode room in rooms)
        {
            CreateTorch(room.TopWalls, room.TopWalls);
            CreateTorch(room.BottomWalls, room.BottomWalls);
            CreateTorch(room.LeftWalls, room.LeftWalls);
            CreateTorch(room.RightWalls, room.RightWalls);
            // foreach (Transform topWall in room.TopWalls.transform)
            // {
            //     foreach (Transform topWallTransform in topWall.transform)
            //     {
            //         // Debug.Log(topWallTransform);
            //     }
            // }
            
            // Vector3 bottomCenter = new Vector3(room.BottomCenterArea.x, 1, room.BottomCenterArea.y);
            // Vector3 topCenter = new Vector3(room.TopCenterArea.x, 1, room.TopCenterArea.y);
            // Vector3 LeftCenter = new Vector3(room.LeftCenterArea.x, 1, room.LeftCenterArea.y);
            // Vector3 RightCenter = new Vector3(room.RightCenterArea.x, 1, room.RightCenterArea.y);
            //
            // List<Vector3> positions = new List<Vector3>()
            // {
            //     bottomCenter,
            //     topCenter,
            //     LeftCenter,
            //     RightCenter
            // };
            //
            // List<Quaternion> rotationAngles = new List<Quaternion>()
            // {
            //     Quaternion.identity,
            //     Quaternion.Euler(0, 180, 0),
            //     Quaternion.Euler(0, 90, 0),
            //     Quaternion.Euler(0, -90, 0),
            // };
            //
            // if (room.DoorTopSide) break;
            //
            // if (room.DoorBottomSide) break;
            //
            // if (room.DoorLeftSide) break;
            //
            // if (room.DoorRightSide) break;
            //
            // for (int i = 0; i < positions.Count; i++)
            // {
            //     GameObject torch = CreateObject(
            //         prefabs.torch,
            //         room.gameObject,
            //         positions[i],
            //         rotationAngles[i]
            //     );
            // }
        }
    }

    private void CreateTorch(GameObject parentObject, GameObject wallsParentObject)
    {
        List<Transform> walls = new List<Transform>();
        
        foreach (Transform wall in wallsParentObject.transform)
        {
            walls.Add(wall);

            int rand = Random.Range(1, walls.Count - 1);
            Debug.Log(rand);
            float x = walls[rand].transform.position.x;
            float y = 1f;
            float z = walls[rand].transform.position.z;
            Vector3 torchPosition = new Vector3(x, y, z);
            Quaternion torchRotation = walls[rand].transform.localRotation;
                
            GameObject torch = CreateObject(prefabs.torch, parentObject, torchPosition, torchRotation);
        }
    }

    private void CreateObjectsToRoom()
    {
    }

    private GameObject CreateObject(GameObject prefab, GameObject parent, Vector3 position, Quaternion rotation)
    {
        GameObject createdObject = Instantiate(prefab, position, rotation, parent.transform);
        // createdObject.GetComponentInChildren<MeshRenderer>().material = material;

        return createdObject;
    }

    private void CreateMesh(Vector2 bottomLeftCorner, Vector2 topRightCorner)
    {
        Vector3 bottomLeftV = new Vector3(bottomLeftCorner.x, 0, bottomLeftCorner.y);
        Vector3 bottomRightV = new Vector3(topRightCorner.x, 0, bottomLeftCorner.y);
        Vector3 topLeftV = new Vector3(bottomLeftCorner.x, 0, topRightCorner.y);
        Vector3 topRightV = new Vector3(topRightCorner.x, 0, topRightCorner.y);

        Vector3[] vertices = { topLeftV, topRightV, bottomLeftV, bottomRightV };
        int[] triangles = { 0, 1, 2, 2, 1, 3 };

        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }

        Mesh mesh = new Mesh { vertices = vertices, uv = uvs, triangles = triangles };

        GameObject dungeonFloor = new GameObject(
            "Mesh",
            typeof(MeshFilter),
            typeof(MeshRenderer));

        dungeonFloor.GetComponent<MeshFilter>().mesh = mesh;
        dungeonFloor.GetComponent<MeshRenderer>().material = materials.floorMaterial;

        CollectAllAddedNodes(topRightCorner, bottomLeftCorner, dungeonFloor, mesh);
    }

    #endregion

    private void CollectAllAddedNodes(Vector2 topRight, Vector2 bottomLeft, GameObject dungeonFloor, Mesh mesh)
    {
        Node node = null;
        
        node = AddRoomNodes(bottomLeft, dungeonFloor, null);

        node = AddCorridorNodes(bottomLeft, dungeonFloor, node);

        if (node != null)
        {
            node.NodeBounds = CalculateBounds(mesh);

            node.TopCenterArea =
                new Vector2Int((int)(topRight.x + bottomLeft.x) / 2, (int)topRight.y);
            node.BottomCenterArea =
                new Vector2Int((int)(topRight.x + bottomLeft.x) / 2, (int)bottomLeft.y);
            node.LeftCenterArea =
                new Vector2Int((int)bottomLeft.x, (int)(topRight.y + bottomLeft.y) / 2);
            node.RightCenterArea =
                new Vector2Int((int)topRight.x, (int)(topRight.y + bottomLeft.y) / 2);

            dungeonFloor.name += $" | {node.NodeBounds.Position.x} / {node.NodeBounds.Position.z}";
            mesh.name = node.name;

            GameObject roomWalls = new GameObject("RoomWalls");
            roomWalls.transform.SetParent(node.transform);
            node.RoomWalls = roomWalls;

            GameObject topWalls = new GameObject("TopRoomWalls");
            topWalls.transform.SetParent(roomWalls.transform);
            // CreateTorch(topWalls, topWalls);
            node.TopWalls = topWalls;

            GameObject bottomWalls = new GameObject("BottomRoomWalls");
            bottomWalls.transform.SetParent(roomWalls.transform);
            // CreateTorch(bottomWalls, bottomWalls);
            node.BottomWalls = bottomWalls;

            GameObject leftWalls = new GameObject("LeftRoomWalls");
            leftWalls.transform.SetParent(roomWalls.transform);
            // CreateTorch(leftWalls, leftWalls);
            node.LeftWalls = leftWalls;

            GameObject rightWalls = new GameObject("RightRoomWalls");
            rightWalls.transform.SetParent(roomWalls.transform);
            // CreateTorch(rightWalls, rightWalls);
            node.RightWalls = rightWalls;

            Vector3 bottomLeftV = new Vector3(bottomLeft.x, 0, bottomLeft.y);
            Vector3 bottomRightV = new Vector3(topRight.x, 0, bottomLeft.y);
            Vector3 topLeftV = new Vector3(bottomLeft.x, 0, topRight.y);
            Vector3 topRightV = new Vector3(topRight.x, 0, topRight.y);

            AddWallsPositionToFloor(
                bottomLeftV,
                bottomRightV,
                topLeftV,
                topRightV,
                topWalls,
                bottomWalls,
                rightWalls,
                leftWalls);

            node.Walls = new List<GameObject>();

            FindEmptyObjects(topWalls, node, node.DoorTopSide);
            FindEmptyObjects(bottomWalls, node, node.DoorBottomSide);
            FindEmptyObjects(leftWalls, node, node.DoorLeftSide);
            FindEmptyObjects(rightWalls, node, node.DoorRightSide);
        }
    }

    private Node AddRoomNodes(Vector2 bottomLeftCorner, GameObject dungeonFloor, Node node)
    {
        for (int i = 0; i < components.generator.RoomList.Count; i++)
        {
            if (components.generator.RoomList[i].BottomLeftAreaCorner == bottomLeftCorner)
            {
                RoomNode roomNode = dungeonFloor.AddComponent<RoomNode>();

                roomNode.BottomLeftAreaCorner = components.generator.RoomList[i].BottomLeftAreaCorner;
                roomNode.BottomRightAreaCorner = components.generator.RoomList[i].BottomRightAreaCorner;
                roomNode.TopLeftAreaCorner = components.generator.RoomList[i].TopLeftAreaCorner;
                roomNode.TopRightAreaCorner = components.generator.RoomList[i].TopRightAreaCorner;

                dungeonFloor.name = $"Room ";

                node = roomNode;
                node.transform.parent = roomsParent.transform;

                GameObject torches = new GameObject("Torches");
                torches.transform.SetParent(node.transform);

                rooms.Add(roomNode);
            }
        }
        
        return node;
    }

    private Node AddCorridorNodes(Vector2 bottomLeftCorner, GameObject dungeonFloor, Node node)
    {
        for (int i = 0; i < components.generator.CorridorList.Count; i++)
        {
            if (components.generator.CorridorList[i].BottomLeftAreaCorner == bottomLeftCorner)
            {
                CorridorNode corridorNode = dungeonFloor.AddComponent<CorridorNode>();

                corridorNode.Orientation = components.corridorGenerator.Orientations[i];

                corridorNode.BottomLeftAreaCorner = components.generator.CorridorList[i].BottomLeftAreaCorner;
                corridorNode.BottomRightAreaCorner = components.generator.CorridorList[i].BottomRightAreaCorner;
                corridorNode.TopLeftAreaCorner = components.generator.CorridorList[i].TopLeftAreaCorner;
                corridorNode.TopRightAreaCorner = components.generator.CorridorList[i].TopRightAreaCorner;

                dungeonFloor.name = $"Corridor ";

                node = corridorNode;
                node.transform.parent = corridorsParent.transform;

                corridors.Add(corridorNode);
            }
        }

        return node;
    }

    private void AddWallsPositionToFloor(Vector3 bottomLeftV, Vector3 bottomRightV, Vector3 topLeftV,
        Vector3 topRightV, GameObject topParentGame, GameObject bottomParentGame, GameObject rightParentGame,
        GameObject leftParentGame)
    {
        Vector3 wallPosition = Vector3.zero;

        for (int row = (int)bottomLeftV.x; row < (int)bottomRightV.x; row++)
        {
            wallPosition = new Vector3(row, 0, bottomLeftV.z);
            AddWallPositionToList(
                wallPosition,
                possibleWallHorizontalPosition,
                possibleDoorHorizontalPosition,
                bottomParentGame);
        }

        for (int row = (int)topLeftV.x; row < (int)topRightV.x; row++)
        {
            wallPosition = new Vector3(row, 0, topRightV.z);
            AddWallPositionToList(
                wallPosition,
                possibleWallHorizontalPosition,
                possibleDoorHorizontalPosition,
                topParentGame);
        }

        for (int col = (int)bottomLeftV.z; col < (int)topLeftV.z; col++)
        {
            wallPosition = new Vector3(bottomLeftV.x, 0, col);
            AddWallPositionToList(
                wallPosition,
                possibleWallVerticalPosition,
                possibleDoorVerticalPosition,
                leftParentGame);
        }

        for (int col = (int)bottomRightV.z; col < (int)topRightV.z; col++)
        {
            wallPosition = new Vector3(bottomRightV.x, 0, col);
            AddWallPositionToList(
                wallPosition,
                possibleWallVerticalPosition,
                possibleDoorVerticalPosition,
                rightParentGame);
        }
    }

    private void AddWallPositionToList(Vector3 wallPosition, List<Vector3> wallList, List<Vector3> doorList,
        GameObject parentObject)
    {
        if (wallList.Contains(wallPosition))
        {
            doorList.Add(wallPosition);
            wallList.Remove(wallPosition);
        }
        else
        {
            wallList.Add(wallPosition);
        }

        // Добавляем стены к родительскому объекту
        GameObject wallObject = new GameObject("Wall");
        wallObject.transform.position = wallPosition;
        wallObject.transform.SetParent(parentObject.transform);
    }

    #region AdditionalCode

    private Box CalculateBounds(Mesh mesh)
    {
        Vector3 min = new Vector3(
            mesh.bounds.min.x,
            mesh.bounds.min.y,
            mesh.bounds.min.z);

        Vector3 max = new Vector3(
            mesh.bounds.max.x,
            mesh.bounds.max.y,
            mesh.bounds.max.z);

        Vector3 position = new Vector3(
            mesh.bounds.center.x,
            mesh.bounds.center.y,
            mesh.bounds.center.z);

        Vector3 size = new Vector3(
            mesh.bounds.size.x,
            mesh.bounds.size.y,
            mesh.bounds.size.z);

        float x = position.x;
        float y = position.y;
        float z = position.z;

        float xMin = Math.Min(position.x, position.x + size.x);
        float yMin = Math.Min(position.y, position.y + size.y);
        float zMin = Math.Min(position.z, position.z + size.z);

        float xMax = Math.Max(position.x, position.x + size.x);
        float yMax = Math.Max(position.y, position.y + size.y);
        float zMax = Math.Max(position.z, position.z + size.z);

        return new Box
        {
            Position = position,
            Size = size,
            x = x,
            y = y,
            z = z,
            Min = min,
            Max = max,
            xMin = xMin,
            yMin = yMin,
            zMin = zMin,
            xMax = xMax,
            yMax = yMax,
            zMax = zMax
        };

        // new Vector3((float) this.x + (float) this.m_Size.x / 2f,
        // (float) this.y + (float) this.m_Size.y / 2f,
        // (float) this.z + (float) this.m_Size.z / 2f);

        // Vector3 center = new Vector3(
        //     node.NodeBounds.x + node.NodeBounds.size.x / 2,
        //     node.NodeBounds.y + node.NodeBounds.size.y / 2,
        //     node.NodeBounds.z + node.NodeBounds.size.z / 2);
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
        rooms.Clear();
        corridors.Clear();

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

    private void FindEmptyObjects(GameObject transformObject, Node node, bool side)
    {
        foreach (Transform child in transformObject.transform)
        {
            List<GameObject> emptyObjects = new List<GameObject>();
            FindEmptyChildObjects(child, emptyObjects);

            if (emptyObjects.Count >= 1)
            {
                side = true;
            }
            
            node.Walls.Add(child.gameObject);
        }
    }

    private void FindEmptyChildObjects(Transform parent, List<GameObject> emptyObjects)
    {
        foreach (Transform child in parent)
        {
            if (child.childCount == 0)
            {
                emptyObjects.Add(child.gameObject);
            }
            else
            {
                FindEmptyChildObjects(child, emptyObjects); // Рекурсивно вызываем функцию для обхода всех дочерних объектов
            }
        }
    }

    #endregion
}