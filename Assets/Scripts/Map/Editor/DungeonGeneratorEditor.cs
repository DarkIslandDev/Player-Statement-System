using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DungeonGenerator), true)]
public class DungeonGeneratorEditor : Editor
{
    private DungeonGenerator generator;

    private void Awake()
    {
        generator = (DungeonGenerator)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        if (GUILayout.Button("Create dungeon"))
        {
            generator.CreateDungeon();
        }
        
        if (GUILayout.Button("Destroy dungeon"))
        {
            generator.ResetDungeon();
        }
    }
}