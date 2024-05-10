using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CorridorGenerator : MonoBehaviour
{
    [SerializeField] private DungeonGenerator dungeonGenerator;
    [SerializeField] private List<CorridorNode> corridorList = new List<CorridorNode>();

    private List<Vector3> possibleWallHorizontalPosition;
    private List<Vector3> possibleDoorHorizontalPosition;
    private List<Vector3> possibleWallVerticalPosition;
    private List<Vector3> possibleDoorVerticalPosition;

    public List<DungeonEnums.Orientation> Orientations { get; private set; } = new();

    public List<CorridorNode> CorridorList => corridorList;

    public void Init(DungeonGenerator g, List<Vector3> pwhp, List<Vector3> pdhp, List<Vector3> pwvp, List<Vector3> pdvp)
    {
        dungeonGenerator = g;
        possibleWallHorizontalPosition = pwhp;
        possibleWallVerticalPosition = pwvp;
        possibleDoorHorizontalPosition = pdhp;
        possibleDoorVerticalPosition = pdvp;
    }

    public void CalculateCorridor(List<Node> rooms, GameObject doorPrefab, int corridorWidth)
    {
        corridorList = new List<CorridorNode>();

        corridorList = CreateCorridor(rooms, corridorWidth);

        GenerateCorridors(corridorList);

        ObjectCreator.CreateDoorInCorridor(corridorList, doorPrefab);
    }

    public void GenerateCorridors(List<CorridorNode> nodes)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            GameObject meshGO = ObjectCreator.CreateMesh(
                nodes[i].BottomLeftAreaCorner,
                nodes[i].TopRightAreaCorner,
                dungeonGenerator.DungeonData.floorMaterial);

            MeshFilter meshFilter = meshGO.GetComponent<MeshFilter>();

            CalculateValuesFromNodes(
                nodes[i].TopRightAreaCorner,
                nodes[i].BottomLeftAreaCorner,
                meshGO,
                meshFilter.sharedMesh);
        }
    }

    private void CalculateValuesFromNodes(Vector2 topRight, Vector2 bottomLeft, GameObject dungeonFloor, Mesh mesh)
    {
        Node node = null;

        node = AddCorridorNodes(bottomLeft, dungeonFloor);

        if (node != null)
        {
            CalculateAdditionalValuesFromNode(topRight, bottomLeft, mesh, node);

            node.BoxCollider = dungeonFloor.GetComponent<BoxCollider>();
            dungeonFloor.name += $" | {node.NodeBounds.Position.x} / {node.NodeBounds.Position.z}";
            mesh.name = node.name;

            CreateWallsParentObjects(node);

            AddWallsPositionToFloor(topRight, bottomLeft, node);

            FindEmptyWallsObjects(node);
        }
    }

    private void CalculateAdditionalValuesFromNode(Vector2 topRight, Vector2 bottomLeft, Mesh mesh, Node node)
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
    }

    private void CreateWallsParentObjects(Node node)
    {
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

    public List<CorridorNode> CreateCorridor(List<Node> allNodesCollection, int corridorWidth)
    {
        Orientations = new List<DungeonEnums.Orientation>();

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
        DungeonEnums.RelativePosition relativePosition = corridor.CheckPositionStructure2AgainstStructure1();

        switch (relativePosition)
        {
            case DungeonEnums.RelativePosition.Up:
                corridor.ProcessRoomInRelationUpOrDown(corridor.Structure1, corridor.Structure2);
                break;
            case DungeonEnums.RelativePosition.Down:
                corridor.ProcessRoomInRelationUpOrDown(corridor.Structure2, corridor.Structure1);
                break;
            case DungeonEnums.RelativePosition.Right:
                corridor.ProcessRoomInRelationRightOrLeft(corridor.Structure1, corridor.Structure2);
                break;
            case DungeonEnums.RelativePosition.Left:
                corridor.ProcessRoomInRelationRightOrLeft(corridor.Structure2, corridor.Structure1);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        corridor.SetOrientationForDoor(relativePosition);
        Orientations.Add(corridor.Orientation);
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

    public void ResetCorridors()
    {
        corridorList.Clear();
    }
}