public struct Enums {
    
    public enum roomSize
    {
        Tiny = 10,
        Small = 20,
        Medium = 30,
        Big = 40,
        Large = 50
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
