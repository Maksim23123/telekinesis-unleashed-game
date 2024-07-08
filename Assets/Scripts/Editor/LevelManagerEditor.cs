using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelManager))]
public class LevelManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelManager myScript = (LevelManager)target;

        if (GUILayout.Button("Generate"))
        {
            myScript.Generate();
        }

        if (GUILayout.Button("Clear"))
        {
            myScript.ClearLevel();
        }
    }
}
