using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;


// TODO: finish updating
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

    private Vector2Int _capturedZoneInBlockGridStart;
    private Vector2Int _capturedZoneInBlockGridEnd;
    private List<Connection> _enteranceConnections = new();
    private Connection _exitConnection;

    public Vector2Int CapturedZoneInBlockGridStart { get; private set; }
    public Vector2Int CapturedZoneInBlockGridEnd { get; private set; }
    public List<Connection> EnteranceConnections { get => _enteranceConnections; }
    public Connection ExitConnection { get => _exitConnection; }

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
        InitStructureConnections(blockGridSettings);
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


    private void InitStructureConnections(BlockGridSettings blockGridSettings)
    {
        _enteranceConnections.Clear();

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

            currentEnteranceConnection.ConnectionType = ConnectionType.Enterance;
            _enteranceConnections.Add(currentEnteranceConnection);
        }
        

        _exitConnection = new Connection();
        _exitConnection.RelativePositionInBlockGrid
            = ConvertWorldSizeIntoBlockGridSize(blockGridSettings, _relativeExitPosition);
        _exitConnection.Orientation = Orientation.Left;
        _exitConnection.ConnectionType = ConnectionType.Exit;
    }

    private void InstantiateConnetion(ref Connection connection, Vector2Int centerPosition, LevelManager levelManager)
    {
        levelManager.TryGetSuitableBlock(false, false, true, true, out BlockInfoHolder connectionBase);

        connection.GameObject = levelManager.InstantiateBlock(centerPosition
            + connection.RelativePositionInBlockGrid, connectionBase);
    }

    public static GameObject InstantiateStructure(GameObject structurePrefab, Vector2Int structureCenterGridPosition
            , LevelManager levelManager)
    {
        BlockInfoHolder structureBlock = new BlockInfoHolder(structurePrefab, Vector2Int.zero);

        GameObject structureGameObjectInstance = levelManager.InstantiateCustomBlock(structureBlock, structureCenterGridPosition);

        if (structureGameObjectInstance.TryGetComponent(out BlockStructure blockStructure))
        {

            levelManager.FillRectWithPlaceholders(structureCenterGridPosition + blockStructure.CapturedZoneInBlockGridStart
                , structureCenterGridPosition + blockStructure.CapturedZoneInBlockGridEnd);

            blockStructure.InitStructureConnections(levelManager.BlockGridSettings);

            for (int i = 0; i < blockStructure._enteranceConnections.Count; i++)
            {
                Connection currentEnteranceConnection = blockStructure._enteranceConnections[i];
                blockStructure.InstantiateConnetion(ref currentEnteranceConnection, structureCenterGridPosition, levelManager);
            }

            blockStructure.InstantiateConnetion(ref blockStructure._exitConnection, structureCenterGridPosition, levelManager);
            return structureGameObjectInstance;
        }
        Debug.Log("Unexpected Structure prefab was given");
        DestroyImmediate(structureGameObjectInstance);
        return null;
    }
}
