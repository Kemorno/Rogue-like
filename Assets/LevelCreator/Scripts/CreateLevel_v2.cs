using UnityEngine;
using System.Collections.Generic;

public class CreateLevel : MonoBehaviour
{
    public bool drawGizmos = false;

    public int RoomPasses = 5;
    public int DistanceBetweenRooms = 10;

    Vector2Int Start;
    List<Tile> Tiles = new List<Tile>();
    List<Room> Rooms = new List<Room>();

    private void Awake()
    {
        Start = new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.x));
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SetRoomOrigins();
        }
    }

    void SetRoomOrigins()
    {
        for(int i = 0; i < RoomPasses; i++)
        {
            for(int x = -DistanceBetweenRooms; x <= DistanceBetweenRooms; x++)
            {
                for (int y = -DistanceBetweenRooms; y <= DistanceBetweenRooms; y++)
                {
                    if(Vector2Int.Distance(new Vector2Int(x,y), new Vector2Int(DistanceBetweenRooms, DistanceBetweenRooms)) == DistanceBetweenRooms)
                    {
                        Tiles.Add(new Tile(new Vector2Int(x,y)));
                    }
                }
            }
        }
    }
    List<Tile> GetNearestTiles(Tile Start, Enums.tileType tileToSearch, bool SearchDiagonal = false, int Radius = 1)
    {
        List<Tile> tiles = new List<Tile>();
        List<Vector2Int> mapFlags = new List<Vector2Int>();

        Queue<Tile> queue = new Queue<Tile>();
        queue.Enqueue(tile);

        while(queue.Count > 0)
        {
            Tile tile = queue.Dequeue();
            mapFlags.Add(tile.Coord);
            if (tile.Type == tileToSearch)
                tiles.Add(tile);
            for(int neighbourX = tile.Coord.x - Radius; neighbourX < tile.Coord.x + Radius; neighbourX++)
            {
                for (int neighbourY = tile.Coord.y - Radius; neighbourY < tile.Coord.y + Radius; neighbourY++)
                {
                    if (!SearchDiagonal)
                        if(neighbourX == tile.Coord.x || neighbourY == tile.Coord.y)
                            if(tile.Type == tileToSearch && !mapFlags.Contains(curTile.Coord))
                            {
                                queue.Enqueue()
                            }
                }
            }
        }

        return tiles;
    }
    public class Tile
    {
        public Vector2Int Coord = Vector2Int.zero;
        public Enums.tileType Type = Enums.tileType.Wall;
        public bool walkable = false;

        public Tile(Vector2Int _Coord)
        {
            Coord = _Coord;
        }
    }
    public class Room
    {
        public int RoomID;
        public List<Tile> FloorTiles = new List<Tile>();
        public List<Tile> WallTiles = new List<Tile>();

        public Room(int _RoomID)
        {
            RoomID = _RoomID;
        }
    }
    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            if (Tiles.Count != 0)
            {
                for (int i = 0; i < Tiles.Count; i++)
                {
                    Vector3 pos = new Vector3(Tiles[i].Coord.x, Tiles[i].Coord.y);

                    Gizmos.color = (Tiles[i].Type == Enums.tileType.Wall) ? Color.black : Color.white;
                    Gizmos.DrawCube(pos, Vector3.one);
                }
            }
        }
    }
}