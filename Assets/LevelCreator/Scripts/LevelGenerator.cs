using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour {

    public int size;
    public bool drawGizmos = false;
    public bool showGrid = false;
    public bool GenMesh = false;
    public bool showOverlay = false;
    public bool onMouse;
    
    public Enums.roomSize RoomSize;
    public Enums.roomClass RoomClass;
    public Enums.roomType RoomType;
    public List<Room> Rooms;
    public Tile[,] globalMap;

    public Vector2 mousePos;

    System.Random globalPrng = new System.Random();

    public string globalSeed;
    public bool randomSeed;

    public GameObject overlay;
    public GameObject grid;
    public Sprite gridCell;
    public enum OverlayType { Tile, Room };
    public OverlayType overlayType;

    bool oldshowGrid;
    bool oldGenMesh;
    OverlayType oldOverlay;


    [Range(1, 10)]
    public int SmoothMultiplier;

    [Range(0, 100)]
    public int randomFillPercent;

    string Alphanumeric = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    //STRUCTS

    public struct Coord
    {
        public Vector2Int coords;
        int tileX;
        int tileY;

        public Coord(int x, int y)
        {
            tileX = x;
            tileY = y;

            coords = new Vector2Int(tileX, tileY);
        }
    }
    public struct Tile
    {
        public Coord Coord; //Coordinates From Center
        public Coord RawCoord; //Coordinates From Map 0,0
        public Enums.tileType tileType;
        public int RoomID;
        public Tile(Coord _RawCoord, Coord _Coord, Enums.tileType _tileType = Enums.tileType.Wall, int _RoomID = -1)
        {
            RawCoord = _RawCoord;
            Coord = _Coord;
            tileType = _tileType;
            RoomID = _RoomID;
        }
    }
    public struct Room
    {
        public int RoomID;
        public Enums.roomClass roomClass;
        public Enums.roomSize roomSize;
        public Enums.roomType roomType;
        public List<Tile> roomTiles;
        public List<Tile> wallTiles;
        public Tile[,] roomMap;
        public string roomSeed;
        public bool isValid;

        public Room(int _RoomID, Enums.roomSize _roomSize, Enums.roomType _roomType, Enums.roomClass _roomClass)
        {
            RoomID = _RoomID;
            roomClass = _roomClass;
            roomSize = _roomSize;
            roomType = _roomType;
            isValid = true;
            roomMap = new Tile[0,0];
            roomTiles = new List<Tile>();
            wallTiles = new List<Tile>();
            roomSeed = "";
        }
    }

    //END STRUCTS



    //UNITY METHODS

    private void Awake()
    {
        if (globalSeed == "")
            NewSeed();
        oldGenMesh = !GenMesh;
        oldshowGrid = !showGrid;
    }
    private void Start()
    {
        LevelCreate();
    }
    private void Update()
    {
        {
            if(GenMesh != oldGenMesh)
            {
                oldGenMesh = GenMesh;
                CreateMesh();
                if(oldGenMesh == false)
                {
                    GetComponent<MeshFilter>().mesh = new Mesh();
                }
            }
            if (showGrid != oldshowGrid)
            {
                oldshowGrid = showGrid;
                updateOverlay(BorderedMap());
            }
            if (overlayType != oldOverlay)
            {
                oldOverlay = overlayType;
                updateOverlay(BorderedMap());
            }

            overlay.SetActive(showOverlay);
            grid.SetActive(showGrid);
        }//Check If New

        if (size % 2 == 0)
            mousePos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x + size / 2, Camera.main.ScreenToWorldPoint(Input.mousePosition).y + size / 2);
        else
            mousePos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x + size / 2-.5f, Camera.main.ScreenToWorldPoint(Input.mousePosition).y + size / 2-.5f);

        if (size % 2 == 0)
            GameObject.Find("Guide").transform.position = new Vector3((int)mousePos.x - size/2+.5f, (int)mousePos.y - size / 2+.5f, -2);
        else
            GameObject.Find("Guide").transform.position = new Vector3((int)mousePos.x - size / 2 + 1f, (int)mousePos.y - size / 2 + 1f, -2);


        if (Input.GetKeyDown(KeyCode.G)) {
            LevelCreate(true);
            Room room = newRoom(size/2,size/2, RoomSize, RoomType, RoomClass);
            if (room.isValid)
            {
                Rooms.Add(room);

                CreateMesh();
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
        }
        if (Input.GetMouseButtonDown(0))
        {
            Room room = newRoom((int)mousePos.x, (int)mousePos.y, RoomSize, RoomType, RoomClass);
            if (room.isValid)
            {
                Rooms.Add(room);

                CreateMesh();
            }
        }
    }
    void OnDrawGizmos()
    {
        if (globalMap != null && drawGizmos)
        {
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    Gizmos.color = (BorderedMap(0)[x,y].tileType == Enums.tileType.Wall) ? Color.black : Color.white;
                    Vector3 pos = new Vector3(-size / 2 + x + .5f, -size / 2 + y + .5f, 0);
                    Gizmos.DrawCube(pos, Vector3.one);
                }
            }
        }
    }

    //END UNITY METHODS



    //CREATE STRUCTS

    Room newRoom(int originX, int originY, Enums.roomSize _RoomSize = Enums.roomSize.Medium, Enums.roomType _RoomType = Enums.roomType.None, Enums.roomClass _RoomClass = Enums.roomClass.Neutral)
    {
        Vector2Int origin = new Vector2Int(originX, originY);
    restart:
        Room room = new Room(Rooms.Count, _RoomSize, _RoomType, _RoomClass);

        Vector2Int RoomBoundMin = new Vector2Int(origin.x - (int)room.roomSize / 2, origin.y - (int)room.roomSize / 2);
        Vector2Int RoomBoundMax = new Vector2Int(origin.x + (int)room.roomSize / 2, origin.y + (int)room.roomSize / 2);
        
        string _RoomSeed = newRoomSeed();

        System.Random roomPrng = new System.Random(_RoomSeed.GetHashCode());
        room.roomSeed = _RoomSeed;
        
        {
            if (RoomBoundMin.x <= 0)
                RoomBoundMin.x = 0;
            if (RoomBoundMin.y <= 0)
                RoomBoundMin.y = 0;
            if (RoomBoundMax.x >= size)
                RoomBoundMax.x = size;
            if (RoomBoundMax.y >= size)
                RoomBoundMax.y = size;
        }

        Tile[,] roomMap = new Tile[RoomBoundMax.x - RoomBoundMin.x+2, RoomBoundMax.y - RoomBoundMin.y+2];
        int roomArea = (RoomBoundMax.x - RoomBoundMin.x) * (RoomBoundMax.y - RoomBoundMin.y);
        int tileCount = 0;

        for (int x = 0; x < roomMap.GetLength(0); x++)
        {
            for (int y = 0; y < roomMap.GetLength(1); y++)
            {
                roomMap[x, y] = globalMap[x + RoomBoundMin.x, y + RoomBoundMin.y];
            }
        }

        List<Tile> floorTiles = new List<Tile>();
        List<Tile> wallTiles = new List<Tile>();

        {
            for (int x = 0; x < roomMap.GetLength(0); x++)
            {
                for (int y = 0; y < roomMap.GetLength(1); y++)
                {
                    if (x > 0 && x < roomMap.GetLength(0) - 2 && y > 0 && y < roomMap.GetLength(1) - 2)
                    {
                        if (room.roomType != Enums.roomType.Spawn)
                            roomMap[x, y].tileType = (roomPrng.Next(0, 100) < randomFillPercent) ? Enums.tileType.Wall : Enums.tileType.Floor;
                        else
                        {
                            roomMap[x, y].tileType = Enums.tileType.Floor;
                        }
                    }
                }
            }
        } // Room Creation
        {
            for (int i = 0; i < SmoothMultiplier; i++)
            {
                for (int x = 0; x < roomMap.GetLength(0); x++)
                {
                    for (int y = 0; y < roomMap.GetLength(1); y++)
                    {
                        if (x > 0 && x < roomMap.GetLength(0) - 2 && y > 0 && y < roomMap.GetLength(1) - 2)
                        {
                            int neighbourWallTiles = GetSurroundingWallCount(x, y, roomMap);

                            if (neighbourWallTiles > 4)
                                roomMap[x, y].tileType = Enums.tileType.Wall;
                            else if (neighbourWallTiles < 4)
                                roomMap[x, y].tileType = Enums.tileType.Floor;
                        }
                    }
                }
            }
        } // Room Smoothing
        {
            Tile[,] checkMap = globalMap;
            floorTiles = GetFloorTiles(origin, ApplyMap(roomMap, checkMap, RoomBoundMin));
            wallTiles = GetWallTiles(origin, ApplyMap(roomMap, checkMap, RoomBoundMin));
            tileCount = floorTiles.Count;
            if (floorTiles.Count == 0 || wallTiles.Count == 0)
            {
                Debug.Log("Room is coliding, try again");
                room.isValid = false;
                return room;
            }
        } // Room Check

        if (tileCount > roomArea / 3f && room.isValid)
        {
            roomMap = ChangeRoomID(roomMap, floorTiles, wallTiles, room.RoomID, RoomBoundMin);

            room.roomTiles = floorTiles;
            room.wallTiles = wallTiles;

            room.roomMap = roomMap;
            globalMap = ApplyMap(roomMap, globalMap, RoomBoundMin);
            return room;
        }
        else
        {
            goto restart;
        }
    }

    //END CREATE STRUCTS



    //MY METHODS

    char NewChar()
    {
        return Alphanumeric[globalPrng.Next(0, Alphanumeric.Length)];
    }
    void ProcessMap()
    {/*
        List<List<Coord>> wallRegions = GetRegions(Enums.tileType.Wall);
        int wallThresholdSize = 30;

        foreach (List<Coord> wallRegion in wallRegions)
        {
            if (wallRegion.Count < wallThresholdSize)
            {
                foreach (Coord tile in wallRegion)
                {
                    globalMap[tile.coords.x, tile.coords.y].tileType = Enums.tileType.Floor;
                }
            }
        }

        List<List<Coord>> roomRegions = GetRegions(Enums.tileType.Floor);
        int roomThresholdSize = 30;

        foreach (List<Coord> roomRegion in roomRegions)
        {
            if (roomRegion.Count < roomThresholdSize)
            {
                foreach (Coord tile in roomRegion)
                {
                    globalMap[tile.coords.x, tile.coords.y].tileType = Enums.tileType.Wall;
                }
            }
        }*/
    }
    public void NewSeed()
    {
        globalPrng = new System.Random();

        string generatedString = "";

        for (int i = 0; i < 8; i++)
        {
            generatedString += NewChar();
        }

        globalSeed = generatedString;
        globalPrng = new System.Random(globalSeed.GetHashCode());
    }
    public void resetPrng()
    {
        globalPrng = new System.Random(globalSeed.GetHashCode());
    }
    public string newRoomSeed()
    {
        string generatedString = "";

        for (int i = 0; i < 8; i++)
        {
            generatedString += NewChar();
        }
        return generatedString;
    }
    public void LevelCreate(bool firstRoom = false)
    {
        Camera.main.orthographicSize = size / 2f + size / 20f;
        overlay.transform.localScale = new Vector3(-size/10f, -1, size/10f);
        grid.transform.localScale = new Vector3(-size / 10f, -1, size / 10f);

        globalMap = new Tile[size, size];
        if (randomSeed)
        {
            NewSeed();
        }
        resetPrng();
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                globalMap[x, y] = new Tile(new Coord(x, y), new Coord(x - size/2,y - size/2));
            }
        }
        Rooms = new List<Room>();
        if (!firstRoom)
        {
            Room room = newRoom(size / 2, size / 2, Enums.roomSize.Tiny, Enums.roomType.Spawn);
            if (room.isValid)
            {
                Rooms.Add(room);
            }
        }
        CreateMesh();
    }
    Tile[,] BorderedMap(int _borderThickness = 2)
    {
        int borderThickness;
        Tile[,] borderedMap = new Tile[size, size];
        if (_borderThickness != 2)
            borderThickness = _borderThickness;
        else
            borderThickness = _borderThickness;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                borderedMap[x, y] = globalMap[x, y];
                if (x < borderThickness || x > size - 1 - borderThickness || y < borderThickness || y > size - borderThickness - 1)
                {
                    borderedMap[x, y].tileType = Enums.tileType.Wall;
                    if (x < borderThickness - 1 || x > size - borderThickness + 1 || y < borderThickness - 1 || y > size - borderThickness + 1)
                        borderedMap[x, y].RoomID = -1;
                }
            }
        }

        return borderedMap;
    }
    List<Tile> GetWallTiles(Vector2Int start, Tile[,] Map, int _RoomID = -1)
    {
        bool isColiding = false;
        List<Tile> wallTiles = new List<Tile>();
        int[,] mapFlags = new int[Map.GetLength(0), Map.GetLength(1)];
        Enums.tileType tileType = Enums.tileType.Wall;

        Queue<Tile> queue = new Queue<Tile>();
        queue.Enqueue(Map[start.x, start.y]);
        mapFlags[start.x, start.y] = 1;

        while (queue.Count > 0)
        {
            Tile tile = queue.Dequeue();
            if (!isColiding)
            {
                for (int x = tile.RawCoord.coords.x - 1; x <= tile.RawCoord.coords.x + 1; x++)
                {
                    for (int y = tile.RawCoord.coords.y - 1; y <= tile.RawCoord.coords.y + 1; y++)
                    {
                        if (IsInMapRange(x, y, Map))
                        {
                            if (mapFlags[x, y] == 0)
                            {
                                if (Map[x, y].RoomID == -1 || Map[x, y].RoomID == _RoomID)
                                {
                                    mapFlags[x, y] = 1;
                                    if (Map[x, y].tileType == tileType)
                                    {
                                        wallTiles.Add(Map[x, y]);
                                    }
                                    else
                                    {
                                        queue.Enqueue(Map[x, y]);
                                    }
                                }
                                else
                                {
                                    isColiding = true;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                queue.Clear();
                break;
            }
        }
        if (isColiding)
            return new List<Tile>();
        else
            return wallTiles;
    }
    List<Tile> GetFloorTiles(Vector2Int start, Tile[,] Map, int _RoomID = -1)
    {
        bool isColiding = false;
        List<Tile> tiles = new List<Tile>();
        int[,] mapFlags = new int[Map.GetLength(0), Map.GetLength(1)];
        Enums.tileType tileType = Enums.tileType.Floor;

        Queue<Tile> queue = new Queue<Tile>();
        queue.Enqueue(Map[start.x, start.y]);
        mapFlags[start.x, start.y] = 1;

        while (queue.Count > 0)
        {
            Tile tile = queue.Dequeue();

            if (tile.RoomID == -1 || tile.RoomID == _RoomID)
            {
                tiles.Add(tile);
            }
            else
            {
                isColiding = true;
            }
            if (!isColiding)
            {
                for (int x = tile.RawCoord.coords.x - 1; x <= tile.RawCoord.coords.x + 1; x++)
                {
                    for (int y = tile.RawCoord.coords.y - 1; y <= tile.RawCoord.coords.y + 1; y++)
                    {
                        if (IsInMapRange(x, y, Map) && (y == tile.RawCoord.coords.y || x == tile.RawCoord.coords.x))
                        {
                            if (mapFlags[x, y] == 0 && Map[x, y].tileType == tileType)
                            {
                                mapFlags[x, y] = 1;
                                if (Map[x, y].RoomID == -1 || Map[x, y].RoomID == _RoomID)
                                {
                                    queue.Enqueue(Map[x, y]);
                                }
                                else
                                {
                                    //Debug.Log("Tile Coliding:" + tile.RawCoord.coords);
                                    isColiding = true;
                                    queue.Clear();
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                queue.Clear();
                break;
            }
        }
        if (isColiding)
            return new List<Tile>();
        else
            return tiles;
    }

    //END MY METHODS



    //FOR EFFICIENCY

    void CreateMesh()
    {
        updateOverlay(BorderedMap());
        if (GenMesh == true)
        {
            ProcessMap();
            MeshGenerator meshGen = GetComponent<MeshGenerator>();
            meshGen.GenerateMesh(BorderedMap(),1);
        }
    }
    void updateOverlay(Tile[,] Map)
    {
        if (overlayType == OverlayType.Room)
            overlay.GetComponent<MeshRenderer>().material.mainTexture = TextureGenerator.OverlayTexture(Map, Rooms);
        if (overlayType == OverlayType.Tile)
            overlay.GetComponent<MeshRenderer>().material.mainTexture = TextureGenerator.TileOverlayTexture(Map);
        grid.GetComponent<MeshRenderer>().material.mainTexture = TextureGenerator.gridTexture(Map, TextureGenerator.textureFromSprite(gridCell));
    }
    int GetSurroundingWallCount(int gridX, int gridY, Tile[,] Map)
    {
        int WallCount = 0;
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                if (IsInMapRange(neighbourX, neighbourY, Map))
                {
                    int x = neighbourX;
                    int y = neighbourY;
                    if (neighbourX < 0)
                        x = 0;
                    if (neighbourY < 0)
                        y = 0;
                    if (neighbourX > size)
                        x = size;
                    if (neighbourY > size)
                        y = size;

                    if (IsInMapRange(x, y, Map))
                    {
                        if ((neighbourX != gridX || neighbourY != gridY) && Map[x, y].tileType == Enums.tileType.Wall)
                        {
                            WallCount++;
                        }
                    }
                }
                else
                    WallCount++;
            }
        }
        return WallCount;
    }
    public bool IsInMapRange(int x, int y, Tile[,] Map, bool isFloat = false)
    {
        if (!isFloat)
            return (x >= 0 && x <= Map.GetLength(0) - 1) && (y >= 0 && y <= Map.GetLength(1) - 1);
        else
            return (x >= 0 && x <= Map.GetLength(0) - 1) && (y >= 0 && y <= Map.GetLength(1) - 1);
    }
    Tile[,] ApplyMap(Tile[,] _fromMap, Tile[,] _toMap, Vector2Int RoomBoundMin)
    {
        Tile[,] fromMap = new Tile[_fromMap.GetLength(0), _fromMap.GetLength(1)];

        for (int x = 0; x < fromMap.GetLength(0); x++)
        {
            for (int y = 0; y < fromMap.GetLength(1); y++)
            {
                fromMap[x, y] = _fromMap[x, y];
            }
        }

        Tile[,] toMap = new Tile[_toMap.GetLength(0), _toMap.GetLength(1)];

        for (int x = 0; x < toMap.GetLength(0); x++)
        {
            for (int y = 0; y < toMap.GetLength(1); y++)
            {
                toMap[x, y] = _toMap[x, y];
            }
        }

        for (int x = 0; x < fromMap.GetLength(0); x++)
        {
            for (int y = 0; y < fromMap.GetLength(1); y++)
            {
                toMap[x + RoomBoundMin.x, y + RoomBoundMin.y] = fromMap[x, y];
            }
        }
        return toMap;
    }
    Tile GetNearestTile(int startX, int startY, Enums.tileType tileType, Tile[,] Map)
    {
        Tile NearestTile = Map[startX, startY];
        int[,] mapFlags = new int[Map.GetLength(0), Map.GetLength(1)];
        
        Queue<Tile> queue = new Queue<Tile>();
        queue.Enqueue(Map[startX, startY]);
        mapFlags[startX, startY] = 1;

        while (queue.Count > 0)
        {
            Tile tile = queue.Dequeue();

            for (int x = tile.RawCoord.coords.x - 1; x <= tile.RawCoord.coords.x + 1; x++)
            {
                for (int y = tile.RawCoord.coords.y - 1; y <= tile.RawCoord.coords.y + 1; y++)
                {
                    if (IsInMapRange(x, y, Map) && (y == tile.RawCoord.coords.y || x == tile.RawCoord.coords.x))
                    {
                        if (mapFlags[x, y] == 0)
                        {
                            if (Map[x, y].tileType != tileType)
                            {
                                mapFlags[x, y] = 1;
                                queue.Enqueue(Map[x, y]);
                            }
                            else
                            {
                                NearestTile = tile;
                                while (queue.Count > 0)
                                {
                                    queue.Clear();
                                }
                            }
                        }
                    }
                }
            }
        }

        return NearestTile;
    }
    Tile[,] ChangeRoomID(Tile[,] _Map,List<Tile> floorTiles, List<Tile> wallTiles, int _RoomID, Vector2Int RoomBoundMin)
    {
        Tile[,] Map = _Map;

        for (int i = 0; i < floorTiles.Count; i++)
        {
            Map[floorTiles[i].RawCoord.coords.x - RoomBoundMin.x, floorTiles[i].RawCoord.coords.y - RoomBoundMin.y ].RoomID = _RoomID;
        }
        for (int i = 0; i < wallTiles.Count; i++)
        {
            Map[wallTiles[i].RawCoord.coords.x - RoomBoundMin.x, wallTiles[i].RawCoord.coords.y - RoomBoundMin.y].RoomID = _RoomID;
        }

        return Map;
    }

    //END FOR EFFICIENCY
}