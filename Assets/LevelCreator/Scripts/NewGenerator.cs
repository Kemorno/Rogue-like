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

    public Vector2Int mousePos;
    public GameObject guide;
    public Material plainMaterial;

    Queue<List<Chunk>> queue = new Queue<List<Chunk>>();

    List<Chunk> coords;

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
            if(Input.GetMouseButtonDown(0))
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
        RoomSettings Settings = new RoomSettings(3, 50, 4, roomSize.Tiny, roomType.None, roomClass.Neutral);
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
            yield return new WaitForSeconds(0.3f);

            for (int s = 0; s < room.SmoothingMultiplier; s++)
            {
                room.SmoothChunks();
                map.ApplyChunks(room.GetChunkList());
                yield return new WaitForSeconds(0.3f);
            }
            room.GetMap();

            float check = room.Chunks.Count * Mathf.Pow(room.GetChunkSize(), 2) * TilePercentageFilled;

            if ((Mathf.Pow(room.GetChunkSize(), 2)* room.Chunks.Count) * TilePercentageFilled > (Mathf.Pow(room.GetChunkSize(), 2) * room.Chunks.Count) - (room.GetChunkSize() - 1) * 4)
                check = (Mathf.Pow(room.GetChunkSize(), 2)*room.Chunks.Count) - (room.GetChunkSize() - 1) * 4;
            
            if (room.GetTilesByType(tileType.Floor).Count >= check)
            {
                room.GetRegions();
                map.ApplyChunks(room.GetChunkList());
                yield return new WaitForSeconds(0.3f);
                FinishRoom(room);
                break;
            }
            else
            {
                foreach(Chunk c in Chunks)
                {
                    map.Chunks[c.Coordinates].RegenerateTiles();
                }
                if (Settings.RandomFillPercent < 90)
                    Settings.RandomFillPercent += 5;
                else if (Settings.SmoothingMultiplier > 1)
                {
                    Settings.RandomFillPercent = 40;
                    Settings.SmoothingMultiplier--;
                }
                else
                {
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
        if (drawGizmos)
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
                        foreach (System.Tuple<Tile, Tile> t in rg.ConnectionTile.Values)
                        {
                            Gizmos.color = Color.green;
                            Gizmos.DrawLine(new Vector3(t.Item1.Coord.GetVector2Int().x, t.Item1.Coord.GetVector2Int().y), new Vector3(t.Item2.Coord.GetVector2Int().x, t.Item2.Coord.GetVector2Int().y));
                        }
            }
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
            if (coords != null && coords.Count != 0)
            {
                foreach (Chunk c in coords)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawSphere(new Vector3(c.Coordinates.x * ChunkSize, c.Coordinates.y * ChunkSize), 2f);
                }
            }
        }
    }
    public void FinishRoom(Room room)
    {
        room.SetColor();
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
}