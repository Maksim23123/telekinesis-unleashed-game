using Codice.Client.Common;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RoomData : MonoBehaviour
{
    [Header("Initialization")]
    [SerializeField] private Transform _startRoomPointer;
    [SerializeField] private Transform _endRoomPointer;
    [SerializeField] private Transform _enterencePointer;
    [SerializeField] private Transform _exitPointer;

    private readonly Vector2 STANDART_CENTER_BIAS = new Vector2(0.5f, 0.5f);

    private Vector2 _relativeRoomStartPosition;
    private Vector2 _relativeRoomEndPosition;
    private Vector2 _relativeEnterancePosition;
    private Vector2 _relativeExitPosition;

    private Vector2Int _capturedZoneInBlockGridStart;
    private Vector2Int _capturedZoneInBlockGridEnd;
    private Connection _enteranceConnection;
    private Connection _exitConnection;

    public Vector2Int CapturedZoneInBlockGridStart { get; private set; }
    public Vector2Int CapturedZoneInBlockGridEnd { get; private set; }
    public Connection EnteranceConnection { get => _enteranceConnection; }
    public Connection ExitConnection { get => _exitConnection; }

    /// <summary>
    /// Function for initializing and recording room data. Supposed to be called only in edit mod
    /// </summary>
    public void InitRoomParams()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying && _startRoomPointer != null && _endRoomPointer != null)
        {
            _relativeRoomStartPosition = _startRoomPointer.position - transform.position - (Vector3)STANDART_CENTER_BIAS;
            _relativeRoomEndPosition = _endRoomPointer.position - transform.position - (Vector3)STANDART_CENTER_BIAS;

            _relativeEnterancePosition = _enterencePointer.position - transform.position - (Vector3)STANDART_CENTER_BIAS;
            _relativeExitPosition = _exitPointer.position - transform.position - (Vector3)STANDART_CENTER_BIAS;
        }
        else if (Application.isPlaying)
        {
            Debug.LogError("Can't initialize room parameters while application is runing.");
        }
        else
        {
            Debug.LogError("Room pointers wasn't assigned.");
        }
#endif
    }

    public void InitInGridParams(BlockGridSettings blockGridSettings)
    {
        CapturedZoneInBlockGridStart = ConvertWorldSizeIntoBlockGridSize(blockGridSettings, _relativeRoomStartPosition);
        CapturedZoneInBlockGridEnd = ConvertWorldSizeIntoBlockGridSize(blockGridSettings, _relativeRoomEndPosition);
        InitRoomConnections(blockGridSettings);
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

    private void InitRoomConnections(BlockGridSettings blockGridSettings)
    {
        _enteranceConnection = new Connection();
        _enteranceConnection.RelativePositionInBlockGrid 
            = ConvertWorldSizeIntoBlockGridSize(blockGridSettings, _relativeEnterancePosition);
        _enteranceConnection.Orientation = Orientation.Right;
        _enteranceConnection.ConnectionType = ConnectionType.Enterance;

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

    public static GameObject InstantiateRoom(GameObject roomPrefab, Vector2Int roomCenterGridPosition
            , LevelManager levelManager)
    {
        if (roomPrefab.TryGetComponent(out RoomData roomData))
        {
            BlockInfoHolder roomBlock = new BlockInfoHolder(roomPrefab, Vector2Int.zero);

            levelManager.InstantiateCustomBlock(roomBlock, roomCenterGridPosition);
            roomData.InitInGridParams(levelManager.BlockGridSettings);

            levelManager.FillRectWithPlaceholders(roomCenterGridPosition + roomData.CapturedZoneInBlockGridStart
                , roomCenterGridPosition + roomData.CapturedZoneInBlockGridEnd);

            roomData.InstantiateConnetion(ref roomData._enteranceConnection, roomCenterGridPosition, levelManager);
            roomData.InstantiateConnetion(ref roomData._exitConnection, roomCenterGridPosition, levelManager);
            return roomBlock.Block;
        }
        return null;
    }
}
