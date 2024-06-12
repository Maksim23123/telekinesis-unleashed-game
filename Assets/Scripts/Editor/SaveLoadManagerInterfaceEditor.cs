using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SaveLoadManagerInterface))]
public class SaveLoadManagerInterfaceEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SaveLoadManagerInterface myScript = (SaveLoadManagerInterface)target;

        if (GUILayout.Button("Save Game"))
        {
            myScript.RequestSaveGame();
        }

        if (GUILayout.Button("Load Game"))
        {
            myScript.RequestLoadGame();
        }
    }
}
