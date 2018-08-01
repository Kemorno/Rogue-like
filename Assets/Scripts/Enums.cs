public struct Enums {
    
    public enum roomSize
    {
        Tiny = 11,
        Small = 21,
        Medium = 31,
        Big = 41,
        Large = 51
    }
    public enum roomClass
    {
        Neutral,
        Scientific,
        Spiritual,
        Physical,
        Social
    }
    public enum roomType
    {
        None,
        Spawn,
        Enemy,
        Secret,
        Boss,
        MiniBoss,
        Shop
    }
    public enum tileType
    {
        Floor,
        Wall
    }
    public enum mapShape
    {
        Square,
        Rectangular,
        Hexagon,
        Diamond,
        Circle
    }
    public enum OverlayType {
        Tile,
        RoomTiles,
        RoomClass,
        RoomSize,
        RoomType
    }
}
