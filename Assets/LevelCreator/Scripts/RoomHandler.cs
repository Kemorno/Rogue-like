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


    public static Room RoomFromRegion(Region region, Room MotherRoom, Map map)
    {
        return new Room(map.nextRoomID, region.GetChunks(MotherRoom), MotherRoom.GetSettings());
    }
}
