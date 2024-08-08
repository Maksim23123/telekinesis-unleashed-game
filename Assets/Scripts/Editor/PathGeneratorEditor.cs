using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PathGenerator))]
public class PathGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PathGenerator myScript = (PathGenerator)target;

        if (GUILayout.Button("Convert GameObjects To Pointers"))
        {
            Debug.LogError("Removed feature use request");
        }
    }
}
