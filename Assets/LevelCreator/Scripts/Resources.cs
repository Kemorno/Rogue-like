using UnityEngine;
namespace Resources
{
    public class Tuple<T1, T2>
    {
        public T1 First { get; private set; }
        public T2 Second { get; private set; }

        public Tuple(T1 _First, T2 _Second)
        {
            First = _First;
            Second = _Second;
        }
    }
    public static class Tuple
    {
        public static Tuple<T1, T2> New<T1, T2>(T1 first, T2 second)
        {
            var tuple = new Tuple<T1, T2>(first, second);
            return tuple;
        }
    }

    public class Tile
    {
        public int RoomId = -1;
        public Vector2Int Coord = Vector2Int.zero;
        public Enums.tileType Type = Enums.tileType.Wall;
        public Enums.roomClass Class = Enums.roomClass.Neutral;
        public bool walkable = false;

        public Tile(Vector2Int _Coord)
        {
            Coord = _Coord;
        }
        public Tile(Vector2Int _Coord, Enums.tileType _Type)
        {
            Coord = _Coord;
            Type = _Type;
            walkable = (_Type == Enums.tileType.Floor) ? true : false;
        }

        public string Info()
        {
            return "Tile from Room #" + RoomId + " at Coords " + Coord.ToString() + " is a " + Type.ToString() + " and belongs to the " + Class.ToString() + " Class." + ((walkable) ? " You CAN walk on it" : " You CANNOT walk on it");
        }
    }
}