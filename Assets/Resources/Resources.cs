using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using Enums;

namespace Resources
{
    public class IRoom
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
        public List<IRoom> Connections { get; private set; } = new List<IRoom>();

        #region Constructors
        public IRoom(int _RoomId, RoomSettings _RoomSettings)
        {
            RoomId = _RoomId;
            Settings = _RoomSettings;
        }
        public IRoom(IRoom room)
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
        public bool isValid()
        {
            if (Settings == null)
            {
                Debug.Log("Room Settings are Invalid. (null)");
                return false;
            }
            if (Bound == null)
            {
                Debug.Log("Room bounds does not exist. (null)");
                return false;
            }
            if (Bound == new Rect())
            {
                Debug.Log("Room bounds not set.");
                return false;
            }
            if (CenterTile == null)
            {
                Debug.Log("Center Tile does not exist. (null)");
                return false;
            }
            if (!Map.ContainsKey(CenterTile.Coord) || Map[CenterTile.Coord] != CenterTile)
            {
                Debug.Log("Center Tile is not present in room map.");
                return false;
            }
            if (Color == null)
            {
                Debug.Log("Room has no color. (is null)");
                return false;
            }
            if (Color == new Color())
            {
                Debug.Log("Room color not set.");
                return false;
            }
            if (Color == new Color())
            {
                Debug.Log("Room color not set.");
                return false;
            }
            if (FloorTiles.Count == 0 || FloorTiles == null)
            {
                Debug.Log("Room has no floor tiles.");
                return false;
            }
            if (WallTiles.Count == 0 || WallTiles == null)
            {
                Debug.Log("Room has no wall tiles.");
                return false;
            }
            if (Tiles.Count == 0 || Tiles == null)
            {
                Debug.Log("Room has no tiles.");
                return false;
            }
            if (Map.Count == 0 || Map == null)
            {
                Debug.Log("Room map has no tiles or was not created.");
                return false;
            }
            if (HasError == true && ErrorMessage != null)
            {
                Debug.Log("Room has an error.\n" + ErrorMessage);
                return false;
            }
            if (Map.Count == 0 || Map == null)
            {
                Debug.Log("Room map has no tiles or was not created.");
                return false;
            }
            if (Finished == false)
            {
                Debug.Log("Room was not finished.");
                return false;
            }
            if (roomGo == null)
            {
                Debug.Log("Room has no GameObject thus having no mesh neither sprites.");
                return false;
            }
            if (FinishedGeneration == false)
            {
                Debug.Log("Room Generation was not finalized.");
                return false;
            }
            if (SpritesGrouper == null)
            {
                Debug.Log("Room has no Sprites GameObject thus having no sprites.");
                return false;
            }

            return true;
        }
        public void SetColor()
        {
            System.Random prng = new System.Random(Settings.Seed.GetHashCode());
            Color = new Color32((byte)prng.Next(0, 255), (byte)prng.Next(0, 255), (byte)prng.Next(0, 255), 255 / 2);
        }
        public void resetMap()
        {
            Map = new Dictionary<CoordInt, Tile>();
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
        public string ToLongString()
        {
            return "RoomID " + RoomId + "\nBounds: " + Bound.ToString() + "\nTile Count: " + Tiles.Count + "\n \nSettings \n" + Settings.ToString();
        }
        public void ConnectRoom(IRoom other)
        {
            if (!Connections.Contains(other))
                Connections.Add(other);
            if (!other.Connections.Contains(this))
                other.Connections.Add(this);
        }
        public List<GameObject> GetSprites()
        {
            List<GameObject> list = new List<GameObject>();

            foreach (Transform transform in SpritesGrouper.transform)
                list.Add(transform.gameObject);

            return list;
        }
        public void SetError(string Message)
        {
            HasError = true;
            ErrorMessage = Message;
        }
        public void SetGameObject(GameObject go)
        {
            roomGo = go;
        }
        public void SetCenterTile(Tile _CenterTile)
        {
            CenterTile = _CenterTile;
        }
        public void SetSpritesGrouper(GameObject GO)
        {
            SpritesGrouper = GO;
        }
        public void SetWallTiles(List<Tile> _WallTiles)
        {
            WallTiles = _WallTiles;
        }
        public void SetFloorTiles(List<Tile> _FloorTiles)
        {
            FloorTiles = _FloorTiles;
        }
        public void SetMap(Dictionary<CoordInt, Tile> _map)
        {
            Map = _map;
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
        public static implicit operator CoordInt(IRoom other)
        {
            return other.CenterTile.Coord;
        }
        public static implicit operator Tile(IRoom other)
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
        public int SmoothingMultiplier;
        public int RandomFillPercent;
        public int ComparisonFactor;

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
        public RoomSettings GetSettings()
        {
            return this;
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
    public class Tile : CoordInt
    {
        public tileType Type { get; private set; } = tileType.Wall;

        #region Constructors
        public Tile(Tile _Tile) : base(_Tile.GetCoordInt())
        {
            RoomId = _Tile.RoomId;
            Type = _Tile.Type;
            Class = _Tile.Class;
            walkable = _Tile.walkable;
        }
        public Tile(CoordInt _Coord) : base(_Coord)
        {
        }
        public Tile(CoordInt _Coord, tileType _Type) : base(_Coord)
        {
            Type = _Type;
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
        public CoordInt GetCoordInt()
        {
            return this;
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
        #endregion
    }
    public class CoordInt
    {
        public int x { get; private set; }
        public int y { get; private set; }

        #region Constructors
        public CoordInt(int _x, int _y)
        {
            x = _x;
            y = _y;
        }
        public CoordInt(CoordInt coord)
        {
            x = coord.x;
            y = coord.y;
            tile = coord.tile;
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
    public class Mob
    {
        public int ID;
        public int Health { get; private set; } = 1;
        public int Mana { get; private set; } = 0;
        public MobType Type { get; private set; } = MobType.Neutral;
        public AttackType AttackType { get; private set; } = AttackType.None;
        public float AttackDamage { get; private set; } = 0.5f;
        public float AttackSpeed { get; private set; } = 3;
        public List<AttackEffects> AttackEffects { get; private set; } = new List<AttackEffects>();
        public float MovementSpeed { get; private set; } = 1;
        public List<MobMovementEnv> MovementEnviroment { get; private set; } = new List<MobMovementEnv>();
        public MobMovementPattern MovementPattern { get; private set; } = MobMovementPattern.None;
        public float Height { get; private set; } = 2;
        public Sprite Sprite { get; private set; } = null;
        public float Invencibility { get; private set; } = 0.5f;

        public Dictionary<string, Effect> Effects { get; private set; } = new Dictionary<string, Effect>();
        public Dictionary<string, Modifier> Modifiers { get; private set; } = new Dictionary<string, Modifier>();

        #region Constructor
        public Mob(int _ID)
        {
            ID = _ID;
        }
        public Mob(int _ID, int _Health, MobType _Type)
        {
            ID = _ID;
            Health = _Health;
            Type = _Type;
        }
        #endregion

        public void RecieveDamage(int Damage)
        {
            Health -= Damage;
        }
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
    public class Item
    {
        public int ID;
        public ISprite Sprite = null;
        public GameObject gameObject = null;
        public int rarity = 1;

        #region Constructor

        public Item()
        {

        }
        public Item(int ID)
        {
            this.ID = ID;
        }
        public Item(int ID, int rarity)
        {
            this.ID = ID;
            this.rarity = rarity;
        }

        #endregion
    }

    public class Map
    {
        public Dictionary<CoordInt, Chunk> Chunks = new Dictionary<CoordInt, Chunk>();
        public Dictionary<int, Room> Rooms = new Dictionary<int, Room>();
        public int nextRoomID = 0;
        public Seed seed = new Seed();
        public int ChunkSize;

        public Map(int ChunkSize)
        {
            this.ChunkSize = ChunkSize;
        }

        public Map(List<Chunk> Chunks)
        {
            foreach (Chunk c in Chunks)
            {
                ChunkSize = c.Size;
                this.Chunks.Add(c.Coordinates, c);
            }
        }

        public CoordInt GetChunkCoord(CoordInt TileCoord)
        {
            return new CoordInt(Mathf.RoundToInt(TileCoord.x / (float)ChunkSize), Mathf.RoundToInt(TileCoord.y / (float)ChunkSize));
        }
        public Tile GetTile(CoordInt TileCoord)
        {
            return Chunks[GetChunkCoord(TileCoord)].Tiles[TileCoord];
        }
        public bool ContainsCoord(CoordInt Coord)
        {
            return Chunks.ContainsKey(GetChunkCoord(Coord));
        }
        public void CreateChunk(CoordInt TileCoord)
        {
            TileCoord = GetChunkCoord(TileCoord);
            Chunk chunk = new Chunk(TileCoord, ChunkSize);
            Chunks.Add(TileCoord, chunk);
        }
        public void ApplyChunks(List<Chunk> ChunksToApply)
        {
            foreach (Chunk c in ChunksToApply)
            {
                if (Chunks.ContainsKey(c.Coordinates))
                    Chunks[c.Coordinates] = c;
                else
                    Chunks.Add(c.Coordinates, c);
            }
        }
        public void AddRoom(Room room)
        {
            Rooms.Add(room.ID, room);
            ApplyChunks(room.GetChunkList());
            nextRoomID++;
        }
        public void RemoveRoom(Room room)
        {
            foreach(Chunk c in Rooms[room.ID].Chunks.Values)
                if (Chunks.ContainsKey(c.Coordinates))
                    Chunks[c.Coordinates].ResetChunk();

            Rooms.Remove(room.ID);
        }
    }
    public class Chunk : CoordInt
    {
        public Room room = null;
        public Dictionary<CoordInt, Tile> Tiles = new Dictionary<CoordInt, Tile>();
        public int Size;

        public Chunk(CoordInt coord, int Size): base(coord)
        {
            this.Size = Size;
            GenerateTiles();
        }

        public CoordInt GetChunkCoord(CoordInt TileCoord)
        {
            return new CoordInt(Mathf.RoundToInt(TileCoord.x / (float)Size), Mathf.RoundToInt(TileCoord.y / (float)Size));
        }
        public Tile GetTile(int x, int y)
        {
            return Tiles[new CoordInt(Coordinates.x * Size + x, Coordinates.y * Size + y)];
        }
        public bool ContainsCoord(CoordInt Coord)
        {
            return Tiles.ContainsKey(Coord);
        }
        private void GenerateTiles()
        {
            Dictionary<CoordInt, Tile> Generation = new Dictionary<CoordInt, Tile>();
            System.Random Prng = new System.Random(DateTime.Now.GetHashCode());
            
            for (int x = Coordinates.x * Size - (int)(Size / 2f); x <= Coordinates.x * Size + (int)(Size / 2f); x++)
            {
                for (int y = Coordinates.y * Size - (int)(Size / 2f); y <= Coordinates.y * Size + (int)(Size / 2f); y++)
                {
                    CoordInt coord = new CoordInt(x, y);

                    int random = Prng.Next(0, 100);

                    Tile tile = new Tile(coord, (random > 50) ? tileType.Wall : tileType.Floor);
                    Generation.Add(coord, tile);
                }
            }
            SetTiles(Generation);
        }
        public void GenerateTiles(RoomSettings Settings)
        {
            Dictionary<CoordInt, Tile> Generation = new Dictionary<CoordInt, Tile>();

            for (int x = Coordinates.x * Size - (int)(Size / 2f); x <= Coordinates.x * Size + (int)(Size / 2f); x++)
            {
                for (int y = Coordinates.y * Size - (int)(Size / 2f); y <= Coordinates.y * Size + (int)(Size / 2f); y++)
                {
                    CoordInt coord = new CoordInt(x, y);

                    int random = Settings.Prng.Next(0, 100);

                    Tile tile = new Tile(coord, (random > Settings.RandomFillPercent) ? tileType.Wall : tileType.Floor);
                    Generation.Add(coord, tile);
                }
            }
            SetTiles(Generation);
        }
        public void SetTiles(Dictionary<CoordInt, Tile> Tiles)
        {
            this.Tiles = Tiles;
        }
        public void RegenerateTiles()
        {
            Tiles.Clear();
            GenerateTiles();
        }
        public void ResetChunk()
        {
            room = null;
            GenerateTiles();
        }
    }

    public class Room: RoomSettings
    {
        public int ID;
        public Dictionary<CoordInt, Chunk> Chunks = new Dictionary<CoordInt, Chunk>();
        private Dictionary<CoordInt, Chunk> OriginalChunksSelection = new Dictionary<CoordInt, Chunk>();
        public List<Room> Connections = new List<Room>();
        public Dictionary<CoordInt, Tile> Map = new Dictionary<CoordInt, Tile>();
        public Color Color;
        public Dictionary<int, Region> Regions = new Dictionary<int, Region>();
        public int nextRegionID = 0;
        public GameObject GameObject = null;

        public Room(int ID, List<Chunk> Chunks, RoomSettings settings) : base(settings)
        {
            this.ID = ID;
            RemoveNotAdjacent(Chunks);
            OriginalChunksSelection = Conversion.ListToDictionary(Chunks);
        }
        public Room(List<Chunk> Chunks, RoomSettings settings) : base(settings)
        {
            RemoveNotAdjacent(Chunks);
            OriginalChunksSelection = Conversion.ListToDictionary(Chunks);
        }

        public void RemoveNotAdjacent(List<Chunk> ChunkListRaw)
        {
            Dictionary<CoordInt, Chunk> ChunkDict = GetDictionaryFromList(ChunkListRaw);

            Queue<Chunk> queue = new Queue<Chunk>();
            List<CoordInt> MapFlags = new List<CoordInt>();

            queue.Enqueue(ChunkListRaw[0]);

            Dictionary<CoordInt, Chunk> AdjacentChunks = new Dictionary<CoordInt, Chunk>();
            AdjacentChunks.Add(ChunkListRaw[0].Coordinates, ChunkListRaw[0]);

            while (queue.Count > 0)
            {
                Chunk c = queue.Dequeue();
                MapFlags.Add(c.Coordinates);

                for (int NeighbourX = c.Coordinates.x - 1; NeighbourX <= c.Coordinates.x + 1; NeighbourX++)
                {
                    for (int NeighbourY = c.Coordinates.y - 1; NeighbourY <= c.Coordinates.y + 1; NeighbourY++)
                    {
                        CoordInt curCoord = new CoordInt(NeighbourX, NeighbourY);
                        if (curCoord.isAdjacent(c.Coordinates))
                        {
                            if (!MapFlags.Contains(curCoord))
                            {
                                MapFlags.Add(curCoord);
                                if (ChunkDict.ContainsKey(curCoord))
                                {
                                    AdjacentChunks.Add(ChunkDict[curCoord].Coordinates, ChunkDict[curCoord]);
                                    queue.Enqueue(ChunkDict[curCoord]);
                                }
                            }
                        }
                    }
                }
            }

            SetChunks(AdjacentChunks);
        }

        #region Create Room

        public void GenerateTiles()
        {
            foreach (Chunk c in Chunks.Values)
                c.GenerateTiles(GetSettings());
        }
        public void SmoothChunks()
        {
            SetChunks(TileHandler.Smooth(GetChunkList(), SmoothingMultiplier, ComparisonFactor, false));
        }
        public void GetMap()
        {
            Map = new Dictionary<CoordInt, Tile>();
            foreach (Chunk c in Chunks.Values)
                foreach (Tile t in c.Tiles.Values)
                    Map.Add(t.Coord, t);
        }
        public void GetRegions()
        {
            Dictionary<CoordInt, Tile> MapFlags = new Dictionary<CoordInt, Tile>();
            foreach (Tile t in Map.Values)
            {
                if (!MapFlags.ContainsKey(t.Coord))
                    {
                    if (t.Type == tileType.Floor)
                    {
                        Region Region = new Region(nextRegionID, GetRegion(t));
                        float threshold = Region.GetChunks(this).Count * Mathf.Pow(GetChunkSize(), 2) * 0.4f;
                        if (Region.Map.Count > threshold)
                            AddRegion(Region);
                        else if (Regions.Count > 0)
                        {
                            Debug.Log("Region has Tiles below threshold");
                            foreach (Tile tr in Region.Map.Values)
                            {
                                Chunks[GetChunkCoord(tr.Coord)].Tiles[tr.Coord].SetType(tileType.Wall);
                                MapFlags.Add(t.Coord, t);
                            }
                        }
                        else
                            AddRegion(Region);

                        foreach (Tile tr in Region.Map.Values)
                            if (!MapFlags.ContainsKey(tr.Coord))
                                MapFlags.Add(tr.Coord, tr);
                    }
                    else
                        MapFlags.Add(t.Coord, t);
                }
            }
            Debug.Log("Found " + Regions.Count + " Regions");
        }
        public void AddRegion(Region Region)
        {
            Regions.Add(Region.ID, Region);
            nextRegionID++;
        }
        public void CreateConnections()
        {
            if (Regions.Count > 1)
            {
                foreach (Region from in Regions.Values)
                {
                    if (from.Connection.Count >= 2)
                        continue;
                    float dist = 1000000;
                    Region ToRegion = null;
                    foreach (Region to in Regions.Values)
                    {
                        if (from.ID == to.ID)
                            continue;
                        if (from.Connection.ContainsKey(to.ID))
                            continue;
                        if (to.Connection.Count >= 2)
                            continue;
                        if (Region.Distance(from, to) < dist)
                        {
                            dist = Region.Distance(from, to);
                            ToRegion = to;
                        }
                    }
                    if(ToRegion != null)
                        from.ConnectRegions(ToRegion, NearestTileBetweenRegions(from, ToRegion));
                }
                CheckConnections();
            }
        }
        public void CheckConnections()
        {
            Queue<Region> queue = new Queue<Region>(Conversion.DictionaryToList(Regions));
            Dictionary<int, bool> ConnectionFlags = new Dictionary<int, bool>();

            List<List<Region>> MasterRegions = new List<List<Region>>();

            while (queue.Count > 0)
            {
                Region region = queue.Dequeue();
                if (!ConnectionFlags.ContainsKey(region.ID))
                {
                    List<Region> MasterRegionList = new List<Region>();
                    MasterRegionList.Add(region);
                    ConnectionFlags.Add(region.ID, true);

                    Queue<Region> innerqueue = new Queue<Region>();
                    innerqueue.Enqueue(region);

                    while (innerqueue.Count > 0)
                    {
                        Region innerRegion = innerqueue.Dequeue();
                        foreach (int r in innerRegion.Connection.Keys)
                        {
                            if (ConnectionFlags.ContainsKey(r))
                                continue;
                            ConnectionFlags.Add(r, true);
                            MasterRegionList.Add(Regions[r]);
                            innerqueue.Enqueue(Regions[r]);
                        }
                    }
                    MasterRegions.Add(MasterRegionList);
                }
            }

            if (MasterRegions.Count > 1)
            {
                foreach (List<Region> from in MasterRegions)
                {
                    float dist = 1000000;
                    Region nearestToRegion = null;
                    Region nearestFromRegion = null;
                    for (int fromIndex = 0; fromIndex < from.Count; fromIndex++)
                    {
                        foreach (List<Region> to in MasterRegions)
                        {
                            if (from == to)
                                continue;
                            for (int toIndex = 0; toIndex < to.Count; toIndex++)
                            {
                                if (Region.Distance(from[fromIndex], to[toIndex]) < dist)
                                {
                                    nearestToRegion = to[toIndex];
                                    nearestFromRegion = from[fromIndex];
                                    dist = from[fromIndex].Distance(to[toIndex]);
                                }
                            }
                        }
                    }
                    nearestFromRegion.ConnectRegions(nearestToRegion, NearestTileBetweenRegions(nearestFromRegion, nearestToRegion));
                }
            }
        }
        public void GenerateRegionConnections()
        {
            foreach(Region r in Regions.Values)
            {
                foreach(Tuple<Tile, Tile> t in r.Connection.Values)
                {
                    List<CoordInt> tilestocreate = GetLine(t.Item1.Coord, t.Item2.Coord);

                    for (int i = 0; i < tilestocreate.Count; i++)
                    {
                        CoordInt coord = tilestocreate[i];

                        for (int neighbourX = (int)(coord.x -.5f); neighbourX <= (int)(coord.x + .5f); neighbourX++)
                        {
                            for (int neighbourY = (int)(coord.y - .5f); neighbourY <= (int)(coord.y + .5f); neighbourY++)
                            {
                                CoordInt curCoord = new CoordInt(neighbourX, neighbourY);

                                Tile tile = Map.ContainsKey(curCoord) ? Map[curCoord] : null;
                                if(tile == null)
                                {
                                    if(OriginalChunksSelection.ContainsKey(GetChunkCoord(curCoord)))
                                        tile = OriginalChunksSelection[GetChunkCoord(curCoord)].Tiles[curCoord];
                                }
                                if (tile.Type != tileType.Floor)
                                    Map[curCoord].SetType(tileType.Floor);
                            }
                        }
                    }
                }
            }
            RemoveCleanChunks();
        }
        private void RemoveCleanChunks()
        {
            List<Chunk> toRemove = new List<Chunk>();
            foreach (Chunk c in Chunks.Values)
            {
                bool Clean = true;
                foreach (Tile t in c.Tiles.Values)
                {
                    if (t.Type == tileType.Wall)
                        for (int NeighbourX = t.Coord.x - 1; NeighbourX <= t.Coord.x + 1; NeighbourX++)
                        {
                            for (int NeighbourY = t.Coord.y - 1; NeighbourY <= t.Coord.y + 1; NeighbourY++)
                            {
                                CoordInt curCoord = new CoordInt(NeighbourX, NeighbourY);

                                if (!Chunks.ContainsKey(GetChunkCoord(curCoord)))
                                    continue;
                                if (GetChunkCoord(curCoord) == c.Coordinates)
                                    continue;
                                if (Chunks[GetChunkCoord(curCoord)].Tiles[curCoord].Type == tileType.Floor)
                                {
                                    Clean = false;
                                    break;
                                }
                            }
                        }
                    if (t.Type == tileType.Floor)
                    {
                        Clean = false;
                        break;
                    }
                }
                if (Clean)
                    toRemove.Add(c);
            }
            foreach (Chunk c in toRemove)
            {
                c.RegenerateTiles();
                c.room = null;
                Chunks.Remove(c.Coordinates);
            }
        }
        private Dictionary<CoordInt, Tile> GetRegion(Tile startTile)
        {
            Dictionary<CoordInt, Tile> Region = new Dictionary<CoordInt, Tile>();
            Queue<Tile> queue = new Queue<Tile>();
            queue.Enqueue(startTile);
            Region.Add(startTile.Coord, startTile);

            while (queue.Count > 0)
            {
                Tile tile = queue.Dequeue();

                for (int NeighbourX = tile.Coord.x - 1; NeighbourX <= tile.Coord.x + 1; NeighbourX++)
                {
                    for (int NeighbourY = tile.Coord.y - 1; NeighbourY <= tile.Coord.y + 1; NeighbourY++)
                    {
                        CoordInt curCoord = new CoordInt(NeighbourX, NeighbourY);
                        if (curCoord.isAdjacent(tile.Coord))
                            if (!Region.ContainsKey(curCoord) && Map.ContainsKey(curCoord))
                            {
                                Region.Add(curCoord, Map[curCoord]);
                                Chunks[GetChunkCoord(curCoord)].Tiles[curCoord].RoomId = ID;
                                if (Map[curCoord].Type == tileType.Floor)
                                    queue.Enqueue(Map[curCoord]);
                            }
                    }
                }
            }
            return Region;
        }
        public void SetColor()
        {
            System.Random prng = new System.Random(Seed.GetHashCode());
            Color = new Color32((byte)prng.Next(0, 255), (byte)prng.Next(0, 255), (byte)prng.Next(0, 255), 255 / 2);
        }

        #endregion

        #region Get Methods
        public CoordInt GetChunkCoord(CoordInt TileCoord)
        {
            return new CoordInt(Mathf.RoundToInt(TileCoord.x / (float)GetChunkSize()), Mathf.RoundToInt(TileCoord.y / (float)GetChunkSize()));
        }
        public Tile GetTile(CoordInt TileCoord)
        {
            return Chunks.ContainsKey(GetChunkCoord(TileCoord)) ? Chunks[GetChunkCoord(TileCoord)].Tiles[TileCoord] : null;
        }
        public List<Chunk> GetChunkList()
        {
            List<Chunk> ChunkList = new List<Chunk>();
            foreach (Chunk c in Chunks.Values)
                ChunkList.Add(c);
            return ChunkList;
        }
        private Dictionary<CoordInt, Chunk> GetDictionaryFromList(List<Chunk> list)
        {
            Dictionary<CoordInt, Chunk> dict = new Dictionary<CoordInt, Chunk>();

            foreach(Chunk c in list)
            {
                dict.Add(c.Coordinates, c);
            }

            return dict;
        }
        public Dictionary<CoordInt, Tile> GetTilesByType(tileType Type)
        {
            Dictionary<CoordInt, Tile> Tiles = new Dictionary<CoordInt, Tile>();
            foreach (Tile t in Map.Values)
                if (t.Type == Type)
                    Tiles.Add(t.Coord, t);
            return Tiles;
        }
        public Vector2 GetMiddle()
        {
            int minX = 0;
            int minY = 0;
            int maxX = 0;
            int maxY = 0;

            bool firstLoop = true;

            int Size = 0;

            foreach (CoordInt coord in Chunks.Keys)
            {
                if (firstLoop)
                {
                    Size = Chunks[coord].Size;
                    minX = coord.x;
                    minY = coord.y;
                    maxX = coord.x;
                    maxY = coord.y;
                    firstLoop = false;
                }
                else
                {
                    if (coord.x < minX)
                        minX = coord.x;
                    if (coord.y < minY)
                        minY = coord.y;
                    if (coord.x > maxX)
                        maxX = coord.x;
                    if (coord.y > maxY)
                        maxY = coord.y;
                }
            }

            return new Vector2((minX + maxX)/2f * Size,(minY + maxY)/2f * Size);
        }
        public int GetChunkSize()
        {
            int Size = 0;
            foreach (Chunk c in Chunks.Values)
                Size = c.Size;
            return Size;
        }
        public Tile[] NearestTileBetweenRegions(Region FromTiles, Region ToTiles)
        {
            Tile[] tiles = new Tile[2];

            float dist = 25;

            Dictionary<CoordInt, Tile> FromOuterTiles = FromTiles.GetOuterTiles();
            Dictionary<CoordInt, Tile> ToOuterTiles = ToTiles.GetOuterTiles();

            foreach (Tile t in FromOuterTiles.Values)
            {
                if (t.Type == tileType.Wall)
                {
                    Vector2Int FromPos = t.Coord.GetVector2Int();
                    foreach (Tile f in ToOuterTiles.Values)
                    {
                        Vector2Int ToPos = f.Coord.GetVector2Int();
                        if (Vector2Int.Distance(FromPos, ToPos) < dist)
                        {
                            dist = Vector2Int.Distance(FromPos, ToPos);
                            tiles[0] = t;
                            tiles[1] = f;
                        }
                    }
                }
            }

            return tiles;
        }

        #endregion

        #region Set Methods

        public void SetChunks(List<Chunk> ChunksToSet)
        {
            foreach (Chunk c in ChunksToSet)
            {
                c.room = this;
                if (!Chunks.ContainsKey(c.Coordinates))
                    Chunks.Add(c.Coordinates, c);
                else
                    Chunks[c.Coordinates] = c;
            }
        }
        public void SetChunks(Dictionary<CoordInt, Chunk> ChunksToSet)
        {
            foreach (Chunk c in ChunksToSet.Values)
            {
                c.room = this;
                if (!Chunks.ContainsKey(c.Coordinates))
                    Chunks.Add(c.Coordinates, c);
                else
                    Chunks[c.Coordinates] = c;
            }
        }
        public void SetConnection(Room toRoom)
        {
            this.Connections.Add(toRoom);
            toRoom.Connections.Add(this);
        }

        #endregion

        #region Static Methods

        public static float Distance(Room A, Room B)
        {
            return Vector2.Distance(A.GetMiddle(), B.GetMiddle());
        }
        public static Room NewRoomFromRegion(Region region, Room MotherRoom, Map map)
        {
          
  return new Room(map.nextRoomID, region.GetChunks(MotherRoom), MotherRoom.GetSettings());
        }
        public static bool isValid(Room room)
        {
            if (room == null)
                return false;
            else if (room.Chunks == null || room.Map == null || room.Color == null || room.Regions == null)
                return false;
            else if (room.Chunks.Count > 0 || room.Map.Count > 0 || room.Regions.Count > 0)
                return false;
            return true;
        }
        public static List<CoordInt> GetLine(CoordInt from, CoordInt to)
        {
            List<CoordInt> line = new List<CoordInt>();

            int x = from.x;
            int y = from.y;

            int dx = to.x - from.x;
            int dy = to.y - from.y;

            bool inverted = false;
            int step = Math.Sign(dx);
            int gradientStep = Math.Sign(dy);

            int longest = Mathf.Abs(dx);
            int shortest = Mathf.Abs(dy);

            if (longest < shortest)
            {
                inverted = true;
                longest = Mathf.Abs(dy);
                shortest = Mathf.Abs(dx);

                step = Math.Sign(dy);
                gradientStep = Math.Sign(dx);
            }

            int gradientAccumulation = longest / 2;
            for (int i = 0; i < longest; i++)
            {
                line.Add(new CoordInt(x, y));

                if (inverted)
                {
                    y += step;
                }
                else
                {
                    x += step;
                }

                gradientAccumulation += shortest;
                if (gradientAccumulation >= longest)
                {
                    if (inverted)
                    {
                        x += gradientStep;
                    }
                    else
                    {
                        y += gradientStep;
                    }
                    gradientAccumulation -= longest;
                }
            }

            return line;
        }
        
        #endregion
    }

    public class Region
    {
        public int ID;
        public Dictionary<CoordInt, Tile> Map = new Dictionary<CoordInt, Tile>();
        public Dictionary<int, Tuple<Tile, Tile>> Connection = new Dictionary<int, Tuple<Tile, Tile>>();

        public Region(int ID, Dictionary<CoordInt, Tile> Map)
        {
            this.ID = ID;
            this.Map = Map;
        }

        public List<Chunk> GetChunks(Room room)
        {
            List<Chunk> chunks = new List<Chunk>();
            List<CoordInt> mapFlags = new List<CoordInt>();

            foreach (Tile t in Map.Values)
            {
                CoordInt chunkCoord = room.GetChunkCoord(t.Coord);
                if (!mapFlags.Contains(chunkCoord))
                {
                    mapFlags.Add(chunkCoord);
                    chunks.Add(room.Chunks[chunkCoord]);
                }
            }
            return chunks;
        }
        public Dictionary<CoordInt, Tile> GetOuterTiles()
        {
            Dictionary<CoordInt, Tile> tiles = new Dictionary<CoordInt, Tile>();
            foreach (Tile tile in Map.Values)
            {
                for (int NeighbourX = tile.Coord.x - 1; NeighbourX <= tile.Coord.x + 1; NeighbourX++)
                {
                    for (int NeighbourY = tile.Coord.y - 1; NeighbourY <= tile.Coord.y + 1; NeighbourY++)
                    {
                        CoordInt curCoord = new CoordInt(NeighbourX, NeighbourY);
                        if (tile.Coord.isAdjacent(curCoord))
                            if (Map.ContainsKey(curCoord))
                                if (Map[curCoord].Type == tileType.Wall)
                                {
                                    tiles.Add(tile.Coord, tile);
                                    goto next;
                                }
                    }
                }
                next:
                continue;
            }

            return tiles;
        }
        public void ConnectRegions(Region other, Tile[] tiles)
        {

            if (other == null)
            {
                Debug.Log("Other Region is Null, Cannot Create Connection");
                return;
            }
            if (tiles[0] == null)
            {
                Debug.Log("Connection tile from origin Region is Null, Cannot Create Connection");
                return;
            }
            if (tiles[1] == null)
            {
                Debug.Log("Connection tile from origin Region is Null, Cannot Create Connection");
                return;
            }
            Connection.Add(other.ID, new Tuple<Tile, Tile>(tiles[0], tiles[1]));
            other.Connection.Add(ID, new Tuple<Tile, Tile>(tiles[1], tiles[0]));
            Debug.Log("Created Connection from " + ID + " to " + other.ID + " on tiles " + tiles[0].Coord.ToString() + " to " + tiles[1].Coord.ToString() + " respectively");
        }

        public float Distance(Region other)
        {
            float dist = 1000000;

            Dictionary<CoordInt, Tile> thisOuter = GetOuterTiles();
            Dictionary<CoordInt, Tile> otherOuter = other.GetOuterTiles();

            foreach (Tile t in thisOuter.Values)
            {
                    Vector2Int FromPos = t.Coord.GetVector2Int();
                    foreach (Tile f in otherOuter.Values)
                    {
                        Vector2Int ToPos = f.Coord.GetVector2Int();
                        if (Vector2Int.Distance(FromPos, ToPos) < dist)
                            dist = Vector2Int.Distance(FromPos, ToPos);
                    }
            }
            return dist;
        }
        public static float Distance(Region A, Region B)
        {
            float dist = 1000000;

            Dictionary<CoordInt, Tile> AOuter = A.GetOuterTiles();
            Dictionary<CoordInt, Tile> BOuter = B.GetOuterTiles();

            foreach (Tile t in AOuter.Values)
            {
                Vector2Int FromPos = t.Coord.GetVector2Int();
                foreach (Tile f in BOuter.Values)
                {
                    Vector2Int ToPos = f.Coord.GetVector2Int();
                    if (Vector2Int.Distance(FromPos, ToPos) < dist)
                        dist = Vector2Int.Distance(FromPos, ToPos);
                }
            }
            return dist;
        }
    }

    public class EffectItem : Item
    {
        public List<Effect> EffectsOnPickup;

        public EffectItem(int id, Effect effect) : base(id)
        {
            EffectsOnPickup = new List<Effect>();
            EffectsOnPickup.Add(effect);
        }
        public EffectItem(int id, List<Effect> effects) : base(id)
        {
            EffectsOnPickup = new List<Effect>(effects);
        }
    }

    public class Effect
    {
        public string Name { get; private set; } = "";
        public bool Active { get; private set; } = false;
        public ActivationType ActivationType { get; private set; } = ActivationType.None;
        public float Damage { get; private set; } = 0;
        public float duration { get; private set; } = 0;
        public float interval { get; private set; } = 1;
        public ISprite Sprite { get; private set; } = new ISprite();
        public Dictionary<string, Tuple<Modifiable, float>> ModifiedBy { get; private set; } = new Dictionary<string, Tuple<Modifiable, float>>();
        public Dictionary<string, Tuple<string, OperatorType, string>> InflictIf { get; private set; } = new Dictionary<string, Tuple<string, OperatorType, string>>();
        public Dictionary<string, Tuple<string, OperatorType, string>> TriggerIf { get; private set; } = new Dictionary<string, Tuple<string, OperatorType, string>>();
        public Dictionary<string, Tuple<string, OperatorType, string>> StopIf { get; private set; } = new Dictionary<string, Tuple<string, OperatorType, string>>();
        public Dictionary<string, Tuple<string, OperatorType, string>> RemoveIf { get; private set; } = new Dictionary<string, Tuple<string, OperatorType, string>>();

        public Mob Infected { get; private set; } = null;

        public Effect()
        {
        }
        #region Methods
        public void SetName(string newName)
        {
            Name = newName;
        }
        public void SetActivation(ActivationType newActivation)
        {
            ActivationType = newActivation;
        }
        public void SetDamage(float newDamage)
        {
            Damage = newDamage;
        }
        public void SetDuration(float newDuration)
        {
            duration = newDuration;
        }
        public void SetInterval(float newinterval)
        {
            interval = newinterval;
        }

        private bool CanInflict()
        {
            if (!IfMethod(InflictIf))
                return false;
            return true;
        }
        private bool CanTrigger()
        {
            return IfMethod(TriggerIf);
        }
        private bool CanStop()
        {
            return IfMethod(StopIf);
        }
        private bool CanRemove()
        {
            return IfMethod(RemoveIf);
        }

        private void Enter()
        {

        }
        private void Exit()
        {

        }

        private bool IfMethod(Dictionary<string, Tuple<string, OperatorType, string>> rules)
        {
            foreach (Tuple<string, OperatorType, string> t in rules.Values)
            {
                switch (t.Item1.ToUpper())
                {
                    case "HEALTH":
                        switch (t.Item2)
                        {
                            case OperatorType.BiggerThan:
                                if (Infected.Health < float.Parse(t.Item3))
                                    return false;
                                break;
                            case OperatorType.BiggerOrEqualsThan:
                                if (Infected.Health <= float.Parse(t.Item3))
                                    return false;
                                break;
                            case OperatorType.EqualsTo:
                                if (Infected.Health != float.Parse(t.Item3))
                                    return false;
                                break;
                            case OperatorType.LowerOrEqualsThan:
                                if (Infected.Health >= float.Parse(t.Item3))
                                    return false;
                                break;
                            case OperatorType.LowerThan:
                                if (Infected.Health > float.Parse(t.Item3))
                                    return false;
                                break;
                            default:
                                break;
                        }
                        break;
                    case "EFFECT":
                        switch (t.Item2)
                        {
                            case OperatorType.Have:
                                if (!Infected.Effects.ContainsKey(t.Item3))
                                    return false;
                                break;
                            case OperatorType.NotHave:
                                if (Infected.Effects.ContainsKey(t.Item3))
                                    return false;
                                break;
                            default:
                                break;
                        }
                        break;
                    case "MODIFIER":
                        switch (t.Item2)
                        {
                            case OperatorType.Have:
                                if (!Infected.Modifiers.ContainsKey(t.Item3))
                                    return false;
                                break;
                            case OperatorType.NotHave:
                                if (Infected.Modifiers.ContainsKey(t.Item3))
                                    return false;
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }
            return true;
        }
        
        private void CalculateDamage()
        {
            foreach(string mod in ModifiedBy.Keys)
            {
                if(Infected.Modifiers.ContainsKey(mod))
                    switch (ModifiedBy[mod].Item1)
                    {
                        case Modifiable.Damage:
                            Damage *= ModifiedBy[mod].Item2;
                            break;
                        case Modifiable.Duration:
                            duration *= ModifiedBy[mod].Item2;
                            break;
                        case Modifiable.Interval:
                            interval *= ModifiedBy[mod].Item2;
                            break;
                        default:
                            break;
                    }
            }
        }

        public IEnumerator LifeSpan()
        {
            Active = true;

            if (duration == 0)
                yield return null;
            else
            {
                yield return new WaitForSeconds(duration);
                Active = false;
            }
        }
        public IEnumerator DamageInterval()
        {
            while (Active)
            {
                if (Infected.Health <= 0)
                    break;

                Infected.RecieveDamage((int)Damage);
                yield return new WaitForSeconds(interval);
            }
        }
        #endregion

        /*
         * projetil vai ser atirado e vai conter classes de efeitos
         * quando acertar o objetivo vai verificar se o objetivo tem algum modifier na black list
         * vai verificar se tem algum efeito na black list
         * vai verificar se tem os efeitos no InflictIf
         * se tiver vai começar um IEnumerator
         * 
         * sempre que o player receber ou perder um efeito ou um modifier sera verificado o
         * StopIf e o RemoveIf para saber se o player ganhou ou perdeu algum efeito necessario
         * 
         * caso as condições do RemoveIf sejam verdadeiras, a classe será apagada
         * 
         * se a duração for 0 o efeito é infinito
         * 
         * ao ser inflingido começará um ienumerator contando a duração e no fim dela a classe será apagada
         * enquanto o efeito estiver ativo começará um ienumerator que com um waitforseconds(interval)
         * 
         * onEnter,  onExit por implantar
         * onEnter - quando a classe for criada ira fazer o que está no metodo, caso queira colocar buff ou debuffs a stats usar modifier
         * onExit - quando a classe for para ser destruida chamar este metodo, para retirar ou colocar debuffs usar modifier
         * 
         * classe modifier terá que ter suporte para modificar os stats do player
         * 
         * pensar em colocar as Lists como Arrays para melhor performance
        */

        public string ToLongString()
        {
            string s = "Effect: " + Name +
                "\nis Active? " + Active +
                "\nActivation Type: " + ActivationType.ToString() +
                "\nDamage: " + Damage +
                ((duration > 0) ? "\nDuaration: " + duration : "\nDuration: Infinite") +
                ((ActivationType == ActivationType.Periodical) ? "\nInterval: " + interval : "\nInterval: Not Applied");

            s += "\nModified By";

            foreach(string st in ModifiedBy.Keys)
            {
                s += "\n\t" + st + ": " + ModifiedBy[st].Item2;
            }

            s += "\nInflicted When";

            foreach (Tuple<string, OperatorType, string> t in InflictIf.Values)
            {
                s += "\n\t" + t.Item1 + " " + t.Item2.ToString() + " " + t.Item3;
            }

            s += "\nTrigger When";

            foreach (Tuple<string, OperatorType, string> t in TriggerIf.Values)
            {
                s += "\n\t" + t.Item1 + " " + t.Item2.ToString() + " " + t.Item3;
            }

            s += "\nStop When";

            foreach (Tuple<string, OperatorType, string> t in StopIf.Values)
            {
                s += "\n\t" + t.Item1 + " " + t.Item2.ToString() + " " + t.Item3;
            }

            s += "\nRemove When";

            foreach (Tuple<string, OperatorType, string> t in RemoveIf.Values)
            {
                s += "\n\t" + t.Item1 + " " + t.Item2.ToString() + " " + t.Item3;
            }
            return s;
        }
    }
    public class Modifier
    {
        public string Name { get; private set; }
    }

    public class ISprite
    {
        public string Path { get; private set; } = string.Empty;
        public bool Animated { get; private set; } = false;
        public Dictionary<int, Sprite> Sprites = new Dictionary<int, Sprite>();

        #region Constructors
        public ISprite()
        {
        }
        public ISprite(string path)
        {
            Path = path;
        }
        public ISprite(string path, bool animated)
        {
            Path = path;
            Animated = animated;
        }
        public ISprite(string path, Dictionary<int, Sprite> sprites)
        {
            Path = path;
            Sprites = sprites;
        }
        public ISprite(string path, bool animated, Dictionary<int, Sprite> sprites)
        {
            Path = path;
            Animated = animated;
            Sprites = sprites;
        }
        #endregion

        public void SetPath(string path)
        {
            Path = path;
        }
        public void SetAnimated(bool isAnimated)
        {
            Animated = isAnimated;
        }
        public void GetSprites()
        {
            if(Path != string.Empty && Sprites != null)
            {
                Sprites = FileHandler.GetStaticSprites(Sprites, Path);
            }
        }
    }
    public class IGame
    {
        public Dictionary<string, Effect> Effects { get; private set; } = new Dictionary<string, Effect>();
        public List<Modifier> Modifiers = new List<Modifier>();
        public List<Mob> Mobs = new List<Mob>();
    }

    public static class Conversion
    {
        public static Dictionary<CoordInt, Tile> ListToDictionary(List<Tile> List)
        {
            Dictionary<CoordInt, Tile> dict = new Dictionary<CoordInt, Tile>();

            for (int i = 0; i < List.Count; i++)
            {
                dict.Add(List[i].Coord,List[i]);
            }
            return dict;
        }
        public static Dictionary<CoordInt, Chunk> ListToDictionary(List<Chunk> List)
        {
            Dictionary<CoordInt, Chunk> dict = new Dictionary<CoordInt, Chunk>();

            for (int i = 0; i < List.Count; i++)
            {
                dict.Add(List[i].Coordinates, List[i]);
            }
            return dict;
        }
        public static Dictionary<int, Room> ListToDictionary(List<Room> List)
        {
            Dictionary<int, Room> dict = new Dictionary<int, Room>();

            for (int i = 0; i < List.Count; i++)
            {
                dict.Add(List[i].ID, List[i]);
            }
            return dict;
        }
        public static Dictionary<int, Region> ListToDictionary(List<Region> List)
        {
            Dictionary<int, Region> dict = new Dictionary<int, Region>();

            for (int i = 0; i < List.Count; i++)
            {
                dict.Add(List[i].ID, List[i]);
            }
            return dict;
        }

        public static List<Tile> DictionaryToList(Dictionary<CoordInt, Tile> dict)
        {
            List<Tile> List = new List<Tile>();

            foreach (Tile t in dict.Values)
            {
                List.Add(t);
            }

            return List;
        }
        public static List<Chunk> DictionaryToList(Dictionary<CoordInt, Chunk> dict)
        {
            List<Chunk> List = new List<Chunk>();

            foreach (Chunk t in dict.Values)
            {
                List.Add(t);
            }
            return List;
        }
        public static List<Room> DictionaryToList(Dictionary<int, Room> dict)
        {
            List<Room> List = new List<Room>();

            foreach (Room t in dict.Values)
            {
                List.Add(t);
            }
            return List;
        }
        public static List<Region> DictionaryToList(Dictionary<int, Region> dict)
        {
            List<Region> List = new List<Region>();

            foreach (Region t in dict.Values)
            {
                List.Add(t);
            }
            return List;
        }
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

    public enum ItemType
    {
        None,
        Attack,
        Auxiliar,
        Magic,
        Especial,
        Taunt,
        Item,
        Effect
    }

    #region Effects

    public enum ActivationType
    {
        None,
        Once,
        Periodical,
        Triggered
    }

    #endregion

    #region FileReader

    public enum state
    {
        None,
        Definition,
        NewEffect,
        SetModifiedBy,
        SetInflictIfConditions,
        SetTriggerIfConditions,
        SetStopIfConditions,
        SetRemoveIfConditions,
        SetSprite,
        GetSpriteSizes
    }

    public enum Modifiable
    {
        Damage,
        Duration,
        Interval
    }

    public enum OperatorType
    {

        LowerThan,
        LowerOrEqualsThan,
        EqualsTo,
        BiggerThan,
        BiggerOrEqualsThan,
        Have,
        NotHave
    }

    #endregion
}