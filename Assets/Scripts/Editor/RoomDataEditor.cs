using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RoomData))]
public class RoomDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RoomData myScript = (RoomData)target;

        if (GUILayout.Button("Init Room Params"))
        {
            myScript.InitRoomParams();
        }
    }
}
