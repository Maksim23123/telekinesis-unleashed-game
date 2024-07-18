using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class BlockGridBrush
{
    protected LevelManager _levelManager;

    public abstract string PreatyName { get; }

    public string MousePosition { 
        get
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            return $"{_levelManager.BlockGridSettings.WorldToGridPosition(ray.GetPoint(0))}";
        } 
    }

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

    protected void DefaultErase()
    {
        if (_levelManager != null)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            _levelManager.TryDestroyBlockByPosition(_levelManager.BlockGridSettings.WorldToGridPosition(ray.GetPoint(0)));
        }
    }
}
