using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(LevelManager))]
public class RoomGenerator : MonoBehaviour
{
    [SerializeField] LevelManager _levelManager;
    [SerializeField] GameObject[] _roomPrefabs = new GameObject[0];
    [SerializeField][Min(2)] int _roomLevelsCount = 5;
    [SerializeField] int _betweenRoomLevelSpaceSize = 10;
    [SerializeField] int _horizontalOffset;
    [SerializeField] List<RoomLevelInfo> _roomLevels = new();

    BlockGridSettings _blockGridSettings;

    public GameObject[] RoomPrefabs { get => _roomPrefabs.ToArray(); }
    public BlockGridSettings BlockGridSettings 
    { 
        get
        {
            return _levelManager.BlockGridSettings;
        }
    }

    public void GenerateRooms()
    {
        GenerateRoomsTemp();
    }

    public void GenerateRoomsTemp()
    {
        int verticalPosition = 0;

        for (int i = _roomLevels.Count - 1; i >= 0; i--)
        {
            _roomLevels[i].GenerateRooms(_roomPrefabs, verticalPosition);
            verticalPosition += _roomLevels[i].CapturedVerticalPlace + _betweenRoomLevelSpaceSize;
        }

        foreach (RoomLevelInfo roomLevelInfo in _roomLevels)
        {
            foreach (RoomInfo roomInfo in roomLevelInfo.LevelRoomsContainer)
            {
                CreateRoom(roomInfo.PositionInGrid - new Vector2Int(roomLevelInfo.HorizontalPos / 2, 0) 
                    + new Vector2Int(_horizontalOffset, 0), roomInfo.RoomObjectPrefab);
            }
        }
    }

    public void CreateRoom(Vector2Int roomCenterGridPosition, GameObject currentRoom)
    {
        if (currentRoom.TryGetComponent(out RoomData roomData))
        {
            BlockInfoHolder roomBlock = new BlockInfoHolder(currentRoom, Vector2Int.zero);
            _levelManager.InstantiateCustomBlock(roomBlock, roomCenterGridPosition);
            roomData.InitInGridParams(BlockGridSettings);
            _levelManager.FillRectWithPlaceholders(roomCenterGridPosition + roomData.CapturedZoneInBlockGridStart
                , roomCenterGridPosition + roomData.CapturedZoneInBlockGridEnd);
        }
    }
}
