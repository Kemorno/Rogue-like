using System.Collections.Generic;
using System;
using UnityEngine;
using Enums;

namespace Resources
{
    public class Room
    {
        public int RoomId { get; private set; }
        public RoomSettings Settings { get; private set; }
        public Rect Bound { get; private set; }
        public Tile CenterTile { get; private set; }
        public List<Tile> FloorTiles { get; private set; }= new List<Tile>();
        public List<Tile> WallTiles { get; private set; }= new List<Tile>();
        public List<Tile> Tiles { get; private set; } = new List<Tile>();
        public Color Color { get; private set; }

        #region Constructors
        public Room(int _RoomId, RoomSettings _RoomSettings)
        {
            RoomId = _RoomId;
            Settings = _RoomSettings;
        }
        public Room(Room room)
        {
            RoomId = room.RoomId;
            Settings = room.Settings;
            Bound = room.Bound;
            CenterTile = room.CenterTile;
            FloorTiles = room.FloorTiles;
            WallTiles = room.WallTiles;
            Tiles = room.Tiles;
        }
        #endregion

        #region Methods
        void SetBounds()
        {
            CoordInt Min = new CoordInt(CenterTile.Coord.x, CenterTile.Coord.y);
            CoordInt Max = new CoordInt(CenterTile.Coord.x, CenterTile.Coord.y);

            foreach(Tile tile in FloorTiles)
            {
                if (tile.Coord.x < Min.x)
                    Min.x = tile.Coord.x;
                if (tile.Coord.y < Min.y)
                    Min.y = tile.Coord.y;
                if (tile.Coord.x > Max.x)
                    Max.x = tile.Coord.x;
                if (tile.Coord.y > Max.y)
                    Max.y = tile.Coord.y;
            }
            Bound = new Rect(Min.GetVector2Int(), (Max - Min).GetVector2Int());
        }
        public void SetCenterTile(Tile _CenterTile)
        {
            CenterTile = _CenterTile;
        }
        public void SetFloorTiles(List<Tile> _FloorTiles)
        {
            FloorTiles = _FloorTiles;
        }
        public void SetWallTiles(List<Tile> _WallTiles)
        {
            WallTiles = _WallTiles;
        }
        public void FinishRoom()
        {
            Tiles.AddRange(FloorTiles);
            Tiles.AddRange(WallTiles);
            SetBounds();
            SetColor();
        }
        public void SetColor()
        {
            System.Random prng = new System.Random(Settings.Seed.GetHashCode());
            Color = new Color32((byte)prng.Next(0, 255), (byte)prng.Next(0, 255), (byte)prng.Next(0, 255), 255 / 2);
        }
        public bool isFinished()
        {
            return Tiles.Count > 0 && Color != null && Bound !=null;
        }
        #endregion

        #region overrides
        public override string ToString()
        {
                return "RoomID#"+ RoomId + " Bounds:" + Bound.ToString();
        }
        public override int GetHashCode()
        {
            unchecked
            {
                return RoomId << 8 + Settings.GetHashCode() + Bound.GetHashCode() + CenterTile.GetHashCode() << 2 + ((FloorTiles.GetHashCode() + WallTiles.GetHashCode()) - Tiles.GetHashCode());
            }
        }
        #endregion

        #region Conversion
        public static implicit operator CoordInt(Room other)
        {
            return other.CenterTile.Coord;
        }
        public static implicit operator Tile(Room other)
        {
            return other.CenterTile;
        }
        #endregion
    }
    public class RoomSettings
    {
        public string Seed { get; private set; }
        public roomSize Size { get; private set; }
        public roomType Type { get; private set; }
        public roomClass Class { get; private set; }
        public int SmoothingMultiplier { get; private set; }
        public int RandomFillPercent { get; private set; }
        public int ComparisonFactor { get; private set; }

        #region Constructors
        public RoomSettings(int _SmoothingMultiplier, int _RandomFillPercent, int _ComparisonFactor, roomSize _Size, roomType _Type, roomClass _Class)
        {
            SmoothingMultiplier = _SmoothingMultiplier;
            RandomFillPercent = _RandomFillPercent;
            ComparisonFactor = _ComparisonFactor;
            Size = _Size;
            Type = _Type;
            Class = _Class;
        }
        public RoomSettings(RoomSettings settings)
        {
            Seed = settings.Seed;
            Size = settings.Size;
            Type = settings.Type;
            Class = settings.Class;
            SmoothingMultiplier = settings.SmoothingMultiplier;
            RandomFillPercent = settings.RandomFillPercent;
            ComparisonFactor = settings.ComparisonFactor;
        }
        #endregion
        
        #region Methods
        public void SetSeed(string _Seed)
        {
            Seed = _Seed;
        }
        #endregion

        #region overrides
        public override string ToString()
        {
            return "Seed"+ '"' + Seed+ '"' + " Size:" + Size.ToString() + " Type:" + Type.ToString() + " Class:" + Class.ToString() + " Smoothing Multiplier#" + SmoothingMultiplier + " Random Fill Percentage#" + RandomFillPercent + " Comparison Factor#" + ComparisonFactor;
        }
        public override int GetHashCode()
        {
            unchecked
            {
                return Seed.GetHashCode() * 17 + Size.ToString().GetHashCode() + Type.ToString().GetHashCode() + Class.ToString().GetHashCode() + SmoothingMultiplier * 13 + RandomFillPercent << 4 + ComparisonFactor;
            }
        }
        #endregion
    }
    public class Tile
    {
        public int RoomId = -1;
        public CoordInt Coord;
        public tileType Type { get; private set; } = tileType.Wall;
        public roomClass Class = roomClass.Neutral;
        public bool walkable { get; private set; }

        #region Constructors
        public Tile(Tile _Tile)
        {
            RoomId = _Tile.RoomId;
            Coord = _Tile.Coord;
            Type = _Tile.Type;
            Class = _Tile.Class;
            walkable = _Tile.walkable;
        }
        public Tile(CoordInt _Coord)
        {
            Coord = _Coord;
        }
        public Tile(CoordInt _Coord, tileType _Type)
        {
            Coord = _Coord;
            Type = _Type;
            walkable = (Type == tileType.Floor) ? true : false;
        }
        #endregion

        #region Methods
        public void SetType(tileType _type)
        {
            Type = _type;
            walkable = (Type == tileType.Floor) ? true : false;
        }
        public string ToLongString()
        {
            return ("Tile at " + Coord.GetVector2Int().ToString() + " from room " + RoomId + " is a " + Type.ToString() + " and is " + Class.ToString() + "\nYou " + ((walkable) ? "CAN walk in it" : "CANNOT walk in it"));
        }
        public bool IsValid()
        {
            if (Coord == null || RoomId < -1)
                return false;
            return true;
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return "RoomID#" + RoomId + " Coord" + Coord.GetVector2Int().ToString() + " Type:" + Type.ToString() + " Class:" + Class.ToString() + " isWalkable?" + walkable;
        }
        public override int GetHashCode()
        {
            return RoomId << 3 + Coord.GetHashCode() + Type.ToString().GetHashCode() * 23 + Class.ToString().GetHashCode() * 17 + walkable.GetHashCode() << 4;
        }
        #endregion

        #region Conversion
        public static implicit operator CoordInt(Tile other)
        {
            return other.Coord;
        }
        #endregion
    }
    public class CoordInt
    {
        public int x { get; private set; }
        public int y { get; private set; }
        public Tile tile { get; private set; }

        #region Constructors
        public CoordInt(int _x, int _y)
        {
            x = _x;
            y = _y;
        }
        public CoordInt(int _x, int _y, Tile _tile)
        {
            x = _x;
            y = _y;
            tile = _tile;
        }
        public CoordInt(CoordInt coord)
        {
            x = coord.x;
            y = coord.y;
            tile = coord.tile;
        }
        public CoordInt(CoordInt coord, Tile _tile)
        {
            x = coord.x;
            y = coord.y;
            tile = _tile;
        }
        #endregion

        #region Methods
        public bool Equals(CoordInt other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return (x == other.x
                && y == other.y);
        }
        public Vector2Int GetVector2Int()
        {
            return new Vector2Int(x, y);
        }
        public Tuple<int, int> GetTuple()
        {
            return new Tuple<int, int>(x, y);
        }
        public bool isAdjacent(CoordInt other)
        {
            if (other == null)
                return false;
            return (x == other.x || y == other.y);
        }
        public bool HasATile()
        {
            return tile != null;
        }
        public bool HasATile(Dictionary<CoordInt, Tile> map)
        {
            if (!map.ContainsKey(this))
                return false;

            return map[this].IsValid();
        }
        public void SetTile(Tile _tile)
        {
            tile = _tile;
        }
        #endregion

        #region overrides
        public override string ToString()
        {
            return (x + "," + y);
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as CoordInt);
        }
        public override int GetHashCode()
        {
            return new Tuple<int, int>(x, y).GetHashCode();
        }
        #endregion

        #region Operators
        public static bool operator ==(CoordInt obj1, CoordInt obj2)
        {
            if (ReferenceEquals(null, obj2)) return false;
            if (ReferenceEquals(obj1, obj2)) return true;
            return (obj1.Equals(obj2));
        }
        public static bool operator !=(CoordInt obj1, CoordInt obj2)
        {
            if (ReferenceEquals(null, obj2)) return true;
            if (ReferenceEquals(obj1, obj2)) return false;
            return !(obj1.Equals(obj2));
        }
        public static bool operator <(CoordInt obj1, CoordInt obj2)
        {
            return (obj1.x < obj2.x && obj1.y < obj2.y);
        }
        public static bool operator >(CoordInt obj1, CoordInt obj2)
        {
            return (obj1.x > obj2.x && obj1.y > obj2.y);
        }
        public static bool operator <=(CoordInt obj1, CoordInt obj2)
        {
            return (obj1.x <= obj2.x && obj1.y <= obj2.y);
        }
        public static bool operator >=(CoordInt obj1, CoordInt obj2)
        {
            return (obj1.x >= obj2.x && obj1.y >= obj2.y);
        }
        public static CoordInt operator +(CoordInt obj1, CoordInt obj2)
        {
            return new CoordInt(obj1.x + obj2.x, obj1.y + obj2.y);
        }
        public static CoordInt operator -(CoordInt obj1, CoordInt obj2)
        {
            return new CoordInt(obj1.x - obj2.x, obj1.y - obj2.y);
        }
        public static CoordInt operator *(CoordInt obj1, CoordInt obj2)
        {
            return new CoordInt(obj1.x * obj2.x, obj1.y * obj2.y);
        }
        public static CoordInt operator /(CoordInt obj1, CoordInt obj2)
        {
            return new CoordInt(obj1.x / obj2.x, obj1.y / obj2.y);
        }
        #endregion

        #region Conversion
        public static implicit operator CoordInt(Vector3 other)
        {
            return new CoordInt(Mathf.FloorToInt(other.x), Mathf.FloorToInt(other.y));
        }
        public static implicit operator CoordInt(Vector3Int other)
        {
            return new CoordInt(other.x, other.y);
        }
        public static implicit operator CoordInt(Vector2 other)
        {
            return new CoordInt(Mathf.FloorToInt(other.x), Mathf.FloorToInt(other.y));
        }
        public static implicit operator CoordInt(Vector2Int other)
        {
            return new CoordInt(other.x, other.y);
        }
        public static implicit operator Vector2(CoordInt other)
        {
            return new Vector2(other.x, other.y);
        }
        public static implicit operator Vector2Int(CoordInt other)
        {
            return new Vector2Int(other.x, other.y);
        }
        public static implicit operator Vector3(CoordInt other)
        {
            return new Vector3(other.x, other.y);
        }
        public static implicit operator Vector3Int(CoordInt other)
        {
            return new Vector3Int(other.x, other.y);
        }
        #endregion
    }
}
namespace Enums
{
    public enum roomSize
    {
        Tiny = 5,
        Small = 10,
        Medium = 15,
        Big = 20,
        Large = 25
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
    public enum OverlayType
    {
        Tile,
        RoomTiles,
        RoomClass,
        RoomSize,
        RoomType
    }
}