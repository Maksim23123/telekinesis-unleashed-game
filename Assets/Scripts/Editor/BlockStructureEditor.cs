using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BlockStructure))]
public class BlockStructureEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        BlockStructure myScript = (BlockStructure)target;

        if (GUILayout.Button("Init Structure Params"))
        {
            myScript.InitStructureParams();
        }
    }
}
