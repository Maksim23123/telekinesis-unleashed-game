using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class TripletGenerator
{
    [SerializeField] private GameObject _tripletRightOut;
    [SerializeField] private GameObject _tripletLeftOut;

    const int VERTICAL_POSITION_OFFSET = 3;
    const int VERTICAL_POSITION_ABOVE_OFFSET = 1;
    const int VERTICAL_POSITION_BELOW_OFFSET = 1;

    private List<Triplet> _instantiatedTriplets = new();
    private LevelManager _levelManager;
    
    public bool Initialized
    {
        get
        {
            return _levelManager == null ? false : true;
        }
    }

    public BlockGridSettings BlockGridSettings
    {
        get
        {
            return _levelManager.BlockGridSettings == null ? null : _levelManager.BlockGridSettings;
        }
    }

    public void Initialize(LevelManager levelManager)
    {
        if (levelManager != null)
        {
            _levelManager = levelManager;
        }
    } 

    public List<Triplet> InstantiateTriplets(HashSet<PathUnit> pathPlan, List<List<PathUnit>[]> roomStructure)
    {
        _instantiatedTriplets.Clear();
        if (CheckIfTripletPrefabsAssigned())
        {
            Triplet currentTriplet;

            do
            {
                currentTriplet = GetTripletWithInstantiatedBackConnections(pathPlan);

                if (currentTriplet != null)
                {
                    FindPositionForTriplet(pathPlan, roomStructure, currentTriplet
                        , out int horizontalPosition, out int verticalPosition);

                    InstantiateTriplet(ref currentTriplet, new Vector2Int(horizontalPosition, verticalPosition));
                }
            }
            while (currentTriplet != null);
        }
        return _instantiatedTriplets;
    }

    private void FindPositionForTriplet(HashSet<PathUnit> pathPlan, List<List<PathUnit>[]> roomStructure
            , Triplet currentTriplet, out int horizontalPosition, out int verticalPosition)
    {
        Vector2Int[] backConnectionPositions = GetBackConnectionsPositions(pathPlan, currentTriplet);

        horizontalPosition = (int)backConnectionPositions.Average(position => position.x);
        bool currentTripletIsBackConnection = CheckIfTripletIsBackConnection(pathPlan, currentTriplet);
        bool currentTripletConnectsPathEnds = TryGetBasePositionFromRooms(pathPlan, roomStructure, currentTriplet
            , out int baseVerticalPositionAccordingRooms);

        if (!currentTripletIsBackConnection)
        {
            verticalPosition = (int)backConnectionPositions.Average(position => position.y);
        }
        else if (currentTriplet.placement == Placement.Above)
        {
            int highestBackConnectionPosition = backConnectionPositions.Max(position => position.y);
            int basePosition = currentTripletConnectsPathEnds ?
                baseVerticalPositionAccordingRooms : highestBackConnectionPosition;

            verticalPosition = basePosition + VERTICAL_POSITION_ABOVE_OFFSET + VERTICAL_POSITION_OFFSET;
        }
        else
        {
            int lowestBackConnectionPosition = backConnectionPositions.Min(position => position.y);
            int basePosition = currentTripletConnectsPathEnds ?
                baseVerticalPositionAccordingRooms : lowestBackConnectionPosition;

            verticalPosition = basePosition - VERTICAL_POSITION_BELOW_OFFSET - VERTICAL_POSITION_OFFSET;
        }
    }

    private static bool CheckIfTripletIsBackConnection(HashSet<PathUnit> pathPlan, Triplet currentTriplet)
    {
        return pathPlan.Any(x =>
        {
            if (x is Triplet)
            {
                Triplet tempTriplet = (Triplet)x;
                return tempTriplet.BackConnections.Contains(currentTriplet.Id);
            }
            return false;
        });
    }

    private Vector2Int[] GetBackConnectionsPositions(HashSet<PathUnit> pathPlan, Triplet currentTriplet)
    {
        return pathPlan
            .Where(x => currentTriplet.BackConnections.Any(g => g == x.Id))
            .Select(x => x.ExtractConnectionPointPosition(BlockGridSettings, _instantiatedTriplets))
            .ToArray();
    }

    private Triplet GetTripletWithInstantiatedBackConnections(HashSet<PathUnit> pathPlan)
    {
        return pathPlan.Where(pathUnit =>
                CheckIfTripletHasAllBackConnectionsInstantiated(pathPlan, pathUnit) && !_instantiatedTriplets
                    .Any(g => g.Id == pathUnit.Id))
            .Select(x => (Triplet)x)
            .FirstOrDefault();
    }

    private bool CheckIfTripletPrefabsAssigned()
    {
        if (_tripletRightOut == null || _tripletLeftOut == null)
        {
            Debug.LogError("Triplet prefabs weren't assigned.");
            return false;
        }
        return true;
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

    private bool CheckIfTripletHasAllBackConnectionsInstantiated(HashSet<PathUnit> pathPlan, PathUnit pathUnit)
    {
        if (pathUnit is Triplet)
        {
            Triplet tempTriplet = (Triplet)pathUnit;
            foreach (int connectionId in tempTriplet.BackConnections)
            {
                bool currentBackConnectionIsPathEnd = (PathUnit.GetById(pathPlan, connectionId) is PathEnd);
                bool currentBackConnectionWasntInstantiated = PathUnit.GetById(_instantiatedTriplets, connectionId) == null;
                if (!currentBackConnectionIsPathEnd && currentBackConnectionWasntInstantiated)
                {
                    return false;
                }
            }
            return true;
        }
        return false;
    }

    private void ShowTripletsResume(HashSet<PathUnit> pathPlan)
    {
        if (Initialized)
        {
            foreach (PathUnit triplet in _instantiatedTriplets)
            {
                Triplet currentTriplet = triplet as Triplet;
                string resume = string.Empty;

                resume += "ID: " + currentTriplet.Id + " | ";
                resume += "Position: " + _levelManager.BlockGridSettings
                    .WorldToGridPosition(currentTriplet.GameObject.transform.position) + " | ";

                PathUnit firstBackConnectionUnit = PathUnit.GetById(pathPlan, currentTriplet.BackConnections[0]);
                resume += "First connection point: " + firstBackConnectionUnit.ExtractConnectionPointPosition(BlockGridSettings
                    , _instantiatedTriplets ) + " | ";
                PathUnit secondBackConnectionUnit = PathUnit.GetById(pathPlan, currentTriplet.BackConnections[1]);
                resume += "Second connection point: " + secondBackConnectionUnit.ExtractConnectionPointPosition(BlockGridSettings
                    , _instantiatedTriplets) + " | ";

                Debug.Log(resume);
            }
        }
        
    }

    private bool TryGetBasePositionFromRooms(HashSet<PathUnit> pathPlan, List<List<PathUnit>[]> roomStructure
            , Triplet currentTriplet, out int baseVerticalPosition)
    {
        bool extractionSuccesful = false;
        baseVerticalPosition = 0;

        if (PathUnit.GetById(pathPlan, currentTriplet.BackConnections[0]) is PathEnd firstPahtEnd
                && PathUnit.GetById(pathPlan, currentTriplet.BackConnections[1]) is PathEnd secondPahtEnd)
        {
            List<List<PathUnit>> connectionLayers = RoomGenerator.GetRoomConnectionLayers(roomStructure);
            List<PathEnd> currentConnectionLayer = connectionLayers
                .Where(x => x
                    .Where(g => g.Id == firstPahtEnd.Id || g.Id == secondPahtEnd.Id)
                    .FirstOrDefault() != null)
                .FirstOrDefault()
                .Select(x => x as PathEnd)
                .ToList();

            if (currentConnectionLayer != null)
            {
                int roomLayerCenter = currentConnectionLayer.First().Connection.AttachedStructure.StructureCenterGridPosition.y;
                if (currentTriplet.placement == Placement.Above)
                {
                    int offsetFromRoomLayerCenter = currentConnectionLayer
                        .Max(x => x.Connection.AttachedStructure.CapturedPlaceAboveCenter);
                    baseVerticalPosition = roomLayerCenter + offsetFromRoomLayerCenter;
                }
                else if (currentTriplet.placement == Placement.Bellow)
                {
                    int offsetFromRoomLayerCenter = currentConnectionLayer
                        .Max(x => x.Connection.AttachedStructure.CapturedPlaceBelowCenter);
                    baseVerticalPosition = roomLayerCenter - offsetFromRoomLayerCenter;
                }
                extractionSuccesful = true;
            }
        }
        return extractionSuccesful;
    }
}
