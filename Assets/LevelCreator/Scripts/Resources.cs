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
        public Bound Bound { get; private set; }
        public Tile CenterTile { get; private set; }
        public List<Tile> FloorTiles { get; private set; }
        public List<Tile> WallTiles { get; private set; }
        public List<Tile> Tiles { get; private set; }

        public Room(int _RoomId, RoomSettings _RoomSettings)
        {
            RoomId = _RoomId;
            Settings = _RoomSettings;
            
            Bound = null;
            CenterTile = null;
            FloorTiles = new List<Tile>();
            WallTiles = new List<Tile>();
            Tiles = new List<Tile>();
        }

        public void SetBounds(Bound _Bound)
        {
            Bound = _Bound;
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
        public void SetTiles()
        {
            Tiles.AddRange(FloorTiles);
            Tiles.AddRange(WallTiles);
        }

        #region overrides
        public override string ToString()
        {
                return ("ID:" + RoomId + " ###Total Tile Count:" + (FloorTiles.Count+ WallTiles.Count) +
                    "\nFloor Tile Count:" + FloorTiles.Count +
                    " ###Wall Tile Count:" + WallTiles.Count);

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

        public RoomSettings(int _SmoothingMultiplier, int _RandomFillPercent, int _ComparisonFactor, roomSize _Size, roomType _Type, roomClass _Class)
        {
            SmoothingMultiplier = _SmoothingMultiplier;
            RandomFillPercent = _RandomFillPercent;
            ComparisonFactor = _ComparisonFactor;
            Size = _Size;
            Type = _Type;
            Class = _Class;
        }
        public void SetSeed(string _Seed)
        {
            Seed = _Seed;
        }

        #region overrides
        #endregion
    }
    public class Tile
    {
        public int RoomId = -1;
        public CoordInt Coord;
        public tileType Type = tileType.Wall;
        public roomClass Class = roomClass.Neutral;
        public bool walkable = false;

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
            walkable = (_Type == tileType.Floor) ? true : false;
        }
        #endregion

        #region Overrides
        #endregion

        public string ToLongString()
        {
            return ("Tile at " + Coord.GetVector2Int().ToString() + " from room " + RoomId + " is a " + Type.ToString() + " and is " + Class.ToString() + "\nYou " + ((walkable) ? "CAN walk in it" : "CANNOT walk in it"));
        }
    }
    public class CoordInt
    {
        public int X;
        public int Y;

        #region Constructors
        public CoordInt(int x, int y)
        {
            X = x;
            Y = y;
        }
        #endregion

        public override int GetHashCode()
        {
            return new Tuple<int, int>(X, Y).GetHashCode();
        }
        public bool Equals(CoordInt other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return (X == other.X
                && Y == other.Y);
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals(obj as CoordInt);
        }
        public Vector2Int GetVector2Int()
        {
            return new Vector2Int(X, Y);
        }
        public override string ToString()
        {
            return (X + "," + Y);
        }
        public Tuple<int, int> GetTuple()
        {
            return new Tuple<int, int>(X, Y);
        }
        public bool isAdjacent(CoordInt other)
        {
            if (other == null)
                return false;
            return (X == other.X || Y == other.Y);
        }

        #region Operators
        public static bool operator==(CoordInt obj1, CoordInt obj2)
        {
            if (ReferenceEquals(null, obj2)) return false;
            if (ReferenceEquals(obj1, obj2)) return true;
            return (obj1.Equals(obj2));
        }
        public static bool operator!=(CoordInt obj1, CoordInt obj2)
        {
            if (ReferenceEquals(null, obj2)) return true;
            if (ReferenceEquals(obj1, obj2)) return false;
            return !(obj1.Equals(obj2));
        }
        public static bool operator<(CoordInt obj1, CoordInt obj2)
        {
            return (obj1.X < obj2.X && obj1.Y < obj2.Y);
        }
        public static bool operator >(CoordInt obj1, CoordInt obj2)
        {
            return (obj1.X > obj2.X && obj1.Y > obj2.Y);
        }
        public static bool operator <=(CoordInt obj1, CoordInt obj2)
        {
            return (obj1.X <= obj2.X && obj1.Y <= obj2.Y);
        }
        public static bool operator >=(CoordInt obj1, CoordInt obj2)
        {
            return (obj1.X >= obj2.X && obj1.Y >= obj2.Y);
        }
        public static CoordInt operator+(CoordInt obj1, CoordInt obj2)
        {
            return new CoordInt(obj1.X + obj2.X, obj1.Y + obj2.Y);
        }
        public static CoordInt operator-(CoordInt obj1, CoordInt obj2)
        {
            return new CoordInt(obj1.X - obj2.X, obj1.Y - obj2.Y);
        }
        public static CoordInt operator*(CoordInt obj1, CoordInt obj2)
        {
            return new CoordInt(obj1.X * obj2.X, obj1.Y * obj2.Y);
        }
        public static CoordInt operator/(CoordInt obj1, CoordInt obj2)
        {
            return new CoordInt(obj1.X / obj2.X, obj1.Y / obj2.Y);
        }
        #endregion
    }
    public class Bound
    {
        public Vector2 Min { get; private set; }
        public Vector2 Max { get; private set; }

        public Bound(Vector2 min, Vector2 max)
        {
            Min = min;
            Max = max;
        }

        #region overrides
        public static bool operator ==(Bound obj1, Bound obj2)
        {
            return obj1.Equals(obj2);
        }
        public static bool operator !=(Bound obj1, Bound obj2)
        {
            return !obj1.Equals(obj2);
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            Bound other = obj as Bound;
                return (Min == other.Min
                    && Max == other.Max);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                return Min.GetHashCode() * 353 + Max.GetHashCode();
            }
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