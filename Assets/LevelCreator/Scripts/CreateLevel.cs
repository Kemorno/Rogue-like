using UnityEngine;
using Resources;
using Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class CreateLevel : MonoBehaviour
{
    #region Variables
    public Dictionary<CoordInt, Tile> Map;

    public Sprite Cell;
    public Material plainMaterial;
    public bool drawGizmos = false;

    public bool randomSeed = false;
    public Seed globalSeed;
    System.Random globalPrng;

    private IEnumerator waitfor;

    public int Size = 10;
    [Range(3, 20)]
    public int noRooms = 10;

    public int dist = 1000;

    public roomSize RoomSize;
    public roomType RoomType;
    public roomClass RoomClass;
    GameObject grouper;

    [Range(0, 100)]
    public int randomFillPercent = 50;
    [Range(0, 5)]
    public int smoothMultiplier = 4;
    [Range(0, 8)]
    public int comparisonFactor = 4;

    public Vector2 floatMousePos;
    public Vector2Int mousePos;
    public GameObject guide;

    public List<Room> Rooms = new List<Room>();
    public RoomSettings Settings;
    #endregion

    public bool SeeProgress = false;
    Queue<Tuple<CoordInt, Room>> RoomCreationQueue;

    private void Awake()
    {
        RoomCreationQueue = new Queue<Tuple<CoordInt, Room>>();
        Map = new Dictionary<CoordInt, Tile>();

        if (globalSeed != null)
        {
            if (!SeedController.isSeedValid(globalSeed) || randomSeed)
                ResetMap(true);
            else
                ResetMap();
        }
        else
            ResetMap(true);

        grouper = new GameObject();
        Rooms = new List<Room>();
    }
    private void Update()
    {
        floatMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos = new Vector2Int(Mathf.FloorToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).x + .5f), Mathf.FloorToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).y + .5f));

        if (guide != null)
            guide.transform.position = new Vector3(mousePos.x, mousePos.y);

        if (Input.GetMouseButtonDown(0))
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                RoomSettings Settings = new RoomSettings(smoothMultiplier, randomFillPercent, comparisonFactor, RoomSize, RoomType, RoomClass);
                Room room = new Room(Rooms.Count, Settings);

                StartCoroutine(EnqueueRoom(mousePos, room));
            }
    }


    public void newSeed()
    {
        globalSeed = new Seed();
        reloadPrng(globalSeed);
    }
    public void reloadPrng(Seed seed)
    {
        globalPrng = new System.Random(seed.GetHashCode());
    }
    public void ResetMap(bool _newSeed = false)
    {
        if (_newSeed)
            newSeed();
        else
            reloadPrng(globalSeed);

        foreach (Room room in Rooms)
            Destroy(room.roomGo);

        Rooms = new List<Room>();
        Map.Clear();
    }
    

    public void StartGeneration()
    {
        StartCoroutine(GenerateLevel());
    }
    private IEnumerator GenerateLevel()
    {
        ResetMap(true);
        System.Random moreRooms = new System.Random(DateTime.Now.GetHashCode());
        Dictionary<CoordInt, bool> mapFlags = new Dictionary<CoordInt, bool>();
        for (int t = 0; t < 4; t++)
        {
            for (int i = 0; i < noRooms; i++)
            {
                Room room = null;
                if (Rooms.Count == 0 && t == 0)
                {
                    Debug.Log("Creating Initial Room");
                    i--;

                    RoomSettings Settings = new RoomSettings(4, 20, 4,
                        roomSize.Tiny, roomType.Spawn, roomClass.Neutral);

                    room = new Room(Rooms.Count, Settings);

                    StartCoroutine(EnqueueRoom(new CoordInt(0, 0), room));
                    while (!room.FinishedGeneration)
                    {
                        yield return new WaitForSeconds(1f);
                        room = Rooms[room.RoomId];
                    }
                }
                else
                {
                    RoomSettings Settings = new RoomSettings(4, moreRooms.Next(35, 55), 4, roomSize.Tiny, roomType.None, roomClass.Neutral);
                    Debug.Log("Creating room Nº " + i);

                    switch (t)
                    {
                        case 0:
                            Settings.SetClass(roomClass.Scientific);
                            for (int y = 0; y <= dist / 2; y++)
                            {
                                for (int x = -dist / 2; x <= dist / 2; x++)
                                {
                                    if (x < y * (6f / 9f) && x > -y * (6f / 9f))
                                    {
                                        CoordInt coord = new CoordInt(x, y);
                                        if (Map.ContainsKey(coord))
                                            continue;

                                        if (mapFlags.ContainsKey(coord))
                                            continue;

                                        mapFlags.Add(coord, true);

                                        switch (moreRooms.Next(0, 4))
                                        {
                                            case 0:
                                                Settings.SetSize(roomSize.Tiny);
                                                break;
                                            case 1:
                                                Settings.SetSize(roomSize.Small);
                                                break;
                                            case 2:
                                                Settings.SetSize(roomSize.Medium);
                                                break;
                                            case 3:
                                                Settings.SetSize(roomSize.Large);
                                                break;
                                            case 4:
                                                Settings.SetSize(roomSize.Big);
                                                break;
                                        }

                                        if (NearTiles(coord, (int)Settings.Size / 2))
                                            continue;

                                        room = new Room(Rooms.Count, Settings);

                                        StartCoroutine(EnqueueRoom(new CoordInt(x, y), room));
                                        while (!room.FinishedGeneration)
                                        {
                                            yield return new WaitForSeconds(2f);
                                            room = Rooms[room.RoomId];
                                            if (room == null)
                                                break;
                                            if (room.FinishedGeneration)
                                                break;
                                        }
                                    }
                                    if (room != null)
                                        break;
                                }
                                if (room != null)
                                    break;
                            }
                            break;
                        case 1:
                            Settings.SetClass(roomClass.Physical);
                            for (int y = 0; y >= -dist / 2; y--)
                            {
                                for (int x = -dist / 2; x <= dist / 2; x++)
                                {
                                    if (-x > y * (6f / 9f) && -x < -y * (6f / 9f))
                                    {
                                        CoordInt coord = new CoordInt(x, y);
                                        if (Map.ContainsKey(coord))
                                            continue;

                                        if (mapFlags.ContainsKey(coord))
                                            continue;

                                        mapFlags.Add(coord, true);

                                        switch (moreRooms.Next(0, 4))
                                        {
                                            case 0:
                                                Settings.SetSize(roomSize.Tiny);
                                                break;
                                            case 1:
                                                Settings.SetSize(roomSize.Small);
                                                break;
                                            case 2:
                                                Settings.SetSize(roomSize.Medium);
                                                break;
                                            case 3:
                                                Settings.SetSize(roomSize.Large);
                                                break;
                                            case 4:
                                                Settings.SetSize(roomSize.Big);
                                                break;
                                        }

                                        if (NearTiles(coord, (int)Settings.Size / 2))
                                            continue;

                                        room = new Room(Rooms.Count, Settings);

                                        StartCoroutine(EnqueueRoom(new CoordInt(x, y), room));
                                        while (!room.FinishedGeneration)
                                        {
                                            yield return new WaitForSeconds(1f);
                                            room = Rooms[room.RoomId];
                                        }
                                    }
                                    if (room != null)
                                        break;
                                }
                                if (room != null)
                                    break;
                            }
                            break;
                        case 2:
                            Settings.SetClass(roomClass.Spiritual);
                            for (int x = 0; x <= dist / 2; x++)
                            {
                                for (int y = -dist / 2; y <= dist / 2; y++)
                                {
                                    if (y < x * (6f / 9f) && y > -x * (6f / 9f))
                                    {
                                        CoordInt coord = new CoordInt(x, y);
                                        if (Map.ContainsKey(coord))
                                            continue;

                                        if (mapFlags.ContainsKey(coord))
                                            continue;

                                        mapFlags.Add(coord, true);

                                        switch (moreRooms.Next(0, 4))
                                        {
                                            case 0:
                                                Settings.SetSize(roomSize.Tiny);
                                                break;
                                            case 1:
                                                Settings.SetSize(roomSize.Small);
                                                break;
                                            case 2:
                                                Settings.SetSize(roomSize.Medium);
                                                break;
                                            case 3:
                                                Settings.SetSize(roomSize.Large);
                                                break;
                                            case 4:
                                                Settings.SetSize(roomSize.Big);
                                                break;
                                        }

                                        if (NearTiles(coord, (int)Settings.Size / 2))
                                            continue;

                                        room = new Room(Rooms.Count, Settings);

                                        StartCoroutine(EnqueueRoom(new CoordInt(x, y), room));
                                        while (!room.FinishedGeneration)
                                        {
                                            yield return new WaitForSeconds(1f);
                                            room = Rooms[room.RoomId];
                                        }
                                    }
                                    if (room != null)
                                        break;
                                }
                                if (room != null)
                                    break;
                            }
                            break;
                        case 3:
                            Settings.SetClass(roomClass.Social);
                            for (int x = 0; x >= -dist / 2; x--)
                            {
                                for (int y = -dist / 2; y <= dist / 2; y++)
                                {
                                    if (-y > x * (6f / 9f) && -y < -x * (6f / 9f))
                                    {
                                        CoordInt coord = new CoordInt(x, y);
                                        if (Map.ContainsKey(coord))
                                            continue;

                                        if (mapFlags.ContainsKey(coord))
                                            continue;

                                        mapFlags.Add(coord, true);

                                        switch (moreRooms.Next(0, 4))
                                        {
                                            case 0:
                                                Settings.SetSize(roomSize.Tiny);
                                                break;
                                            case 1:
                                                Settings.SetSize(roomSize.Small);
                                                break;
                                            case 2:
                                                Settings.SetSize(roomSize.Medium);
                                                break;
                                            case 3:
                                                Settings.SetSize(roomSize.Large);
                                                break;
                                            case 4:
                                                Settings.SetSize(roomSize.Big);
                                                break;
                                        }

                                        if (NearTiles(coord, (int)Settings.Size / 2))
                                            continue;

                                        room = new Room(Rooms.Count, Settings);

                                        StartCoroutine(EnqueueRoom(new CoordInt(x, y), room));
                                        while (!room.FinishedGeneration)
                                        {
                                            yield return new WaitForSeconds(1f);
                                            room = Rooms[room.RoomId];
                                        }
                                    }
                                    if (room != null)
                                        break;
                                }
                                if (room != null)
                                    break;
                            }
                            break;
                    }
                }
                
                if (room != null)
                {
                    if (room.HasError)
                    {
                        i--;
                        Debug.Log(room.ErrorMessage);
                        continue;
                    }
                    else
                    {
                        foreach (Tile tile in room.Tiles)
                        {
                            if (!mapFlags.ContainsKey(tile.Coord))
                                mapFlags.Add(tile.Coord, true);
                        }
                    }
                }
                else
                {
                    i--;
                }
            }
        }
    }

    bool isRunning = false;

    private IEnumerator EnqueueRoom(CoordInt startCoord, Room room)
    {
        Rooms.Add(room);
        RoomCreationQueue.Enqueue(new Tuple<CoordInt, Room>(startCoord, room));

        if (!isRunning)
            StartCoroutine(RunQueue());
        yield return null;
    }
    private IEnumerator RunQueue()
    {
        if (isRunning)
            yield return null;

        while (RoomCreationQueue.Count > 0)
        {
            isRunning = true;
            Tuple<CoordInt, Room> queue = RoomCreationQueue.Dequeue();

            Room room = queue.Item2;

            StartCoroutine(CreateRoom(queue.Item1, room));

            while (!room.FinishedGeneration)
            {
                yield return new WaitForSeconds(2f);
                room = Rooms[room.RoomId];
            }

            Debug.Log("Ended Room, Still " + RoomCreationQueue.Count + " To Go");
            isRunning = false;
        }

        if(RoomCreationQueue.Count == 0)
            yield return null;
    }
    private IEnumerator CreateRoom(CoordInt startCoord, Room room)
    {
        Room legacy = room;
        int Tries = 0;
        while (Tries < 3)
        {
            System.Diagnostics.Stopwatch roomCreation = new System.Diagnostics.Stopwatch();
            if (legacy == null)
                room = new Room(Rooms.Count, legacy.Settings);
            else
                room = new Room(legacy);

            roomCreation.Start();
            
            StartCoroutine(CreateRoomCoroutine(startCoord, room));

            roomCreation.Stop();

            while (!room.FinishedGeneration)
                yield return new WaitForSeconds(2f);
            if (room != null)
            {
                if (!room.Finished)
                {
                    //Debug.Log(room.ErrorMessage);
                    if (Tries < 2)
                    {
                        Tries++;
                        Destroy(room.SpritesGrouper);
                        continue;
                    }
                    else
                    {
                        Destroy(room.SpritesGrouper);
                        break;
                    }
                }
                Debug.Log("Room Took " + (roomCreation.ElapsedTicks / 100000f).ToString("0.000") + "ms to create.");
                room.ExtraInformation.Add("Room Took " + (roomCreation.ElapsedTicks / 100000f).ToString("0.000") + "ms to create.");
                FinishRoom(room);
                break;
            }

            else if (room == null)
            {
                Debug.Log("Cannot Create Room Here.");
                break;
            }
        }
    }
    private IEnumerator CreateRoomCoroutine(CoordInt startCoord, Room room)
    {
        RoomSettings Settings = room.Settings;
        Settings.SetSeed(new Seed(globalPrng));

        GameObject roomGo = new GameObject()
        {
            name = "Room Sprites"
        };
        room.SetSpritesGrouper(roomGo);
        {
            room.resetMap();
            for (int x = startCoord.x - (int)room.Settings.Size; x <= startCoord.x + (int)room.Settings.Size; x++)
            {
                for (int y = startCoord.y - (int)room.Settings.Size; y <= startCoord.y + (int)room.Settings.Size; y++)
                {
                    CoordInt coord = new CoordInt(x, y);

                    int random = room.Settings.Prng.Next(0, 100);

                    Tile tile = (Map.ContainsKey(coord)) ? new Tile(Map[coord]) : new Tile(coord, (random < room.Settings.RandomFillPercent) ? tileType.Wall : tileType.Floor);
                    room.Map.Add(coord, tile);

                    if (x >= startCoord.x - 2 && y >= startCoord.y - 2 && x <= startCoord.x + 2 && y <= startCoord.y + 2)
                    {
                        if (tile.RoomId != room.RoomId && tile.RoomId != -1)
                        {
                            room.FinishedGeneration = true;
                            Destroy(room.SpritesGrouper);
                            yield break;
                        }
                        if (startCoord == coord)
                            room.SetCenterTile(room.Map[coord]);

                        room.Map[coord].SetType(tileType.Floor);
                    }

                    if (Map.ContainsKey(coord))
                        room.Map[coord].SetType((random < room.Settings.RandomFillPercent) ? tileType.Wall : tileType.Floor);

                    CreateSpriteTile(room.Map[coord], room);
                }
                if (SeeProgress)
                {
                    yield return new WaitForSeconds(1f / (float)room.Settings.Size);
                }
            }
        }//Raw Room
        if (Settings.SmoothingMultiplier > 0)
        {
            {
                for (int smooth = 0; smooth < room.Settings.SmoothingMultiplier; smooth++)
                {
                    Queue<Tile> queue = new Queue<Tile>();
                    queue.Enqueue(room.CenterTile);

                    List<CoordInt> MapFlags = new List<CoordInt>();
                    MapFlags.Add(room.CenterTile.Coord);

                    while (queue.Count > 0)
                    {
                        Tile cur = queue.Dequeue();

                        int wallCount = CountNearWallTiles(cur.Coord, room.Map);

                        if (wallCount > room.Settings.ComparisonFactor)
                            cur.SetType(tileType.Wall);
                        else if (wallCount < room.Settings.ComparisonFactor)
                            cur.SetType(tileType.Floor);

                        SpriteRenderer sr = room.SpritesGrouper.transform.Find("Room " + room.RoomId + " : " + cur.Coord.ToString()).GetComponent<SpriteRenderer>();
                        sr.color = (cur.Type == tileType.Wall) ? Color.black : Color.white;


                        for (int NeighbourX = cur.Coord.x - 1; NeighbourX <= cur.Coord.x + 1; NeighbourX++)
                        {
                            for (int NeighbourY = cur.Coord.y - 1; NeighbourY <= cur.Coord.y + 1; NeighbourY++)
                            {
                                CoordInt curCoord = new CoordInt(NeighbourX, NeighbourY);

                                if (cur.Coord.isAdjacent(curCoord))
                                {
                                    if (!MapFlags.Contains(curCoord))
                                    {
                                        MapFlags.Add(curCoord);

                                        Tile nextTile = room.Map.ContainsKey(curCoord) ? room.Map[curCoord] : null;

                                        if (nextTile != null)
                                        {
                                            queue.Enqueue(nextTile);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (SeeProgress)
                    {
                        yield return new WaitForSeconds(1f / room.Settings.SmoothingMultiplier);
                    }
                }
            }//Room Smoothing
            {
                {
                    Tile centerTile = GetNearestTile(room.CenterTile, room.RoomId, room.Map);
                    if (centerTile == null)
                        room.SetError("Cannot set center tile.");
                    else if (centerTile.Collided)
                        room.SetError("Cannot set center tile. " +
                            "\n Center Tile Collides with Tile In Position " + startCoord.ToString() +
                            "\n Tile belongs to room # " + Map[startCoord].RoomId);
                    else
                    {
                        room.SetCenterTile(centerTile);

                        SpriteRenderer sr = room.SpritesGrouper.transform.Find("Room " + room.RoomId + " : " + room.CenterTile.Coord.ToString()).GetComponent<SpriteRenderer>();
                        sr.color = Color.green;
                    }


                }//Check Central Tile
                if (!room.HasError)
                {
                    List<Tile> FloorTiles = GetTiles(room.CenterTile, room.RoomId, room.Map);
                    if (FloorTiles == null || FloorTiles.Count == 0)
                        room.SetError("Floor tiles have collided.");
                    else
                        room.SetFloorTiles(FloorTiles);
                }//Check for Colliding Floor Tiles
                if (!room.HasError)
                {
                    List<Tile> WallTiles = GetWallTiles(room.CenterTile, room.RoomId, room.Map);
                    if (WallTiles == null || WallTiles.Count == 0)
                        room.SetError("Wall tiles have collided.");
                    else
                        room.SetWallTiles(WallTiles);
                }//Check for Colliding Wall Tiles
                if (!room.HasError)
                {
                    if (room.FloorTiles.Count < ((int)room.Settings.Size * (int)room.Settings.Size) / 3)
                    {
                        room.SetError("Room area is too small." +
                            "\n Floor Tiles Count:" + room.FloorTiles.Count + " is lower than half the room maximum area.");
                    }
                    else if(!room.HasError)
                    {
                        room.FinishRoom();
                        foreach (Tile tile in room.Tiles)
                        {
                            if (tile.RoomId == -1 || tile.RoomId == room.RoomId)
                            {
                                tile.Coord.SetTile(tile);
                                tile.RoomId = room.RoomId;
                                tile.Class = room.Settings.Class;
                            }
                        }
                        List<GameObject> goToRemove = new List<GameObject>();

                        foreach (Transform child in room.SpritesGrouper.transform)
                        {
                            CoordInt curCoord = new CoordInt((int)child.position.x, (int)child.position.y);

                            if (room.Map[curCoord].RoomId != room.RoomId)
                                goToRemove.Add(child.gameObject);
                        }

                        foreach (GameObject go in goToRemove)
                            Destroy(go);

                        if (SeeProgress)
                            yield return new WaitForSeconds(.5f);
                    }
                }//Check Room Size
            }//Check if Valid
        }
        room.FinishedGeneration = true;
    }

    void CreateSpriteTile(Tile tile, Room room)
    {
        GameObject go = new GameObject();
        go.name = "Room " + room.RoomId + " : " + tile.Coord.ToString();
        go.transform.parent = room.SpritesGrouper.transform;
        go.transform.position = new Vector3Int(tile.Coord.x, tile.Coord.y, 0);
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = Cell;
        sr.color = (tile.Type == tileType.Wall) ? Color.black : Color.white;
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
                    else if (tile.Type == tileType.Floor)
                        Gizmos.color = Color.white;

                    Gizmos.DrawCube(pos, Vector3.one);
                    if (tile.RoomId >= 0)
                    {
                        Gizmos.color = Color.cyan;
                        Gizmos.DrawCube(pos, Vector3.one * 0.3f);
                    }
                }
            }
        }
    }
    void RoomMesh(Room room)
    {
        GameObject roomGo = new GameObject()
        {
            name = "Room #" + room.RoomId
        };
        {
            GameObject wallGo = new GameObject()
            {
                name = "Room " + room.RoomId + " : Walls"
            };
            wallGo.transform.position = new Vector3(room.CenterTile.Coord.x, room.CenterTile.Coord.y, 0);
            wallGo.AddComponent<MeshGenerator>();
            wallGo.AddComponent<MeshFilter>();
            wallGo.AddComponent<MeshRenderer>();
            MeshGenerator meshGen = wallGo.GetComponent<MeshGenerator>();
            meshGen.GenerateMesh(DictionaryToArray(room.Map), 1, wallGo, room.RoomId, true);

            Material material = new Material(plainMaterial);
            material.color = Color.black;
            wallGo.GetComponent<MeshRenderer>().material = material;

            wallGo.transform.parent = roomGo.transform;
        }
        {
            GameObject FloorGo = new GameObject()
            {
                name = "Room " + room.RoomId + " : Floor"
            };
            FloorGo.transform.position = new Vector3(room.CenterTile.Coord.x, room.CenterTile.Coord.y, 0);
            FloorGo.AddComponent<MeshGenerator>();
            FloorGo.AddComponent<MeshFilter>();
            FloorGo.AddComponent<MeshRenderer>();
            MeshGenerator meshGen = FloorGo.GetComponent<MeshGenerator>();
            meshGen.GenerateMesh(DictionaryToArray(room.Map), 1, FloorGo, room.RoomId);

            Material material = new Material(plainMaterial);
            material.color = room.Color;
            FloorGo.GetComponent<MeshRenderer>().material = material;

            FloorGo.transform.parent = roomGo.transform;
        }

        room.SpritesGrouper.transform.parent = roomGo.transform;
        room.SpritesGrouper.SetActive(false);

        room.SetGameObject(roomGo);
    }

    public Dictionary<CoordInt, Tile> ListToDictionary(List<Tile> list)
    {
        Dictionary<CoordInt, Tile> map = new Dictionary<CoordInt, Tile>();
        foreach (Tile tile in list)
        {
            map.Add(tile, tile);
        }
        return map;
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
        Tile[,] array = new Tile[maxX - minX + 1, maxY - minY + 1];
        foreach (CoordInt coord in map.Keys)
        {
            array[coord.x - minX, coord.y - minY] = map[coord];
        }
        return array;
    }
    
    private void PasteRoom(Room room)
    {
        if (room.Finished)
        {
            room.SetMap(room.Map);

            foreach (Tile tile in room.Tiles)
            {
                if (tile.RoomId == -1 || tile.RoomId == room.RoomId)
                {
                    if (!Map.ContainsKey(tile.Coord))
                        Map.Add(tile.Coord, tile);
                    else
                        Map[tile.Coord] = room.Map[tile.Coord];
                }
            }
        }
        else
        {
            Debug.Log("Tried to paste an unifinished room.");
            return;
        }
    }
    private void FinishRoom(Room room)
    {
        Rooms[room.RoomId] = room;
        PasteRoom(room);
        RoomMesh(Rooms[room.RoomId]);
    }

    private Tile GetNearestTile(Tile Start, int RoomID, Dictionary<CoordInt, Tile> Map, tileType TypeToSearch = tileType.Floor)
    {
        List<CoordInt> mapFlags = new List<CoordInt>();
        Queue<Tile> queue = new Queue<Tile>();

        if (Start.Type == TypeToSearch && (Start.RoomId == -1 || Start.RoomId == RoomID))
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
                                if (nextTile.RoomId == -1 || nextTile.RoomId == RoomID)
                                {
                                    if (nextTile.Type == TypeToSearch)
                                        return nextTile;
                                    else
                                        queue.Enqueue(nextTile);
                                }
                            }
                            else
                            {
                                if (tile.Type == TypeToSearch)
                                    return tile;
                            }
                        }
                }
            }
        }
        return null;
    }
    private List<Tile> GetTiles(Tile Start, int RoomID, Dictionary<CoordInt, Tile> Map, tileType typeToSearch = tileType.Floor)
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
                                if (nextTile.Type == typeToSearch)
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
    private List<Tile> GetWallTiles(Tile Start, int RoomID, Dictionary<CoordInt, Tile> Map)
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
                        Tile nextTile = Map.ContainsKey(curCoord) ? Map[curCoord] : null;
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
    private int CountNearWallTiles(CoordInt Coord, Dictionary<CoordInt, Tile> Map)
    {
        int Count = 0;

        for (int NeighbourX = Coord.x - 1; NeighbourX <= Coord.x + 1; NeighbourX++)
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
                    else if (Coord.isAdjacent(curCoord))
                        return 16;
                }
            }
        }
        return Count;
    }

    private bool NearTiles(CoordInt coord, int SearchRange = 1)
    {
        for (int neighbourX = coord.x - SearchRange; neighbourX <= coord.x + SearchRange; neighbourX++)
        {
            for (int neighbourY = coord.y - SearchRange; neighbourY <= coord.y + SearchRange; neighbourY++)
            {
                if (Map.ContainsKey(new CoordInt(neighbourX, neighbourY)))
                {
                    return true;
                }
            }
        }
        return false;
    }
}