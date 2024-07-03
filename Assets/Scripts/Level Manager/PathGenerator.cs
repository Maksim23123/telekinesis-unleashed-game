using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Assertions.Must;


public class PathGenerator : MonoBehaviour
{
    // ATTENTION: DEBUG
    //----------
    [SerializeField] private Transform _startPositionBeacon, _endPositionBeacon;
    //----------
    [SerializeField] private Vector2 _startPosition, _endPosition;
    [SerializeField] private LevelManager _levelManager;
    [SerializeField] private PathType _pathType = PathType.OneTurnManhattan;
    [SerializeField] private float _straightWayMaxError;
    private BlockGridSettings _blockGridSettings;

    

    public void Generate()
    {
        InitPositionsFromBeacons();
        GetBlockGridSettings();

        if (_pathType == PathType.OneTurnManhattan) 
        {
            GenerateOneTurnManhattanWay();
        }
        else if (_pathType == PathType.Straight)
        {
            GenerateStraightWay();
        }
    }

    private void GenerateStraightWay()
    {
        Vector2Int startPositionInGrid = _levelManager.BlockGridSettings.WorldToGridPosition(_startPosition);
        Vector2Int endPositionInGrid = _levelManager.BlockGridSettings.WorldToGridPosition(_endPosition);

        Vector2 pathDirection = ((Vector2)(endPositionInGrid - startPositionInGrid)).normalized;

        // Horizontal Path
        List<Vector3Int> horizontalWays = new(); // start coordinate x, start coordinate y, Length 
        int horizontalLength = endPositionInGrid.x - startPositionInGrid.x;
        int horizontalDirection = Mathf.Clamp(endPositionInGrid.x - startPositionInGrid.x, -1, 1);
        int currentHorizontalPosition = 0;

        Action<Vector3Int> setLastHorizWay = (Vector3Int horizWay) =>
        {
            if (horizontalWays.Count > 0)
            {
                horizontalWays[horizontalWays.Count - 1] = horizWay;
                return;
            }
            horizontalWays.Add(horizWay);
        };

        // Vertical Path
        List<Vector3Int> verticalWays = new(); // start coordinate x, start coordinate y, Height
        int verticalLength = endPositionInGrid.y - startPositionInGrid.y;
        int verticalDirection = Mathf.Clamp(endPositionInGrid.y - startPositionInGrid.y, -1, 1);
        int currentVerticalPosition = 0;

        Action<Vector3Int> setLastVertWay = (Vector3Int vertWay) =>
        {
            if (verticalWays.Count > 0)
            {
                verticalWays[verticalWays.Count - 1] = vertWay;
                return;
            }
            verticalWays.Add(vertWay);
        };

        bool verticalCorection = false;

        Vector2Int currentPosition = startPositionInGrid;

        bool upperTrasholdTriggered = verticalDirection > 0;

        //TODO: remove infinite loop stoper
        int iterations = 0;

        while (horizontalLength != currentHorizontalPosition && iterations < 1000) 
        {
            Vector2 currentPositionFromZero = currentPosition - startPositionInGrid;
            float pathDirectionMultiplier = currentPositionFromZero.x / pathDirection.x;
            Vector2 estimatedPath = pathDirection * pathDirectionMultiplier;
            bool currentPointBellowEstimatedPath = currentPositionFromZero.y < estimatedPath.y;
            if (!verticalCorection && horizontalLength > 0)
            {
                if (Mathf.Abs(currentPositionFromZero.y - estimatedPath.y) > _straightWayMaxError // check if way within the line
                    && (currentPointBellowEstimatedPath == upperTrasholdTriggered))
                {
                    verticalCorection = true;
                    upperTrasholdTriggered = currentPositionFromZero.y > estimatedPath.y; 
                }
                else
                {
                    if (horizontalWays.Count > 0 && currentPosition.y == horizontalWays.Last().y)
                    {
                        Vector3Int currentWay = horizontalWays.Last();
                        currentWay.z += horizontalDirection;
                        setLastHorizWay(currentWay);
                    }
                    else
                    {
                        horizontalWays.Add((Vector3Int)currentPosition);
                    }

                    currentPosition.x += horizontalDirection;
                    currentHorizontalPosition += horizontalDirection;
                }
            }
            else
            {

                if (Mathf.Abs(currentPositionFromZero.y - estimatedPath.y) > _straightWayMaxError // check if way within the line
                    && (currentPointBellowEstimatedPath == upperTrasholdTriggered))
                {
                    verticalCorection = false;
                    upperTrasholdTriggered = !currentPointBellowEstimatedPath;
                }
                else
                {
                    if (verticalWays.Count > 0 && currentPosition.x == verticalWays.Last().x)
                    {
                        Vector3Int currentWay = verticalWays.Last();
                        currentWay.z += verticalDirection;
                        setLastVertWay(currentWay);
                    }
                    else
                    {
                        verticalWays.Add((Vector3Int)currentPosition);
                    }

                    currentPosition.y += verticalDirection;
                    currentVerticalPosition += verticalDirection;
                }
            }

            foreach (Vector3Int horizontalWay in horizontalWays)
            {
                _levelManager.BuildHorizontalPath(horizontalWay.x, horizontalWay.x + horizontalWay.z, horizontalWay.y);
            }

            foreach (Vector3Int verticalWay in verticalWays)
            {
                _levelManager.BuildVerticalPath(verticalWay.y, verticalWay.y + verticalWay.z + 1, verticalWay.x, true);
            }

            iterations++;
        }

        foreach (var horizWay in horizontalWays)
        {
            Debug.Log(horizWay);
        }

        Debug.Log("-----");

        foreach (var vertWay in verticalWays)
        {
            Debug.Log(vertWay);
        }
    }

    private void GenerateOneTurnManhattanWay()
    {
        Vector2Int startPositionInGrid = _levelManager.BlockGridSettings.WorldToGridPosition(_startPosition);
        Vector2Int endPositionInGrid = _levelManager.BlockGridSettings.WorldToGridPosition(_endPosition);
        Vector2Int endPositionInGridForHorizWay = endPositionInGrid;

        if (startPositionInGrid.x != endPositionInGrid.x)
            endPositionInGridForHorizWay.x = endPositionInGrid.x
                + (endPositionInGrid.x < startPositionInGrid.x ? 1 : -1);

        _levelManager.BuildHorizontalPath(startPositionInGrid.x, endPositionInGridForHorizWay.x
            , _levelManager.BlockGridSettings.WorldToGridPosition(_startPosition).y);

        int horizExpandDirectionFactor = _blockGridSettings.HorizontalExpandDirectionFactor;

        if (startPositionInGrid.y != endPositionInGrid.y)
        {
            _levelManager.BuildVerticalPath(startPositionInGrid.y, endPositionInGrid.y, endPositionInGrid.x
                , horizExpandDirectionFactor > 0 ? startPositionInGrid.x > endPositionInGrid.x : startPositionInGrid.x < endPositionInGrid.x);
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
