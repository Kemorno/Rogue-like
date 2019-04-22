using System;
using System.Collections.Generic;
using UnityEngine;
using Resources;
using Enums;

public static class RoomHandler
{
    public static Dictionary<int, Room> SeparateRoomByRegion()
    {

    }
    public static Dictionary<int, Region> ConnectRegions()
    {

    }
    public static Dictionary<CoordInt, Tile> OuterTiles(Room room)
    {
        Dictionary<CoordInt, Tile> tiles = new Dictionary<CoordInt, Tile>();
            foreach (Tile tile in room.Map.Values)
            {
                for (int NeighbourX = tile.Coord.x - 1; NeighbourX <= tile.Coord.x + 1; NeighbourX++)
                {
                    for (int NeighbourY = tile.Coord.y - 1; NeighbourY <= tile.Coord.y + 1; NeighbourY++)
                    {
                        CoordInt curCoord = new CoordInt(NeighbourX, NeighbourY);
                        if (tile.Coord.isAdjacent(curCoord))
                        {
                        if (room.Map.Contains(curCoord))
                            if (room.Map[curCoord].Type == tileType.Wall)
                            {
                                tiles.Add(tile.Coord, tile);
                                goto next;
                            }
                        }
                    }
                }
            next:
                continue;
            }

        return tiles;
    }
}
