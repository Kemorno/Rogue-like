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
        public Color Color { get; private set; }

        public List<Tile> FloorTiles { get; private set; } = new List<Tile>();
        public List<Tile> WallTiles { get; private set; } = new List<Tile>();
        public List<Tile> Tiles { get; private set; } = new List<Tile>();
        public Dictionary<CoordInt, Tile> Map { get; private set; } = new Dictionary<CoordInt, Tile>();
        public bool HasError { get; private set; } = false;
        public string ErrorMessage { get; private set; } = null;
        public List<string> ExtraInformation { get; private set; } = new List<string>();
        public bool Finished { get; private set; } = false;
        public GameObject roomGo { get; private set; } = null;
        public bool FinishedGeneration = false;
        public GameObject SpritesGrouper { get; private set; } = null;

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
            Color = room.Color;
            Map = room.Map;
            HasError = room.HasError;
            ErrorMessage = room.ErrorMessage;
            ExtraInformation = room.ExtraInformation;
            Finished = room.Finished;
            roomGo = room.roomGo;
            FinishedGeneration = room.FinishedGeneration;
        }
        #endregion
        #region Methods
        void SetBounds()
        {
            Vector2Int Min = new Vector2Int(CenterTile.Coord.x, CenterTile.Coord.y);
            Vector2Int Max = new Vector2Int(CenterTile.Coord.x, CenterTile.Coord.y);

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
            Bound = new Rect(Min, (Max - Min));
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
        public void SetMap(Dictionary<CoordInt, Tile> _map)
        {
            Map = _map;
        }
        public void resetMap()
        {
            Map = new Dictionary<CoordInt, Tile>();
        }
        public void SetGameObject(GameObject go)
        {
            roomGo = go;
        }
        public void SetError(string Message)
        {
            HasError = true;
            ErrorMessage = Message;
        }
        public void FinishRoom()
        {
            if (!Finished && !HasError)
            {
                Tiles.AddRange(FloorTiles);
                Tiles.AddRange(WallTiles);
                SetBounds();
                SetColor();

                Finished = true;
                return;
            }
            if (HasError)
                Debug.Log("Couldn't Finish Room\n" + ErrorMessage);
            return;
        }
        public void SetColor()
        {
            System.Random prng = new System.Random(Settings.Seed.GetHashCode());
            Color = new Color32((byte)prng.Next(0, 255), (byte)prng.Next(0, 255), (byte)prng.Next(0, 255), 255 / 2);
        }
        public void SetSpritesGrouper(GameObject GO)
        {
            SpritesGrouper = GO;
        }
        public string ToLongString()
        {
            return "RoomID " + RoomId + "\nBounds: " + Bound.ToString() + "\nTile Count: " + Tiles.Count + "\n \nSettings \n" + Settings.ToString();
        }
        #endregion
        #region overrides
        public override string ToString()
        {
                return "RoomID "+ RoomId + "\nTile Count: " + Tiles.Count + "\n \nSettings \n" + Settings.ToString();
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
        public Seed Seed { get; private set; }
        public System.Random Prng { get; private set; }
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
            Prng = settings.Prng;
        }
        #endregion
        
        #region Methods
        public void SetSeed(Seed _Seed)
        {
            Seed = _Seed;
            Prng = new System.Random(Seed.GetHashCode());
        }
        public void SetSize(roomSize _Size)
        {
            Size = _Size;
        }
        public void SetType(roomType _Type)
        {
            Type = _Type;
        }
        public void SetClass(roomClass _Class)
        {
            Class = _Class;
        }
        public string ToLongString()
        {
            return "Seed " + Seed.ToString() + "\nSize:" + Size.ToString() +
                "\nType:" + Type.ToString() + "\tClass:" + Class.ToString() +
                "\nSmoothing Multiplier: " + SmoothingMultiplier +
                "\nRandom Fill Percentage; " + RandomFillPercent + "%" +
                "\nComparison Factor: " + ComparisonFactor;
        }
        #endregion

        #region overrides
        public override string ToString()
        {
            return "Seed "+ Seed.ToString() + "\nSize:" + Size.ToString() + 
                "\nType:" + Type.ToString() + "\nClass:" + Class.ToString();
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
        public bool Collided { get; private set; } = false;

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
        public void hasCollided()
        {
            Collided = true;
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return "RoomID " + RoomId + "\nCoord " + Coord.GetVector2Int().ToString() + "\nType: " + Type.ToString() + "\nClass: " + Class.ToString() + " \nsWalkable? " + walkable;
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
            return new Vector3Int(other.x, other.y, 0);
        }
        #endregion
    }
    public class IMap
    {
        public Dictionary<CoordInt, Tile> map { get; private set; }
        public List<Room> Rooms { get; private set; }

        #region Constructor
        public IMap()
        {
            Rooms = new List<Room>();
            map = new Dictionary<CoordInt, Tile>();
        }
        public IMap(Dictionary<CoordInt, Tile> _map)
        {
            map = _map;
            Rooms = new List<Room>();
        }
        public IMap(List<Room> rooms)
        {
            map = new Dictionary<CoordInt, Tile>();
            Rooms = rooms;
        }
        public IMap(Dictionary<CoordInt, Tile> _map, List<Room> rooms)
        {
            map = _map;
            Rooms = rooms;
        }
        #endregion

        #region Methods
        public bool ContainsTile(Tile other)
        {
            return map.ContainsValue(other);
        }
        public bool ContainsCoord(CoordInt other)
        {
            return map.ContainsKey(other);
        }
        public bool ContainsCoord(Tile other)
        {
            return map.ContainsKey(other);
        }
        public void SetTile(Tile tile)
        {
            if (ContainsCoord(tile))
                map[tile] = tile;
            else
                map.Add(tile, tile);
        }
        public void AddRoom(Room other)
        {
            Rooms.Add(other);
        }
        #endregion
    }
    public class Mob
    {
        public int ID;
        public int Health { get; private set; } = 1;
        public int Mana { get; private set; } = 0;
        public MobType Type { get; private set; } = MobType.Neutral;
        public AttackType AttackType { get; private set; } = AttackType.None;
        public double AttackDamage { get; private set; } = 0.0;
        public double AttackSpeed { get; private set; } = 0.0;
        public List<AttackEffects> AttackEffects { get; private set; } = new List<AttackEffects>();
        public double MovementSpeed { get; private set; } = 0.0;
        public List<MobMovementEnv> MovementEnviroment { get; private set; } = new List<MobMovementEnv>();
        public MobMovementPattern MovementPattern { get; private set; } = MobMovementPattern.None;
        public double Height { get; private set; } = 0.0;

        #region Constructor
        public Mob(int _ID)
        {
            ID = _ID;
        }
        #endregion


    }
    public class Seed
    {
        public string Value { get; private set; }
        private int Identifier;

        #region Constructors
        public Seed()
        {
            Value = GenerateSeed(new System.Random(DateTime.UtcNow.GetHashCode()));
            GenerateIdentifier();
        }
        public Seed(string _Seed)
        {
            if (_Seed == "" || _Seed == null)
            {
                Value = GenerateSeed(new System.Random(DateTime.UtcNow.GetHashCode()));
                GenerateIdentifier();
            }
            else
            {
                Value = _Seed.ToUpper();
                GenerateIdentifier();
            }
        }
        public Seed(System.Random prng)
        {
            Value = GenerateSeed(prng);
            GenerateIdentifier();
        }
        #endregion

        #region Methods
        public static string GenerateSeed(System.Random Prng)
        {
            string seed = "";

            for (int i = 0; i < 8; i++)
            {
                seed += SeedController.newChar(Prng);
            }

            return seed;
        }
        public void GenerateIdentifier()
        {
            Identifier = Value[0].GetHashCode() + Value[1].GetHashCode() * 10 +
                Value[2].GetHashCode() * 100 + Value[3].GetHashCode() * 1000 +
                Value[4].GetHashCode() * 10000 + Value[5].GetHashCode() * 100000 +
                Value[6].GetHashCode() * 1000000 + Value[7].GetHashCode() * 10000000;
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return SeedController.FormatedSeed(Value);
        }
        public override int GetHashCode()
        {
            return Identifier;
        }
        #endregion

        #region Operators
        public static implicit operator string(Seed other)
        {
            return other.Value;
        }
        #endregion
    }
}
namespace Enums
{
    public enum roomSize
    {
        Tiny = 8,
        Small = 13,
        Medium = 16,
        Big = 19,
        Large = 22
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

    public enum MobType
    {
        Neutral,
        Player,
        SmallEnemy,
        Enemy,
        BigEnemy,
        MiniBoss,
        Boss,
        Static,
        Friendly
    }
    public enum AttackType
    {
        None,
        Ranged,
        Meele,
        Universal
    }
    public enum AttackEffects
    {
        Poison,
        Bleed,
        Dilaceration,
        Blunt,
        Contagious,
        Slow,
        Confusion
    }
    public enum MobMovementPattern
    {
        None,
        Cubed,
        Round,
        Free
    }
    public enum MobMovementEnv
    {
        Ground,
        Sky,
        Water
    }
}