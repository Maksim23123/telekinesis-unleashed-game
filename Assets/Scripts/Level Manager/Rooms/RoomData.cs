using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RoomData : MonoBehaviour
{
    [SerializeField] private Vector2 _relativeRoomStartPosition;
    [SerializeField] private Vector2 _relativeRoomEndPosition;
    [SerializeField] private List<Vector2> _connections = new();

    private readonly Vector2 _standartCenterBias = new Vector2(0.5f, 0.5f);

    private Vector2Int _capturedZoneInBlockGridStart;
    private Vector2Int _capturedZoneInBlockGridEnd;

    [Header("Initialization")]
    [SerializeField] private Transform _startRoomPointer;
    [SerializeField] private Transform _endRoomPointer;
    [SerializeField] private Transform[] _connectionPointers = new Transform[0];

    public Vector2Int CapturedZoneInBlockGridStart { get; private set; }
    public Vector2Int CapturedZoneInBlockGridEnd { get; private set; }

    /// <summary>
    /// Function for initializing and recording room data. Supposed to be called only in edit mod
    /// </summary>
    public void InitRoomParams()
    {
#if UNITY_EDITOR
        if (_startRoomPointer != null && _endRoomPointer != null)
        {
            _relativeRoomStartPosition = _startRoomPointer.position - transform.position - (Vector3)_standartCenterBias;
            _relativeRoomEndPosition = _endRoomPointer.position - transform.position - (Vector3)_standartCenterBias;

            foreach (Transform connectionPointer in _connectionPointers)
            {
                _connections.Add(connectionPointer.position - transform.position - (Vector3)_standartCenterBias);
            }
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
    }

    private Vector2Int ConvertWorldSizeIntoBlockGridSize(BlockGridSettings blockGridSettings, Vector2 worldSize)
    {
        Vector2 correctionVector = Vector2.one / 2; // (0.5, 0.5)
        Vector2 signs = new Vector2(worldSize.x < 0 ? -1 : 1, worldSize.y < 0 ? -1 : 1);
        Vector2 blockGridSize = new Vector2(Mathf.Abs(worldSize.x / blockGridSettings.BlocksSize.x)
            , Mathf.Abs(worldSize.y / blockGridSettings.BlocksSize.y)) - correctionVector;
        blockGridSize = new Vector2(Mathf.Ceil(blockGridSize.x), Mathf.Ceil(blockGridSize.y));
        blockGridSize *= signs;

        Vector2 horizExpandDirectCorrectionVector = new Vector2(blockGridSettings.HorizontalExpandDirectionFactor, 1);
        blockGridSize *= horizExpandDirectCorrectionVector;

        return new Vector2Int((int)blockGridSize.x, (int)blockGridSize.y);
    }
}
