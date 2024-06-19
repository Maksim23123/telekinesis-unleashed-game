using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyMovementOperator))]
public class EnemyMovementEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EnemyMovementOperator myScript = (EnemyMovementOperator)target;

        if (GUILayout.Button("Show vector position"))
        {
            myScript.ShowUpVectors();
        }
    }
}
