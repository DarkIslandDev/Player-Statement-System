using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    [SerializeField] private List<RoomNode> roomList = new List<RoomNode>();
    [SerializeField] private BinarySpacePartitioner binarySpacePartitioner;
    [SerializeField] private DungeonGenerator dungeonGenerator;

    public List<RoomNode> RoomList => roomList;

    private void Awake()
    {
        if (binarySpacePartitioner == null) binarySpacePartitioner = GetComponent<BinarySpacePartitioner>();
        if (dungeonGenerator == null) dungeonGenerator = GetComponent<DungeonGenerator>();
    }

    public void CalculateRooms(List<Vector3> possibleWallHorizontalPosition,
        List<Vector3> possibleDoorHorizontalPosition, List<Vector3> possibleWallVerticalPosition,
        List<Vector3> possibleDoorVerticalPosition, float bottomCorner, float topCorner, int roomOffset)
    {
        roomList = new List<RoomNode>();

        List<Node> roomSpaces = StructureHelper.TraverseGraphToExtractLowestLeafes(binarySpacePartitioner.RoomNode);

        roomList = GenerateRoomsInGivenSpaces(
            roomSpaces,
            bottomCorner,
            topCorner,
            roomOffset
        );

        GenerateRooms(
            possibleWallHorizontalPosition,
            possibleDoorHorizontalPosition,
            possibleWallVerticalPosition,
            possibleDoorVerticalPosition,
            roomList);
    }

    public void GenerateRooms(List<Vector3> possibleWallHorizontalPosition,
        List<Vector3> possibleDoorHorizontalPosition, List<Vector3> possibleWallVerticalPosition,
        List<Vector3> possibleDoorVerticalPosition, List<RoomNode> roomNodes)
    {
        for (int i = 0; i < roomNodes.Count; i++)
        {
            GameObject meshGO = ObjectCreator.CreateMesh(
                roomNodes[i].BottomLeftAreaCorner,
                roomNodes[i].TopRightAreaCorner,
                dungeonGenerator.materials.floorMaterial
            );

            MeshFilter meshFilter = meshGO.GetComponent<MeshFilter>();

            CalculateValuesFromNodes(
                possibleWallHorizontalPosition,
                possibleDoorHorizontalPosition,
                possibleWallVerticalPosition,
                possibleDoorVerticalPosition,
                roomNodes[i].TopRightAreaCorner,
                roomNodes[i].BottomLeftAreaCorner,
                meshGO,
                meshFilter.sharedMesh);
        }
    }

    private void CalculateValuesFromNodes(List<Vector3> possibleWallHorizontalPosition,
        List<Vector3> possibleDoorHorizontalPosition, List<Vector3> possibleWallVerticalPosition,
        List<Vector3> possibleDoorVerticalPosition, Vector2 topRight, Vector2 bottomLeft, GameObject dungeonFloor,
        Mesh mesh)
    {
        Node node = null;

        node = AddRoomNodeToMesh(bottomLeft, dungeonFloor);

        if (node != null)
        {
            node.NodeBounds = Box.CalculateBounds(mesh);

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

            AddWallsPositionToFloor(
                possibleWallHorizontalPosition,
                possibleDoorHorizontalPosition,
                possibleWallVerticalPosition,
                possibleDoorVerticalPosition,
                topRight,
                bottomLeft,
                topWalls,
                bottomWalls,
                rightWalls,
                leftWalls);

            node.Walls = new List<GameObject>();
            node.TopWallsArray = new List<GameObject>();
            node.BottomWallsArray = new List<GameObject>();
            node.LeftWallsArray = new List<GameObject>();
            node.RightWallsArray = new List<GameObject>();

            node.TopWallsArray = FindEmptyObjects(topWalls, node);
            node.BottomWallsArray = FindEmptyObjects(bottomWalls, node);
            node.LeftWallsArray = FindEmptyObjects(leftWalls, node);
            node.RightWallsArray = FindEmptyObjects(rightWalls, node);
        }
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

    public List<RoomNode> GenerateRoomsInGivenSpaces(List<Node> roomSpaces, float roomBottomCornerModifier,
        float roomTopCornerModifier, int roomOffset)
    {
        List<RoomNode> listToReturn = new List<RoomNode>();

        foreach (Node space in roomSpaces)
        {
            Vector2Int newBottomLeftPoint = StructureHelper.GenerateBottomLeftCornerBetween(
                space.BottomLeftAreaCorner, space.TopRightAreaCorner, roomBottomCornerModifier, roomOffset);

            Vector2Int newTopRightPoint = StructureHelper.GenerateTopRightCornerBetween(
                space.BottomLeftAreaCorner, space.TopRightAreaCorner, roomTopCornerModifier, roomOffset);

            space.BottomLeftAreaCorner = newBottomLeftPoint;
            space.TopRightAreaCorner = newTopRightPoint;
            space.BottomRightAreaCorner = new Vector2Int(newTopRightPoint.x, newBottomLeftPoint.y);
            space.TopLeftAreaCorner = new Vector2Int(newBottomLeftPoint.x, newTopRightPoint.y);

            listToReturn.Add((RoomNode)space);
        }

        return listToReturn;
    }

    private static void AddWallsPositionToFloor(List<Vector3> possibleWallHorizontalPosition,
        List<Vector3> possibleDoorHorizontalPosition, List<Vector3> possibleWallVerticalPosition,
        List<Vector3> possibleDoorVerticalPosition, Vector2 topRight, Vector2 bottomLeft, GameObject topWalls,
        GameObject bottomWalls,
        GameObject rightWalls, GameObject leftWalls)
    {
        Vector3 bottomLeftV = new Vector3(bottomLeft.x, 0, bottomLeft.y);
        Vector3 bottomRightV = new Vector3(topRight.x, 0, bottomLeft.y);
        Vector3 topLeftV = new Vector3(bottomLeft.x, 0, topRight.y);
        Vector3 topRightV = new Vector3(topRight.x, 0, topRight.y);

        AddWallsPosition(
            possibleWallHorizontalPosition,
            possibleDoorHorizontalPosition,
            possibleWallVerticalPosition,
            possibleDoorVerticalPosition,
            bottomLeftV,
            bottomRightV,
            topLeftV,
            topRightV,
            topWalls,
            bottomWalls,
            rightWalls,
            leftWalls);
    }

    private static void AddWallsPosition(List<Vector3> possibleWallHorizontalPosition,
        List<Vector3> possibleDoorHorizontalPosition, List<Vector3> possibleWallVerticalPosition,
        List<Vector3> possibleDoorVerticalPosition, Vector3 bottomLeftV, Vector3 bottomRightV, Vector3 topLeftV,
        Vector3 topRightV, GameObject topParentGameObject, GameObject bottomParentGameObject,
        GameObject rightParentGameObject,
        GameObject leftParentGameObject)
    {
        Vector3 wallPosition = Vector3.zero;

        for (int row = (int)bottomLeftV.x; row < (int)bottomRightV.x; row++)
        {
            wallPosition = new Vector3(row, 0, bottomLeftV.z);
            AddWallPositionToList(
                wallPosition,
                possibleWallHorizontalPosition,
                possibleDoorHorizontalPosition,
                bottomParentGameObject);
        }

        for (int row = (int)topLeftV.x; row < (int)topRightV.x; row++)
        {
            wallPosition = new Vector3(row, 0, topRightV.z);
            AddWallPositionToList(
                wallPosition,
                possibleWallHorizontalPosition,
                possibleDoorHorizontalPosition,
                topParentGameObject);
        }

        for (int col = (int)bottomLeftV.z; col < (int)topLeftV.z; col++)
        {
            wallPosition = new Vector3(bottomLeftV.x, 0, col);
            AddWallPositionToList(
                wallPosition,
                possibleWallVerticalPosition,
                possibleDoorVerticalPosition,
                leftParentGameObject);
        }

        for (int col = (int)bottomRightV.z; col < (int)topRightV.z; col++)
        {
            wallPosition = new Vector3(bottomRightV.x, 0, col);
            AddWallPositionToList(
                wallPosition,
                possibleWallVerticalPosition,
                possibleDoorVerticalPosition,
                rightParentGameObject);
        }
    }

    private static void AddWallPositionToList(Vector3 wallPosition, List<Vector3> wallList, List<Vector3> doorList,
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
    
    private static List<GameObject> FindEmptyObjects(GameObject transformObject, Node node)
    {
        List<GameObject> childTransforms = new List<GameObject>();

        foreach (Transform child in transformObject.transform)
        {
            childTransforms.Add(child.gameObject);

            node.Walls.Add(child.gameObject);
        }

        return childTransforms;
    }
}