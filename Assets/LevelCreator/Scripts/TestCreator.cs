using UnityEngine;
using System.Collections.Generic;
using Resources;

public class TestCreator : MonoBehaviour
{
    public int Size;

    int amountOfRooms;
    int RoomPasses;
    int DistanceBetweenRooms;
    List<List<Vector2Int>> guideTiles = new List<List<Vector2Int>>();
    CreateLevel Creator;

    List<Vector2Int> Tiles = new List<Vector2Int>();
    private void Awake()
    {
        Creator = FindObjectOfType<CreateLevel>();
    }
    void CreateMap()
    {
        List<CoordInt> mapCheck = new List<CoordInt>();
        for(int x = -Size; x <= Size; x++)
        {
            for (int y = -Size; y <= Size; y++)
            {
                Room room;
                CoordInt curCoord = new CoordInt(x, y);
                bool compatible = true;
                for(int neighbourX = x-2; neighbourX <= x+2; x++)
                {
                    for (int neighbourY = x - 2; neighbourY <= x + 2; y++)
                    {
                        CoordInt neighbourCoord = new CoordInt(neighbourX, neighbourY);
                        if (mapCheck.Contains(neighbourCoord))
                        {
                            compatible = false;
                            break;
                        }
                    }
                    if (!compatible)
                        break;
                }
                if (compatible)
                {
                    room = Creator.CreateRoom(Creator.mousePos, Creator.Settings);
                    if (room != null)
                    {
                        foreach (CoordInt tile in room.Tiles)
                        {
                            if (!mapCheck.Contains(tile))
                                mapCheck.Add(tile);
                        }
                        Creator.Rooms.Add(room);
                    }
                }
                else
                    Debug.Log("Couldn't Create Room");
            }
        }
        Debug.Log("Map Info - Size " + Size + " Number of Rooms " + Creator.Rooms.Count);
    }

    void SetRoomOrigins()
    {
        Tiles = new List<Vector2Int>();
        int rawDist = DistanceBetweenRooms;
        for (int i = 0; i < RoomPasses; i++)
        {
            List<Vector2Int> PassTiles = new List<Vector2Int>();
            rawDist += DistanceBetweenRooms;
            for(int x = -rawDist; x <= rawDist; x++)
            {
                for (int y = -rawDist; y <= rawDist; y++)
                {
                    Vector2Int Pos = new Vector2Int(x, y);

                    if (Mathf.FloorToInt(Vector2.Distance(Pos, Vector2.zero)) == rawDist)
                        PassTiles.Add(Pos);
                }
            }
            guideTiles.Add(PassTiles);
        }

        /*
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
    private void OnDrawGizmos()
    {
        if(guideTiles != null)
        {
            Gizmos.color = Color.black;
            foreach (List<Vector2Int> group in guideTiles)
            {
                foreach(Vector2Int coord in group)
                {
                    Gizmos.DrawCube(new Vector3(coord.x, coord.y), Vector3.one * 0.5f);
                }
            }
        }
    }
}