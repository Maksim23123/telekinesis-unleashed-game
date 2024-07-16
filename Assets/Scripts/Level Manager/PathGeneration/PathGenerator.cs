using UnityEngine;


public class PathGenerator : MonoBehaviour
{
    [SerializeField] private Transform _startPositionBeacon, _endPositionBeacon;
    [SerializeField] private GameObject _pathPlaceholder;
    [SerializeField] private Vector2 _startPosition, _endPosition;
    [SerializeField] private LevelManager _levelManager;
    [SerializeField] private float _straightWayMaxError;

    private Vector2Int _startPositionInGrid, _endPositionInGrid;
    private SmartPath _smartPath;
    private BlockGridSettings _blockGridSettings;

    public void Generate()
    {
        InitPositionsFromBeacons();
        GetBlockGridSettings();
        GenerateSmartWay();
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
