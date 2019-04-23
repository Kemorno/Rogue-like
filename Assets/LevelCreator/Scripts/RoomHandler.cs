using System;
using System.Collections.Generic;
using UnityEngine;
using Resources;
using Enums;

public static class RoomHandler
{
    /*
    public static Dictionary<int, Room> SeparateRoomByRegion()
    {

    }
    public static Dictionary<int, Region> ConnectRegions()
    {

    }
<<<<<<< HEAD
    */
=======
>>>>>>> 1918ad9ac84b568a719d326e3b293ee845ef1164


    public static Room RoomFromRegion(Region region, Room MotherRoom, Map map)
    {
        return new Room(map.nextRoomID, region.GetChunks(MotherRoom), MotherRoom.GetSettings());
    }
}
