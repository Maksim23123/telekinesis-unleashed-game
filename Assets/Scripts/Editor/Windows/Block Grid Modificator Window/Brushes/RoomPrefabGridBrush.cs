using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RoomPrefabGridBrush : BlockGridBrush
{
    private RoomGenerator _roomGenerator;
    private GameObject[] _roomPrefabs;
    private int _selectedRoomIndex = -1;

    private GUIStyle normalButtonStyle;
    private GUIStyle selectedButtonStyle;

    public override string PreatyName => "Room Prefab Brush";

    public override void OnGUI()
    {
        if (_roomGenerator != null)
        {
            InitButtonStyles();
            GUILayout.Label("Block Prefabs", EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical("box");

            _roomPrefabs = _roomGenerator.RoomPrefabs;

            for (int i = 0; i < _roomPrefabs.Length; i++)
            {
                GUIStyle buttonStyle = (_selectedRoomIndex == i) ? selectedButtonStyle : normalButtonStyle;
                if (GUILayout.Button(_roomPrefabs[i].name, buttonStyle, GUILayout.Height(20)))
                {
                    _selectedRoomIndex = i;
                }
            }

            EditorGUILayout.EndVertical();
        }
        else
        {
            Debug.LogError("Instance of LevelManager class not found.");
        }
    }

    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; i++)
        {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }

    private void InitButtonStyles()
    {
        normalButtonStyle = new GUIStyle(GUI.skin.button);
        normalButtonStyle.normal.textColor = Color.white;

        selectedButtonStyle = new GUIStyle(GUI.skin.button);
        selectedButtonStyle.normal.textColor = Color.white;
        selectedButtonStyle.normal.background = MakeTex(2, 2, Color.green);
    }

    public override void Draw()
    {
        if (_selectedRoomIndex < 0) return;

        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        _roomGenerator.CreateRoom(_levelManager.BlockGridSettings.WorldToGridPosition(ray.GetPoint(0)), _roomPrefabs[_selectedRoomIndex]);
    }

    public override void OnEnable()
    {
        _roomGenerator = GameObject.FindObjectOfType<RoomGenerator>();
        _levelManager = GameObject.FindObjectOfType<LevelManager>();
    }

    public override void Erase()
    {
        DefaultErase();
    }
}
