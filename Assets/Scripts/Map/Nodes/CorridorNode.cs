using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class CorridorNode : Node
{
    [SerializeField] private DungeonEnums.Orientation orientation;

    private Node structure1;
    private Node structure2;
    private int corridorWidth;
    private int modifierDistanceFromWall = 1;

    public DungeonEnums.Orientation Orientation
    {
        get => orientation;
        set => orientation = value;
    }

    public Node Structure1 => structure1;
    public Node Structure2 => structure2;
    
    public CorridorNode(Node node1, Node node2, int corridorWidth) : base(null)
    {
        structure1 = node1;
        structure2 = node2;
        this.corridorWidth = corridorWidth;
    }

    public void SetOrientationForDoor(DungeonEnums.RelativePosition relativePosition)
    {
        orientation = relativePosition is DungeonEnums.RelativePosition.Down or DungeonEnums.RelativePosition.Up
            ? DungeonEnums.Orientation.Vertical
            : DungeonEnums.Orientation.Horizontal;
    }

    public void ProcessRoomInRelationRightOrLeft(Node structure1, Node structure2)
    {
        Node leftStructure;
        List<Node> leftStructureChildren = StructureHelper.TraverseGraphToExtractLowestLeaves(structure1);

        List<Node> rightStructureChildren = StructureHelper.TraverseGraphToExtractLowestLeaves(structure2);

        List<Node> sortedLeftStructure =
            leftStructureChildren.OrderByDescending(child => child.TopRightAreaCorner.x).ToList();

        if (sortedLeftStructure.Count == 1)
        {
            leftStructure = sortedLeftStructure[0];
        }
        else
        {
            int maxX = sortedLeftStructure[0].TopRightAreaCorner.x;
            sortedLeftStructure = sortedLeftStructure
                .Where(children => Math.Abs(maxX - children.TopRightAreaCorner.x) < 10).ToList();

            int index = Random.Range(0, sortedLeftStructure.Count);
            leftStructure = sortedLeftStructure[index];
        }

        List<Node> possibleNeighboursInRightStructureList = rightStructureChildren.Where(
            child => GetValidYForNeighbourLeftRight(
                leftStructure.TopRightAreaCorner,
                leftStructure.BottomRightAreaCorner,
                child.TopLeftAreaCorner,
                child.BottomLeftAreaCorner
            ) != -1
        ).OrderBy(child => child.BottomRightAreaCorner.x).ToList();

        Node rightStructure = possibleNeighboursInRightStructureList.Count <= 0
            ? structure2
            : possibleNeighboursInRightStructureList[0];

        int y = GetValidYForNeighbourLeftRight(
            leftStructure.TopLeftAreaCorner,
            leftStructure.BottomRightAreaCorner,
            rightStructure.TopLeftAreaCorner,
            rightStructure.BottomLeftAreaCorner);

        while (y == -1 && sortedLeftStructure.Count > 1)
        {
            sortedLeftStructure = sortedLeftStructure.Where(
                child => child.TopLeftAreaCorner.y != leftStructure.TopLeftAreaCorner.y).ToList();

            leftStructure = sortedLeftStructure[0];

            y = GetValidYForNeighbourLeftRight(
                leftStructure.TopLeftAreaCorner,
                leftStructure.BottomRightAreaCorner,
                rightStructure.TopLeftAreaCorner,
                rightStructure.BottomLeftAreaCorner);
        }
        
        // Test create middle point
        // int middleY = (leftStructure.BottomRightAreaCorner.y + rightStructure.TopLeftAreaCorner.y) / 2;
        //
        // BottomLeftAreaCorner = new Vector2Int(leftStructure.BottomRightAreaCorner.x , middleY);
        // TopRightAreaCorner = new Vector2Int(rightStructure.TopLeftAreaCorner.x, middleY + corridorWidth);

        // Test create from original values
        BottomLeftAreaCorner = new Vector2Int(leftStructure.BottomRightAreaCorner.x , y);
        TopRightAreaCorner = new Vector2Int(rightStructure.TopLeftAreaCorner.x, y + corridorWidth);
    }

    private int GetValidYForNeighbourLeftRight(Vector2Int leftNodeUp, Vector2Int leftNodeDown, Vector2Int rightNodeUp,
        Vector2Int rightNodeDown)
    {
        if (rightNodeUp.y >= leftNodeUp.y && leftNodeDown.y >= rightNodeDown.y)
        {
            return StructureHelper.CalculateMiddlePoint(
                leftNodeDown + new Vector2Int(0, modifierDistanceFromWall ),
                leftNodeUp - new Vector2Int(0, modifierDistanceFromWall + corridorWidth)
            ).y;
        }

        if (rightNodeUp.y <= leftNodeUp.y && leftNodeDown.y <= rightNodeDown.y)
        {
            return StructureHelper.CalculateMiddlePoint(
                rightNodeDown + new Vector2Int(0, modifierDistanceFromWall),
                rightNodeUp - new Vector2Int(0, modifierDistanceFromWall + corridorWidth)
            ).y;
        }

        if (leftNodeUp.y >= rightNodeDown.y && leftNodeUp.y <= rightNodeUp.y)
        {
            return StructureHelper.CalculateMiddlePoint(
                rightNodeDown + new Vector2Int(0, modifierDistanceFromWall),
                leftNodeUp - new Vector2Int(0, modifierDistanceFromWall)
            ).y;
        }

        if (leftNodeDown.y >= rightNodeDown.y && leftNodeDown.y <= rightNodeUp.y)
        {
            return StructureHelper.CalculateMiddlePoint(
                leftNodeDown + new Vector2Int(0, modifierDistanceFromWall),
                rightNodeUp - new Vector2Int(0, modifierDistanceFromWall + corridorWidth)
            ).y;
        }

        return -1;
    }

    public void ProcessRoomInRelationUpOrDown(Node structure1, Node structure2)
    {
        Node bottomStructure;
        List<Node> structureBottomChildren = StructureHelper.TraverseGraphToExtractLowestLeaves(structure1);

        List<Node> structureAboveChildren = StructureHelper.TraverseGraphToExtractLowestLeaves(structure2);

        List<Node> sortedBottomStructure =
            structureBottomChildren.OrderByDescending(child => child.TopRightAreaCorner.y).ToList();

        if (sortedBottomStructure.Count == 1)
        {
            bottomStructure = structureBottomChildren[0];
        }
        else
        {
            int maxY = sortedBottomStructure[0].TopLeftAreaCorner.y;
            sortedBottomStructure = sortedBottomStructure
                .Where(child => Mathf.Abs(maxY - child.TopLeftAreaCorner.y) < 10).ToList();

            int index = Random.Range(0, sortedBottomStructure.Count);
            bottomStructure = sortedBottomStructure[index];
        }

        List<Node> possibleNeighboursInTopStructure = structureAboveChildren.Where(
            child => GetValidXForNeighbourUpDown(
                         bottomStructure.TopLeftAreaCorner,
                         bottomStructure.TopRightAreaCorner,
                         child.BottomLeftAreaCorner,
                         child.BottomRightAreaCorner)
                     != -1).OrderBy(child => child.BottomRightAreaCorner.y).ToList();

        Node topStructure = possibleNeighboursInTopStructure.Count == 0
            ? structure2
            : possibleNeighboursInTopStructure[0];

        int x = GetValidXForNeighbourUpDown(
            bottomStructure.TopLeftAreaCorner,
            bottomStructure.TopRightAreaCorner,
            topStructure.BottomLeftAreaCorner,
            topStructure.BottomRightAreaCorner);

        while (x == -1 && sortedBottomStructure.Count > 1)
        {
            sortedBottomStructure = sortedBottomStructure
                .Where(child => child.TopLeftAreaCorner.x != topStructure.TopLeftAreaCorner.x).ToList();

            bottomStructure = sortedBottomStructure[0];

            x = GetValidXForNeighbourUpDown(
                bottomStructure.TopLeftAreaCorner,
                bottomStructure.TopRightAreaCorner,
                topStructure.BottomLeftAreaCorner,
                topStructure.BottomRightAreaCorner);
        }

        BottomLeftAreaCorner = new Vector2Int(x, bottomStructure.TopLeftAreaCorner.y);
        TopRightAreaCorner = new Vector2Int(x + corridorWidth, topStructure.BottomLeftAreaCorner.y);
    }

    private int GetValidXForNeighbourUpDown(Vector2Int bottomNodeLeft,
        Vector2Int bottomNodeRight, Vector2Int topNodeLeft, Vector2Int topNodeRight)
    {
        if (topNodeLeft.x < bottomNodeLeft.x && bottomNodeRight.x < topNodeRight.x)
        {
            return StructureHelper.CalculateMiddlePoint(
                bottomNodeLeft + new Vector2Int(modifierDistanceFromWall, 0),
                bottomNodeRight - new Vector2Int(corridorWidth + modifierDistanceFromWall, 0)
            ).x;
        }

        if (topNodeLeft.x >= bottomNodeLeft.x && bottomNodeRight.x >= topNodeRight.x)
        {
            return StructureHelper.CalculateMiddlePoint(
                topNodeLeft + new Vector2Int(modifierDistanceFromWall, 0),
                topNodeRight - new Vector2Int(corridorWidth + modifierDistanceFromWall, 0)
            ).x;
        }

        if (bottomNodeLeft.x >= (topNodeLeft.x) && bottomNodeLeft.x <= topNodeRight.x)
        {
            return StructureHelper.CalculateMiddlePoint(
                bottomNodeLeft + new Vector2Int(modifierDistanceFromWall, 0),
                topNodeRight - new Vector2Int(corridorWidth + modifierDistanceFromWall, 0)
            ).x;
        }

        if (bottomNodeRight.x <= topNodeRight.x && bottomNodeRight.x >= topNodeLeft.x)
        {
            return StructureHelper.CalculateMiddlePoint(
                topNodeLeft + new Vector2Int(modifierDistanceFromWall, 0),
                bottomNodeRight - new Vector2Int(corridorWidth + modifierDistanceFromWall, 0)
            ).x;
        }

        return -1;
    }

    public DungeonEnums.RelativePosition CheckPositionStructure2AgainstStructure1()
    {
        Vector2 middlePointStructure1Temp =
            ((Vector2)structure1.TopRightAreaCorner + structure1.BottomLeftAreaCorner) / 2;

        Vector2 middlePointStructure2Temp =
            ((Vector2)structure2.TopRightAreaCorner + structure2.BottomLeftAreaCorner) / 2;

        float angle = CalculateAngle(middlePointStructure1Temp, middlePointStructure2Temp);

        switch (angle)
        {
            case < 45 and >= 0:
            case > -45 and < 0:
                return DungeonEnums.RelativePosition.Right;
            case > 45 and < 135:
                return DungeonEnums.RelativePosition.Up;
            case > -135 and < -45:
                return DungeonEnums.RelativePosition.Down;
            default:
                return DungeonEnums.RelativePosition.Left;
        }
    }

    private float CalculateAngle(Vector2 middlePointStructure1Temp, Vector2 middlePointStructure2Temp)
    {
        return Mathf.Atan2(
            middlePointStructure2Temp.y - middlePointStructure1Temp.y,
            middlePointStructure2Temp.x - middlePointStructure1Temp.x) * Mathf.Rad2Deg;
    }
}