using UnityEngine;

public class Line
{
    private DungeonEnums.Orientation orientation;
    private Vector2Int coordinates;

    public Line(DungeonEnums.Orientation orientation, Vector2Int coordinates)
    {
        this.orientation = orientation;
        this.coordinates = coordinates;
    }

    public DungeonEnums.Orientation Orientation { get => orientation; set => orientation = value; }

    public Vector2Int Coordinates { get => coordinates; set => coordinates = value; }
}