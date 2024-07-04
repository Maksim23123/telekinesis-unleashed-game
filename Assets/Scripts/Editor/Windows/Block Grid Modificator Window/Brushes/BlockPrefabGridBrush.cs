using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class BlockPrefabGridBrush : BlockGridBrush
{
    private LevelManager _levelManager;
    private int _selectedBlockIndex = -1;

    private bool _replaceOldBlocksMod = false;

    private GUIStyle normalButtonStyle;
    private GUIStyle selectedButtonStyle;

    public override string PreatyName => "Block Prefab Brush";

    public override void OnGUI()
    {
        if (_levelManager != null)
        {
            InitButtonStyles();
            GUILayout.Label("Block Prefabs", EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical("box");

            List<BlockInfoHolder> blockPrefabs = _levelManager.BlocksInfo;

            for (int i = 0; i < blockPrefabs.Count; i++)
            {
                GUIStyle buttonStyle = (_selectedBlockIndex == i) ? selectedButtonStyle : normalButtonStyle;
                if (GUILayout.Button(blockPrefabs[i].Name, buttonStyle, GUILayout.Height(20)))
                {
                    _selectedBlockIndex = i;
                }
            }

            EditorGUILayout.EndVertical();

            DrawHorizontalLine();

            _replaceOldBlocksMod = EditorGUILayout.ToggleLeft("Replace old blocks", _replaceOldBlocksMod);
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
        if (_selectedBlockIndex < 0) return;

        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        _levelManager.InstantiateCustomBlock(_levelManager.BlocksInfo[_selectedBlockIndex]
            , _levelManager.BlockGridSettings.WorldToGridPosition(ray.GetPoint(0)), _replaceOldBlocksMod);
    }

    public override void OnEnable()
    {
        _levelManager = GameObject.FindObjectOfType<LevelManager>();
    }

    public override void Erase()
    {
        DefaultErase(_levelManager);
    }
}
