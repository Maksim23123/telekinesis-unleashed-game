#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(AssetPathAttribute))]
public class AssetPathDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        AssetPathAttribute assetPathAttribute = (AssetPathAttribute)attribute;

        EditorGUI.BeginProperty(position, label, property);

        Rect textFieldPosition = new Rect(position.x, position.y, position.width - 70, position.height);
        property.stringValue = EditorGUI.TextField(textFieldPosition, label, property.stringValue);

        Rect buttonPosition = new Rect(position.x + position.width - 65, position.y, 65, position.height);
        if (GUI.Button(buttonPosition, "Browse"))
        {
            EditorGUIUtility.ShowObjectPicker<UnityEngine.Object>(null, false, "", 0);
        }

        if (Event.current.commandName == "ObjectSelectorClosed")
        {
            UnityEngine.Object selectedObject = EditorGUIUtility.GetObjectPickerObject();
            if (selectedObject != null && assetPathAttribute.assetType.IsAssignableFrom(selectedObject.GetType()))
            {
                string assetPath = AssetDatabase.GetAssetPath(selectedObject);
                property.stringValue = assetPath;
            }
        }

        EditorGUI.EndProperty();
    }
}
#endif