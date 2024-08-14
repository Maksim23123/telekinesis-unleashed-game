using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LevelManager))]
public class PathGenerator : MonoBehaviour
{

    [SerializeField] TripletGenerator _tripletGenerator;

    private RestraintBuilder _restraintBuilder;
    private LevelManager _levelManager;
    private SmartPath _smartPath;

    public BlockGridSettings BlockGridSettings { get => _levelManager.BlockGridSettings; }

    private void Initialize()
    {
        _levelManager = GetComponent<LevelManager>();
        if (!_tripletGenerator.Initialized)
        {
            _tripletGenerator.Initialize(_levelManager);
        }
        _restraintBuilder = new RestraintBuilder();
        _restraintBuilder.Initialize(_levelManager);
    }

    public void GeneratePaths(HashSet<PathUnit> pathPlan, List<List<PathUnit>[]> roomStructure)
    {
        Initialize();
        
        List<Triplet> instantiatedTriplets = _tripletGenerator.InstantiateTriplets(pathPlan, roomStructure);

        _restraintBuilder.ActWithRestraintsBetweenRooms(roomStructure);
        _restraintBuilder.SealTripletsEnterences(instantiatedTriplets);

        BuildPathsBetweenConnectionPoints(instantiatedTriplets, pathPlan);

        _restraintBuilder.ActWithRestraintsBetweenRooms(roomStructure, RestraintActionType.Destroy);
    }

    private void BuildPathsBetweenConnectionPoints(List<Triplet> triplets, HashSet<PathUnit> pathPlan)
    {
        foreach (Triplet currentTriplet in triplets)
        {
            for (int i = 0; i < 2; i++)
            {
                Connection currentConnection = currentTriplet.BlockStructureData.EnteranceConnections[i];
                PathUnit currentBackConnection = PathUnit.GetById(pathPlan, currentTriplet.BackConnections[i]);
                Vector2Int destinationPoint = currentBackConnection.ExtractConnectionPointPosition(BlockGridSettings
                    , triplets);
                if (currentConnection.SealedZoneParametersInitialized)
                {
                    _levelManager.DestroyBlocksInArea(currentConnection.SealedZoneStart, currentConnection.SealedZoneEnd);
                }
                GenerateSmartWay(currentConnection.GetConnectionPoint(BlockGridSettings), destinationPoint);
            }
        }
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
            _smartPath.TryGeneratePath(startPositionInGrid, endPositionInGrid, out Vector2Int[] path);

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
}
