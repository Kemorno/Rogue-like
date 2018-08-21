using UnityEngine;
using System.Collections.Generic;

public class TestCreator : MonoBehaviour
{
    int amountOfRooms;
    int RoomPasses;
    int DistanceBetweenRooms;
    List<List<Vector2Int>> guideTiles = new List<List<Vector2Int>>();

    List<Vector2Int> Tiles = new List<Vector2Int>();

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