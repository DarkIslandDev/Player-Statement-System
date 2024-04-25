using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ObjectCreator
{
    public static GameObject CreateMesh(Vector2 bottomLeftCorner, Vector2 topRightCorner, Material meshMaterial)
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
        dungeonFloor.GetComponent<MeshRenderer>().material = meshMaterial;

        BoxCollider boxCollider = dungeonFloor.AddComponent<BoxCollider>();
        boxCollider.size = new Vector3(mesh.bounds.size.x, 5, mesh.bounds.size.z);
        boxCollider.center = new Vector3(mesh.bounds.center.x, 2.5f, mesh.bounds.center.z);

        boxCollider.isTrigger = true;

        dungeonFloor.tag = "Ground";

        return dungeonFloor;
    }

    public static void CreateDoorInCorridor(List<CorridorNode> corridors, GameObject doorPrefab)
    {
        foreach (CorridorNode corridor in corridors)
        {
            CreateObject(
                doorPrefab,
                corridor.gameObject,
                corridor.NodeBounds.Position,
                doorPrefab.transform.localRotation = corridor.Orientation == DungeonEnums.Orientation.Vertical
                    ? Quaternion.Euler(0, 90, 0)
                    : Quaternion.identity);
        }
    }

    // public static void CreateWalls(List<Node> nodes, GameObject wallH, GameObject wallV, List<Vector3> posWallHPos,
    //     List<Vector3> posWallVPos)
    // {
    //     Dictionary<Vector3, GameObject> wallPositionToGameObject = new Dictionary<Vector3, GameObject>();

    //     foreach (Node node in nodes)
    //     {
    //         foreach (GameObject wall in node.Walls)
    //         {
    //             Vector3 wallPosition = wall.transform.position;
    //             wallPositionToGameObject[wallPosition] = wall;
    //         }
    //     }

    //     CreateWallsOfType(wallH, posWallHPos, wallPositionToGameObject);
    //     CreateWallsOfType(wallV, posWallVPos, wallPositionToGameObject);

    //     foreach (KeyValuePair<Vector3, GameObject> pair in wallPositionToGameObject)
    //     {
    //         pair.Value.tag = "Wall";
    //     }

    //     FindEmptyChildObjects(nodes);
    // }

    // private static void CreateWallsOfType(GameObject wall, List<Vector3> posWallPos,Dictionary<Vector3, GameObject> wallPositionToGameObject)
    // {
    //     foreach (Vector3 wallPosition in posWallPos)
    //     {
    //         if (wallPositionToGameObject.TryGetValue(wallPosition, out GameObject value))
    //         {
    //             if (value == null)
    //             {
    //                 CreateWall(wall, null, wallPosition);
    //             }
    //         }
    //         else
    //         {
    //             CreateWall(wall, null, wallPosition);
    //         }
    //     }
    // }

    // public static GameObject CreateWall(GameObject wall, GameObject wallParent, Vector3 wallPos)
    // {
    //     GameObject wallGO = CreateObject(wall, wallParent, wallPos, Quaternion.identity);

    //     return wallGO;
    // }

    public static void CreateWalls(List<Node> nodes, GameObject wallH, GameObject wallV, List<Vector3> posWallHPos, 
        List<Vector3> posWallVPos)
    {
        Dictionary<Vector3, GameObject> wallPositionToGameObject = new Dictionary<Vector3, GameObject>();

        List<RoomNode> roomNodes = new List<RoomNode>();

        foreach (Node node in nodes)
        {
            foreach (GameObject wall in node.Walls)
            {
                Vector3 wallPosition = wall.transform.position;
                wallPositionToGameObject[wallPosition] = wall;
            }
        }

        CreateWallsOfType(wallH, posWallHPos, wallPositionToGameObject);
        CreateWallsOfType(wallV, posWallVPos, wallPositionToGameObject);


        FindEmptyChildObjects(nodes);
    }

    private static void CreateWallsOfType(GameObject wall, List<Vector3> posWallPos, 
        Dictionary<Vector3, GameObject> wallPositionToGameObject)
    {
        foreach (Vector3 wallPosition in posWallPos)
        {
            if (wallPositionToGameObject.TryGetValue(wallPosition, out GameObject value))
            {
                CreateWall(wall, value, wallPosition);
                value.tag = "Wall";
            }
        }
    }

    public static GameObject CreateWall(GameObject wall, GameObject wallParent, Vector3 wallPos)
    {
        GameObject wallGO = CreateObject(wall, wallParent, wallPos, Quaternion.identity);

        return wallGO;
    }

    public static GameObject CreateWallsParentObject(Transform parent, string objectName)
    {
        GameObject wallsParentObject = new GameObject(objectName);
        wallsParentObject.transform.SetParent(parent);

        return wallsParentObject;
    }

    private static void FindEmptyChildObjects(List<Node> rooms)
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            foreach (Transform child in rooms[i].TopWalls.transform)
            {
                int j = child.Cast<Transform>().Count(grandChild => grandChild.childCount == 0);

                if (j == 6)
                {
                    rooms[i].DoorTopSide = true;
                }
            }
        }
    }

    public static void CreatePillars(List<RoomNode> rooms, GameObject[] pillarPrefabs)
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

                int randomPillar = Random.Range(0, pillarPrefabs.Length);

                for (int i = 0; i < positions.Count; i++)
                {
                    GameObject pillar = CreateObject(
                        pillarPrefabs[randomPillar],
                        room.gameObject,
                        positions[i],
                        Quaternion.identity);
                }
            }
        }
    }

    public static void CreateTorches(List<RoomNode> rooms, GameObject torchPrefab)
    {
        foreach (RoomNode room in rooms)
        {
            List<bool> roomsPassage = new List<bool>()
            {
                room.DoorTopSide,
                room.DoorBottomSide,
                room.DoorRightSide,
                room.DoorLeftSide
            };

            Vector3 topCenter = new Vector3(room.TopCenterArea.x, 1, room.TopCenterArea.y);
            Vector3 bottomCenter = new Vector3(room.BottomCenterArea.x, 1, room.BottomCenterArea.y);
            Vector3 rightCenter = new Vector3(room.RightCenterArea.x, 1, room.RightCenterArea.y);
            Vector3 leftCenter = new Vector3(room.LeftCenterArea.x, 1, room.LeftCenterArea.y);

            List<Vector3> torchesPosition = new List<Vector3>()
            {
                topCenter,
                bottomCenter,
                rightCenter,
                leftCenter
            };

            List<Quaternion> rotationAngles = new List<Quaternion>()
            {
                Quaternion.Euler(0, 180, 0),
                Quaternion.Euler(0, 0, 0),
                Quaternion.Euler(0, -90, 0),
                Quaternion.Euler(0, 90, 0)
            };

            for (int i = 0; i < roomsPassage.Count; i++)
            {
                if (roomsPassage[i] == false)
                {
                    GameObject torch = CreateObject(
                        torchPrefab,
                        room.TorchParent,
                        torchesPosition[i],
                        rotationAngles[i]);
                }
            }
        }
    }

    public static void CreateDecoration(Node node, Decoration decoration)
    {
        GameObject prefab = decoration.prefab;

        Vector3 position = node.NodeBounds.Position;
        position += (Vector3)Random.insideUnitCircle * 10;
        position.y = 0;

        Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        rotation.x = 0;
        rotation.z = 0;

        GameObject decor = CreateObject(prefab, node.gameObject, position, rotation);
        decor.transform.localPosition = position;
    }

    public static GameObject CreateObject(GameObject prefab, GameObject parent, Vector3 position, Quaternion rotation)
    {
        GameObject createdObject = Instantiate(prefab, position, rotation, parent.transform);

        return createdObject;
    }

    // Rewriting a method from Unity code
    private static T Instantiate<T>(
        T original,
        Vector3 position,
        Quaternion rotation,
        Transform parent)
        where T : Object
    {
        return (T)Object.Instantiate((Object)original, position, rotation, parent);
    }
}