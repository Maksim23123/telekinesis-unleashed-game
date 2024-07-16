using System.Collections;
using System.Collections.Generic;
using UnityEditor.Profiling;
using UnityEngine;

[RequireComponent(typeof(RoomGenerator))]
public class LevelGenerator : MonoBehaviour
{
    private RoomGenerator _roomGenerator;

    private void InitReferences()
    {
        _roomGenerator = GetComponent<RoomGenerator>();
    }

    public void ExecuteMapGenerationAlgorithm()
    {
        InitReferences();

        Debug.Log("It does nothing for now");
    }
}
