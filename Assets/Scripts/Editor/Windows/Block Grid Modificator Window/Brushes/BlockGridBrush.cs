using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class BlockGridBrush
{
    public abstract string PreatyName { get; }

    public abstract void OnGUI();

    public abstract void OnEnable();

    public abstract void Draw();

    public abstract void Erase();

    protected void DrawHorizontalLine()
    {
        GUILayout.Space(10);
        Rect rect = EditorGUILayout.GetControlRect(false, 2);
        EditorGUI.DrawRect(rect, Color.gray);
        GUILayout.Space(10);
    }

    protected void DefaultErase(LevelManager levelManager)
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        levelManager.TryDestroyBlockByPosition(levelManager.BlockGridSettings.WorldToGridPosition(ray.GetPoint(0)));
    }
}
