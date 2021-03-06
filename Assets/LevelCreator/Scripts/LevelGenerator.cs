﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Resources;

public class LevelGenerator : MonoBehaviour {
    public int size;

    public bool GenMesh = false;
    public bool showGrid = false;
    public Sprite gridCell;
    public bool showOverlay = false;
    public Enums.OverlayType overlayType;
    public bool onMouse;

    public bool createFirstRoom;
    public Enums.roomSize RoomSize;
    public Enums.roomClass RoomClass;
    public Enums.roomType RoomType;
    public List<Room> Rooms;
    public Tile[,] globalMap;

    public Vector2 mousePos;

    System.Random globalPrng = new System.Random();

    public string globalSeed;
    public bool randomSeed;

    public GameObject grid;
    public GameObject overlay;

    bool oldshowGrid;
    bool oldGenMesh;
    Enums.OverlayType oldOverlay;


    [Range(0, 5)]
    public int SmoothMultiplier;

    [Range(0, 100)]
    public int randomFillPercent;



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
        {
            globalSeed = new Seed(new System.Random(DateTime.Now.GetHashCode()));
            globalPrng = new System.Random(globalSeed.GetHashCode());
        }
        oldGenMesh = !GenMesh;
        oldshowGrid = !showGrid;

        LevelCreate(createFirstRoom);
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
                updateOverlay(globalMap);
            }
            if (overlayType != oldOverlay)
            {
                oldOverlay = overlayType;
                updateOverlay(globalMap);
            }

            overlay.SetActive(showOverlay);
            grid.SetActive(showGrid);
        }//Check If New

        mousePos = new Vector2(Mathf.FloorToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).x) + size/2, Mathf.FloorToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).y)+ size/2);

        if (Input.GetKeyDown(KeyCode.G)) {
            LevelCreate(false);

            Room room = newRoom(size / 2, size / 2, RoomSize, RoomType, RoomClass);

            if (room.isValid)
                Rooms.Add(room);
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            Room room = newRoom((int)mousePos.x, (int)mousePos.y, RoomSize, RoomType, RoomClass);

            if (room.isValid)
                Rooms.Add(room);
        }
    }

    //END UNITY METHODS

    
    
    //CREATE STRUCTS

    Room newRoom(int originX, int originY, Enums.roomSize _RoomSize = Enums.roomSize.Medium, Enums.roomType _RoomType = Enums.roomType.None, Enums.roomClass _RoomClass = Enums.roomClass.Neutral)
    {
        Vector2Int origin = new Vector2Int(originX, originY);
    restart:
        Room room = new Room(Rooms.Count, _RoomSize, _RoomType, _RoomClass);
        
        List<Tile> floorTiles = new List<Tile>();
        List<Tile> wallTiles = new List<Tile>();

        Vector2Int RoomBoundMin = new Vector2Int(origin.x - (int)room.roomSize / 2, origin.y - (int)room.roomSize / 2);
        Vector2Int RoomBoundMax = new Vector2Int(origin.x + (int)room.roomSize / 2, origin.y + (int)room.roomSize / 2);

        room.roomSeed = new Seed(globalPrng);

        System.Random roomPrng = new System.Random(room.roomSeed.GetHashCode());


        {
            if (RoomBoundMin.x < 0)
                RoomBoundMin.x = 0;
            if (RoomBoundMin.y < 0)
                RoomBoundMin.y = 0;
            if (RoomBoundMax.x > size)
                RoomBoundMax.x = size;
            if (RoomBoundMax.y > size)
                RoomBoundMax.y = size;
        }// Check If In Map

        Tile[,] roomMap = new Tile[(RoomBoundMax.x - RoomBoundMin.x)+2, (RoomBoundMax.y - RoomBoundMin.y)+2];
        int roomArea = (RoomBoundMax.x - RoomBoundMin.x) * (RoomBoundMax.y - RoomBoundMin.y);
        int tileCount = 0;

        for (int x = 0; x < roomMap.GetLength(0); x++)
        {
            for (int y = 0; y < roomMap.GetLength(1); y++)
            {
                roomMap[x, y] = globalMap[x + RoomBoundMin.x, y + RoomBoundMin.y];
            }
        }


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
        } // Room Check

        if (tileCount > roomArea / 3f && room.isValid)
        {


            room.roomTiles = floorTiles;
            room.wallTiles = wallTiles;

            room.roomMap = roomMap;

            CreateMesh();

            return room;
        }
        else
        {
            goto restart;
        }
    }

    //END CREATE STRUCTS

    
    
    //MY METHODS
    
    Tile[,] BorderedMap(int _borderThickness = 2)
    {
        int borderThickness;
        Tile[,] borderedMap = new Tile[globalMap.GetLength(0), globalMap.GetLength(1)];
        if (_borderThickness != 2)
            borderThickness = _borderThickness;
        else
            borderThickness = _borderThickness;

        for (int x = 0; x < borderedMap.GetLength(0); x++)
        {
            for (int y = 0; y < borderedMap.GetLength(1); y++)
            {
                borderedMap[x, y] = globalMap[x, y];
                if (y+1 - size * (3 / 4f) == x && x == y-1 + size * (3 / 4f))
                    if (y+1 - size * (3 / 4f) == -x && -x == y-1 + size * (3 / 4f))
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
    public void LevelCreate(bool firstRoomIsSpawn = true)
    {
        Camera.main.orthographicSize = size / 2f + size / 20f;

        Rooms = new List<Room>();
        if (randomSeed)
            globalSeed = new Seed(new System.Random(DateTime.Now.GetHashCode()));

        globalPrng = new System.Random(globalSeed.GetHashCode());
        if (firstRoomIsSpawn)
        {
            Room room = newRoom(size / 2, size / 2, Enums.roomSize.Tiny, Enums.roomType.Spawn);
            if (room.isValid)
            {
                Rooms.Add(room);
            }
        }
        CreateMesh();
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
                                Debug.Log(Map[x, y].RoomID);
                                if (Map[x, y].RoomID == -1 || Map[x, y].RoomID == _RoomID)
                                {
                                    queue.Enqueue(Map[x, y]);
                                }
                                else
                                {
                                    Debug.Log("Tile Coliding:" + tile.RawCoord.coords);
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
        updateOverlay(globalMap);
        if (GenMesh == true)
        {
            MeshGenerator meshGen = GetComponent<MeshGenerator>();
            //meshGen.GenerateMesh(globalMap,1);
        }
    }
    void updateOverlay(Tile[,] Map)
    {
        /*
    SpriteRenderer OverlaySprite = overlay.GetComponent<SpriteRenderer>();
    SpriteRenderer GridSprite = grid.GetComponent<SpriteRenderer>();

    switch (overlayType)
    {
        case Enums.OverlayType.Tile:
            OverlaySprite.sprite = Sprite.Create(Overlay.Tiles(Map), new Rect(0,0, size, size), Vector2.one * .5f,1);
            break;
        case Enums.OverlayType.RoomTiles:
            OverlaySprite.sprite = Sprite.Create(Overlay.RoomTiles(Map, Rooms), new Rect(0, 0, size, size), Vector2.one * .5f, 1);
            break;
        case Enums.OverlayType.RoomSize:
            OverlaySprite.sprite = Sprite.Create(Overlay.RoomSize(Map, Rooms), new Rect(0, 0, size, size), Vector2.one * .5f, 1);
            break;
        case Enums.OverlayType.RoomClass:
            OverlaySprite.sprite = Sprite.Create(Overlay.RoomClass(Map, Rooms), new Rect(0, 0, size, size), Vector2.one * .5f, 1);
            break;
        case Enums.OverlayType.RoomType:
            OverlaySprite.sprite = Sprite.Create(Overlay.RoomType(Map, Rooms), new Rect(0, 0, size, size), Vector2.one * .5f, 1);
            break;
    }
        if (showGrid)
        {
            Texture2D gridCellTexture = TextureGenerator.textureFromSprite(gridCell);
            GridSprite.sprite = Sprite.Create(Grid.gridTexture(Map, gridCellTexture), new Rect(0, 0, 24, 24), Vector2.one * .5f, 1);
        }
        
            */
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
            return (x >= 0 && x < Map.GetLength(0)) && (y >= 0 && y < Map.GetLength(1));
        else
            return (x >= 0 && x < Map.GetLength(0)) && (y >= 0 && y < Map.GetLength(1));
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

    //END FOR EFFICIENCY
}