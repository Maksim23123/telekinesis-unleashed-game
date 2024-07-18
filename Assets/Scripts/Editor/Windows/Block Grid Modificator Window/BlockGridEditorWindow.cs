using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class BlockGridEditorWindow : EditorWindow
{
    private GameObject _objectToSpawn;
    private int _brushIndex = 0;
    private List<BlockGridBrush> _brushes = new();

    private string _mousePositionInGrid = string.Empty;

    [MenuItem("Window/Block Grid Editor")]
    public static void ShowWindow()
    {
        GetWindow<BlockGridEditorWindow>("Block Grid Editor");
    }

    private void OnGUI()
    {
        _brushIndex = EditorGUILayout.Popup(_brushIndex, _brushes.Select(x => x.PreatyName).ToArray());

        if (_brushes.Count > 0)
        {
            _brushes[_brushIndex].OnGUI();
        }

        GUILayout.Label("Info", EditorStyles.boldLabel);
        GUILayout.Label(_mousePositionInGrid);

    }

    private void OnEnable()
    {
        InitBrushes();
        foreach (BlockGridBrush blockGridBrush in _brushes)
        {
            blockGridBrush.OnEnable();
        }
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && !Event.current.shift)
        {
            if (_brushes.Count > 0)
            {
                _brushes[_brushIndex].Draw();
            }
            Event.current.Use();
        }

        if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.shift)
        {
            if (_brushes.Count > 0)
            {
                _brushes[_brushIndex].Erase();
            }
            Event.current.Use();
        }

        if (_brushes.Count > 0)
        {
            _mousePositionInGrid = "Mouse Grid Position: " + _brushes[_brushIndex].MousePosition;
        }

        Repaint();
    }

    private void InitBrushes()
    {
        _brushes.Clear();
        _brushes.Add(new BlockPrefabGridBrush());
        _brushes.Add(new RoomPrefabGridBrush());
    }

    private void SpawnObjectAtMousePosition()
    {
        if (_objectToSpawn == null) return;

        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        GameObject spawnedObject = Instantiate(_objectToSpawn);
        if (spawnedObject != null)
        {
            Undo.RegisterCreatedObjectUndo(spawnedObject, "Spawned Object");
            spawnedObject.transform.position = ray.GetPoint(0);
        }
    }
}