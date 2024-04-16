using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum PassageOrientation
{
    None,
    Top,
    Bottom,
    Left,
    Right
}

public class CorridorGenerator : MonoBehaviour
{
    [SerializeField] private DungeonGenerator dungeonGenerator;
    [SerializeField] private List<CorridorNode> corridorList = new List<CorridorNode>();
    public List<Orientation> Orientations { get; private set; } = new();

    public List<CorridorNode> CorridorList
    {
        get => corridorList;
        set => corridorList = value;
    }

    private void Awake()
    {
        if (dungeonGenerator == null) dungeonGenerator = GetComponent<DungeonGenerator>();
    }

    public void CalculateCorridor(List<Vector3> possibleWallHorizontalPosition,
        List<Vector3> possibleDoorHorizontalPosition, List<Vector3> possibleWallVerticalPosition,
        List<Vector3> possibleDoorVerticalPosition, List<Node> roomsCollection, GameObject doorPrefab)
    {
        corridorList = new List<CorridorNode>();

        corridorList = CreateCorridor(
            roomsCollection,
            dungeonGenerator.values.corridorWidth);

        GenerateCorridors(
            possibleWallHorizontalPosition,
            possibleDoorHorizontalPosition,
            possibleWallVerticalPosition,
            possibleDoorVerticalPosition,
            corridorList);

        ObjectCreator.CreateDoorInCorridor(corridorList, doorPrefab);
    }

    public void GenerateCorridors(List<Vector3> possibleWallHorizontalPosition,
        List<Vector3> possibleDoorHorizontalPosition, List<Vector3> possibleWallVerticalPosition,
        List<Vector3> possibleDoorVerticalPosition, List<CorridorNode> nodes)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            GameObject meshGO = ObjectCreator.CreateMesh(
                nodes[i].BottomLeftAreaCorner,
                nodes[i].TopRightAreaCorner,
                dungeonGenerator.materials.floorMaterial);

            MeshFilter meshFilter = meshGO.GetComponent<MeshFilter>();

            CalculateValuesFromNodes(
                possibleWallHorizontalPosition,
                possibleDoorHorizontalPosition,
                possibleWallVerticalPosition,
                possibleDoorVerticalPosition,
                nodes[i].TopRightAreaCorner,
                nodes[i].BottomLeftAreaCorner,
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

        node = AddCorridorNodes(bottomLeft, dungeonFloor);

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

            GameObject roomWalls = ObjectCreator.CreateWallsParentObject(node.transform, "CorridorWalls");
            node.RoomWalls = roomWalls;
            GameObject topWalls = ObjectCreator.CreateWallsParentObject(roomWalls.transform, "TopCorridorWalls");
            node.TopWalls = topWalls;
            GameObject bottomWalls = ObjectCreator.CreateWallsParentObject(roomWalls.transform, "BottomCorridorWalls");
            node.BottomWalls = bottomWalls;
            GameObject leftWalls = ObjectCreator.CreateWallsParentObject(roomWalls.transform, "LeftCorridorWalls");
            node.LeftWalls = leftWalls;
            GameObject rightWalls = ObjectCreator.CreateWallsParentObject(roomWalls.transform, "RightCorridorWalls");
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

    private Node AddCorridorNodes(Vector2 bottomLeftCorner, GameObject dungeonFloor)
    {
        CorridorNode corridorNode = null;

        for (int i = 0; i < corridorList.Count; i++)
        {
            if (corridorList[i].BottomLeftAreaCorner == bottomLeftCorner)
            {
                corridorNode = dungeonFloor.AddComponent<CorridorNode>();
                corridorNode.transform.SetParent(dungeonGenerator.CorridorsParent.transform);

                corridorNode.BottomLeftAreaCorner = corridorList[i].BottomLeftAreaCorner;
                corridorNode.BottomRightAreaCorner = corridorList[i].BottomRightAreaCorner;
                corridorNode.TopRightAreaCorner = corridorList[i].TopRightAreaCorner;
                corridorNode.TopLeftAreaCorner = corridorList[i].TopLeftAreaCorner;

                corridorList[i] = corridorNode;

                corridorNode.Orientation = Orientations[i];

                dungeonFloor.name = "Corridor ";

                dungeonGenerator.Corridors.Add(corridorNode);
            }
        }

        return corridorNode;
    }

    public List<PassageOrientation> GetPassageSides(RoomNode node)
    {
        List<PassageOrientation> passageSides = new List<PassageOrientation>();

        // foreach (var VARIABLE in COLLECTION)
        // {
        //     
        // }

        return passageSides;
    }

    public List<CorridorNode> CreateCorridor(List<Node> allNodesCollection, int corridorWidth)
    {
        Orientations = new List<Orientation>();

        List<CorridorNode> corridorList = new List<CorridorNode>();
        Queue<Node> structuresToCheck =
            new Queue<Node>(
                allNodesCollection
                    .OrderByDescending(node => node.TreeLayerIndex)
                    .ToList()
            );

        while (structuresToCheck.Count > 0)
        {
            RoomNode node = (RoomNode)structuresToCheck.Dequeue();

            if (node.ChildrenNodeList.Count == 0) continue;

            CorridorNode corridor = new CorridorNode(
                node.ChildrenNodeList[0],
                node.ChildrenNodeList[1],
                corridorWidth);

            GenerateCorridor(corridor);

            corridorList.Add(corridor);
        }

        return corridorList;
    }

    private void GenerateCorridor(CorridorNode corridor)
    {
        RelativePosition relativePosition = corridor.CheckPositionStructure2AgainstStructure1();

        switch (relativePosition)
        {
            case RelativePosition.Up:
                corridor.ProcessRoomInRelationUpOrDown(corridor.Structure1, corridor.Structure2);
                break;
            case RelativePosition.Down:
                corridor.ProcessRoomInRelationUpOrDown(corridor.Structure2, corridor.Structure1);
                break;
            case RelativePosition.Right:
                corridor.ProcessRoomInRelationRightOrLeft(corridor.Structure1, corridor.Structure2);
                break;
            case RelativePosition.Left:
                corridor.ProcessRoomInRelationRightOrLeft(corridor.Structure2, corridor.Structure1);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        corridor.SetOrientationForDoor(relativePosition);
        Orientations.Add(corridor.Orientation);
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