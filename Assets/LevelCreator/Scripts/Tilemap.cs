using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LevelGenerator))]
public static class Tilemap
{
    public static LevelGenerator.Tile[,] newMap(int size, Enums.tileType startingTile = Enums.tileType.Wall)
    {
        if (size % 2 == 0)
            size++;

        LevelGenerator.Tile[,] Map = new LevelGenerator.Tile[size, size];

        for (int x = -size / 2; x <= size / 2; x++)
        {
            for (int y = -size / 2; y <= size / 2; y++)
            {
                if (y - size * (2 / 3f) < x && x < y + size * (2 / 3f))
                    if (y - size * (2 / 3f) < -x && -x < y + size * (2 / 3f))
                        Map[x + size / 2, y + size / 2] = new LevelGenerator.Tile(new LevelGenerator.Coord(x, y), new LevelGenerator.Coord(x - size / 2, y - size / 2));
            }
        }

        return Map;
    }

    public static LevelGenerator.Tile[,] TransferMap(LevelGenerator.Tile[,] _fromMap, LevelGenerator.Tile[,] _toMap, Vector2Int FromMapBoundMin)
    {
        LevelGenerator.Tile[,] toMap = new LevelGenerator.Tile[_toMap.GetLength(0), _toMap.GetLength(1)];

        for (int x = 0; x < toMap.GetLength(0); x++)
        {
            for (int y = 0; y < toMap.GetLength(1); y++)
            {
                toMap[x, y] = _toMap[x, y];
            }
        }
        for (int x = 0; x < _fromMap.GetLength(0); x++)
        {
            for (int y = 0; y < _fromMap.GetLength(1); y++)
            {
                toMap[x + FromMapBoundMin.x, y + FromMapBoundMin.y] = _fromMap[x, y];
            }
        }
        return toMap;
    }
    public static LevelGenerator.Tile[,] ChangeRoomID(LevelGenerator.Tile[,] _Map, List<LevelGenerator.Tile> floorTiles, List<LevelGenerator.Tile> wallTiles, int _RoomID, Vector2Int RoomBoundMin)
    {
        LevelGenerator.Tile[,] Map = _Map;

        for (int i = 0; i < floorTiles.Count; i++)
            Map[floorTiles[i].RawCoord.coords.x - RoomBoundMin.x, floorTiles[i].RawCoord.coords.y - RoomBoundMin.y].RoomID = _RoomID;
        for (int i = 0; i < wallTiles.Count; i++)
            Map[wallTiles[i].RawCoord.coords.x - RoomBoundMin.x, wallTiles[i].RawCoord.coords.y - RoomBoundMin.y].RoomID = _RoomID;

        return Map;
    }
}