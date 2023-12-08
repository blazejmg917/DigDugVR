using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static UnityEngine.GraphicsBuffer;

[CustomEditor(typeof(DestructibleObject))]

public class BlockEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DestructibleObject ds = (DestructibleObject)target;

        base.OnInspectorGUI();
        if (GUILayout.Button("Destroy"))
        {
            ds.Break();
        }
        
    }
}
