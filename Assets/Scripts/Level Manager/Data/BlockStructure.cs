using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockStructure : MonoBehaviour
{
    [Header("Initialization")]
    [SerializeField] private Transform _startStructurePointer;
    [SerializeField] private Transform _endStructurePointer;
    [SerializeField] private List<Transform> _enterencePointers;
    [SerializeField] private Transform _exitPointer;


    [SerializeField] private Vector2 _relativeStructureStartPosition;
    [SerializeField] private Vector2 _relativeStructureEndPosition;
    [SerializeField] private List<Vector2> _relativeEnterancePositions;
    [SerializeField] private Vector2 _relativeExitPosition;

    private readonly Vector2 STANDART_CENTER_BIAS = new Vector2(0.5f, 0.5f);

    public Vector2Int CapturedZoneInBlockGridStart { get; private set; }
    public Vector2Int CapturedZoneInBlockGridEnd { get; private set; }
    public Vector2Int StructureCenterGridPosition { get; private set; }

    public int CapturedPlaceBelowCenter 
    { 
        get
        {
            int[] capturedVerticalPlace = 
                { 
                    CapturedZoneInBlockGridEnd.y, 
                    CapturedZoneInBlockGridStart.y 
                };

            return Mathf.Abs(capturedVerticalPlace.Min());
        } 
    }

    public int CapturedPlaceAboveCenter
    {
        get
        {
            int[] capturedVerticalPlace =
                {
                    CapturedZoneInBlockGridEnd.y,
                    CapturedZoneInBlockGridStart.y
                };

            return Mathf.Abs(capturedVerticalPlace.Max());
        }
    }

    /// <summary>
    /// Function for initializing and recording Structure data. Supposed to be called only in Unity Edit Mod
    /// </summary>
    public void InitStructureParams()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying && _startStructurePointer != null && _endStructurePointer != null)
        {
            _relativeStructureStartPosition = _startStructurePointer.position - transform.position - (Vector3)STANDART_CENTER_BIAS;
            _relativeStructureEndPosition = _endStructurePointer.position - transform.position - (Vector3)STANDART_CENTER_BIAS;

            _relativeEnterancePositions.Clear();
            foreach (Transform enteranceTransform in _enterencePointers)
            {
                _relativeEnterancePositions.Add(enteranceTransform.position - transform.position - (Vector3)STANDART_CENTER_BIAS);
            }
            _relativeExitPosition = _exitPointer.position - transform.position - (Vector3)STANDART_CENTER_BIAS;
        }
        else if (Application.isPlaying)
        {
            Debug.LogError("Can't initialize Structure parameters while application is runing.");
        }
        else
        {
            Debug.LogError("Structure pointers wasn't assigned.");
        }
#endif
    }

    public void InitInGridParams(BlockGridSettings blockGridSettings)
    {
        CapturedZoneInBlockGridStart = ConvertWorldSizeIntoBlockGridSize(blockGridSettings, _relativeStructureStartPosition);
        CapturedZoneInBlockGridEnd = ConvertWorldSizeIntoBlockGridSize(blockGridSettings, _relativeStructureEndPosition);
    }

    private Vector2Int ConvertWorldSizeIntoBlockGridSize(BlockGridSettings blockGridSettings, Vector2 worldSize)
    {
        Vector2 correctionVector = Vector2.one / 2;
        Vector2 signs = new Vector2(worldSize.x < 0 ? -1 : 1, worldSize.y < 0 ? -1 : 1);
        Vector2 blockGridSize = new Vector2(Mathf.Abs(worldSize.x / blockGridSettings.BlocksSize.x)
            , Mathf.Abs(worldSize.y / blockGridSettings.BlocksSize.y)) - correctionVector;
        blockGridSize = new Vector2(Mathf.Ceil(blockGridSize.x), Mathf.Ceil(blockGridSize.y));
        blockGridSize *= signs;

        Vector2 horizExpandDirectCorrectionVector = new Vector2(blockGridSettings.HorizontalExpandDirectionFactor, 1);
        blockGridSize *= horizExpandDirectCorrectionVector;

        return new Vector2Int((int)blockGridSize.x, (int)blockGridSize.y);
    }


    private void InitStructureConnections(ref BlockStructureData blockStructureData, BlockGridSettings blockGridSettings)
    {
        blockStructureData.EnteranceConnections.Clear();

        foreach (Vector2 currentRelativeEnterancePosition in _relativeEnterancePositions)
        {
            Connection currentEnteranceConnection = new Connection();
            currentEnteranceConnection.RelativePositionInBlockGrid
                = ConvertWorldSizeIntoBlockGridSize(blockGridSettings, currentRelativeEnterancePosition);

            if (currentEnteranceConnection.RelativePositionInBlockGrid.x > 0)
            {
                currentEnteranceConnection.Orientation = Orientation.Right;
            }
            else
            {
                currentEnteranceConnection.Orientation = Orientation.Left;
            }
            currentEnteranceConnection.AttachedStructureData = blockStructureData;
            currentEnteranceConnection.ConnectionType = ConnectionType.Enterance;
            blockStructureData.EnteranceConnections.Add(currentEnteranceConnection);
        }

        blockStructureData.ExitConnection = new Connection();
        blockStructureData.ExitConnection.RelativePositionInBlockGrid
            = ConvertWorldSizeIntoBlockGridSize(blockGridSettings, _relativeExitPosition);
        if (blockStructureData.ExitConnection.RelativePositionInBlockGrid.x > 0)
        {
            blockStructureData.ExitConnection.Orientation = Orientation.Right;
        }
        else
        {
            blockStructureData.ExitConnection.Orientation = Orientation.Left;
        }
        blockStructureData.ExitConnection.AttachedStructureData = blockStructureData;
        blockStructureData.ExitConnection.ConnectionType = ConnectionType.Exit;
    }

    private void CreateConnetion(ref Connection connection, Vector2Int centerPosition, LevelManager levelManager)
    {
        levelManager.TryGetSuitableBlock(false, false, true, true, out BlockInfoHolder connectionBase);

        connection.PositionInGrid = centerPosition
            + connection.RelativePositionInBlockGrid;

        levelManager.AddBlock(connection.PositionInGrid, connectionBase);
    }

    public static BlockStructureData CreateBlockStructureData(GameObject structurePrefab, Vector2Int structureCenterGridPosition
            , LevelManager levelManager)
    {
        BlockInfoHolder structureBlock = new BlockInfoHolder(structurePrefab, Vector2Int.zero);

        if (structurePrefab.TryGetComponent(out BlockStructure blockStructure))
        {
            levelManager.AddCustomBlock(structureBlock, structureCenterGridPosition);

            blockStructure.InitInGridParams(levelManager.BlockGridSettings);

            levelManager.FillRectWithPlaceholders(structureCenterGridPosition + blockStructure.CapturedZoneInBlockGridStart
                , structureCenterGridPosition + blockStructure.CapturedZoneInBlockGridEnd);

            BlockStructureData blockStructureData = new BlockStructureData();
            blockStructureData.GridPosition = structureCenterGridPosition;
            blockStructureData.StructurePrefab = structurePrefab;
            blockStructureData.BlockStructure = blockStructure;
            blockStructure.InitStructureConnections(ref blockStructureData, levelManager.BlockGridSettings);

            for (int i = 0; i < blockStructureData.EnteranceConnections.Count; i++)
            {
                Connection currentEnteranceConnection = blockStructureData.EnteranceConnections[i];
                blockStructure.CreateConnetion(ref currentEnteranceConnection, structureCenterGridPosition, levelManager);
            }

            Connection currentExitConnection = blockStructureData.ExitConnection;
            blockStructure.CreateConnetion(ref currentExitConnection, structureCenterGridPosition, levelManager);
            blockStructure.StructureCenterGridPosition = structureCenterGridPosition;
            return blockStructureData;
        }
        Debug.Log("Unexpected Structure prefab was given.");
        return null;
    }
}
