using UnityEditor;

// ATTENTION: Need to renamed after InstanceSaveLoadManager
[CustomEditor(typeof(InstanceSaveLoadManager))]
public class PerGObjectSaveLoadManagerEditor : Editor
{
    SerializedProperty staticAddress;
    InstanceSaveLoadManager script;

    private void OnEnable()
    {
        script = (InstanceSaveLoadManager)target;
        staticAddress = serializedObject.FindProperty(nameof(script._staticAddress));
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultInspector();
            
        if (script.OnLoadDataDestributionType == GameObjectIdentificatorType.StaticAddress)
        {
            EditorGUI.BeginChangeCheck();

            string newStaticAddress = EditorGUILayout.TextField("Static Address", staticAddress.stringValue);

            if (EditorGUI.EndChangeCheck())
            {
                staticAddress.stringValue = newStaticAddress;
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
