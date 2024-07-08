using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PathGenerator))]
public class PathGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PathGenerator myScript = (PathGenerator)target;

        if (GUILayout.Button("Build Path"))
        {
            myScript.Generate();
        }
    }
}
