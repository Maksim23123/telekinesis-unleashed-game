using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RestraintBuilder
{
    private const string RESTRAINT_BLOCK_TAG = "Restreint";
    private LevelManager _levelManager;

    public BlockGridSettings BlockGridSettings
    {
        get
        {
            return _levelManager.BlockGridSettings == null ? null : _levelManager.BlockGridSettings;
        }
    }

    public bool Initialized
    {
        get
        {
            return _levelManager == null ? false : true;
        }
    }

    public void Initialize(LevelManager levelManager)
    {
        if (levelManager != null)
        {
            _levelManager = levelManager;
        }
    }

    public void SealTripletsEnterences(List<Triplet> triplets)
    {
        if (Initialized)
        {
            foreach (Triplet triplet in triplets)
            {
                if (triplet.GameObject.TryGetComponent(out BlockStructure tripletBlockStructure)
                        && _levelManager.TryGetSuitableBlock(RESTRAINT_BLOCK_TAG, out BlockInfoHolder restreint))
                {
                    Connection firstConnection = tripletBlockStructure.EnteranceConnections[0];
                    firstConnection.InitSealedAreaParameters(BlockGridSettings, Placement.Bellow);
                    _levelManager.FillRect(firstConnection.SealedZoneStart, firstConnection.SealedZoneEnd, restreint);

                    Connection secondConnection = tripletBlockStructure.EnteranceConnections[1];
                    secondConnection.InitSealedAreaParameters(BlockGridSettings, Placement.Above);
                    _levelManager.FillRect(secondConnection.SealedZoneStart, secondConnection.SealedZoneEnd, restreint);
                }
            }
        }
    }

    public void BuildRestraintsBetweenRooms(List<List<PathUnit>[]> roomStructure)
    {
        if (Initialized)
        {
            List<List<PathUnit>> roomConnectionLayers = RoomGenerator.GetRoomConnectionLayers(roomStructure);
            List<PathUnit[]> pairedRoomConnections = PairRoomConnections(roomConnectionLayers);
            for (int i = 0; i < pairedRoomConnections.Count; i++)
            {
                BuildRestraintsBetweenRoomConnectionPairs(pairedRoomConnections[i].Select(g => g as PathEnd).ToArray());
            }
        }
    }

    private List<PathUnit[]> PairRoomConnections(List<List<PathUnit>> roomConnectionLayers)
    {
        List<PathUnit[]> pairedRoomConnections = new();

        foreach (List<PathUnit> roomConnectionLayer in roomConnectionLayers)
        {
            if (roomConnectionLayer.Count > 2)
            {
                List<PathEnd> orderedRoomConnections = roomConnectionLayer
                    .OrderBy(g =>
                    {
                        if (g is PathEnd pathEnd)
                        {
                            return pathEnd.Connection.GetConnectionPoint(BlockGridSettings).x;
                        }
                        return 0;
                    })
                    .Select(g => g as PathEnd)
                    .ToList();

                List<PathUnit[]> roomConnectionPairs = new();

                for (int i = 1; i < orderedRoomConnections.Count - 1; i += 2)
                {
                    PathUnit[] roomConnectionPair = new PathUnit[2];

                    roomConnectionPair[0] = orderedRoomConnections[i];

                    if (i + 1 < orderedRoomConnections.Count)
                    {
                        roomConnectionPair[1] = orderedRoomConnections[i + 1];
                    }
                    else
                    {
                        break;
                    }

                    roomConnectionPairs.Add(roomConnectionPair);
                }

                pairedRoomConnections.AddRange(roomConnectionPairs);
            }
        }

        return pairedRoomConnections;
    }

    private void BuildRestraintsBetweenRoomConnectionPairs(PathEnd[] pathEndsPair)
    {
        if (pathEndsPair.Length == 2
                && _levelManager.TryGetSuitableBlock(RESTRAINT_BLOCK_TAG, out BlockInfoHolder restraint))
        {
            Vector2Int firstPoint = pathEndsPair[0].Connection.GetConnectionPoint(BlockGridSettings);
            Vector2Int secondPoint = pathEndsPair[1].Connection.GetConnectionPoint(BlockGridSettings);

            int horizontalPosition = (firstPoint.x + secondPoint.x) / 2;
            BuildVerticalRestreint(restraint, firstPoint, secondPoint, horizontalPosition);
            BuildHorizontalRestreints(pathEndsPair, restraint, horizontalPosition);
        }
    }

    private void BuildHorizontalRestreints(PathEnd[] pathEndsPair, BlockInfoHolder restraint, int horizontalPosition)
    {
        foreach (Connection connection in pathEndsPair.Select(g => g.Connection))
        {
            Vector2Int connectionPoint = connection.GetConnectionPoint(BlockGridSettings);
            int verticalPosition = connectionPoint.y;
            if (connection.ConnectionType == ConnectionType.Exit)
            {
                verticalPosition--;
            }
            else
            {
                verticalPosition++;
            }

            int horizontalStartPosition = connectionPoint.x;
            int horizontalEndPosition = horizontalPosition;

            _levelManager.FillRect(new Vector2Int(horizontalStartPosition, verticalPosition)
                , new Vector2Int(horizontalEndPosition, verticalPosition), restraint);
        }
    }

    private void BuildVerticalRestreint(BlockInfoHolder restraint, Vector2Int firstPoint
            , Vector2Int secondPoint, int horizontalPosition)
    {
        int[] verticalPositions = new int[]
            {
                firstPoint.y,
                secondPoint.y
            };

        int startVerticalPosition = verticalPositions.Max();
        int endVerticalPosition = verticalPositions.Min();

        _levelManager.FillRect(new Vector2Int(horizontalPosition, startVerticalPosition)
            , new Vector2Int(horizontalPosition, endVerticalPosition), restraint);
    }
}
