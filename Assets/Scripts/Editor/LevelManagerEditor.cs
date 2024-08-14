using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelManager))]
public class LevelManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelManager myScript = (LevelManager)target;

        if (GUILayout.Button("Clear"))
        {
            myScript.ClearLevel();
        }

        if (GUILayout.Button("Show block markers"))
        {
            myScript.ShowBlocks();
        }

        if (GUILayout.Button("Remove block markers"))
        {
            myScript.RemoveBlockMarkers();
        }
    }
}
