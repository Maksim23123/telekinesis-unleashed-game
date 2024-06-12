using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class GameObjectIdentificator : MonoBehaviour
{
    [AssetPath(typeof(GameObject))]
    [SerializeField]
    string _gameObjectPrefabPath;

    public string GameObjectPrefabPath { get => _gameObjectPrefabPath; }
}
