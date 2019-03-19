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

    public float seconds;

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
    Dictionary<int, List<GameObject>> RoomSprites;
    Queue<Tuple<CoordInt, Room, bool>> RoomCreationQueue;

    private void Awake()
    {
        RoomCreationQueue = new Queue<Tuple<CoordInt, Room, bool>>();
        RoomSprites = new Dictionary<int, List<GameObject>>();
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
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                RoomSettings Settings = new RoomSettings(smoothMultiplier, randomFillPercent, comparisonFactor, RoomSize, RoomType, RoomClass);
                Room room = new Room(Rooms.Count, Settings);

                StartCoroutine(EnqueueRoom(mousePos, room));
            }
        }
        if (Input.GetKeyDown(KeyCode.T))
            GenerateLevel();
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
        {
            newSeed();
        }
        else
        {
            reloadPrng(globalSeed);
        }

        foreach (Room room in Rooms)
        {
            Destroy(room.roomGo);
        }

        Rooms = new List<Room>();
        Map.Clear();
        RoomSprites.Clear();
        //GenerateMeshCube(Map);
    }
    

    public void StartGeneration()
    {
        StartCoroutine(GenerateLevel());
    }
    public IEnumerator GenerateLevel()
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

                    bool FinishedCoroutine = false;

                    StartCoroutine(CreateRoom(new CoordInt(0, 0), room, FinishedCoroutine));
                    while (!FinishedCoroutine)
                        yield return new WaitForSeconds(0.1f);
                }
                else
                {
                    Debug.Log("Creating room Nº " + i);
                    switch (t)
                    {
                        case 0:
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
                                        roomSize Size = roomSize.Tiny;

                                        switch (moreRooms.Next(0, 4))
                                        {
                                            case 0:
                                                Size = roomSize.Tiny;
                                                break;
                                            case 1:
                                                Size = roomSize.Small;
                                                break;
                                            case 2:
                                                Size = roomSize.Medium;
                                                break;
                                            case 3:
                                                Size = roomSize.Big;
                                                break;
                                            case 4:
                                                Size = roomSize.Large;
                                                break;
                                        }


                                        if (NearTiles(coord, (int)Size / 2))
                                            continue;

                                        room = new Room(Rooms.Count, new RoomSettings(4, moreRooms.Next(25, 45), 4, Size, roomType.None, roomClass.Scientific));

                                        bool FinishedCoroutine = false;

                                        StartCoroutine(CreateRoom(new CoordInt(x, y), room, FinishedCoroutine));
                                        while (!FinishedCoroutine)
                                            yield return new WaitForSeconds(0.1f);
                                    }
                                    if (room != null)
                                        break;
                                }
                                if (room != null)
                                    break;
                            }
                            break;
                        case 1:
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
                                        roomSize Size = roomSize.Tiny;

                                        switch (moreRooms.Next(0, 4))
                                        {
                                            case 0:
                                                Size = roomSize.Tiny;
                                                break;
                                            case 1:
                                                Size = roomSize.Small;
                                                break;
                                            case 2:
                                                Size = roomSize.Medium;
                                                break;
                                            case 3:
                                                Size = roomSize.Big;
                                                break;
                                            case 4:
                                                Size = roomSize.Large;
                                                break;
                                        }


                                        if (NearTiles(coord, (int)Size / 2))
                                            continue;

                                        room = new Room(Rooms.Count, new RoomSettings(4, moreRooms.Next(25, 45), 4, Size, roomType.None, roomClass.Physical));

                                        bool FinishedCoroutine = false;

                                        StartCoroutine(CreateRoom(new CoordInt(x, y), room, FinishedCoroutine));
                                        while (!FinishedCoroutine)
                                            yield return new WaitForSeconds(0.1f);
                                    }
                                    if (room != null)
                                        break;
                                }
                                if (room != null)
                                    break;
                            }
                            break;
                        case 2:
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
                                        roomSize Size = roomSize.Tiny;

                                        switch (moreRooms.Next(0, 4))
                                        {
                                            case 0:
                                                Size = roomSize.Tiny;
                                                break;
                                            case 1:
                                                Size = roomSize.Small;
                                                break;
                                            case 2:
                                                Size = roomSize.Medium;
                                                break;
                                            case 3:
                                                Size = roomSize.Big;
                                                break;
                                            case 4:
                                                Size = roomSize.Large;
                                                break;
                                        }


                                        if (NearTiles(coord, (int)Size / 2))
                                            continue;

                                        room = new Room(Rooms.Count, new RoomSettings(4, moreRooms.Next(25, 45), 4, Size, roomType.None, roomClass.Spiritual));

                                        bool FinishedCoroutine = false;

                                        StartCoroutine(CreateRoom(new CoordInt(x, y), room, FinishedCoroutine));
                                        while (!FinishedCoroutine)
                                            yield return new WaitForSeconds(0.1f);
                                    }
                                    if (room != null)
                                        break;
                                }
                                if (room != null)
                                    break;
                            }
                            break;
                        case 3:
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
                                        roomSize Size = roomSize.Tiny;

                                        switch (moreRooms.Next(0, 4))
                                        {
                                            case 0:
                                                Size = roomSize.Tiny;
                                                break;
                                            case 1:
                                                Size = roomSize.Small;
                                                break;
                                            case 2:
                                                Size = roomSize.Medium;
                                                break;
                                            case 3:
                                                Size = roomSize.Big;
                                                break;
                                            case 4:
                                                Size = roomSize.Large;
                                                break;
                                        }


                                        if (NearTiles(coord, (int)Size / 2))
                                            continue;

                                        room = new Room(Rooms.Count, new RoomSettings(4, moreRooms.Next(25, 45), 4, Size, roomType.None, roomClass.Social));

                                        bool FinishedCoroutine = false;

                                        StartCoroutine(CreateRoom(new CoordInt(x, y), room, FinishedCoroutine));
                                        while (!FinishedCoroutine)
                                            yield return new WaitForSeconds(0.1f);
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
                Debug.Log("Time Before Wait = " + DateTime.Now);
                yield return new WaitForSeconds(seconds);
                Debug.Log("Time After Wait = " + DateTime.Now);
            }
        }
    }

    bool isRunning = false;

    IEnumerator EnqueueRoom(CoordInt startCoord, Room room, bool isFinished = false)
    {
        Rooms.Add(room);
        RoomCreationQueue.Enqueue(new Tuple<CoordInt, Room, bool>(startCoord, room, isFinished));

        if (!isRunning)
            StartCoroutine(RunQueue());
        yield return null;
    }


    IEnumerator RunQueue()
    {
        if (isRunning)
            yield return null;

        while (RoomCreationQueue.Count > 0)
        {
            isRunning = true;
            Tuple<CoordInt, Room, bool> queue = RoomCreationQueue.Dequeue();

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

    IEnumerator CreateRoom(CoordInt startCoord, Room room, bool isFinished = false)
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

            RoomSprites.Add(room.RoomId, new List<GameObject>());
            StartCoroutine(CreateRoomCoroutine(startCoord, room));

            roomCreation.Stop();

            while (!room.FinishedGeneration)
            {
                yield return new WaitForSeconds(2f);
            }
            if (room != null)
            {
                if (!room.Finished)
                {
                    //Debug.Log(room.ErrorMessage);
                    if (Tries < 2)
                    {
                        Tries++;
                        RoomSprites.Remove(room.RoomId);
                        continue;
                    }
                    else
                    {
                        RoomSprites.Remove(room.RoomId);
                        isFinished = true;
                        break;
                    }
                }
                Debug.Log("Room Took " + (roomCreation.ElapsedTicks / 100000f).ToString("0.000") + "ms to create.");
                room.ExtraInformation.Add("Room Took " + (roomCreation.ElapsedTicks / 100000f).ToString("0.000") + "ms to create.");
                FinishRoom(room);
                isFinished = true;
                break;
            }

            else if (room == null)
            {
                Debug.Log("Cannot Create Room Here");
                RoomSprites.Remove(room.RoomId);
                break;
            }
        }
        yield return null;
    }

    IEnumerator CreateRoomCoroutine(CoordInt startCoord, Room room)
    {
        RoomSettings Settings = room.Settings;
        Settings.SetSeed(new Seed(globalPrng));
        
        GameObject roomGo = new GameObject();
        roomGo.name = "Room Sprites";
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

                    if (x >= startCoord.x - 1 && y >= startCoord.y - 1 && x <= startCoord.x + 1 && y <= startCoord.y + 1)
                    {
                        if (tile.RoomId != room.RoomId && tile.RoomId != -1)
                        {
                            room.FinishedGeneration = true;
                            yield return null;
                        }
                        if (startCoord == coord)
                            room.SetCenterTile(room.Map[coord]);

                        room.Map[coord].SetType(tileType.Floor);
                    }

                    if (Map.ContainsKey(coord))
                        room.Map[coord].SetType((random < room.Settings.RandomFillPercent) ? tileType.Wall : tileType.Floor);

                    CreateSpriteTile(roomGo, room.Map[coord], room.RoomId);
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
                    int i = 0;

                    foreach (Tile tile in room.Map.Values)
                    {
                        int wallCount = CountNearWallTiles(tile.Coord, room.Map);

                        if (wallCount > room.Settings.ComparisonFactor)
                            tile.SetType(tileType.Wall);
                        else if (wallCount < room.Settings.ComparisonFactor)
                            tile.SetType(tileType.Floor);


                        RoomSprites[room.RoomId][i].GetComponent<SpriteRenderer>().color = (tile.Type == tileType.Wall) ? Color.black : Color.white;

                        i++;
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

                        RoomSprites[room.RoomId].Find(item => new CoordInt((int)item.transform.position.x, (int)item.transform.position.y) == room.CenterTile.Coord).GetComponent<SpriteRenderer>().color = Color.green;

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
                        foreach (GameObject child in RoomSprites[room.RoomId])
                        {
                            CoordInt curCoord = new CoordInt((int)child.transform.position.x, (int)child.transform.position.y);

                            if (room.Map[curCoord].RoomId != room.RoomId)
                            {
                                goToRemove.Add(child);
                            }
                        }

                        foreach (GameObject go in goToRemove)
                        {
                            RoomSprites[room.RoomId].Remove(go);
                            Destroy(go);
                        }

                        if (SeeProgress)
                            yield return new WaitForSeconds(.5f);
                    }
                }//Check Room Size

                if (room.HasError)
                {
                    Destroy(roomGo);
                }
            }//Check if Valid
        }
        room.FinishedGeneration = true;
    }

    void CreateSpriteTile(GameObject Grouper, Tile tile, int roomId)
    {
        GameObject go = new GameObject();
        go.name = tile.Coord.ToString();
        go.transform.parent = Grouper.transform;
        go.transform.position = new Vector3Int(tile.Coord.x, tile.Coord.y, 0);
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = Cell;
        sr.color = (tile.Type == tileType.Wall) ? Color.black : Color.white;
        RoomSprites[roomId].Add(go);
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
    void GenerateMeshCube(Dictionary<CoordInt, Tile> _Map)
    {
        GameObject[] go = GameObject.FindGameObjectsWithTag("Map");
        foreach (GameObject GO in go)
        {
            Destroy(GO);
        }

        foreach (Tile tile in _Map.Values)
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
        if (RoomSprites[room.RoomId].Count > 0)
        {
            RoomSprites[room.RoomId][0].transform.parent.parent = roomGo.transform;
            RoomSprites[room.RoomId][0].transform.parent.gameObject.SetActive(false);
        }

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


    public void PasteRoom(Room room)
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
        //GenerateMeshCube(Map);
        RoomMesh(Rooms[room.RoomId]);
    }


    Tile GetNearestTile(Tile Start, int RoomID, Dictionary<CoordInt, Tile> Map, tileType TypeToSearch = tileType.Floor)
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
    List<Tile> GetTiles(Tile Start, int RoomID, Dictionary<CoordInt, Tile> Map, tileType typeToSearch = tileType.Floor)
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
    int CountNearWallTiles(CoordInt Coord, Dictionary<CoordInt, Tile> Map)
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

    bool NearTiles(CoordInt coord, int SearchRange = 1)
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