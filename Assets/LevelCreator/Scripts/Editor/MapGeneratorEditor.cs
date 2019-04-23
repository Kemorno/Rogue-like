using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Resources;

[CustomEditor(typeof(CreateLevel))]
public class MapGeneratorEditor : Editor
{
    Vector2Int TileFinder;
    string RoomInfo;
    string TileInfo;
    Seed seed;
    bool showInfo;
    bool onMouse;

    public override void OnInspectorGUI()
    {
        CreateLevel main = (CreateLevel)target;
        if (GUILayout.Button("Reset Map"))
            main.ResetMap(main.randomSeed);

        if (GUILayout.Button("Generate Level"))
            main.StartGeneration();

        if (GUILayout.Button("New Seed"))
            main.newSeed();

        main.randomSeed = EditorGUILayout.Toggle("Use Random Seed? ", main.randomSeed);
        if (main.globalSeed != null)
        {
            EditorGUILayout.DelayedTextField("Seed",main.globalSeed.ToString());
        }
        else
        {
            EditorGUILayout.DelayedTextField("Seed", "No Seed");
        }

        if (DrawDefaultInspector())
        {

        }

        showInfo = EditorGUILayout.Toggle("Show Info", showInfo);
        onMouse = EditorGUILayout.Toggle("On Mouse", onMouse);
        if (showInfo)
        {
            {
                if (onMouse && main.mousePos != null)
                    TileFinder = new Vector2Int(Mathf.FloorToInt(main.mousePos.x), Mathf.FloorToInt(main.mousePos.y));
                if(main.Map != null) {
                    if (main.Map.ContainsKey(TileFinder))
                    {
                        TileInfo = main.Map[TileFinder].ToString();
                        if (main.Map[TileFinder].RoomId >= 0)
                        {

                            RoomInfo = main.Rooms[main.Map[TileFinder].RoomId].ToString();
                        }
                    }
                    else
                    {
                        TileInfo = "Not a Tile";
                        RoomInfo = "Not a Room";
                    }
                }
            }// Info
            TileFinder = EditorGUILayout.Vector2IntField("Tile Finder", TileFinder);
            EditorGUILayout.TextArea(TileInfo);
            EditorGUILayout.TextArea(RoomInfo);
        }
    }
}
