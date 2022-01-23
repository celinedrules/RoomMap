using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Room))]
public class RoomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var room = (Room)target;
        
        if (GUILayout.Button("Visit Room Test"))
        {
            room.Visited();
        }
    }
}
