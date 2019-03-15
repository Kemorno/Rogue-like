using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelGenerator))]
public class MapGeneratorEditor : Editor
{
    Vector2Int TileFinder;
    string RoomInfo;
    string TileInfo;
    string SeedGen;
    bool showInfo;

    public override void OnInspectorGUI()
    {
        LevelGenerator lg = (LevelGenerator)target;
        if (GUILayout.Button("New"))
        {
            lg.LevelCreate(lg.createFirstRoom);
        }

        if (DrawDefaultInspector())
        {

        }
        showInfo = EditorGUILayout.Toggle("Show Info", showInfo);
        if (showInfo)
        {
            {
                if (lg.onMouse)
                    TileFinder = new Vector2Int(Mathf.FloorToInt(lg.mousePos.x), Mathf.FloorToInt(lg.mousePos.y));

                if (lg.IsInMapRange(TileFinder.x, TileFinder.y, lg.globalMap, true) && lg.globalMap != null)
                {
                    int roomId = lg.globalMap[TileFinder.x, TileFinder.y].RoomID;
                    TileInfo = "Tile Info"
                            + "\nRoom ID: " + lg.globalMap[TileFinder.x, TileFinder.y].RoomID
                            + "\nTile Type: " + lg.globalMap[TileFinder.x, TileFinder.y].tileType.ToString()
                            + "\nCoord: " + lg.globalMap[TileFinder.x, TileFinder.y].Coord.coords
                            + "\nRaw Coord: " + lg.globalMap[TileFinder.x, TileFinder.y].RawCoord.coords;
                    if (lg.globalMap[TileFinder.x, TileFinder.y].RoomID >= 0)
                    {

                        RoomInfo = "Room Info"
                            + "\nRoom ID: " + lg.Rooms[roomId].RoomID
                            + "\nRoom Type: " + lg.Rooms[roomId].roomType.ToString()
                            + "\nRoom Class: " + lg.Rooms[roomId].roomClass.ToString()
                            + "\nRoom Size: " + lg.Rooms[roomId].roomSize.ToString()
                            + "\nRoom Tiles = " + (lg.Rooms[roomId].roomTiles.Count + lg.Rooms[roomId].wallTiles.Count)
                            + "\n   Floor Tiles = " + lg.Rooms[roomId].roomTiles.Count
                            + "\n   Wall Tiles = " + lg.Rooms[roomId].wallTiles.Count
                            + "\nRoom Seed: " + lg.Rooms[roomId].roomSeed;
                    }
                    else
                    {
                        RoomInfo = "Room Info"
                            + "\nRoom ID: NaN"
                            + "\nRoom Type: NaN"
                            + "\nRoom Class: NaN"
                            + "\nRoom Size: NaN"
                            + "\nRoom Tiles = NaN"
                            + "\n   Floor Tiles = NaN"
                            + "\n   Wall Tiles = NaN"
                            + "\nRoom Seed = NaN";
                    }
                }
                else if (!lg.IsInMapRange(TileFinder.x, TileFinder.y, lg.globalMap, true))
                {
                    TileInfo = "Tile Info"
                            + "\nRoom ID: NaN"
                            + "\nTile Type: NaN"
                            + "\nCoord: Not in Map"
                            + "\nRaw Coord: Not in Map";
                }
            }// Info
            TileFinder = EditorGUILayout.Vector2IntField("Tile Finder", TileFinder);
            EditorGUILayout.TextArea(TileInfo);
            EditorGUILayout.TextArea(RoomInfo);
        }
    }
}
