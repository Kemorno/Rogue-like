using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Resources;
using Enums;

public static class TileHandler
{
    public static Dictionary<CoordInt, Tile> Smooth(Dictionary<CoordInt, Tile> TilesToSmooth, Tile startTile, int SmoothMultiplier, int ComparisonFactor)
    {
        Dictionary<CoordInt, Tile> SmoothedTiles = new Dictionary<CoordInt, Tile>(TilesToSmooth);

        for (int smooth = 0; smooth < SmoothMultiplier; smooth++)
        {
            Queue<Tile> queue = new Queue<Tile>();
            queue.Enqueue(SmoothedTiles[startTile.Coord]);

            List<CoordInt> MapFlags = new List<CoordInt>();
            MapFlags.Add(SmoothedTiles[startTile.Coord].Coord);

            while (queue.Count > 0)
            {
                Tile cur = queue.Dequeue();

                int wallCount = CountNearWallTiles(cur.Coord, SmoothedTiles);

                if (wallCount > ComparisonFactor)
                    cur.SetType(tileType.Wall);
                else if (wallCount < ComparisonFactor)
                    cur.SetType(tileType.Floor);

                for (int NeighbourX = cur.Coord.x - 1; NeighbourX <= cur.Coord.x + 1; NeighbourX++)
                {
                    for (int NeighbourY = cur.Coord.y - 1; NeighbourY <= cur.Coord.y + 1; NeighbourY++)
                    {
                        CoordInt curCoord = new CoordInt(NeighbourX, NeighbourY);

                        if (cur.Coord.isAdjacent(curCoord))
                            if (!MapFlags.Contains(curCoord))
                            {
                                MapFlags.Add(curCoord);

                                if (SmoothedTiles.ContainsKey(curCoord))
                                    queue.Enqueue(SmoothedTiles[curCoord]);
                            }
                    }
                }
            }
        }

        return SmoothedTiles;
    }
    public static List<Chunk> Smooth(List<Chunk> ChunksToSmooth, int SmoothMultiplier, int ComparisonFactor, bool Regenerate = true)
    {
        List<Chunk> Chunks = new List<Chunk>();
        foreach (Chunk c in ChunksToSmooth)
        {
            if(Regenerate)
                c.RegenerateTiles();
            Chunks.Add(c);
        }

        Map tempMap = new Map(Chunks);

        Queue<Tile> queue = new Queue<Tile>();
        Tile startTile = tempMap.Chunks[Chunks[0].Coordinates].GetTile(0, 0);
        queue.Enqueue(startTile);

        List<CoordInt> MapFlags = new List<CoordInt>();
        MapFlags.Add(startTile.Coord);

        while (queue.Count > 0)
        {
            Tile cur = queue.Dequeue();

            int wallCount = CountNearWallTiles(cur.Coord, tempMap);

            if (wallCount > ComparisonFactor)
                cur.SetType(tileType.Wall);
            else if (wallCount < ComparisonFactor)
                cur.SetType(tileType.Floor);

            for (int NeighbourX = cur.Coord.x - 1; NeighbourX <= cur.Coord.x + 1; NeighbourX++)
            {
                for (int NeighbourY = cur.Coord.y - 1; NeighbourY <= cur.Coord.y + 1; NeighbourY++)
                {
                    CoordInt curCoord = new CoordInt(NeighbourX, NeighbourY);

                    if (cur.Coord.isAdjacent(curCoord))
                    {
                        if (!MapFlags.Contains(curCoord))
                        {
                            MapFlags.Add(curCoord);

                            Tile nextTile = tempMap.ContainsCoord(curCoord) ? tempMap.Chunks[tempMap.GetChunkCoord(curCoord)].Tiles[curCoord] : null;

                            if (nextTile != null)
                            {
                                queue.Enqueue(nextTile);
                            }
                        }
                    }
                }
            }
        }

        return Chunks;
    }

    private static int CountNearWallTiles(CoordInt Coord, Dictionary<CoordInt, Tile> Map)
    {
        int Count = 0;

        for (int NeighbourX = Coord.x - 1; NeighbourX <= Coord.x + 1; NeighbourX++)
        {
            for (int NeighbourY = Coord.y - 1; NeighbourY <= Coord.y + 1; NeighbourY++)
            {
                CoordInt curCoord = new CoordInt(NeighbourX, NeighbourY);
                if (curCoord != Coord)
                {
                    if (Map.ContainsKey(curCoord))
                        if (Map[curCoord].Type == tileType.Wall)
                            Count++;

                    else if (Coord.isAdjacent(curCoord))
                        return 8;
                }
            }
        }
        return Count;
    }
    private static int CountNearWallTiles(CoordInt Coord, Map Map)
    {
        int Count = 0;

        for (int NeighbourX = Coord.x - 1; NeighbourX <= Coord.x + 1; NeighbourX++)
        {
            for (int NeighbourY = Coord.y - 1; NeighbourY <= Coord.y + 1; NeighbourY++)
            {
                CoordInt curCoord = new CoordInt(NeighbourX, NeighbourY);
                if (curCoord != Coord)
                {
                    if (Map.ContainsCoord(curCoord))
                    {
                        if (Map.GetTile(curCoord).Type == tileType.Wall)
                            Count++;
                    }
                    else
                        return 9;
                }
            }
        }
        return Count;
    }
}
