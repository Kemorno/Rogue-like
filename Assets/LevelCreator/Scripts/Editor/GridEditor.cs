using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Grid))]
public class GridEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Grid grid = (Grid)target;
        if (GUILayout.Button("Draw Grid"))
        {
            grid.DestroyGrid();
            grid.InitCells();
        }

        if (DrawDefaultInspector())
        {

        }
    }
}
