using UnityEngine;
using Resources;
using Enums;
using System;
using System.Collections.Generic;

public class CreateLevel : MonoBehaviour
{
    public Dictionary<CoordInt, Tile> Map;

    public Sprite Cell;
    public bool drawGizmos = false;

    public bool randomSeed = false;
    public string globalSeed = "";

    public int Size = 10;
    public int comparisonFactor = 4;
    public roomSize RoomSize;
    public roomType RoomType;
    public roomClass RoomClass;
    GameObject grouper;

    [Range(0, 100)]
    public int randomFillPercent = 50;
    [Range(0,5)]
    public int smoothMultiplier = 4;

    public Vector2Int mousePos;
    public GameObject guide;

    List<Room> Rooms;
    public RoomSettings Settings;

    System.Random globalPrng = new System.Random();

    private void Awake()
    {
        Map = new Dictionary<CoordInt, Tile>();
        if (globalSeed == "" || !Seed.isSeedValid(globalSeed))
            ResetMap(true);
        else
            ResetMap();

        grouper = new GameObject();
        Rooms = new List<Room>();
    }

    private void Update()
    {
        Settings = new RoomSettings(smoothMultiplier, randomFillPercent, comparisonFactor, RoomSize, RoomType, RoomClass);
        mousePos = new Vector2Int(Mathf.FloorToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).x), Mathf.FloorToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).y));
        if(guide != null)
            guide.transform.position = new Vector3(mousePos.x + .5f, mousePos.y + .5f);

        if (Input.GetMouseButtonDown(0))
        {
            System.Diagnostics.Stopwatch roomCreation = new System.Diagnostics.Stopwatch();
            roomCreation.Start();

            Room room = CreateRoom(mousePos, Settings);
            if (room != null)
            {
                Rooms.Add(room);
                roomCreation.Stop();
                Debug.Log("Took " + (roomCreation.ElapsedTicks / 100000f).ToString("0.000") + "ms to create the whole room");
                Debug.Log(room.ToString());
                GenerateMesh();
            }
            else
                Debug.Log("Could not create Room");

        }
        if (Input.GetKeyDown(KeyCode.G))
            CreateMap(Size);
        if (Input.GetKeyDown(KeyCode.R))
            ResetMap();
    }

    private void ResetMap(bool newSeed = false)
    {
        if (newSeed)
        {
            globalSeed = Seed.GenerateSeed(new System.Random(DateTime.Now.GetHashCode()));
            globalPrng = new System.Random(globalSeed.GetHashCode());
        }
        else
        {
            globalPrng = new System.Random(globalSeed.GetHashCode());
        }

        Rooms = new List<Room>();
        Map.Clear();
        GenerateMesh();
    }
    public Room CreateRoom(CoordInt startCoord, RoomSettings Settings)
    {
        int Count = 0;
        restart:
        Room room = newRoom(startCoord, Settings);

        if (room == null)
        {
            if (Count <= 5)
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

            for (int x = startCoord.x - (int)room.Settings.Size; x <= startCoord.x + (int)room.Settings.Size; x++)
            {
                for (int y = startCoord.y - (int)room.Settings.Size; y <= startCoord.y + (int)room.Settings.Size; y++)
                {
                    CoordInt coord = new CoordInt(x, y);

                    int random = roomPrng.Next(0, 100);

                    Tile tile = (Map.ContainsKey(coord)) ? new Tile(Map[coord]) : new Tile(coord, (random < room.Settings.RandomFillPercent) ? tileType.Wall : tileType.Floor);
                    roomMap.Add(coord, tile);
                    
                    if (x >= startCoord.x - 1 && y >= startCoord.y - 1 && x <= startCoord.x + 1 && y <= startCoord.y + 1)
                    {
                        if (tile.RoomId != room.RoomId && tile.RoomId != -1)
                            return null;
                        if (startCoord == coord)
                            room.SetCenterTile(roomMap[coord]);

                        roomMap[coord].SetType(tileType.Floor);
                    }
                    if (Map.ContainsKey(coord))
                        roomMap[coord].SetType((random < room.Settings.RandomFillPercent) ? tileType.Wall : tileType.Floor);

                }
            }
        }//Room Creation
        {
            for (int smooth = 0; smooth < room.Settings.SmoothingMultiplier; smooth++)
            {
                foreach (Tile tile in roomMap.Values)
                {
                    int wallCount = CountNearWallTiles(tile.Coord, roomMap);

                    if (wallCount > room.Settings.ComparisonFactor)
                        tile.SetType(tileType.Wall);
                    else if (wallCount < room.Settings.ComparisonFactor)
                        tile.SetType(tileType.Floor);
                }
            }
        }//Room Smoothing
        {
            if (Settings.SmoothingMultiplier > 0)
            {
                {
                    Tile centerTile = GetNearestTile(room.CenterTile, room.RoomId, roomMap);
                    if (centerTile == null)
                        return null;
                    else
                    {
                        Debug.Log("Room Center Tile is Valid");
                        room.SetCenterTile(centerTile);
                    }
                }//Check Central Tile
                {
                    List<Tile> FloorTiles = GetTiles(room.CenterTile, room.RoomId, roomMap);
                    if (FloorTiles == null)
                        return null;
                    else
                    {
                        Debug.Log("Room has no colliding Floor Tiles");
                        room.SetFloorTiles(FloorTiles);
                    }
                }//Check for Colliding Floor Tiles
                {
                    List<Tile> WallTiles = GetWallTiles(room.CenterTile, room.RoomId, roomMap);
                    if (WallTiles == null)
                        return null;
                    else
                    {
                        Debug.Log("Room has no colliding Wall Tiles");
                        room.SetWallTiles(WallTiles);
                    }
                }//Check for Colliding Wall Tiles
                {
                    if (room.FloorTiles.Count < ((int)room.Settings.Size * (int)room.Settings.Size) / 2)
                        return null;
                    else//Room is completly valid
                    {
                        Debug.Log("Room is completly valid");
                        room.FinishRoom();

                        foreach (Tile tile in room.Tiles)
                        {
                            if (tile.RoomId == -1 || tile.RoomId == room.RoomId)
                            {
                                tile.Coord.SetTile(tile);
                                Debug.Log(tile.ToLongString());
                                tile.RoomId = room.RoomId;
                                tile.Class = room.Settings.Class;
                                if (!Map.ContainsKey(tile.Coord))
                                    Map.Add(tile.Coord, tile);
                                else
                                    Map[tile.Coord] = roomMap[tile.Coord];
                            }
                        }
                    }
                }//Check Room Size
            }
        }//Check if Room Valid

        return room;
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

            for (int NeighbourX = tile.Coord.x - 1; NeighbourX <= tile.Coord.x + 1; NeighbourX++)
            {
                for (int NeighbourY = tile.Coord.y - 1; NeighbourY <= tile.Coord.y + 1; NeighbourY++)
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

            for(int NeighbourX = tile.Coord.x - 1; NeighbourX <= tile.Coord.x + 1; NeighbourX++)
            {
                for (int NeighbourY = tile.Coord.y - 1; NeighbourY <= tile.Coord.y + 1; NeighbourY++)
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
        tileType typeToSearch = tileType.Wall;

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

            for (int NeighbourX = tile.Coord.x - 1; NeighbourX <= tile.Coord.x + 1; NeighbourX++)
            {
                for (int NeighbourY = tile.Coord.y - 1; NeighbourY <= tile.Coord.y + 1; NeighbourY++)
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

        for(int NeighbourX = Coord.x -1; NeighbourX <= Coord.x +1; NeighbourX++)
        {
            for (int NeighbourY = Coord.y - 1; NeighbourY <= Coord.y + 1; NeighbourY++)
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
        if (drawGizmos && Map != null)
        {
            if (Map.Count > 0)
            {
                foreach (Tile tile in Map.Values)
                {
                    Vector3 pos = new Vector3(tile.Coord.x, tile.Coord.y);

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
                        case roomClass.Social:
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
            GameObject mesh = new GameObject();
            mesh.AddComponent<SpriteRenderer>();
            mesh.GetComponent<SpriteRenderer>().sprite = Cell;
            mesh.GetComponent<SpriteRenderer>().color = (tile.Type == tileType.Wall) ? Color.black : Color.white;

            mesh.tag = "Map";
            mesh.name = tile.Coord.ToString();
            mesh.transform.parent = grouper.transform;
            mesh.transform.position = new Vector3(tile.Coord.x, tile.Coord.y);
        }

    }
    public void CreateMap(int Size)
    {
        ResetMap();
        List<CoordInt> mapCheck = new List<CoordInt>();
        for (int x = -Size; x <= Size; x++)
        {
            for (int y = -Size; y <= Size; y++)
            {
                CoordInt curCoord = new CoordInt(x, y);
                bool compatible = true;
                for (int neighbourX = x - 2; neighbourX <= x + 2; neighbourX++)
                {
                    for (int neighbourY = y - 2; neighbourY <= y + 2; neighbourY++)
                    {
                        if (mapCheck.Contains(new CoordInt(neighbourX, neighbourY)))
                        {
                            compatible = false;
                            goto incompatible;
                        }
                    }
                }
                incompatible:
                if (compatible)
                {
                    Room room = CreateRoom(curCoord, Settings);
                    if (room != null)
                    {
                        Rooms.Add(room);
                        foreach (CoordInt tile in room.Tiles)
                        {
                            if (!mapCheck.Contains(tile))
                                mapCheck.Add(tile);
                        }
                    }
                }
                else
                    Debug.Log("Couldn't Create Room");
            }
        }
        Debug.Log("Map Info - Size " + Size + " Number of Rooms " + Rooms.Count);
    }
    public Tile[,] DictionaryToArray(Dictionary<CoordInt, Tile> map)
    {
        int minX = 0;
        int minY = 0;
        int maxX = 0;
        int maxY = 0;

        bool firstLoop = true;

        List<int> X = new List<int>();
        List<int> Y = new List<int>();

        foreach (CoordInt coord in map.Keys)
        {
            if (firstLoop)
            {
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
        Tile[,] array = new Tile[maxX - minX, maxY - minY];
        foreach (CoordInt coord in map.Keys)
        {
            array[coord.x - minX, coord.y - minY] = map[coord];
        }
        return array;
    }
}