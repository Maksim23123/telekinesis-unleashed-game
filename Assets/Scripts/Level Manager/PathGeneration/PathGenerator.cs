using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Assertions.Must;


public class PathGenerator : MonoBehaviour
{
    //TODO: Remove debug functions after they aren't needed anymore 
    //----------
    [SerializeField] private Transform _startPositionBeacon, _endPositionBeacon;
    [SerializeField] private GameObject _pathPlaceholder;
    //----------
    [SerializeField] private Vector2 _startPosition, _endPosition;
    [SerializeField] private LevelManager _levelManager;
    [SerializeField] private PathType _pathType = PathType.OneTurnManhattan;
    [SerializeField] private float _straightWayMaxError;
    private Vector2Int _startPositionInGrid, _endPositionInGrid;
    private SmartPath _smartPath;
    private BlockGridSettings _blockGridSettings;

    

    public void Generate()
    {
        InitPositionsFromBeacons();
        GetBlockGridSettings();

        if (_pathType == PathType.OneTurnManhattan) 
        {
            GenerateOneTurnManhattanWay();
        }
        else if (_pathType == PathType.Smart)
        {
            GenerateSmartWay();
        }
    }

    private void GenerateSmartWay()
    {
        if (_smartPath == null)
        {
            _smartPath = new SmartPath(_levelManager);
        }

        UpdateStartEndPositionsInGrid();

        if (_levelManager.TryGetBlockInfoByPosition(_startPositionInGrid, out var _) 
                || _levelManager.TryGetBlockInfoByPosition(_endPositionInGrid, out var _))
        {
            Debug.LogWarning("Start or End position of a path is blocked");
        }
        else
        {
            _smartPath.AStar(_startPositionInGrid, _endPositionInGrid, out Vector2Int[] path);
            BlockInfoHolder pathPart = new BlockInfoHolder(_pathPlaceholder, Vector2Int.zero);

            for (int i = 0; i < path.Length; i++)
            {
                Vector2Int[] currentCellNeighbors;
                bool onStart = !(i > 0);
                bool onEnd = !(i < path.Length - 1);

                if (!onStart && !onEnd)
                {
                    currentCellNeighbors = new Vector2Int[]
                    {
                    path[i - 1],
                    path[i + 1]
                    };
                    _levelManager.BuildPathPart(path[i], currentCellNeighbors);
                }
                else if (onStart && !onEnd)
                {
                    currentCellNeighbors = new Vector2Int[]
                    {
                    path[i + 1]
                    };
                    _levelManager.BuildPathPart(path[i], currentCellNeighbors);
                }
                else if (!onStart && onEnd)
                {
                    currentCellNeighbors = new Vector2Int[]
                    {
                    path[i - 1]
                    };
                    _levelManager.BuildPathPart(path[i], currentCellNeighbors);
                }
                else
                {
                    _levelManager.BuildPathPart(path[i], new Vector2Int[0]);
                }
            }
        }
    }

    private void UpdateStartEndPositionsInGrid()
    {
        _startPositionInGrid = _levelManager.BlockGridSettings.WorldToGridPosition(_startPosition);
        _endPositionInGrid = _levelManager.BlockGridSettings.WorldToGridPosition(_endPosition);
    }

    private void GenerateOneTurnManhattanWay()
    {
        UpdateStartEndPositionsInGrid();
        Vector2Int endPositionInGridForHorizWay = _endPositionInGrid;

        if (_startPositionInGrid.x != _endPositionInGrid.x)
            endPositionInGridForHorizWay.x = _endPositionInGrid.x
                + (_endPositionInGrid.x < _startPositionInGrid.x ? 1 : -1);

        _levelManager.BuildHorizontalPath(_startPositionInGrid.x, endPositionInGridForHorizWay.x
            , _levelManager.BlockGridSettings.WorldToGridPosition(_startPosition).y);

        int horizExpandDirectionFactor = _blockGridSettings.HorizontalExpandDirectionFactor;

        if (_startPositionInGrid.y != _endPositionInGrid.y)
        {
            _levelManager.BuildVerticalPath(_startPositionInGrid.y, _endPositionInGrid.y, _endPositionInGrid.x
                , horizExpandDirectionFactor > 0 ? _startPositionInGrid.x > _endPositionInGrid.x : _startPositionInGrid.x < _endPositionInGrid.x);
        }
    }

    private void InitPositionsFromBeacons()
    {
        if (_startPositionBeacon != null)
        {
            _startPosition = _startPositionBeacon.position;
        }

        if (_endPositionBeacon != null)
        {
            _endPosition = _endPositionBeacon.position;
        }
    }

    private void GetBlockGridSettings()
    {
        _blockGridSettings = _levelManager.BlockGridSettings;
    }
}
