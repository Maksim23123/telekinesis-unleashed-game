using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DropSlot
{
    [SerializeField] private GameObject _gameObjectPrefab;
    [SerializeField] private int _weight;

    public GameObject GameObject { get => _gameObjectPrefab; set => _gameObjectPrefab = value; }
    public int Weight { get => _weight; set => _weight = value; }
}
