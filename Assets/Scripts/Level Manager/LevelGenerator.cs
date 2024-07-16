using System.Collections;
using System.Collections.Generic;
using UnityEditor.Profiling;
using UnityEngine;

[RequireComponent(typeof(LevelManager))]
[RequireComponent(typeof(RoomGenerator))]
[RequireComponent(typeof(PathPlanner))]
public class LevelGenerator : MonoBehaviour
{
    private RoomGenerator _roomGenerator;
    private LevelManager _levelManager;
    private PathPlanner _pathPlanner;

    private void InitReferences()
    {
        _roomGenerator = GetComponent<RoomGenerator>();
        _levelManager = GetComponent<LevelManager>();
        _pathPlanner = GetComponent<PathPlanner>();
    }

    public void ExecuteMapGenerationAlgorithm()
    {
        InitReferences();

        _levelManager.ClearLevel();
        List<List<PathUnit>[]> roomStructure = _roomGenerator.GenerateRooms();
        HashSet<PathUnit> pathPlan = _pathPlanner.GeneratePathPlan(roomStructure);
    }
}
