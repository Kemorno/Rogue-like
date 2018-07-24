using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour {

    public int size;
    static int sizeStr;
    public bool drawGizmos = false;
    public bool drawGrid = false;
    public bool onMouse;

    public bool isCreatingRooms;
    public Enums.roomSize RoomSize;
    public Enums.roomClass RoomClass;
    public Enums.roomType RoomType;
    public List<Room> Rooms;
    public Tile[,] Tiles;

    public Vector2 mousePos;

    System.Random globalPrng = new System.Random();

    public string globalSeed;
    public bool randomSeed;


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
        public Tile(Coord _RawCoord, Coord _Coord, Enums.tileType _tileType, int _RoomID)
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
        public string roomSeed;
        public bool isValid;

        public Room(int _RoomID, Enums.roomSize _roomSize, Enums.roomType _roomType, Enums.roomClass _roomClass)
        {
            RoomID = _RoomID;
            roomClass = _roomClass;
            roomSize = _roomSize;
            roomType = _roomType;
            isValid = true;
            roomTiles = new List<Tile>();
            wallTiles = new List<Tile>();
            roomSeed = "";
        }
    }

    //END STRUCTS



    //UNITY METHODS
    
    private void Start()
    {
        LevelCreate(false);
    }

    private void Update()
    {

        mousePos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x + size / 2, Camera.main.ScreenToWorldPoint(Input.mousePosition).y + size / 2);
        GameObject.Find("Guide").transform.position = new Vector3((int)mousePos.x - size/2 +.5f, (int)mousePos.y - size / 2 +.5f, 0);


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
        if (Tiles != null && drawGizmos)
        {
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    Gizmos.color = (Tiles[x, y].tileType == Enums.tileType.Wall) ? Color.black : Color.white;
                    Vector3 pos = new Vector3(-size / 2 + x + .5f, -size / 2 + y + .5f, 0);
                    Gizmos.DrawCube(pos, Vector3.one);
                }
            }
        }
    }

    //END UNITY METHODS



    //CREATE STRUCTS

    Room newRoom(int originX, int originY, Enums.roomSize _RoomSize, Enums.roomType _RoomType, Enums.roomClass _RoomClass)
    {
        int restartCount = 0;

    restart:
        Room room = new Room(Rooms.Count, _RoomSize, _RoomType, _RoomClass);

        Vector2 RoomBoundMin = new Vector2(originX - (int)room.roomSize / 2f, originY - (int)room.roomSize / 2f);
        Vector2 RoomBoundMax = new Vector2(originX + (int)room.roomSize / 2f, originY + (int)room.roomSize / 2f);

        string _RoomSeed = newRoomSeed();
        
        room.roomSeed = _RoomSeed;
        
        {
            if (RoomBoundMin.x <= 0)
                RoomBoundMin.x = 1;
            if (RoomBoundMin.y <= 0)
                RoomBoundMin.y = 1;
            if (RoomBoundMax.x >= size)
                RoomBoundMax.x = size - 1;
            if (RoomBoundMax.y >= size)
                RoomBoundMax.y = size - 1;
        }

        int tileCount = 0;

        {
            for (int neighbouringX = (int)RoomBoundMin.x; neighbouringX < (int)RoomBoundMax.x; neighbouringX++)
            {
                for (int neighbouringY = (int)RoomBoundMin.y; neighbouringY < (int)RoomBoundMax.y; neighbouringY++)
                {
                    if (room.roomType != Enums.roomType.Spawn)
                        Tiles[neighbouringX, neighbouringY].tileType = (globalPrng.Next(0,100) < randomFillPercent) ? Enums.tileType.Wall : Enums.tileType.Floor;
                    else
                        Tiles[neighbouringX, neighbouringY].tileType = Enums.tileType.Floor;
                }
            }
        } // Room Creation
        {
            for (int i = 0; i < SmoothMultiplier; i++)
            {
                for (int neighbouringX = (int)RoomBoundMin.x; neighbouringX < (int)RoomBoundMax.x; neighbouringX++)
                {
                    for (int neighbouringY = (int)RoomBoundMin.y; neighbouringY < (int)RoomBoundMax.y; neighbouringY++)
                    {
                        int neighbourWallTiles = GetSurroundingWallCount(neighbouringX, neighbouringY);

                        if (neighbourWallTiles > 4)
                            Tiles[neighbouringX, neighbouringY].tileType = Enums.tileType.Wall;
                        else if (neighbourWallTiles < 4)
                            Tiles[neighbouringX, neighbouringY].tileType = Enums.tileType.Floor;
                    }
                }
            }
        } // Room Smoothing
        {
            for (int neighbouringX = (int)RoomBoundMin.x; neighbouringX < (int)RoomBoundMax.x; neighbouringX++)
            {
                for (int neighbouringY = (int)RoomBoundMin.y; neighbouringY < (int)RoomBoundMax.y; neighbouringY++)
                {
                    if (Tiles[neighbouringX, neighbouringY].tileType == Enums.tileType.Floor)
                    {
                        if (GetSurroundingWallCount(neighbouringX, neighbouringY) > 6)
                            Tiles[neighbouringX, neighbouringY].tileType = Enums.tileType.Wall;
                        else if(Tiles[neighbouringX, neighbouringY].tileType == Enums.tileType.Floor)
                            tileCount++;
                    }
                }
            }
        } // Room Tile Count

        if (tileCount > ((int)room.roomSize * (int)room.roomSize) / 3f)
        {
            originX = GetNearestTile(originX, originY, Enums.tileType.Floor).RawCoord.coords.x;
            originY = GetNearestTile(originX, originY, Enums.tileType.Floor).RawCoord.coords.y;

            room.roomTiles = GetRoomTiles(originX, originY, room.RoomID);
            room.wallTiles = GetRoomWallTiles(originX, originY, room.RoomID);

            return room;
        }
        else if (restartCount < 9)
        {
            restartCount++;
            goto restart;
        }
        else
        {
            Debug.Log("Could not create room");
            room.isValid = false;
            return room;
        }
    }

    //END CREATE STRUCTS



    //MY METHODS

    public string formatSeed(string Seed)
    {
        Seed = Seed.ToUpper();
        for (int i = 0; i < Seed.Length; i++)
        {
            if (!Alphanumeric.Contains(Seed[i].ToString()) && Seed[i] != ' ')
            {
                Seed = Seed.Insert(i, NewChar().ToString());
                Seed = Seed.Remove(i+1, 1);
            }
        }

        if (Seed == "")
        {
            for(int i = 0; i < 8; i++)
            {
                Seed += NewChar();
            }
            Debug.Log("No Seed Given, New Seed " + Seed);
        }
        else if (Seed.Length > 9)
        {
            Debug.Log("Seed Too Long");
            Seed = Seed.Substring(0, 9);
            Seed = Seed.Insert(4, " ");
            Seed = Seed.Remove(Seed.Length-1, 1);
            Debug.Log("New Seed " + Seed);
        }
        else if (Seed.Length < 8)
        {
            Debug.Log("Seed Too Short");
            for (int i = 0; i < 8 - Seed.Length -1; i++)
            {
                Seed += NewChar();
                Debug.Log("added " + Seed[Seed.Length -1]);
            }
            Debug.Log("New Seed " + Seed);
        }
        if (Seed[4] != ' ')
        {
            Seed = Seed.Insert(4, " ");
            Seed = Seed.Remove(Seed.Length-1, 1);
        }
        Debug.Log("Final Seed " + Seed);
        return Seed;
    }

    char NewChar()
    {
        return Alphanumeric[globalPrng.Next(0, Alphanumeric.Length)];
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

    public void LevelCreate(bool clean)
    {
        Camera.main.orthographicSize = size / 2f + size / 20f;
        
        Tiles = new Tile[size, size];
        if (randomSeed)
        {
            NewSeed();
        }
        resetPrng();
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                Tiles[x, y] = new Tile(new Coord(x, y), new Coord(x - size/2,y - size/2), Enums.tileType.Wall, -1);
            }
        }
        Rooms = new List<Room>();
        if (!clean)
        {
            Room room = newRoom(size / 2, size / 2, Enums.roomSize.Tiny, Enums.roomType.Spawn, Enums.roomClass.Neutral);
            if (room.isValid)
            {
                Rooms.Add(room);
            }
        }
        CreateMesh();
        Grid();
    }
    
    List<Tile> GetRoomTiles(int startX, int startY, int _RoomID)
    {
        List<Tile> tiles = new List<Tile>();
        int[,] mapFlags = new int[size, size];
        Enums.tileType tileType = Tiles[startX, startY].tileType;
        Tiles[startX, startY].RoomID = _RoomID;

        Queue<Tile> queue = new Queue<Tile>();
        queue.Enqueue(Tiles[startX, startY]);
        mapFlags[startX, startY] = 1;

        while (queue.Count > 0)
        {
            Tile tile = queue.Dequeue();
            tiles.Add(tile);
            Vector2Int RawCoord = new Vector2Int(tile.Coord.coords.x + size/2, tile.Coord.coords.y + size/2);

            for (int x = RawCoord.x - 1; x <= RawCoord.x + 1; x++)
            {
                for (int y = RawCoord.y - 1; y <= RawCoord.y + 1; y++)
                {
                    if (IsInMapRange(x, y, false) && (y == RawCoord.y || x == RawCoord.x))
                    {
                        if (mapFlags[x, y] == 0 && Tiles[x, y].tileType == tileType)
                        {
                            mapFlags[x, y] = 1;
                            Tiles[x, y].RoomID = _RoomID;
                            queue.Enqueue(Tiles[x,y]);
                        }
                    }
                }
            }
        }
        return tiles;
    }

    List<Tile> GetRoomWallTiles(int centerX, int centerY, int _RoomID)
    {
        List<Tile> wallTiles = new List<Tile>();
        int[,] mapFlags = new int[size, size];
        Enums.tileType tileType = Enums.tileType.Wall;
        Tiles[centerX, centerY].RoomID = _RoomID;

        Queue<Tile> queue = new Queue<Tile>();
        queue.Enqueue(Tiles[centerX, centerY]);
        mapFlags[centerX, centerY] = 1;

        while (queue.Count > 0)
        {
            Tile tile = queue.Dequeue();
            Vector2Int RawCoord = new Vector2Int(tile.Coord.coords.x + size / 2, tile.Coord.coords.y + size / 2);

            for (int x = RawCoord.x - 1; x <= RawCoord.x + 1; x++)
            {
                for (int y = RawCoord.y - 1; y <= RawCoord.y + 1; y++)
                {
                    if (IsInMapRange(x, y, false))
                    {
                        if (mapFlags[x, y] == 0)
                        {
                            mapFlags[x, y] = 1;
                            if (Tiles[x, y].tileType == tileType)
                            {
                                Tiles[x, y].RoomID = _RoomID;
                                wallTiles.Add(tile);
                            }
                            else
                            {
                                queue.Enqueue(Tiles[x, y]);
                            }
                        }
                    }
                }
            }
        }
        return wallTiles;
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
                    Tiles[tile.coords.x, tile.coords.y].tileType = Enums.tileType.Floor;
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
                    Tiles[tile.coords.x, tile.coords.y].tileType = Enums.tileType.Wall;
                }
            }
        }*/
    }

    Tile[,] BorderedMap(int _borderThickness)
    {
        int borderThickness;
        Tile[,] borderedMap = new Tile[size, size];
        if (_borderThickness == 0)
            borderThickness = 2;
        else
            borderThickness = _borderThickness;

        for(int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                borderedMap[x, y] = Tiles[x, y];
                if (x < borderThickness || x > size - borderThickness || y < borderThickness || y > size - borderThickness)
                {
                    Tiles[x, y].tileType = Enums.tileType.Wall;
                    if (x < borderThickness - 1 || x > size - borderThickness + 1 || y < borderThickness - 1 || y > size - borderThickness + 1)
                        borderedMap[x, y].RoomID = -1;
                }
            }
        }

        return borderedMap;
    }
    
    //END MY METHODS



    //FOR EFFICIENCY

    void Grid()
    {
        if (drawGrid)
        {
            Grid grid = FindObjectOfType<Grid>();
            grid.DestroyGrid();
            grid.rows = size;
            grid.cols = size;
            grid.gridSize = new Vector2(size, size);
            grid.InitCells();
        }
    }

    void CreateMesh()
    {
        ProcessMap();
        MeshGenerator meshGen = GetComponent<MeshGenerator>();
        meshGen.GenerateMesh(BorderedMap(0), 1);
    }

    public bool IsInMapRange(int x, int y,bool isFloat)
    {
        if(!isFloat)
            return (x >= 0 && x <= size) && (y >= 0 && y <= size);
        else
            return (x >= 0 && x <= size-1) && (y >= 0 && y <= size-1);
    }

    int GetSurroundingWallCount(int gridX, int gridY)
    {
        int WallCount = 0;
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                if (IsInMapRange(neighbourX, neighbourY, false))
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

                    if ((neighbourX != gridX || neighbourY != gridY) && Tiles[x, y].tileType == Enums.tileType.Wall)
                    {
                        WallCount++;
                    }
                }
            }
        }
        return WallCount;
    }

    Tile GetNearestTile(int startX, int startY, Enums.tileType tileType)
    {
        Tile NearestWallTile = Tiles[startX, startY];
        int[,] mapFlags = new int[size, size];
        
        Queue<Tile> queue = new Queue<Tile>();
        queue.Enqueue(Tiles[startX, startY]);
        mapFlags[startX, startY] = 1;

        while (queue.Count > 0)
        {
            Tile tile = queue.Dequeue();

            for (int x = tile.RawCoord.coords.x - 1; x <= tile.RawCoord.coords.x + 1; x++)
            {
                for (int y = tile.RawCoord.coords.y - 1; y <= tile.RawCoord.coords.y + 1; y++)
                {
                    if (IsInMapRange(x, y, false) && (y == tile.RawCoord.coords.y || x == tile.RawCoord.coords.x))
                    {
                        if (mapFlags[x, y] == 0)
                        {
                            if (Tiles[x, y].tileType != tileType)
                            {
                                mapFlags[x, y] = 1;
                                queue.Enqueue(Tiles[x, y]);
                            }
                            else
                            {
                                NearestWallTile = tile;
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

        return NearestWallTile;
    }

    bool isSeedValid(string Seed)
    {
        bool isValid = true;

        if (globalSeed[4] != ' ' || globalSeed.Length != 9)
            isValid = false;
        Debug.Log("Seed " + Seed + "is" + ((isValid) ? "Valid" : "Invalid"));
        return isValid;
    }

    //END FOR EFFICIENCY
}

