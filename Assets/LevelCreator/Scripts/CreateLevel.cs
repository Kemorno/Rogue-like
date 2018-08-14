using UnityEngine;
using Resources;
using System;
using System.Collections.Generic;

public class CreateLevel : MonoBehaviour
{
    Dictionary<CoordInt, Tile> mapDict = new Dictionary<CoordInt, Tile>();

    public bool drawGizmos = false;

    public bool randomSeed = false;
    public string seed = "";

    public int size = 10;
    //public int RoomPasses = 5;
    //public int DistanceBetweenRooms = 10;

    [Range(0, 100)]
    public int randomFillPercent = 50;
    [Range(0,5)]
    public int smoothMultiplier = 4;

    List<Room> Rooms;

    System.Random prng = new System.Random();

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            System.Diagnostics.Stopwatch roomCreation = new System.Diagnostics.Stopwatch();
            roomCreation.Start();
            CreateRoom();
            roomCreation.Stop();
            Debug.Log("Took " + roomCreation.ElapsedMilliseconds + "ms to create the whole room");
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            seed = "";
            Rooms = new List<Room>();
            mapDict = new Dictionary<CoordInt, Tile>();
        }
    }

    void CreateRoom()
    {
        Room room = new Room(Rooms.Count);
        mapDict = new Dictionary<CoordInt, Tile>();
        if (seed == "" || randomSeed)
            prng = new System.Random(System.DateTime.Now.GetHashCode());
        else
            prng = new System.Random(seed.GetHashCode());

        System.Diagnostics.Stopwatch assignTiles = new System.Diagnostics.Stopwatch();
        assignTiles.Start();
        for (int x = -size / 2; x <= size / 2; x++)
        {
            for (int y = -size/2; y <= size / 2; y++)
            {
                CoordInt Coord = new CoordInt(x,y);
                //if (Vector2.Distance(Pos, Vector2Int.zero) < size/2) //CIRCLE
                //if (y - size * (3 / 4f) < x && x < y + size * (3 / 4f) && y - size * (3 / 4f) < -x && -x < y + size * (3 / 4f)) //HEXAGON
                if (true)
                {
                    Tile tile = new Tile(Coord, (prng.Next(0, 100) < randomFillPercent) ? Enums.tileType.Wall : Enums.tileType.Floor);
                    mapDict.Add(Coord, tile);
                    if (x == 0 && y == 0)
                        room.CenterTile = mapDict[Coord];
                }
            }
        }
        assignTiles.Stop();
        Debug.Log("Took " + assignTiles.ElapsedMilliseconds + "ms to assign all tiles");

        System.Diagnostics.Stopwatch smoothing = new System.Diagnostics.Stopwatch();
        smoothing.Start();
        for (int i = 0; i < smoothMultiplier; i++)
        {
            System.Diagnostics.Stopwatch foreachloop = new System.Diagnostics.Stopwatch();
            foreachloop.Start();

            List<long> TileTimes = new List<long>();
            foreach(Tile tile in mapDict.Values)
            {
                System.Diagnostics.Stopwatch eachTile = new System.Diagnostics.Stopwatch();
                eachTile.Start();
                int wallCount = CountNearWallTiles(tile.Coord);

                if (wallCount > 6)
                    tile.Type = Enums.tileType.Wall;
                else if (wallCount < 4)
                    tile.Type = Enums.tileType.Floor;
                eachTile.Stop();
                TileTimes.Add(eachTile.ElapsedTicks);

            }
            foreachloop.Stop();

            long timeDelta = 0;
            foreach(long time in TileTimes)
            {
                timeDelta += time;
            }
            Debug.Log("Took " + foreachloop.ElapsedMilliseconds + "ms to smooth the room\nPass #" + (i+1) + "/" + smoothMultiplier +
                " Arround " + (timeDelta /= mapDict.Count) + "ticks per tile (" + Tiles.Count + " total)");
        }
        smoothing.Stop();
        Debug.Log("Took " + smoothing.ElapsedMilliseconds + "ms to smooth the room "+ smoothMultiplier + " times");
        Tile Ntfc = GetNearestTile(room.CenterTile);//Nearest Tile From Center
        room.FloorTiles = GetTiles(Ntfc, Enums.tileType.Floor, room.RoomID);
        room.WallTiles = GetWallTiles(Ntfc, room.RoomID);
        if(room.FloorTiles == null || room.WallTiles == null)
        {
            Debug.Log("Room is Colliding");
        }
        Debug.Log(room.ToString());
    }

    /*
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
    */

    Tile GetNearestTile(Tile Start, Enums.tileType TypeToSearch = Enums.tileType.Floor)
    {
        Tile _tile = null;
        List<CoordInt> mapFlags = new List<CoordInt>();

        Queue<Tile> queue = new Queue<Tile>();
        if (Start.Type == TypeToSearch)
            _tile = Start;
        else
            queue.Enqueue(Start);

        while (queue.Count > 0)
        {
            Tile tile = queue.Dequeue();
            mapFlags.Add(tile.Coord);

            for (int NeighbourX = tile.Coord.X - 1; NeighbourX <= tile.Coord.X + 1; NeighbourX++)
            {
                for (int NeighbourY = tile.Coord.Y - 1; NeighbourY <= tile.Coord.Y + 1; NeighbourY++)
                {
                    CoordInt curCoord = new CoordInt(NeighbourX, NeighbourY);

                    Tile nextTile = mapDict.ContainsKey(curCoord) ? mapDict[curCoord] : null;

                    if (nextTile != null)
                    {
                        if (nextTile.Type != TypeToSearch)
                            queue.Enqueue(nextTile);
                        else {
                            return nextTile;
                        }
                    }
                }
            }
        }

        return _tile;
    }
    List<Tile> GetTiles(Tile Start, Enums.tileType typeToSearch,int RoomID = -1, bool SearchDiagonal = false, int Radius = 1)
    {
        bool isColliding = false;
        List<Tile> tiles = new List<Tile>();
        List<CoordInt> mapFlags = new List<CoordInt>();

        Queue<Tile> queue = new Queue<Tile>();
        if (Start.RoomId == -1 || Start.RoomId == RoomID)
            queue.Enqueue(Start);
        else
            return null;

        while (queue.Count > 0)
        {

            Tile tile = queue.Dequeue();
            if (tile.Type == typeToSearch)
                tiles.Add(tile);

            for(int NeighbourX = tile.Coord.X - Radius; NeighbourX <= tile.Coord.X + Radius; NeighbourX++)
            {
                for (int NeighbourY = tile.Coord.Y - Radius; NeighbourY <= tile.Coord.Y + Radius; NeighbourY++)
                {
                    CoordInt curCoord = new CoordInt(NeighbourX, NeighbourY);
                    if (!mapFlags.Contains(curCoord))
                    {
                        Tile nextTile = mapDict.ContainsKey(curCoord) ? mapDict[curCoord] : null;
                        if (nextTile != null)
                        {
                            if (nextTile.RoomId == -1 || nextTile.RoomId == RoomID)
                            {
                                if (!SearchDiagonal)
                                {
                                    if (NeighbourX == tile.Coord.x || NeighbourY == tile.Coord.y)
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
                            else
                            {
                                return null;
                            }
                        }
                        mapFlags.Add(curCoord);
                    }
                }
            }
        }

        return tiles;
    }
    List<Tile> GetWallTiles(Tile Start, int RoomID = -1, Enums.tileType typeToSearch = Enums.tileType.Wall)
    {
        List<Tile> tiles = new List<Tile>();
        List<CoordInt> mapFlags = new List<CoordInt>();

        Queue<Tile> queue = new Queue<Tile>();
        if (Start.RoomId == -1)
            queue.Enqueue(Start);
        else
            return null;

        while (queue.Count > 0)
        {
            Tile tile = queue.Dequeue();

            for (int NeighbourX = tile.Coord.X - 1; NeighbourX <= tile.Coord.X + 1; NeighbourX++)
            {
                for (int NeighbourY = tile.Coord.Y - 1; NeighbourY <= tile.Coord.Y + 1; NeighbourY++)
                {
                    CoordInt curCoord = new CoordInt(NeighbourX, NeighbourY);
                    if (!mapFlags.Contains(curCoord)) {
                        Tile nextTile = mapDict.ContainsKey(curCoord) ? mapDict[curCoord] : null;
                        if (nextTile != null)
                        {
                            if (nextTile.RoomId == -1 || nextTile.RoomId == RoomID)
                            {
                                if (nextTile.Type == Enums.tileType.Floor)
                                    queue.Enqueue(mapDict[curCoord]);
                                else (nextTile.Type == Enums.tileType.Wall)
                                    tiles.Add(mapDict[curCoord]);
                            }
                            else
                                return null;
                            mapFlags.Add(curCoord);
                        }
                    }
                }
            }
        }

        return tiles;
    }
    int CountNearWallTiles(CoordInt Coord)
    {
        int Count = 0;

        for(int NeighbourX = Coord.X -1; NeighbourX <= Coord.X +1; NeighbourX++)
        {
            for (int NeighbourY = Coord.Y - 1; NeighbourY <= Coord.Y + 1; NeighbourY++)
            {
                CoordInt curCoord = new CoordInt(NeighbourX, NeighbourY);
                if (curCoord != Coord)
                {
                    if(mapDict.ContainsKey(curCoord))
                        if (mapDict[curCoord].Type == Enums.tileType.Wall)
                            Count++;
                    else
                        Count = 16;
                }
            }
        }
        return Count;
    }
    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            if (mapDict.Count != 0)
            {
                foreach(Tile tile in mapDict.Values)
                {
                    Vector3 pos = new Vector3(tile.Coord.x, tile.Coord.y);

                    switch (tile.Class)
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
                    if (tile.Type == Enums.tileType.Wall)
                        Gizmos.color = Color.black;
                    Gizmos.DrawCube(pos, Vector3.one);

                    Gizmos.color = Color.red;
                    Gizmos.DrawCube(pos, Vector3.one * 0.1f);
                }
            }
        }
    }
}