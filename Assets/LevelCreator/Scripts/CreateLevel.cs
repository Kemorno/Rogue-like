using UnityEngine;
using Resources;
using Enums;
using System;
using System.Collections.Generic;

public class CreateLevel : MonoBehaviour
{
    public static Dictionary<CoordInt, Tile> Map = new Dictionary<CoordInt, Tile>();

    public bool drawGizmos = false;

    public bool randomSeed = false;
    public string globalSeed = "";

    public int size = 10;
    public int comparisonFactor = 4;
    public roomSize RoomSize;
    public roomType RoomType;
    public roomClass RoomClass;
    //public int RoomPasses = 5;
    //public int DistanceBetweenRooms = 10;

    [Range(0, 100)]
    public int randomFillPercent = 50;
    [Range(0,5)]
    public int smoothMultiplier = 4;

    public Vector2Int mousePos;
    public GameObject guide;

    List<Room> Rooms = new List<Room>();

    System.Random globalPrng = new System.Random();

    private void Awake()
    {
        if(globalSeed == "")
        {
            globalSeed = Seed.GenerateSeed(new System.Random(DateTime.Now.GetHashCode()));
            globalPrng = new System.Random(globalSeed.GetHashCode());
        }
    }

    private void Update()
    {
        mousePos = new Vector2Int(Mathf.FloorToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).x), Mathf.FloorToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).y));
        if(guide != null)
            guide.transform.position = new Vector3(mousePos.x + .5f, mousePos.y + .5f);

        if (Input.GetMouseButtonDown(0))
        {
            System.Diagnostics.Stopwatch roomCreation = new System.Diagnostics.Stopwatch();
            roomCreation.Start();

            Room room = newRoom(new CoordInt(mousePos.x, mousePos.y), new RoomSettings(smoothMultiplier, randomFillPercent, comparisonFactor, RoomSize, RoomType, RoomClass));
            if (room != null)
            {
                Rooms.Add(room);
                Debug.Log("Took " + (roomCreation.ElapsedTicks / 100000f).ToString("0.000") + "ms to create the whole room");
                Debug.Log(room.ToString());
                GenerateMesh();
            }
            else
                Debug.Log("Could not create Room");

            roomCreation.Stop();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            globalSeed = Seed.GenerateSeed(new System.Random(DateTime.Now.GetHashCode()));
            globalPrng = new System.Random(globalSeed.GetHashCode());
            Rooms = new List<Room>();
            Map.Clear();
        }
    }

    Room CreateRoom(CoordInt startCoord, RoomSettings Settings)
    {
        int Count = 0;
        restart:
        Room room = newRoom(startCoord, Settings);

        if (room == null)
        {
            if (Count <= 3)
            {
                Count++;
                goto restart;
            }
            else
                return null;
        }
        else
            return room;
    }

    Room newRoom(CoordInt startCoord, RoomSettings Settings)
    {
        Dictionary<CoordInt, Tile> roomMap = new Dictionary<CoordInt, Tile>();
        Settings.SetSeed(Seed.GenerateSeed(globalPrng));
        Room room = new Room(Rooms.Count, Settings);

        {
            System.Random roomPrng = new System.Random(Settings.Seed.GetHashCode());

            for (int x = startCoord.X - (int)Settings.Size; x <= startCoord.X + (int)Settings.Size; x++)
            {
                for (int y = startCoord.Y - (int)Settings.Size; y <= startCoord.Y + (int)Settings.Size; y++)
                {
                    CoordInt coord = new CoordInt(x, y);

                    Tile tile = (Map.ContainsKey(coord)) ? Map[coord] : new Tile(coord, (roomPrng.Next(0, 100) < Settings.RandomFillPercent) ? tileType.Wall : tileType.Floor);


                    if (x >= startCoord.X-1 && y >= startCoord.Y-1 && x <= startCoord.X+1 && y <= startCoord.Y+1)
                    {
                        tile.Type = tileType.Floor;
                    }
                    roomMap.Add(coord, tile);
                    if (Map.ContainsKey(coord))
                        roomMap[tile.Coord].Type = tileType.Wall;
                    if (startCoord == coord)
                        room.SetCenterTile(tile);
                }
            }
        }//Room Creation
        {
            for (int smooth = 0; smooth < Settings.SmoothingMultiplier; smooth++)
            {
                foreach (Tile tile in roomMap.Values)
                {
                    int wallCount = CountNearWallTiles(tile.Coord, roomMap);

                    if (wallCount > Settings.ComparisonFactor)
                        tile.Type = tileType.Wall;
                    else if (wallCount < Settings.ComparisonFactor)
                        tile.Type = tileType.Floor;
                }
            }
        }//Room Smoothing
        {
            if (Settings.SmoothingMultiplier > 0)
            {
                {
                    Tile centerTile = GetNearestTile(room.CenterTile,room.RoomId, roomMap);
                    if (centerTile == null)
                        return null;
                    else
                        room.SetCenterTile(centerTile);
                }//Check Central Tile
                {
                    List<Tile> FloorTiles = GetTiles(room.CenterTile, room.RoomId, roomMap);
                    if (FloorTiles == null)
                        return null;
                    else
                        room.SetFloorTiles(FloorTiles);
                }//Check for Colliding Floor Tiles
                {
                    List<Tile> WallTiles = GetWallTiles(room.CenterTile, room.RoomId, roomMap);
                    if (WallTiles == null)
                        return null;
                    else
                        room.SetWallTiles(WallTiles);
                }//Check for Colliding Wall Tiles
                {
                    if (room.FloorTiles.Count < (size * size) / 3)
                        return null;
                    else//Room is completly valid
                    {
                        room.SetTiles();

                        foreach (Tile tile in room.FloorTiles)
                        {
                            tile.RoomId = room.RoomId;
                            tile.Class = room.Settings.Class;
                            if (!Map.ContainsKey(tile.Coord))
                                Map.Add(tile.Coord, tile);
                            else
                                Map[tile.Coord] = roomMap[tile.Coord];
                        }
                        foreach (Tile tile in room.WallTiles)
                        {
                            tile.RoomId = room.RoomId;
                            tile.Class = room.Settings.Class;
                            if (!Map.ContainsKey(tile.Coord))
                                Map.Add(tile.Coord, tile);
                            else
                                Map[tile.Coord] = roomMap[tile.Coord];
                        }
                    }
                }//Check Room Size
            }
        }//Check if Room Valid
        return room;
    }
    
    void SetRoomOrigins()
    {/*
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
        }*/
    }

    Tile GetNearestTile(Tile Start, int RoomId, Dictionary<CoordInt, Tile> Map, tileType TypeToSearch = tileType.Floor)
    {
        List<CoordInt> mapFlags = new List<CoordInt>();
        Queue<Tile> queue = new Queue<Tile>();

        if (Start.Type == TypeToSearch && (Start.RoomId == -1 || Start.RoomId == RoomId))
            return Start;
        else
        {
            mapFlags.Add(Start.Coord);
            queue.Enqueue(Start);
        }

        while (queue.Count > 0)
        {
            Tile tile = queue.Dequeue();

            for (int NeighbourX = tile.Coord.X - 1; NeighbourX <= tile.Coord.X + 1; NeighbourX++)
            {
                for (int NeighbourY = tile.Coord.Y - 1; NeighbourY <= tile.Coord.Y + 1; NeighbourY++)
                {
                    CoordInt curCoord = new CoordInt(NeighbourX, NeighbourY);
                    if (tile.Coord.isAdjacent(curCoord))
                        if (!mapFlags.Contains(curCoord))
                        {
                            mapFlags.Add(curCoord);
                            Tile nextTile = Map.ContainsKey(curCoord) ? Map[curCoord] : null;

                            if (nextTile != null)
                            {
                                if (nextTile.RoomId == -1 || nextTile.RoomId == RoomId)
                                {
                                    if (nextTile.Type == TypeToSearch)
                                        return nextTile;
                                    else
                                        queue.Enqueue(nextTile);
                                }
                            }
                            else
                            {
                                Debug.Log("Could not find center tile");
                                return null;
                            }
                        }
                }
            }
        }
        return null;
    }
    List<Tile> GetTiles(Tile Start,int RoomID, Dictionary<CoordInt, Tile> Map, tileType typeToSearch = tileType.Floor)
    {
        Tile _tile = Start;
        List<Tile> tiles = new List<Tile>();
        List<CoordInt> mapFlags = new List<CoordInt>();

        Queue<Tile> queue = new Queue<Tile>();
        if (Start.RoomId == -1 || Start.RoomId == RoomID)
        {
            queue.Enqueue(_tile);
            mapFlags.Add(Start.Coord);
        }
        else
            return null;

        while (queue.Count > 0)
        {
            Tile tile = queue.Dequeue();
            if (tile.Type == typeToSearch)
                tiles.Add(tile);

            for(int NeighbourX = tile.Coord.X - 1; NeighbourX <= tile.Coord.X + 1; NeighbourX++)
            {
                for (int NeighbourY = tile.Coord.Y - 1; NeighbourY <= tile.Coord.Y + 1; NeighbourY++)
                {
                    CoordInt curCoord = new CoordInt(NeighbourX, NeighbourY);
                    if(tile.Coord.isAdjacent(curCoord))
                        if(!mapFlags.Contains(curCoord))
                        {
                            mapFlags.Add(curCoord);
                            Tile nextTile = Map.ContainsKey(curCoord) ? Map[curCoord] : null;
                            if (nextTile != null)
                            {
                                if(nextTile.Type == typeToSearch)
                                {
                                    if (Start.RoomId == -1 || Start.RoomId == RoomID)
                                        queue.Enqueue(nextTile);
                                    else
                                        return null;
                                }
                            }
                        }
                }
            }
        }

        return tiles;
    }
    List<Tile> GetWallTiles(Tile Start, int RoomID, Dictionary<CoordInt, Tile> Map)
    {
        Tile _tile = Start;
        List<Tile> tiles = new List<Tile>();
        List<CoordInt> mapFlags = new List<CoordInt>();
        Enums.tileType typeToSearch = Enums.tileType.Wall;

        Queue<Tile> queue = new Queue<Tile>();
        if (_tile.RoomId == -1 || _tile.RoomId == RoomID)
        {
            queue.Enqueue(_tile);
            mapFlags.Add(_tile.Coord);
        }
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
                    if (!mapFlags.Contains(curCoord))
                    {
                        mapFlags.Add(curCoord);
                        Tile nextTile = Map.ContainsKey(curCoord)?Map[curCoord] : null;
                        if (nextTile != null)
                        {
                            if (nextTile.RoomId == -1 || nextTile.RoomId == RoomID)
                            {
                                if (nextTile.Type == typeToSearch)
                                    tiles.Add(nextTile);
                                else
                                    queue.Enqueue(nextTile);
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                }
            }
        }

        return tiles;
    }
    int CountNearWallTiles(CoordInt Coord, Dictionary<CoordInt, Tile> Map)
    {
        int Count = 0;

        for(int NeighbourX = Coord.X -1; NeighbourX <= Coord.X +1; NeighbourX++)
        {
            for (int NeighbourY = Coord.Y - 1; NeighbourY <= Coord.Y + 1; NeighbourY++)
            {
                CoordInt curCoord = new CoordInt(NeighbourX, NeighbourY);
                if (curCoord != Coord)
                {
                    Tile nextTile = Map.ContainsKey(curCoord) ? Map[curCoord] : null;
                    if (nextTile != null)
                    {
                        if (nextTile.Type == tileType.Wall)
                            Count++;
                    }
                    else if(Coord.isAdjacent(curCoord))
                        return 16;
                }
            }
        }
        return Count;
    }
    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            if (Map.Count > 0)
            {
                foreach (Tile tile in Map.Values)
                {
                    Vector3 pos = new Vector3(tile.Coord.X, tile.Coord.Y);

                    switch (tile.Class)
                    {
                        case roomClass.Neutral:
                            Gizmos.color = Color.white;
                            break;
                        case roomClass.Scientific:
                            Gizmos.color = Color.blue;
                            break;
                        case roomClass.Physical:
                            Gizmos.color = Color.red;
                            break;
                        case roomClass.Spiritual:
                            Gizmos.color = Color.yellow;
                            break;
                        case .roomClass.Social:
                            Gizmos.color = Color.green;
                            break;
                    }

                    if (tile.Type == tileType.Wall)
                        Gizmos.color = Color.black;
                    else if(tile.Type == tileType.Floor)
                        Gizmos.color = Color.white;

                    Gizmos.DrawCube(pos, Vector3.one);
                    if(tile.RoomId >= 0)
                    {
                        Gizmos.color = Color.cyan;
                        Gizmos.DrawCube(pos, Vector3.one * 0.3f);
                    }
                }

                foreach (Room room in Rooms)
                {/*
                    foreach (Tile tile in room.Tiles)
                    {
                        Vector3 pos = new Vector3(tile.Coord.X, tile.Coord.Y);

                        if (tile == room.CenterTile)
                        {
                            Gizmos.color = Color.green;
                            Gizmos.DrawCube(pos, Vector3.one * 0.5f);
                        }
                        else
                        {
                            Gizmos.color = Color.cyan;
                            Gizmos.DrawCube(pos, Vector3.one * 0.3f);
                        }
                    }*/
                }
            }
        }
    }
    void GenerateMesh()
    {
        GameObject[] go = GameObject.FindGameObjectsWithTag("Map");

        foreach(GameObject GO in go)
        {
            Destroy(GO);
        }

        foreach (Tile tile in Map.Values)
        {
            GameObject mesh = GameObject.CreatePrimitive(PrimitiveType.Cube);
            mesh.tag = "Map";
            mesh.transform.position = new Vector3(tile.Coord.X, tile.Coord.Y);
            mesh.GetComponent<MeshRenderer>().material.color = (tile.Type == tileType.Wall) ? Color.black : Color.white;
        }

    }
}