using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreateLevel : MonoBehaviour
{
    public bool drawGizmos = false;
    public bool showDebug = false;

    public int size = 10;
    public int RoomPasses = 5;
    public int DistanceBetweenRooms = 10;

    [Range(0, 100)]
    public int randomFillPercent = 50;
    [Range(0,5)]
    public int smoothMultiplier = 4;

    System.Random prng = new System.Random();

    Vector2Int Start;
    List<List<Tile>> RoomOriginTiles = new List<List<Tile>>();
    List<Tile> Tiles = new List<Tile>();
    List<Room> Rooms = new List<Room>();

    private void Awake()
    {
        Start = new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.x));
        //SetRoomOrigins();
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //SetRoomOrigins();
        }
        if (Input.GetMouseButtonDown(1))
        {
            CreateRoom();
        }
    }

    void CreateRoom()
    {
        Tile[,] map = new Tile[size, size];
        Room room = new Room(Rooms.Count);
        System.DateTime startTime = System.DateTime.Now;
        prng = new System.Random(System.DateTime.Now.GetHashCode());
        Tile CenterTile = null;
        for(int x = -size / 2; x <= size / 2; x++)
        {
            for (int y = -size/2; y <= size / 2; y++)
            {
                if (y - size * (3 / 4f) < x && x < y + size * (3 / 4f))
                    if (y - size * (3 / 4f) < -x && -x < y + size * (3 / 4f))
                    {
                        Vector2Int Pos = new Vector2Int(x, y);
                        if (true)
                        {
                            Tile tile = new Tile(Pos)
                            {
                                Type = (prng.Next(0, 100) < randomFillPercent) ? Enums.tileType.Wall : Enums.tileType.Floor
                            };
                            Tiles.Add(tile);
                            if (x == 0 && y == 0)
                                CenterTile = tile;
                        }
                    }
            }
        }
        //Debug.Log("this Room has " + Tiles.Count + " Tiles");
        for (int i = 0; i < smoothMultiplier; i++)
        {
            //System.DateTime startTime2 = System.DateTime.Now;
            /*
            foreach (Tile tile in Tiles)
            {
                int wallCount = CountNearWallTiles(tile.Coord);

                if (wallCount > 6)
                    tile.Type = Enums.tileType.Wall;
                if (wallCount < 4)
                    tile.Type = Enums.tileType.Floor;
            }*/
                //System.TimeSpan timeTook2 = System.DateTime.Now - startTime2;
                //Debug.Log("Took " + timeTook2.TotalMilliseconds.ToString("0.000") + "ms to loop through all Tiles, can you improve it?" +
                    //"\nAbout " + (timeTook2.TotalMilliseconds / Tiles.Count).ToString("0.00000") + "ms per tile");
        }

        Tile Ntfc = GetNearestTile(CenterTile);//Nearest Tile From Center

        room.FloorTiles = GetTiles(Ntfc, Enums.tileType.Floor);
            //System.TimeSpan timeTook = System.DateTime.Now - startTime;
            //Debug.Log("Took " + timeTook.TotalMilliseconds.ToString("0.00") + "ms to create this Room");
    }

    void SetRoomOrigins()
    {
        Tiles = new List<Tile>();
        int rawDist = DistanceBetweenRooms;
        for (int i = 0; i < RoomPasses; i++)
        {
            List<Tile> PassTiles = new List<Tile>();
            rawDist += DistanceBetweenRooms;
            for(int x = -rawDist; x <= rawDist; x++)
            {
                for (int y = -rawDist; y <= rawDist; y++)
                {
                    Vector2Int Pos = new Vector2Int(x, y);
                    if (true)
                    {
                        if (Mathf.FloorToInt(Vector2.Distance(Pos, Start)) == rawDist)
                        {
                            PassTiles.Add(new Tile(Pos, Enums.tileType.Floor));
                        }
                        if (x == y)
                            Tiles.Add(new Tile(Pos));
                        if (x == -y)
                            Tiles.Add(new Tile(Pos));
                    }
                }
            }
            RoomOriginTiles.Add(PassTiles);
        }
        foreach(Tile tile in Tiles)
        {
            Vector2Int Pos = tile.Coord;
            if (tile.Type == Enums.tileType.Floor)
            {
                if (-Pos.y < Pos.x && Pos.x < Pos.y && Pos.y > 0)
                    tile.Class = Enums.roomClass.Scientific;
                if (Pos.y < -Pos.x && -Pos.x < -Pos.y && Pos.y < 0)
                    tile.Class = Enums.roomClass.Physical;
                if (-Pos.x > Pos.y && Pos.y > Pos.x && Pos.x < 0)
                    tile.Class = Enums.roomClass.Social;
                if (Pos.x > -Pos.y && -Pos.y > -Pos.x && Pos.x > 0)
                    tile.Class = Enums.roomClass.Spiritual;
            }
        }
    }

    Tile GetNearestTile(Tile Start, Enums.tileType TypeToSearch = Enums.tileType.Floor)
    {
        Tile _tile = null;
        List<Vector2Int> mapFlags = new List<Vector2Int>();

        Queue<Tile> queue = new Queue<Tile>();
        if (Start.Type == TypeToSearch)
            _tile = Start;
        else
            queue.Enqueue(Start);

        while (queue.Count > 0)
        {
            Tile tile = queue.Dequeue();
            mapFlags.Add(tile.Coord);

            for (int neighbourX = tile.Coord.x - 1; neighbourX <= tile.Coord.x + 1; neighbourX++)
            {
                for (int neighbourY = tile.Coord.y - 1; neighbourY <= tile.Coord.y + 1; neighbourY++)
                {
                    Vector2Int nextPos = new Vector2Int(neighbourX, neighbourY);
                    Tile nextTile = tile;

                    if(!mapFlags.Contains(nextTile.Coord))
                    {
                        if (nextTile.Type != TypeToSearch)
                            queue.Enqueue(nextTile);
                        else {
                            queue.Clear();
                            _tile = nextTile;
                        }
                    }
                }
            }
        }

        return _tile;
    }
    List<Tile> GetTiles(Tile Start, Enums.tileType typeToSearch, bool SearchDiagonal = false, int Radius = 1)
    {
        List<Tile> tiles = new List<Tile>();
        List<Vector2Int> mapFlags = new List<Vector2Int>();

        Queue<Tile> queue = new Queue<Tile>();
        queue.Enqueue(Start);

        while(queue.Count > 0)
        {
            Tile tile = queue.Dequeue();
            mapFlags.Add(tile.Coord);
            if (tile.Type == typeToSearch)
                tiles.Add(tile);

            for(int neighbourX = tile.Coord.x - Radius; neighbourX <= tile.Coord.x + Radius; neighbourX++)
            {
                for (int neighbourY = tile.Coord.y - Radius; neighbourY <= tile.Coord.y + Radius; neighbourY++)
                {
                    Vector2Int nextPos = new Vector2Int(neighbourX, neighbourY);
                    Tile nextTile = null;

                    if (!SearchDiagonal)
                    {
                        if (neighbourX == tile.Coord.x || neighbourY == tile.Coord.y)
                        {
                            if (nextTile.Type == typeToSearch && !mapFlags.Contains(nextTile.Coord))
                                queue.Enqueue(nextTile);
                        }
                    }
                    else
                    {
                        if (nextTile.Type == typeToSearch && !mapFlags.Contains(nextTile.Coord))
                            queue.Enqueue(nextTile);
                    }
                }
            }
        }

        return tiles;
    }
    public class Tile
    {
        public int RoomId = -1;
        public Vector2Int Coord = Vector2Int.zero;
        public Vector2Int mapCoord = Vector2Int.zero;
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
        }
    }
    public class Room
    {
        public int RoomID;
        public List<Tile> FloorTiles = new List<Tile>();
        public List<Tile> WallTiles = new List<Tile>();

        public Room(int _RoomID)
        {
            RoomID = _RoomID;
        }
    }
    int CountNearWallTiles(Vector2Int Coord)
    {
        int Count = 0;

        for(int NeighbourX = Coord.x -1; NeighbourX <= Coord.x +1; NeighbourX++)
        {
            for (int NeighbourY = Coord.y - 1; NeighbourY <= Coord.y + 1; NeighbourY++)
            {
                Tile curTile = null;
                Vector2Int curCoord = new Vector2Int(NeighbourX, NeighbourY);
                if (curCoord != Coord)
                {
                    foreach (Tile tile in Tiles)
                    {
                        if (tile.Coord == curCoord)
                            curTile = tile;
                    }
                    if (curTile != null)
                    {
                        if (curTile.Type == Enums.tileType.Wall)
                            Count++;
                    }
                    else
                    {
                        Count = 7;
                    }
                }
            }
        }
        return Count;
    }
    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            if (Tiles.Count != 0)
            {
                for (int i = 0; i < Tiles.Count; i++)
                {
                    Vector3 pos = new Vector3(Tiles[i].Coord.x, Tiles[i].Coord.y);

                    switch (Tiles[i].Class)
                    {
                        case Enums.roomClass.Neutral:
                            Gizmos.color = Color.white;
                            break;
                        case Enums.roomClass.Scientific:
                            Gizmos.color = Color.blue;
                            break;
                        case Enums.roomClass.Physical:
                            Gizmos.color = Color.red;
                            break;
                        case Enums.roomClass.Spiritual:
                            Gizmos.color = Color.yellow;
                            break;
                        case Enums.roomClass.Social:
                            Gizmos.color = Color.green;
                            break;
                    }
                    if (Tiles[i].Type == Enums.tileType.Wall)
                        Gizmos.color = Color.black;
                    Gizmos.DrawCube(pos, Vector3.one);
                }
            }
        }
    }
}