using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Generator : MonoBehaviour
{
    private List<RoomNode> allRoomsCollection = new List<RoomNode>();
    private List<RoomNode> roomList = new List<RoomNode>();
    private List<CorridorNode> corridorList = new List<CorridorNode>();
    private int dungeonWidth;
    private int dungeonLength;

    private DungeonGenerator dungeonGenerator;
    private RoomGenerator roomGenerator;
    private CorridorGenerator corridorGenerator;
    private BinarySpacePartitioner binarySpacePartitioner;

    public List<RoomNode> RoomList => roomList;
    public List<CorridorNode> CorridorList => corridorList;

    public void Init(RoomGenerator roomGenerator, CorridorGenerator corridorGenerator,
        BinarySpacePartitioner binarySpacePartitioner, int dungeonWidth, int dungeonLength)
    {
        this.roomGenerator = roomGenerator;
        this.corridorGenerator = corridorGenerator;
        this.binarySpacePartitioner = binarySpacePartitioner;
        
        this.dungeonWidth = dungeonWidth;
        this.dungeonLength = dungeonLength;
    }

    public List<Node> CalculateDungeon(int maxIterations, int roomWidthMin, int roomLengthMin, int corridorWidth,
        float roomBottomCornerModifier, float roomTopCornerModifier, int roomOffset)
    {
        roomList = new List<RoomNode>();
        corridorList = new List<CorridorNode>();

        binarySpacePartitioner.Init(dungeonWidth, dungeonLength);
        allRoomsCollection = binarySpacePartitioner.PrepareNodesCollection(maxIterations, roomWidthMin, roomLengthMin);
        List<Node> roomSpaces = StructureHelper.TraverseGraphToExtractLowestLeafes(binarySpacePartitioner.RoomNode);
        
        roomList = roomGenerator.GenerateRoomsInGivenSpaces(
            roomSpaces,
            roomBottomCornerModifier,
            roomTopCornerModifier,
            roomOffset);

        corridorList = corridorGenerator.CreateCorridor(allRoomsCollection, corridorWidth);

        return new List<Node>(roomList).Concat(corridorList).ToList();
    }
}