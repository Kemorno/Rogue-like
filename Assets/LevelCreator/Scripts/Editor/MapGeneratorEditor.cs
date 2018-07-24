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

    public override void OnInspectorGUI()
    {
        LevelGenerator lg = (LevelGenerator)target;
        if (GUILayout.Button("New"))
        {
            lg.LevelCreate(false);
        }

        if (DrawDefaultInspector())
        {

        }

        TileFinder = EditorGUILayout.Vector2IntField("Tile Finder", TileFinder);
        if (lg.onMouse)
            TileFinder = new Vector2Int(Mathf.FloorToInt(lg.mousePos.x), Mathf.FloorToInt(lg.mousePos.y));
        
        if (lg.IsInMapRange(TileFinder.x, TileFinder.y, true) && lg.Tiles != null)
        {
            int roomId = lg.Tiles[TileFinder.x, TileFinder.y].RoomID;
            TileInfo = "Tile Info"
                    + "\nRoom ID: " + lg.Tiles[TileFinder.x, TileFinder.y].RoomID
                    + "\nTile Type: " + lg.Tiles[TileFinder.x, TileFinder.y].tileType.ToString()
                    + "\nCoord: " + lg.Tiles[TileFinder.x, TileFinder.y].Coord.coords
                    + "\nRaw Coord: " + lg.Tiles[TileFinder.x, TileFinder.y].RawCoord.coords;

            if (lg.Tiles[TileFinder.x, TileFinder.y].RoomID >= 0)
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
        else if(!lg.IsInMapRange(TileFinder.x, TileFinder.y, true))
        {
            TileInfo = "Tile Info"
                    + "\nRoom ID: NaN"
                    + "\nTile Type: NaN"
                    + "\nCoord: Not in Map"
                    + "\nRaw Coord: Not in Map";
        }

        EditorGUILayout.TextArea(TileInfo);
        EditorGUILayout.TextArea(RoomInfo);

        if (GUILayout.Button("Reset Prng"))
            lg.resetPrng();
        if (GUILayout.Button("Seed Gen"))
            SeedGen = lg.newRoomSeed();
        EditorGUILayout.LabelField(SeedGen);
        if (GUILayout.Button("Global Seed Gen"))
            lg.NewSeed();
        EditorGUILayout.LabelField(lg.globalSeed);
    }
}
