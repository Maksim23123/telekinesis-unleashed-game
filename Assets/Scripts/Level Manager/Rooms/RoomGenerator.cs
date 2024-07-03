using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(LevelManager))]
public class RoomGenerator : MonoBehaviour
{
    [SerializeField] LevelManager _levelManager;
    [SerializeField] GameObject[] _roomPrefabs = new GameObject[0];

    BlockGridSettings _blockGridSettings;
    

    public void GenerateRooms()
    {
        _blockGridSettings = _levelManager.BlockGridSettings;

        if (_roomPrefabs[0].TryGetComponent(out RoomData roomData))
        {
            Vector2Int roomCenterGridPosition = new Vector2Int((int)(_blockGridSettings.MapDimensions.x * Random.value)
                , (int)(_blockGridSettings.MapDimensions.y * Random.value));
            
            _levelManager.InstantiateCustomBlock(_roomPrefabs[0], roomCenterGridPosition);
            roomData.InitInGridParams(_blockGridSettings);
            _levelManager.FillRectWithPlaceholders(roomCenterGridPosition + roomData.CapturedZoneInBlockGridStart
                , roomCenterGridPosition + roomData.CapturedZoneInBlockGridEnd);
        }
    }
}
