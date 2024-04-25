public static class DungeonEnums
{
    public enum Orientation
    {
        Horizontal = 0,
        Vertical = 1
    }
    
    public enum RelativePosition
    {
        Up,
        Down,
        Right,
        Left
    }
    
    public enum RoomType
    {
        Default,
        SafeRoom,
        MonsterRoom,
        BossRoom,
        TreasureRoom,
        MerchantsRoom
    }
}