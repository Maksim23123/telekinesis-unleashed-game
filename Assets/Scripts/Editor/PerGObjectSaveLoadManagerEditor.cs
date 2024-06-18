using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(PerGObjectSaveLoadManager))]
public class PerGObjectSaveLoadManagerEditor : Editor
{
    SerializedProperty staticAddress;

    PerGObjectSaveLoadManager script;

    private void OnEnable()
    {
        script = (PerGObjectSaveLoadManager)target;
        staticAddress = serializedObject.FindProperty(nameof(script._staticAddress));
    }

    public override void OnInspectorGUI()
    {
        // Update the serialized object
        serializedObject.Update();

        // Draw the default inspector
        DrawDefaultInspector();

        if (!script.gameObject.TryGetComponent(out GameObjectIdentificator identificator)
                && script.OnLoadDataDestributionType == GameObjectIdentificatorType.PrefabPath)
        {
            script.OnLoadDataDestributionType = GameObjectIdentificatorType.StaticAddress;
            Debug.LogError("GameObjectIdentificator script Required for Prefab Path distribution type");
        }
            

        // Check the condition
        if (script.OnLoadDataDestributionType == GameObjectIdentificatorType.StaticAddress)
        {
            // Begin property change check
            EditorGUI.BeginChangeCheck();

            // Display the text field
            string newStaticAddress = EditorGUILayout.TextField("Static Address", staticAddress.stringValue);

            // If there are changes, apply them
            if (EditorGUI.EndChangeCheck())
            {
                staticAddress.stringValue = newStaticAddress;
            }
        }

        // Apply the modified properties
        serializedObject.ApplyModifiedProperties();
    }
}
