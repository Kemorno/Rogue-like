using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LevelGenerator))]
public static class Tilemap
{
    public static LevelGenerator.Tile[,] newCircleMap(int width, int height, Enums.tileType startingTile = Enums.tileType.Wall)
    {
        LevelGenerator.Tile[,] Map = new LevelGenerator.Tile[width, height];
        Vector2Int center = new Vector2Int(width / 2, height / 2);

        for (int x = -width / 2; x <= width / 2; x++)
        {
            for (int y = -height / 2; y <= height / 2; y++)
            {
                Vector2Int pos = new Vector2Int(x + width, y + height);
                if (Mathf.Abs(pos.x - center.x) < width && Mathf.Abs(pos.y - center.y) < height)
                    Map[x + width / 2, y + height / 2] = new LevelGenerator.Tile(new LevelGenerator.Coord(x, y), new LevelGenerator.Coord(x - width / 2, y - height / 2), startingTile, -1);
            }
        }

        return Map;
    }
    public static LevelGenerator.Tile[,] newHexagonMap(int width, int height, Enums.tileType startingTile = Enums.tileType.Wall)
    {
        if (width % 2 == 0)
            width++;
        if (height % 2 == 0)
            height++;

        LevelGenerator.Tile[,] Map = new LevelGenerator.Tile[width, height];

        for (int x = -width / 2; x <= width / 2; x++)
        {
            for (int y = -height / 2; y <= height / 2; y++)
            {
                if (y - height * (9 / 16f) < x && x < y + height * (9 / 16f))
                    if (y - height * (9 / 16f) < -x && -x < y + height * (9 / 16f))
                        Map[x + width / 2, y + height / 2] = new LevelGenerator.Tile(new LevelGenerator.Coord(x, y), new LevelGenerator.Coord(x - width / 2, y - height / 2), startingTile, -1);
            }
        }

        return Map;
    }
    public static LevelGenerator.Tile[,] newDiamondMap(int width, int height, Enums.tileType startingTile = Enums.tileType.Wall)
    {
        if (width % 2 == 0)
            width++;
        if (height % 2 == 0)
            height++;

        LevelGenerator.Tile[,] Map = new LevelGenerator.Tile[width, height];

        for (int x = -width/2; x <= width/2; x++)
        {
            for (int y = -height/2; y <= height/2; y++)
            {
                if (y - width * .5f < x && x < y + width * .5f)
                    if (y - width * .5f < -x && -x < y + width * .5f)
                        Map[x + width / 2, y + height / 2] = new LevelGenerator.Tile(new LevelGenerator.Coord(x, y), new LevelGenerator.Coord(x - width / 2, y - height / 2), startingTile, -1);
            }
        }

        return Map;
    }
    public static LevelGenerator.Tile[,] newRectangleMap(int width, int height, Enums.tileType startingTile = Enums.tileType.Wall)
    {
        LevelGenerator.Tile[,] Map = new LevelGenerator.Tile[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Map[x, y] = new LevelGenerator.Tile(new LevelGenerator.Coord(x, y), new LevelGenerator.Coord(x - width / 2, y - height / 2), startingTile, -1);
            }
        }

        return Map;
    }

    public static LevelGenerator.Tile[,] TransferMap(LevelGenerator.Tile[,] _fromMap, LevelGenerator.Tile[,] _toMap, Vector2Int FromMapBoundMin)
    {
        LevelGenerator.Tile[,] fromMap = new LevelGenerator.Tile[_fromMap.GetLength(0), _fromMap.GetLength(1)];

        for (int x = 0; x < fromMap.GetLength(0); x++)
        {
            for (int y = 0; y < fromMap.GetLength(1); y++)
            {
                fromMap[x, y] = _fromMap[x, y];
            }
        }

        LevelGenerator.Tile[,] toMap = new LevelGenerator.Tile[_toMap.GetLength(0), _toMap.GetLength(1)];

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
                toMap[x + FromMapBoundMin.x, y + FromMapBoundMin.y] = fromMap[x, y];
            }
        }
        return toMap;
    }
    public static LevelGenerator.Tile[,] ChangeRoomID(LevelGenerator.Tile[,] _Map, List<LevelGenerator.Tile> floorTiles, List<LevelGenerator.Tile> wallTiles, int _RoomID, Vector2Int RoomBoundMin)
    {
        LevelGenerator.Tile[,] Map = _Map;

        for (int i = 0; i < floorTiles.Count; i++)
        {
            Map[floorTiles[i].RawCoord.coords.x - RoomBoundMin.x, floorTiles[i].RawCoord.coords.y - RoomBoundMin.y].RoomID = _RoomID;
        }
        for (int i = 0; i < wallTiles.Count; i++)
        {
            Map[wallTiles[i].RawCoord.coords.x - RoomBoundMin.x, wallTiles[i].RawCoord.coords.y - RoomBoundMin.y].RoomID = _RoomID;
        }

        return Map;
    }

}