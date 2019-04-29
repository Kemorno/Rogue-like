using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Resources;
using Enums;
using UnityEngine.EventSystems;

public class NewGenerator : MonoBehaviour
{
    public NewcameraGUI.selectedButton selected;
    public Seed GlobalSeed;
    public System.Random GlobalPrng;
    Dictionary<int, string> BlacklistedSeeds = new Dictionary<int, string>();
    GameObject GridGrouper;
    Dictionary<CoordInt, GameObject> ChunkGrids = new Dictionary<CoordInt, GameObject>();

    public int ChunkSize;
    public int amount = 10;
    [Range(0,1)]
    public float TilePercentageFilled = 0.333f;

    Map map;

    public bool drawGizmos;
    public bool SeeProgress;

    public Vector2Int mousePos;
    public GameObject guide;
    public Material plainMaterial;

    Queue<List<Chunk>> queue = new Queue<List<Chunk>>();

    List<Chunk> coords;

    public Vector2 start, end;
    private Rect rect;

    private void Awake()
    {
        GlobalSeed = new Seed();
        GlobalPrng = new System.Random(GlobalSeed.GetHashCode());

        map = new Map(ChunkSize);
        BlacklistedSeeds = LogHandler.GetBlacklistedSeeds();

        GridGrouper = new GameObject()
        {
            name = "Grid"
        };
    }

    private void Update()
    {
        {
            List<CoordInt> inScreen = new List<CoordInt>();
            Vector3 pos = Camera.main.transform.position;
            for (int x = Mathf.FloorToInt(pos.x - (Camera.main.orthographicSize * 16 / 9f) - ChunkSize/2f); x <= Mathf.CeilToInt(pos.x + (Camera.main.orthographicSize * 16 / 9f) + ChunkSize / 2f); x += ChunkSize)
            {
                for (int y = Mathf.FloorToInt(pos.y - Camera.main.orthographicSize - ChunkSize / 2f); y <= Mathf.CeilToInt(pos.y + Camera.main.orthographicSize + ChunkSize / 2f); y += ChunkSize)
                {
                    CoordInt coord = new CoordInt(Mathf.RoundToInt(x / (float)ChunkSize), Mathf.RoundToInt(y / (float)ChunkSize));
                    if (!ChunkGrids.ContainsKey(coord))
                    {
                        GameObject go = new GameObject();
                        go.AddComponent<SpriteRenderer>();
                        Texture2D text = Grid.ChunkGrid(ChunkSize, 32);
                        go.GetComponent<SpriteRenderer>().sprite = Sprite.Create(text, new Rect(0, 0, text.width, text.height), new Vector2(.5f, .5f), 32);
                        go.transform.position = new Vector3(coord.x * ChunkSize, coord.y * ChunkSize);
                        go.name = "Grid " + coord.x + "," + coord.y;
                        go.transform.parent = GridGrouper.transform;
                        inScreen.Add(coord);
                        ChunkGrids.Add(coord, go);
                    }
                    else
                    {
                        inScreen.Add(coord);
                    }
                }
            }

            for (int i = 0; i < inScreen.Count; i++)
            {
                if (ChunkGrids.ContainsKey(inScreen[i]))
                    ChunkGrids[inScreen[i]].SetActive(true);
                else
                    ChunkGrids[inScreen[i]].SetActive(false);
            }
        }//Grid

        mousePos = new Vector2Int(Mathf.FloorToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).x + .5f), Mathf.FloorToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).y + .5f));
        CoordInt Coord = new CoordInt(mousePos);

        if (guide != null)
            guide.transform.position = new Vector3(mousePos.x, mousePos.y);

        if (!EventSystem.current.IsPointerOverGameObject())
        {
            {
                if (Input.GetMouseButtonDown(0))
                    switch (selected)
                    {
                        case NewcameraGUI.selectedButton.None:
                            break;
                        case NewcameraGUI.selectedButton.Noise:
                            break;
                        case NewcameraGUI.selectedButton.Room:
                            coords = new List<Chunk>();
                            break;
                        case NewcameraGUI.selectedButton.Connect:
                            break;
                        case NewcameraGUI.selectedButton.Erase:
                            break;
                    }
                if (Input.GetMouseButton(0))
                    switch (selected)
                    {
                        case NewcameraGUI.selectedButton.None:
                            break;
                        case NewcameraGUI.selectedButton.Noise:
                            if (!map.Chunks.ContainsKey(map.GetChunkCoord(Coord)))
                                map.CreateChunk(Coord);
                            break;
                        case NewcameraGUI.selectedButton.Room:
                            if (map.Chunks.ContainsKey(map.GetChunkCoord(mousePos)) && !coords.Contains(map.Chunks[map.GetChunkCoord(mousePos)]) && map.Chunks[map.GetChunkCoord(Coord)].room == null)
                                coords.Add(map.Chunks[map.GetChunkCoord(mousePos)]);
                            break;
                        case NewcameraGUI.selectedButton.Connect:
                            break;
                        case NewcameraGUI.selectedButton.Erase:
                            if (map.ContainsCoord(Coord))
                            {
                                Chunk c = map.Chunks[map.GetChunkCoord(Coord)];
                                c.Tiles.Clear();
                                if (c.room != null)
                                {
                                    map.Rooms.Remove(c.room.ID);
                                    c.room.Chunks.Remove(c.Coordinates);
                                    foreach (Chunk cu in c.room.Chunks.Values)
                                    {
                                        cu.room = null;
                                        cu.RegenerateTiles();
                                    }
                                }
                                map.Chunks.Remove(c.Coordinates);
                            }
                            break;
                    }

                if (Input.GetMouseButtonUp(0))
                    switch (selected)
                    {
                        case NewcameraGUI.selectedButton.None:
                            break;
                        case NewcameraGUI.selectedButton.Noise:
                            break;
                        case NewcameraGUI.selectedButton.Room:
                            if (coords.Count > 0)
                            {
                                if (queue.Count == 0)
                                {
                                    queue.Enqueue(coords);
                                    RoomQueue();
                                }
                                else
                                    queue.Enqueue(coords);
                                coords.Clear();
                            }
                            break;
                        case NewcameraGUI.selectedButton.Connect:
                            break;
                        case NewcameraGUI.selectedButton.Erase:
                            break;
                    }
            }//0
            {
                if (Input.GetMouseButtonDown(1))
                    switch (selected)
                    {
                        case NewcameraGUI.selectedButton.None:
                            break;
                        case NewcameraGUI.selectedButton.Noise:
                            start = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            break;
                        case NewcameraGUI.selectedButton.Room:
                            start = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            coords = new List<Chunk>();
                            break;
                        case NewcameraGUI.selectedButton.Connect:
                            break;
                        case NewcameraGUI.selectedButton.Erase:
                            start = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            break;
                    }
                if (Input.GetMouseButton(1))
                    switch (selected)
                    {
                        case NewcameraGUI.selectedButton.None:
                            break;
                        case NewcameraGUI.selectedButton.Noise:
                            end = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            rect = FromDragPoints(start, end);
                            break;
                        case NewcameraGUI.selectedButton.Room:
                            end = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            rect = FromDragPoints(start, end);
                            break;
                        case NewcameraGUI.selectedButton.Connect:
                            break;
                        case NewcameraGUI.selectedButton.Erase:
                            end = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            rect = FromDragPoints(start, end);
                            break;
                    }

                if (Input.GetMouseButtonUp(1))
                    switch (selected)
                    {
                        case NewcameraGUI.selectedButton.None:
                            break;
                        case NewcameraGUI.selectedButton.Noise:
                            end = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            rect = FromDragPoints(start, end);
                            for (int x = Mathf.FloorToInt(rect.x); x <= Mathf.CeilToInt(rect.width); x++)
                            {
                                for (int y = Mathf.FloorToInt(rect.y); y <= Mathf.CeilToInt(rect.height); y++)
                                {
                                    CoordInt curCoord = new CoordInt(x, y);
                                    if (!map.Chunks.ContainsKey(map.GetChunkCoord(curCoord)))
                                        map.CreateChunk(curCoord);
                                }
                            }
                            start = new Vector2();
                            end = new Vector2();
                            rect = new Rect();
                            break;
                        case NewcameraGUI.selectedButton.Room:
                            end = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            rect = FromDragPoints(start, end);
                            for (int x = Mathf.FloorToInt(rect.x); x <= Mathf.CeilToInt(rect.width); x++)
                            {
                                for (int y = Mathf.FloorToInt(rect.y); y <= Mathf.CeilToInt(rect.height); y++)
                                {
                                    CoordInt curCoord = new CoordInt(x, y);
                                    if (map.Chunks.ContainsKey(map.GetChunkCoord(curCoord)))
                                        if(map.Chunks[map.GetChunkCoord(curCoord)].room == null)
                                            if(!coords.Contains(map.Chunks[map.GetChunkCoord(curCoord)]))
                                            coords.Add(map.Chunks[map.GetChunkCoord(curCoord)]);
                                }
                            }
                            if (coords.Count > 0)
                            {
                                if (queue.Count == 0)
                                {
                                    queue.Enqueue(coords);
                                    RoomQueue();
                                }
                                else
                                    queue.Enqueue(coords);
                                coords.Clear();
                            }
                            start = new Vector2();
                            end = new Vector2();
                            rect = new Rect();
                            break;
                        case NewcameraGUI.selectedButton.Connect:
                            break;
                        case NewcameraGUI.selectedButton.Erase:
                            end = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            rect = FromDragPoints(start, end); for (int x = Mathf.FloorToInt(rect.x); x <= Mathf.CeilToInt(rect.width); x++)
                            {
                                for (int y = Mathf.FloorToInt(rect.y); y <= Mathf.CeilToInt(rect.height); y++)
                                {
                                    CoordInt curCoord = new CoordInt(x, y);
                                    if (map.ContainsCoord(curCoord))
                                    {
                                        Chunk c = map.Chunks[map.GetChunkCoord(curCoord)];
                                        c.Tiles.Clear();
                                        if (c.room != null)
                                        {
                                            map.Rooms.Remove(c.room.ID);
                                            c.room.Chunks.Remove(c.Coordinates);
                                            foreach (Chunk cu in c.room.Chunks.Values)
                                            {
                                                cu.room = null;
                                                cu.RegenerateTiles();
                                            }
                                        }
                                        map.Chunks.Remove(c.Coordinates);
                                    }
                                }
                            }
                            start = new Vector2();
                            end = new Vector2();
                            rect = new Rect();
                            break;
                    }
            }//1
        }
    }

    public void RoomQueue()
    {
        while(queue.Count > 0)
            StartCoroutine(CreateRoom(queue.Dequeue()));
    }

    IEnumerator CreateRoom(List<Chunk> RoomChunks)
    {
        List<Chunk> Chunks = new List<Chunk>(RoomChunks);
        RoomSettings Settings = new RoomSettings(3, 40, 4, roomSize.Tiny, roomType.None, roomClass.Neutral);
        while (true)
        {
            Room room = new Room(map.nextRoomID, Chunks, Settings);
            map.AddRoom(room);
            while (true)
            {
                room.SetSeed(new Seed(GlobalPrng));
                if (!BlacklistedSeeds.ContainsKey(room.Seed.GetHashCode()))
                    break;
                else
                    Debug.Log("Found Blacklisted Seed: " + room.Seed.ToString());
            }

            room.GenerateTiles();
            map.ApplyChunks(room.GetChunkList());
            if(SeeProgress)
            yield return new WaitForSeconds(0.3f);

            for (int s = 0; s < room.SmoothingMultiplier; s++)
            {
                room.SmoothChunks();
                map.ApplyChunks(room.GetChunkList());
                if (SeeProgress)
                    yield return new WaitForSeconds(0.15f);
            }
            room.GetMap();

            float check = room.Chunks.Count * Mathf.Pow(room.GetChunkSize(), 2) * TilePercentageFilled;
            
            room.GetRegions();
            if (SeeProgress)
                yield return new WaitForSeconds(0.15f);
            room.GetMap();

            Debug.Log("Room has Tiles " + room.GetTilesByType(tileType.Floor).Count + ">=" + check);
            if (room.GetTilesByType(tileType.Floor).Count >= check)
            {
                room.SetColor();
                room.CreateConnections();
                map.ApplyChunks(room.GetChunkList());
                if(room.Regions.Count>1)
                room.GenerateRegionConnections();
                room.GetMap();
                map.ApplyChunks(room.GetChunkList());
                if (SeeProgress)
                    yield return new WaitForSeconds(0.3f);
                FinishRoom(room);
                break;
            }
            else
            {
                if (Settings.RandomFillPercent < 90)
                    Settings.RandomFillPercent += 5;
                else if (Settings.SmoothingMultiplier > 1)
                {
                    Settings.RandomFillPercent = 40;
                    Settings.SmoothingMultiplier--;
                }
                else
                {
                    foreach (Chunk c in Chunks)
                    {
                        map.Chunks[c.Coordinates].RegenerateTiles();
                    }
                    int FloorAmount = room.GetTilesByType(tileType.Floor).Count;
                    LogHandler.BlacklistSeed("\n" + room.Seed.ToString() + " Generated " + FloorAmount + " Tiles for " + room.Chunks.Count + " Chunks of size " + room.GetChunkSize()
                        + ". Expected at least " + (TilePercentageFilled * 100) + "% filled, got " + (room.Chunks.Count * Mathf.Pow(room.GetChunkSize(), 2) / (float)FloorAmount) + "%.");
                }
                continue;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (map != null && map.Chunks.Count != 0)
        {
            foreach (Chunk c in map.Chunks.Values)
                foreach (Tile t in c.Tiles.Values)
                {
                    Gizmos.color = (t.Type == tileType.Floor) ? Color.white : Color.black;
                    Gizmos.DrawCube(new Vector3(t.Coord.x, t.Coord.y), Vector3.one);
                }
            foreach (Room r in map.Rooms.Values)
                foreach (Region rg in r.Regions.Values)
                    foreach (System.Tuple<Tile, Tile> t in rg.Connection.Values)
                    {
                        Gizmos.color = Color.green;
                        Gizmos.DrawLine(new Vector3(t.Item1.Coord.GetVector2Int().x, t.Item1.Coord.GetVector2Int().y), new Vector3(t.Item2.Coord.GetVector2Int().x, t.Item2.Coord.GetVector2Int().y));
                    }
        }
        if (coords != null && coords.Count != 0)
        {
            foreach (Chunk c in coords)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawSphere(new Vector3(c.Coordinates.x * ChunkSize, c.Coordinates.y * ChunkSize), 1f);
            }
        }
        if (start != null && end != null && rect != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(new Vector3(rect.x, rect.y), new Vector3(rect.width, rect.y));
            Gizmos.DrawLine(new Vector3(rect.width, rect.yMin), new Vector3(rect.width, rect.height));
            Gizmos.DrawLine(new Vector3(rect.width, rect.height), new Vector3(rect.xMin, rect.height));
            Gizmos.DrawLine(new Vector3(rect.xMin, rect.height), new Vector3(rect.xMin, rect.yMin));
        }
        if (drawGizmos)
        {
            {
                Vector3 pos = Camera.main.transform.position;
                if (Camera.main.orthographicSize < 50)
                    for (int x = Mathf.FloorToInt(pos.x - (Camera.main.orthographicSize * 16 / 9f) - 0.5f); x <= Mathf.CeilToInt(pos.x + (Camera.main.orthographicSize * 16 / 9f) + 0.5f); x++)
                    {
                        for (int y = Mathf.FloorToInt(pos.y - Camera.main.orthographicSize - 0.5f); y <= Mathf.CeilToInt(pos.y + Camera.main.orthographicSize + 0.5f); y++)
                        {
                            Gizmos.color = Color.grey;
                            Gizmos.DrawWireCube(new Vector3(x, y), Vector3.one);
                        }
                    }

                for (int x = Mathf.FloorToInt(pos.x - (Camera.main.orthographicSize * 16 / 9f) - 0.5f); x <= Mathf.CeilToInt(pos.x + (Camera.main.orthographicSize * 16 / 9f) + 0.5f); x++)
                {
                    for (int y = Mathf.FloorToInt(pos.y - Camera.main.orthographicSize - 0.5f); y <= Mathf.CeilToInt(pos.y + Camera.main.orthographicSize + 0.5f); y++)
                    {
                        CoordInt coord = new CoordInt(Mathf.RoundToInt(x / (float)ChunkSize), Mathf.RoundToInt(y / (float)ChunkSize));
                        Gizmos.color = Color.white;
                        Gizmos.DrawWireCube(new Vector3(coord.x * ChunkSize, coord.y * ChunkSize), Vector3.one * ChunkSize);
                    }
                }
            }//Grid
        }
    }
    public void FinishRoom(Room room)
    {
        GameObject roomGo = new GameObject()
        {
            name = "Room #" + room.ID
        };
        roomGo.transform.position = room.GetMiddle();
        room.GameObject = roomGo;
        {
            GameObject wallGo = new GameObject()
            {
                name = "Room " + room.ID + " : Walls"
            };
            wallGo.transform.position = room.GetMiddle();
            wallGo.AddComponent<MeshGenerator>();
            wallGo.AddComponent<MeshFilter>();
            wallGo.AddComponent<MeshRenderer>();
            MeshGenerator meshGen = wallGo.GetComponent<MeshGenerator>();
            meshGen.GenerateMesh(DictionaryToArray(room.Map), 1, wallGo, room.ID, true);

            Material material = new Material(plainMaterial);
            material.color = Color.black;
            wallGo.GetComponent<MeshRenderer>().material = material;

            wallGo.transform.parent = roomGo.transform;
        }
        {
            GameObject FloorGo = new GameObject()
            {
                name = "Room " + room.ID + " : Floor"
            };
            FloorGo.transform.position = room.GetMiddle();
            FloorGo.AddComponent<MeshGenerator>();
            FloorGo.AddComponent<MeshFilter>();
            FloorGo.AddComponent<MeshRenderer>();
            MeshGenerator meshGen = FloorGo.GetComponent<MeshGenerator>();
            meshGen.GenerateMesh(DictionaryToArray(room.Map), 1, FloorGo, room.ID);

            Material material = new Material(plainMaterial);
            material.color = room.Color;
            FloorGo.GetComponent<MeshRenderer>().material = material;

            FloorGo.transform.parent = roomGo.transform;
        }
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

    Rect FromDragPoints(Vector2 P1, Vector2 P2)
    {
        Rect R = new Rect();
        R.x = Mathf.Min(P1.x, P2.x);
        R.y = Mathf.Min(P1.y, P2.y);
        R.width = Mathf.Max(P1.x, P2.x);
        R.height = Mathf.Max(P1.y, P2.y);
        return R;
    }
}