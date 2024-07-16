using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelGenerator))]
public class LevelGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelGenerator myScript = (LevelGenerator)target;

        if (GUILayout.Button("Generatre map"))
        {
            myScript.ExecuteMapGenerationAlgorithm();
        }
    }
}
