using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(LevelManager))]
public class PathGenerator : MonoBehaviour
{
    [SerializeField] private GameObject _tripletRightOut;
    [SerializeField] private GameObject _tripletLeftOut;
    [SerializeField] private Vector2 _startPosition, _endPosition;
    [SerializeField][HideInInspector] private List<Triplet> _instantiatedTriplets = new();

    const int VERTICAL_POSITION_OFFSET = 3;
    const int VERTICAL_POSITION_ABOVE_OFFSET = 0;
    const int VERTICAL_POSITION_BELOW_OFFSET = 1;

    private LevelManager _levelManager;
    private SmartPath _smartPath;

    public BlockGridSettings BlockGridSettings { get => _levelManager.BlockGridSettings; }

    public void GeneratePaths(HashSet<PathUnit> pathPlan)
    {
        Initialize();
        InstantiateTriplets(pathPlan);
        ShowTripletsResume(pathPlan);
    }

    private void InstantiateTriplets(HashSet<PathUnit> pathPlan)
    {
        if (_tripletRightOut == null || _tripletLeftOut == null)
        {
            Debug.LogError("Triplet prefabs weren't assigned.");
            return;
        }

        Triplet currentTriplet;

        int emergencyStopCounter = 0;

        do
        {
            emergencyStopCounter++;
            currentTriplet = pathPlan.Where(x =>
                    CheckIfTripletHasAllBackConnectionsInstantiated(pathPlan, x) && !_instantiatedTriplets.Any(g => g.Id == x.Id))
                .Select(x => (Triplet)x)
                .FirstOrDefault();

            if (currentTriplet != null)
            {
                Vector2Int[] backConnectionPositions = pathPlan
                    .Where(x => currentTriplet.BackConnections.Any(g => g == x.Id))
                    .Select(x => ExtractConnectionPointPosition(x))
                    .ToArray();

                int horizontalPosition = backConnectionPositions.Sum(g => g.x) / backConnectionPositions.Length;


                int verticalPosition;

                bool currentTripletIsBackConnection = pathPlan.Any(x =>
                {
                    if (x is Triplet)
                    {
                        Triplet tempTriplet = (Triplet)x;
                        return tempTriplet.BackConnections.Contains(currentTriplet.Id);
                    }
                    return false;
                });

                if (!currentTripletIsBackConnection)
                {
                    verticalPosition = backConnectionPositions.Sum(g => g.y) / backConnectionPositions.Length;
                }
                else if (currentTriplet.placement == TripletPlacement.Above)
                {
                    int highestBackConnectionPosition = backConnectionPositions.Max(g => g.y);
                    verticalPosition = highestBackConnectionPosition + VERTICAL_POSITION_ABOVE_OFFSET + VERTICAL_POSITION_OFFSET;
                }
                else
                {
                    int lowestBackConnectionPosition = backConnectionPositions.Min(g => g.y);
                    verticalPosition = lowestBackConnectionPosition - VERTICAL_POSITION_BELOW_OFFSET - VERTICAL_POSITION_OFFSET;
                }

                InstantiateTriplet(ref currentTriplet, new Vector2Int(horizontalPosition, verticalPosition));
            }

            if (emergencyStopCounter >= 10000)
            {
                Debug.LogWarning("Emergency infinit loop stop.");
                break;
            }
        }
        while (currentTriplet != null);
    }

    private void ShowTripletsResume(HashSet<PathUnit> pathPlan)
    {
        foreach (PathUnit triplet in _instantiatedTriplets)
        {
            Triplet currentTriplet = triplet as Triplet;
            string resume = string.Empty;

            resume += "ID: " + currentTriplet.Id + " | ";
            resume += "Position: " + _levelManager.BlockGridSettings
                .WorldToGridPosition(currentTriplet.GameObject.transform.position) + " | ";

            resume += "First connection point: " + ExtractConnectionPointPosition(GetById(pathPlan, currentTriplet.BackConnections[0])) + " | ";
            resume += "Second connection point: " + ExtractConnectionPointPosition(GetById(pathPlan, currentTriplet.BackConnections[1])) + " | ";

            Debug.Log(resume);
        }
    }

    private void InstantiateTriplet(ref Triplet triplet, Vector2Int position)
    {
        GameObject tripletPrefab = null;
        if (triplet.Orientation == Orientation.Right)
        {
            tripletPrefab = _tripletRightOut;
        }
        else if (triplet.Orientation == Orientation.Left)
        {
            tripletPrefab = _tripletLeftOut;
        }

        if (tripletPrefab != null)
        {
            GameObject tripletInstance = BlockStructure.InstantiateStructure(tripletPrefab, position, _levelManager);
            triplet.GameObject = tripletInstance;
            _instantiatedTriplets.Add(triplet);
        }
    }

    private Vector2Int ExtractConnectionPointPosition(PathUnit x)
    {
        if (x is PathEnd)
        {
            return ((PathEnd)x).Connection.GetConnectionPoint(BlockGridSettings);
        }
        else if (x is Triplet)
        {
            Triplet currentTriplet = (Triplet)GetById(_instantiatedTriplets, x.Id);
            BlockStructure blockStructure = currentTriplet.GameObject.GetComponent<BlockStructure>();
            return blockStructure.ExitConnection.GetConnectionPoint(BlockGridSettings);
        }
        else
        {
            return Vector2Int.zero;
        }
    }

    private bool CheckIfTripletHasAllBackConnectionsInstantiated(HashSet<PathUnit> pathPlan, PathUnit pathUnit)
    {
        if (pathUnit is Triplet)
        {
            Triplet tempTriplet = (Triplet)pathUnit;
            foreach (int connectionId in tempTriplet.BackConnections)
            {
                bool currentBackConnectionIsPathEnd = (GetById(pathPlan, connectionId) is PathEnd);
                bool currentBackConnectionWasntInstantiated = GetById(_instantiatedTriplets, connectionId) == null;
                if (!currentBackConnectionIsPathEnd && currentBackConnectionWasntInstantiated)
                {
                    return false;
                }
            }
            return true;
        }
        return false;
    }

    private PathUnit GetById(IEnumerable<PathUnit> pathUnits, int id)
    {
        return pathUnits
            .Where(x => x.Id == id)
            .FirstOrDefault();
    }

    private void GenerateSmartWay(Vector2Int startPositionInGrid, Vector2Int endPositionInGrid)
    {
        if (_smartPath == null)
        {
            _smartPath = new SmartPath(_levelManager);
        }

        if (_levelManager.TryGetBlockInfoByPosition(startPositionInGrid, out var _) 
                || _levelManager.TryGetBlockInfoByPosition(endPositionInGrid, out var _))
        {
            Debug.LogWarning("Start or End position of a path is blocked");
        }
        else
        {
            _smartPath.AStar(startPositionInGrid, endPositionInGrid, out Vector2Int[] path);

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

    private void Initialize()
    {
        _levelManager = GetComponent<LevelManager>();

        _instantiatedTriplets.Clear();
    }
}
