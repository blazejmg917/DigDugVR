using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridSpawner))]
public class GridCreatorEditor : Editor
{
     public override void OnInspectorGUI()
    {
        GridSpawner gs = (GridSpawner)target;
       
        base.OnInspectorGUI();
        if (GUILayout.Button("Spawn Grid"))
        {
            gs.SpawnGrid();
        }
        if (GUILayout.Button("Relink Grid"))
        {
            gs.ConnectGrid();
        }
        if (GUILayout.Button("Clear Grid"))
        {
            gs.ClearGrid();
        }

        // if (GUILayout.Button("Display Paths"))
        // {
        //     if (zpg.IsSceneBound())
        //     {
        //         zpg.DebugLines();
        //     }
        // }
    }
}
