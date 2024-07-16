using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(LevelManager))]
public class RoomGenerator : MonoBehaviour
{
    [SerializeField] LevelManager _levelManager;
    [SerializeField] GameObject[] _roomPrefabs = new GameObject[0];
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

    public List<List<PathUnit>[]> GenerateRooms()
    {
        int verticalPosition = 0;

        for (int i = _roomLevels.Count - 1; i >= 0; i--)
        {
            _roomLevels[i].GenerateRoomLevel(_roomPrefabs, verticalPosition);
            verticalPosition += _roomLevels[i].CapturedVerticalPlace + _betweenRoomLevelSpaceSize;
        }

        List<List<PathUnit>[]> roomStructure = new();

        List<PathEnd>[] currentConnectionLayer = CreateNewConnectionLayer();
        List<PathEnd>[] nextConnectionLayer = CreateNewConnectionLayer();

        for (int i = 0; i < _roomLevels.Count; i++)
        {
            foreach (RoomInfo roomInfo in _roomLevels[i].LevelRoomsContainer)
            {
                RoomData currentRoomData = CreateRoom(roomInfo.PositionInGrid - new Vector2Int(_roomLevels[i].HorizontalPos / 2, 0)
                    + new Vector2Int(_horizontalOffset, 0), roomInfo.RoomObjectPrefab);
                if (i > 0 && i < _roomLevels.Count - 1)
                {
                    currentConnectionLayer[1].Add(ExtractEnterance(currentRoomData));
                    nextConnectionLayer[0].Add(ExtractExit(currentRoomData));
                }
                else if (i == 0)
                {
                    currentConnectionLayer = null;
                    nextConnectionLayer[0].Add(ExtractExit(currentRoomData));
                }
                else if (i == _roomLevels.Count - 1) 
                {
                    currentConnectionLayer[1].Add(ExtractEnterance(currentRoomData));
                }
            }
            if (currentConnectionLayer != null)
            {
                List<PathUnit>[] finishedConnectionLayer = new List<PathUnit>[2];
                finishedConnectionLayer[0] = currentConnectionLayer[0].ToList<PathUnit>();
                finishedConnectionLayer[1] = currentConnectionLayer[1].ToList<PathUnit>();
                roomStructure.Add(finishedConnectionLayer);
            }
            currentConnectionLayer = nextConnectionLayer;
            nextConnectionLayer = CreateNewConnectionLayer();
        }

        return roomStructure;
    }

    private static bool ValidateConnectionLayer(List<PathEnd>[] connectionLayer)
    {
        if (connectionLayer[0] == null || connectionLayer[1] == null)
        {
            return false;
        }
        return true;
    }

    private static List<PathEnd>[] CreateNewConnectionLayer()
    {
        List<PathEnd>[] newConnectionLayer = new List<PathEnd>[2];
        newConnectionLayer[0] = new();
        newConnectionLayer[1] = new();
        return newConnectionLayer;
    }

    private static PathEnd ExtractExit(RoomData currentRoomData)
    {
        PathEnd exit = new PathEnd();
        exit.Connection = currentRoomData.ExitConnection;
        return exit;
    }

    private static PathEnd ExtractEnterance(RoomData currentRoomData)
    {
        PathEnd enterance = new PathEnd();
        enterance.Connection = currentRoomData.EnteranceConnection;
        return enterance;
    }

    public RoomData CreateRoom(Vector2Int roomCenterGridPosition, GameObject currentRoom)
    {
        GameObject roomInstance = RoomData.InstantiateRoom(currentRoom, roomCenterGridPosition, _levelManager);
        return roomInstance.GetComponent<RoomData>();
    }
}
