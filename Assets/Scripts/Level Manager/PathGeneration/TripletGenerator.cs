using Codice.CM.Common;
using System;
using System.Collections;
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
        if (_tripletRightOut == null || _tripletLeftOut == null)
        {
            Debug.LogError("Triplet prefabs weren't assigned.");
            return null;
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
                    .Select(x => PathUnit.ExtractConnectionPointPosition(x, BlockGridSettings, _instantiatedTriplets))
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


                bool currentTripletConnectsPathEnds = false;

                currentTripletConnectsPathEnds = TryGetBasePositionFromRooms(pathPlan, roomStructure, currentTriplet
                    , out int baseVerticalPositionAccordingRooms);

                if (!currentTripletIsBackConnection)
                {
                    verticalPosition = backConnectionPositions.Sum(g => g.y) / backConnectionPositions.Length;
                }
                else if (currentTriplet.placement == Placement.Above)
                {
                    int basePosition = 0;
                    if (currentTripletConnectsPathEnds)
                    {
                        basePosition = baseVerticalPositionAccordingRooms;
                    }
                    else
                    {
                        int highestBackConnectionPosition = backConnectionPositions.Max(g => g.y);
                        basePosition = highestBackConnectionPosition;
                    }

                    verticalPosition = basePosition + VERTICAL_POSITION_ABOVE_OFFSET + VERTICAL_POSITION_OFFSET;
                }
                else
                {
                    int basePosition = 0;
                    if (currentTripletConnectsPathEnds)
                    {
                        basePosition = baseVerticalPositionAccordingRooms;
                    }
                    else
                    {
                        int lowestBackConnectionPosition = backConnectionPositions.Min(g => g.y);
                        basePosition = lowestBackConnectionPosition;
                    }

                    verticalPosition = basePosition - VERTICAL_POSITION_BELOW_OFFSET - VERTICAL_POSITION_OFFSET;
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

        return _instantiatedTriplets;
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
                resume += "First connection point: " + PathUnit.ExtractConnectionPointPosition(firstBackConnectionUnit, BlockGridSettings, _instantiatedTriplets ) + " | ";
                PathUnit secondBackConnectionUnit = PathUnit.GetById(pathPlan, currentTriplet.BackConnections[1]);
                resume += "Second connection point: " + PathUnit.ExtractConnectionPointPosition(secondBackConnectionUnit, BlockGridSettings, _instantiatedTriplets) + " | ";

                Debug.Log(resume);
            }
        }
        
    }

    private bool TryGetBasePositionFromRooms(HashSet<PathUnit> pathPlan, List<List<PathUnit>[]> roomStructure, Triplet currentTriplet, out int baseVerticalPosition)
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
