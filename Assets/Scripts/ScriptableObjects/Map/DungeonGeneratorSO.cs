using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Dungeon Data", fileName = "Dungeon Data", order = 0)]
public class DungeonGeneratorSO : ScriptableObject
{
    [Header("Values")]
    public int dungeonWidth;
    public int dungeonLength;
    public int roomWidthMin;
    public int roomLengthMin;
    public int maxIterations;
    public int corridorWidth;
    
    [Header("Modifiers")]
    [Range(0.0f, 0.3f)] public float roomBottomCornerModifier;
    [Range(0.7f, 1.0f)] public float roomTopCornerModifier;
    [Range(0, 2)] public int roomOffset;

    [Header("Base Dungeon Environment")]
    public GameObject horizontalWall;
    public GameObject verticalWall;
    public GameObject corridorDoor;
    public GameObject torch;

    [Header("Decorations")] 
    public List<Decoration> monsterRoomDecorations;
    public List<Decoration> treasureRoomDecorations;

    [Header("Pillars")]
    public GameObject[] pillars;

    [Header("Materials")]
    public Material floorMaterial;
}