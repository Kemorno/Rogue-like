using System.Collections.Generic;
using System;
using UnityEngine;

namespace Resources
{
    public class Room
    {
        public int RoomID;
        public Tile CenterTile = null;
        public List<Tile> FloorTiles = new List<Tile>();
        public List<Tile> WallTiles = new List<Tile>();

        public Room(int _RoomID)
        {
            RoomID = _RoomID;
        }

        #region Operators
        public static bool operator==(Room obj1, Room obj2)
        {
            return obj1.Equals(obj2);
        }
        public static bool operator!=(Room obj1, Room obj2)
        {
            return !obj1.Equals(obj2);
        }
        public override bool Equals(object other)
        {
            if (other == null) return false;
            Room obj = other as Room;

            return (RoomID == obj.RoomID
                && FloorTiles == obj.FloorTiles
                && WallTiles == obj.WallTiles
                && CenterTile == obj.CenterTile);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                return (FloorTiles.GetHashCode() * 17 + WallTiles.GetHashCode());
            }
        }
        public override string ToString()
        {
            return ("ID:" + RoomID + " ###Total Tile Count:" + (FloorTiles.Count+ WallTiles.Count) +
                "\nFloor Tile Count:" + FloorTiles.Count +
                " ###Wall Tile Count:" + WallTiles.Count);
        }
        #endregion
    }
    public class Tile
    {
        public int RoomId = -1;
        public CoordInt Coord;
        public Enums.tileType Type = Enums.tileType.Wall;
        public Enums.roomClass Class = Enums.roomClass.Neutral;
        public bool walkable = false;

        #region Constructors
        public Tile(CoordInt _Coord)
        {
            Coord = _Coord;
        }
        public Tile(CoordInt _Coord, Enums.tileType _Type)
        {
            Coord = _Coord;
            Type = _Type;
            walkable = (_Type == Enums.tileType.Floor) ? true : false;
        }
        #endregion

        public string Info()
        {
            return "Tile from Room #" + RoomId + " at Coords " + Coord.ToString() + " is a " + Type.ToString() + " and belongs to the " + Class.ToString() + " Class." + ((walkable) ? " You CAN walk on it" : " You CANNOT walk on it");
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
            return new Tuple<int, int>(x, y).GetHashCode();
        }
        public override bool Equals(object other)
        {
            if (other == null) return false;

            CoordInt obj = other as CoordInt;

            return (X == obj.X
                && Y == obj.Y);
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
            return new Tuple<int, int>(x, y);
        }

        #region Operators
        public static bool operator==(CoordInt obj1, CoordInt obj2)
        {
            return (obj1.Equals(obj2));
        }
        public static bool operator!=(CoordInt obj1, CoordInt obj2)
        {
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
}