using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(LevelManager))]
public class RoomGenerator : MonoBehaviour
{
    [SerializeField] LevelManager _levelManager;
    [SerializeField] GameObject[] _roomPrefabs = new GameObject[0];

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
        Vector2Int roomCenterGridPosition = new Vector2Int((int)(BlockGridSettings.MapDimensions.x * Random.value)
                , (int)(BlockGridSettings.MapDimensions.y * Random.value));
        GameObject currentRoom = _roomPrefabs[0];
        CreateRoom(roomCenterGridPosition, currentRoom);
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
