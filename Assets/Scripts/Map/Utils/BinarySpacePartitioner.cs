using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BinarySpacePartitioner : MonoBehaviour
{
    private RoomNode roomNode;

    public RoomNode RoomNode => roomNode;

    public void Init(int dungeonWidth, int dungeonHeight)
    {
        roomNode = new RoomNode(
            new Vector2Int(0, 0),
            new Vector2Int(dungeonWidth, dungeonHeight),
            null,
            0);
    }

    public List<RoomNode> PrepareRoomNodesCollection(int maxIterations, int roomWidthMin, int roomLengthMin)
    {
        Queue<RoomNode> graph = new Queue<RoomNode>();
        List<RoomNode> listToReturn = new List<RoomNode>();

        graph.Enqueue(roomNode);
        listToReturn.Add(roomNode);

        int iterations = 0;

        while (iterations < maxIterations && graph.Count > 0)
        {
            iterations++;
            RoomNode currentNode = graph.Dequeue();

            if (currentNode.Width >= roomWidthMin * 2 || currentNode.Length >= roomLengthMin * 2)
            {
                SplitTheSpace(currentNode, listToReturn, roomLengthMin, roomWidthMin, graph);
            }
        }

        return listToReturn;
    }

    private void SplitTheSpace(Node curNode, List<RoomNode> rooms, int lengthMin, int widthMin, Queue<RoomNode> graph)
    {
        Line line = GetLineDividingSpace(
            curNode.BottomLeftAreaCorner,
            curNode.TopRightAreaCorner,
            widthMin,
            lengthMin);

        RoomNode node1;
        RoomNode node2;

        if (line.Orientation == DungeonEnums.Orientation.Horizontal)
        {
            node1 = new RoomNode(
                curNode.BottomLeftAreaCorner,
                new Vector2Int(curNode.TopRightAreaCorner.x, line.Coordinates.y),
                curNode,
                curNode.TreeLayerIndex + 1);

            node2 = new RoomNode(
                new Vector2Int(curNode.BottomLeftAreaCorner.x, line.Coordinates.y),
                curNode.TopRightAreaCorner,
                curNode,
                curNode.TreeLayerIndex + 1);
        }
        else
        {
            node1 = new RoomNode(
                curNode.BottomLeftAreaCorner,
                new Vector2Int(line.Coordinates.x, curNode.TopRightAreaCorner.y),
                curNode,
                curNode.TreeLayerIndex + 1);

            node2 = new RoomNode(
                new Vector2Int(line.Coordinates.x, curNode.BottomLeftAreaCorner.y),
                curNode.TopRightAreaCorner,
                curNode,
                curNode.TreeLayerIndex + 1);
        }

        AddNewNodeToCollections(rooms, graph, node1);
        AddNewNodeToCollections(rooms, graph, node2);
    }

    private void AddNewNodeToCollections(List<RoomNode> listToReturn, Queue<RoomNode> graph, RoomNode node)
    {
        listToReturn.Add(node);
        graph.Enqueue(node);
    }

    private Line GetLineDividingSpace(Vector2Int blAreaCorner, Vector2Int trAreaCorner, int widthMin, int lengthMin)
    {
        DungeonEnums.Orientation orientation;

        bool lengthStatus = (trAreaCorner.y - blAreaCorner.y) >= 2 * lengthMin;
        bool widthStatus = (trAreaCorner.x - blAreaCorner.x) >= 2 * widthMin;

        if (lengthStatus && widthStatus)
        {
            orientation = (DungeonEnums.Orientation)(Random.Range(0, 2));
        }
        else if (widthStatus)
        {
            orientation = DungeonEnums.Orientation.Vertical;
        }
        else
        {
            orientation = DungeonEnums.Orientation.Horizontal;
        }

        return new Line(
            orientation,
            GetCoordinatesForOrientation(
                orientation,
                blAreaCorner,
                trAreaCorner,
                widthMin,
                lengthMin)
        );
    }

    private Vector2Int GetCoordinatesForOrientation(DungeonEnums.Orientation o, Vector2Int bl, Vector2Int tr, int widM, int lenM)
    {
        int x;
        int y;

        if (o == DungeonEnums.Orientation.Horizontal)
        {
            y = Random.Range(bl.y + lenM, tr.y - lenM);
            x = (bl.x + tr.x) / 2;
        }
        else
        {
            x = Random.Range(bl.x + widM, tr.x - widM);
            y = (bl.y + tr.y) / 2;
        }

        return new Vector2Int(x, y);
    }
}