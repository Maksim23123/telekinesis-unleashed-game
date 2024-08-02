using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LevelManager))]
[RequireComponent(typeof(RoomGenerator))]
[RequireComponent(typeof(PathPlanner))]
[RequireComponent(typeof(PathGenerator))]
public class LevelGenerator : MonoBehaviour
{
    private RoomGenerator _roomGenerator;
    private LevelManager _levelManager;
    private PathPlanner _pathPlanner;
    private PathGenerator _pathGenerator;
    private MapFinalizer _mapFinalizer;

    private void InitReferences()
    {
        _roomGenerator = GetComponent<RoomGenerator>();
        _levelManager = GetComponent<LevelManager>();
        _pathPlanner = GetComponent<PathPlanner>();
        _pathGenerator = GetComponent<PathGenerator>();
        _mapFinalizer = GetComponent<MapFinalizer>();
    }

    public void ExecuteMapGenerationAlgorithm()
    {
        InitReferences();

        _levelManager.ClearLevel();
        List<List<PathUnit>[]> roomStructure = _roomGenerator.GenerateRooms();
        foreach (GridArea gridArea in _roomGenerator.AreasToFinalize)
        {
            Debug.Log(gridArea.AreaStart.y + " " + gridArea.AreaEnd.y);
        }
        HashSet<PathUnit> pathPlan = _pathPlanner.GeneratePathPlan(roomStructure);
        _pathGenerator.GeneratePaths(pathPlan, roomStructure);

        foreach (GridArea areaToFinalize in _roomGenerator.AreasToFinalize)
        {
            _mapFinalizer.FinalizeArea(areaToFinalize);
        }
    }
}
