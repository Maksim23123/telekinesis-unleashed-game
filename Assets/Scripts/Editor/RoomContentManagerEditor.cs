using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RoomContentManager))]
public class RoomContentManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RoomContentManager instance = (RoomContentManager)target;

        if (GUILayout.Button("Convert GameObjects To Pointers"))
        {
            instance.GameObjectsToPointers();
        }
    }
}
