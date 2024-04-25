using System.Collections.Generic;
using UnityEngine;

public static class StructureHelper
{
    public static List<Node> TraverseGraphToExtractLowestLeaves(Node parentNode)
    {
        Queue<Node> nodesToCheck = new Queue<Node>();
        List<Node> listToReturn = new List<Node>();

        if (parentNode.ChildrenNodeList.Count == 0)
        {
            listToReturn.Add(parentNode);
            return listToReturn;
        }

        foreach (Node child in parentNode.ChildrenNodeList)
        {
            nodesToCheck.Enqueue(child);
        }

        while (nodesToCheck.Count > 0)
        {
            Node currentNode = nodesToCheck.Dequeue();

            if (currentNode.ChildrenNodeList.Count == 0)
            {
                listToReturn.Add(currentNode);
            }
            else
            {
                foreach (Node child in currentNode.ChildrenNodeList)
                {
                    nodesToCheck.Enqueue(child);
                }
            }
        }

        return listToReturn;
    }

    public static Vector2Int GeneratePointBetween(Vector2Int leftPoint, Vector2Int rightPoint, float pointModifier, int offset)
    {
        int minX = leftPoint.x + offset;
        int maxX = rightPoint.x - offset;
        int minY = leftPoint.y + offset;
        int maxY = rightPoint.y - offset;

        int x = (int)(minX + (maxX - minX) * pointModifier);
        int y = (int)(minY + (maxY - minY) * pointModifier);

        return new Vector2Int(x, y);
    }

    public static Vector2Int CalculateMiddlePoint(Vector2Int v1, Vector2Int v2)
    {
        return new Vector2Int((v1.x + v2.x) / 2, (v1.y + v2.y) / 2);
    }
}