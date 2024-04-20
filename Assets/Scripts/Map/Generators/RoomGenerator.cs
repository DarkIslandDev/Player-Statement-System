using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    [SerializeField] private List<RoomNode> roomList = new List<RoomNode>();
    [SerializeField] private BinarySpacePartitioner binarySpacePartitioner;
    [SerializeField] private DungeonGenerator dungeonGenerator;

    private List<Vector3> possibleWallHorizontalPosition;
    private List<Vector3> possibleDoorHorizontalPosition;
    private List<Vector3> possibleWallVerticalPosition;
    private List<Vector3> possibleDoorVerticalPosition;

    public List<RoomNode> RoomList => roomList;

    private void Awake()
    {
        binarySpacePartitioner ??= GetComponent<BinarySpacePartitioner>();
        dungeonGenerator ??= GetComponent<DungeonGenerator>();
    }

    public void Init(List<Vector3> possibleWallHorizontalPosition, List<Vector3> possibleDoorHorizontalPosition,
        List<Vector3> possibleWallVerticalPosition, List<Vector3> possibleDoorVerticalPosition)
    {
        this.possibleWallHorizontalPosition = possibleWallHorizontalPosition;
        this.possibleWallVerticalPosition = possibleWallVerticalPosition;
        this.possibleDoorHorizontalPosition = possibleDoorHorizontalPosition;
        this.possibleDoorVerticalPosition = possibleDoorVerticalPosition;
    }

    public void CalculateRooms(float bcModifier, float tcModifier, int roomOffset)
    {
        roomList = new List<RoomNode>();

        List<Node> roomSpaces = StructureHelper.TraverseGraphToExtractLowestLeafes(binarySpacePartitioner.RoomNode);

        roomList = GenerateRoomsInGivenSpaces(
            roomSpaces,
            bcModifier,
            tcModifier,
            roomOffset
        );

        GenerateRooms(roomList);
    }
    
    public List<RoomNode> GenerateRoomsInGivenSpaces(List<Node> rooms, float bcModifier, float tcModifier, int offset)
    {
        List<RoomNode> listToReturn = new List<RoomNode>();

        foreach (Node space in rooms)
        {
            Vector2Int newBottomLeftPoint = StructureHelper.GenerateBottomLeftCornerBetween(
                space.BottomLeftAreaCorner, space.TopRightAreaCorner, bcModifier, offset);

            Vector2Int newTopRightPoint = StructureHelper.GenerateTopRightCornerBetween(
                space.BottomLeftAreaCorner, space.TopRightAreaCorner, tcModifier, offset);

            space.BottomLeftAreaCorner = newBottomLeftPoint;
            space.TopRightAreaCorner = newTopRightPoint;
            space.BottomRightAreaCorner = new Vector2Int(newTopRightPoint.x, newBottomLeftPoint.y);
            space.TopLeftAreaCorner = new Vector2Int(newBottomLeftPoint.x, newTopRightPoint.y);

            listToReturn.Add((RoomNode)space);
        }

        return listToReturn;
    }

    public void GenerateRooms(List<RoomNode> roomNodes)
    {
        for (int i = 0; i < roomNodes.Count; i++)
        {
            GameObject meshGO = ObjectCreator.CreateMesh(
                roomNodes[i].BottomLeftAreaCorner,
                roomNodes[i].TopRightAreaCorner,
                dungeonGenerator.materials.floorMaterial
            );

            Mesh mesh = meshGO.GetComponent<MeshFilter>().sharedMesh;
            BoxCollider boxCollider = meshGO.GetComponent<BoxCollider>();

            CalculateValuesFromNodes(
                roomNodes[i].TopRightAreaCorner,
                roomNodes[i].BottomLeftAreaCorner,
                meshGO,
                mesh);

            SetRoomType(i);
        }
    }

    private void CalculateValuesFromNodes(Vector2 topRight, Vector2 bottomLeft, GameObject dungeonFloor, Mesh mesh)
    {
        Node node = null;

        node = AddRoomNodeToMesh(bottomLeft, dungeonFloor);

        if (node != null)
        {
            CalculateAdditionalValuesFromNode(topRight, bottomLeft, mesh, node);

            dungeonFloor.name += $" | {node.NodeBounds.Position.x} / {node.NodeBounds.Position.z}";
            mesh.name = node.name;

            CreateWallsParentObject(node);

            AddWallsPositionToFloor(topRight,
                bottomLeft, node);

            FindEmptyWallsObjects(node);
        }
    }

    private void CalculateAdditionalValuesFromNode(Vector2 topRight, Vector2 bottomLeft, Mesh mesh, Node node)
    {
        node.Mesh = mesh;

        node.NodeBounds = Box.CalculateBounds(mesh);

        node.TopCenterArea =
            new Vector2Int((int)(topRight.x + bottomLeft.x) / 2, (int)topRight.y);
        node.BottomCenterArea =
            new Vector2Int((int)(topRight.x + bottomLeft.x) / 2, (int)bottomLeft.y);
        node.LeftCenterArea =
            new Vector2Int((int)bottomLeft.x, (int)(topRight.y + bottomLeft.y) / 2);
        node.RightCenterArea =
            new Vector2Int((int)topRight.x, (int)(topRight.y + bottomLeft.y) / 2);
    }

    private void CreateWallsParentObject(Node node)
    {
        GameObject roomWalls = ObjectCreator.CreateWallsParentObject(node.transform, "RoomWalls");
        node.RoomWalls = roomWalls;
        GameObject topWalls = ObjectCreator.CreateWallsParentObject(roomWalls.transform, "TopRoomWalls");
        node.TopWalls = topWalls;
        GameObject bottomWalls = ObjectCreator.CreateWallsParentObject(roomWalls.transform, "BottomRoomWalls");
        node.BottomWalls = bottomWalls;
        GameObject leftWalls = ObjectCreator.CreateWallsParentObject(roomWalls.transform, "LeftRoomWalls");
        node.LeftWalls = leftWalls;
        GameObject rightWalls = ObjectCreator.CreateWallsParentObject(roomWalls.transform, "RightRoomWalls");
        node.RightWalls = rightWalls;
    }

    private Node AddRoomNodeToMesh(Vector2 bottomLeftCorner, GameObject dungeonFloor)
    {
        RoomNode roomNode = null;

        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].BottomLeftAreaCorner == bottomLeftCorner)
            {
                roomNode = dungeonFloor.AddComponent<RoomNode>();
                roomNode.transform.SetParent(dungeonGenerator.RoomsParent.transform);

                roomNode.BottomLeftAreaCorner = roomList[i].BottomLeftAreaCorner;
                roomNode.BottomRightAreaCorner = roomList[i].BottomRightAreaCorner;
                roomNode.TopRightAreaCorner = roomList[i].TopRightAreaCorner;
                roomNode.TopLeftAreaCorner = roomList[i].TopLeftAreaCorner;

                roomList[i] = roomNode;

                GameObject torches = new GameObject("Torches");
                torches.transform.SetParent(roomNode.transform);
                roomNode.TorchParent = torches;

                dungeonFloor.name = "Room ";

                dungeonGenerator.Rooms.Add(roomNode);
            }
        }

        return roomNode;
    }
    
    private void AddWallsPositionToFloor(Vector2 topRight, Vector2 bottomLeft, Node node)
    {
        Vector3 bottomLeftV = new Vector3(bottomLeft.x, 0, bottomLeft.y);
        Vector3 bottomRightV = new Vector3(topRight.x, 0, bottomLeft.y);
        Vector3 topLeftV = new Vector3(bottomLeft.x, 0, topRight.y);
        Vector3 topRightV = new Vector3(topRight.x, 0, topRight.y);

        AddWallsPosition(
            bottomLeftV,
            bottomRightV,
            topLeftV,
            topRightV,
            node);
    }

    private void AddWallsPosition(Vector3 bl, Vector3 br, Vector3 tl, Vector3 tr, Node node)
    {
        Vector3 wallPosition = Vector3.zero;

        for (int row = (int)bl.x; row < (int)br.x; row++)
        {
            wallPosition = new Vector3(row, 0, bl.z);
            AddWallPositionToList(
                wallPosition,
                possibleWallHorizontalPosition,
                possibleDoorHorizontalPosition,
                node.BottomWalls);
        }

        for (int row = (int)tl.x; row < (int)tr.x; row++)
        {
            wallPosition = new Vector3(row, 0, tr.z);
            AddWallPositionToList(
                wallPosition,
                possibleWallHorizontalPosition,
                possibleDoorHorizontalPosition,
                node.TopWalls);
        }

        for (int col = (int)bl.z; col < (int)tl.z; col++)
        {
            wallPosition = new Vector3(bl.x, 0, col);
            AddWallPositionToList(
                wallPosition,
                possibleWallVerticalPosition,
                possibleDoorVerticalPosition,
                node.LeftWalls);
        }

        for (int col = (int)br.z; col < (int)tr.z; col++)
        {
            wallPosition = new Vector3(br.x, 0, col);
            AddWallPositionToList(
                wallPosition,
                possibleWallVerticalPosition,
                possibleDoorVerticalPosition,
                node.RightWalls);
        }
    }

    private void AddWallPositionToList(Vector3 wallPos, List<Vector3> walls, List<Vector3> doors, GameObject parent)
    {
        if (walls.Contains(wallPos))
        {
            doors.Add(wallPos);
            walls.Remove(wallPos);
        }
        else
        {
            walls.Add(wallPos);
        }

        GameObject wallObject = new GameObject("Wall");
        wallObject.transform.position = wallPos;
        wallObject.transform.SetParent(parent.transform);
    }

    private void FindEmptyWallsObjects(Node node)
    {
        node.Walls = new List<GameObject>();
        node.TopWallsArray = new List<GameObject>();
        node.BottomWallsArray = new List<GameObject>();
        node.LeftWallsArray = new List<GameObject>();
        node.RightWallsArray = new List<GameObject>();

        node.TopWallsArray = FindEmptyObjects(node.TopWalls, node);
        node.BottomWallsArray = FindEmptyObjects(node.BottomWalls, node);
        node.LeftWallsArray = FindEmptyObjects(node.LeftWalls, node);
        node.RightWallsArray = FindEmptyObjects(node.RightWalls, node);
    }

    private List<GameObject> FindEmptyObjects(GameObject transformObject, Node node)
    {
        List<GameObject> childTransforms = new List<GameObject>();

        foreach (Transform child in transformObject.transform)
        {
            childTransforms.Add(child.gameObject);

            node.Walls.Add(child.gameObject);
        }

        return childTransforms;
    }

    private void SetRoomType(int i)
    {
        if (i == 0)
        {
            roomList[i].RoomType = RoomType.SafeRoom;
        }
        else if (i == roomList.Count - 1)
        {
            roomList[i].RoomType = RoomType.BossRoom;
        }
        else
        {
            int randomValue = Random.Range(0, 100);

            switch (randomValue)
            {
                // 60% вероятности для MonsterRoom
                case < 60:
                    roomList[i].RoomType = RoomType.MonsterRoom;
                    break;
                // 30% вероятности для TreasureRoom
                case < 90:
                    roomList[i].RoomType = RoomType.TreasureRoom;
                    break;
                // 10% вероятности для других типов комнат
                default:
                    roomList[i].RoomType =
                        (RoomType)Random.Range((int)RoomType.MerchantsRoom, (int)RoomType.TreasureRoom);
                    break;
            }
        }
    }
}