using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CorridorGenerator : MonoBehaviour
{
    public List<Orientation> Orientations { get; private set; }

    public List<CorridorNode> CreateCorridor(List<RoomNode> allNodesCollection, int corridorWidth)
    {
        Orientations = new List<Orientation>();
        
        List<CorridorNode> corridorList = new List<CorridorNode>();
        Queue<RoomNode> structuresToCheck = 
            new Queue<RoomNode>(
                allNodesCollection
                    .OrderByDescending(node => node.TreeLayerIndex)
                    .ToList()
                );

        while (structuresToCheck.Count > 0)
        {
            RoomNode node = structuresToCheck.Dequeue();
            
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
}
