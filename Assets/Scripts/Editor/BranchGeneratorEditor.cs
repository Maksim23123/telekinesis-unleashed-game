using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BranchGenerator))]
public class BranchGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        BranchGenerator myScript = (BranchGenerator)target;

        if (GUILayout.Button("Generate branches"))
        {
            myScript.Generate();
        } 
    }
}
