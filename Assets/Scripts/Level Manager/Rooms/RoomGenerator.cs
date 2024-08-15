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
    public List<GridArea> AreasToFinalize { get; private set; } = new();
    public List<GridArea> AreasForBrunches { get; private set; } = new();
    public BlockGridSettings BlockGridSettings 
    { 
        get
        {
            return _levelManager.BlockGridSettings;
        }
    }

    public List<List<PathUnit>[]> GenerateRooms()
    {
        AreasToFinalize.Clear();
        AreasForBrunches.Clear();

        int verticalPosition = 0;

        GridArea areaForBrunches = new();

        int roomLevelsFirstIndex = _roomLevels.Count - 1;
        for (int i = roomLevelsFirstIndex; i >= 0; i--)
        {
            _roomLevels[i].GenerateRoomLevel(_levelManager, _roomPrefabs, verticalPosition);
            verticalPosition += _roomLevels[i].CapturedVerticalPlace + _betweenRoomLevelSpaceSize
                + _roomLevels[i].ConnectionsDedicatedSpace * 2;
            AppendAreasToFinalize(i);
            AppendAreasForBrunches(ref areaForBrunches, roomLevelsFirstIndex, i);
        }

        List<List<PathUnit>[]> roomStructure = OrganizeIntoRoomStructure();

        return roomStructure;
    }

    private void AppendAreasForBrunches(ref GridArea areaForBrunches, int roomLevelsFirstIndex, int i)
    {
        areaForBrunches.AreaEnd = new Vector2Int(100
                        , _roomLevels[i].CenterVerticalPosition - _roomLevels[i].MaxCapturedPlaceBelow - 1);

        if (i != roomLevelsFirstIndex)
        {
            AreasForBrunches.Add(areaForBrunches);
        }

        areaForBrunches.AreaStart = new Vector2Int(-100
            , _roomLevels[i].CenterVerticalPosition + _roomLevels[i].MaxCapturedPlaceAbove + 1);
    }

    private void AppendAreasToFinalize(int index)
    {
        GridArea areaToFinalize = new();
        areaToFinalize.AreaStart = new Vector2Int(-100
            , _roomLevels[index].CenterVerticalPosition + _roomLevels[index].MaxCapturedPlaceAbove);
        areaToFinalize.AreaEnd = new Vector2Int(100
            , _roomLevels[index].CenterVerticalPosition - _roomLevels[index].MaxCapturedPlaceBelow);
        AreasToFinalize.Add(areaToFinalize);
    }

    private List<List<PathUnit>[]> OrganizeIntoRoomStructure()
    {
        List<List<PathUnit>[]> roomStructure = new();

        List<PathEnd>[] currentConnectionLayer = CreateNewConnectionLayer();
        List<PathEnd>[] nextConnectionLayer = CreateNewConnectionLayer();

        for (int i = 0; i < _roomLevels.Count; i++)
        {
            foreach (RoomInfo roomInfo in _roomLevels[i].LevelRoomsContainer)
            {
                BlockStructureData currentRoomData = CreateRoom(roomInfo.PositionInGrid 
                    - new Vector2Int(_roomLevels[i].HorizontalPos / 2, 0) + new Vector2Int(_horizontalOffset, 0)
                    , roomInfo.RoomObjectPrefab);
                if (i > 0 && i < _roomLevels.Count - 1)
                {
                    currentConnectionLayer[0].Add(ExtractExit(currentRoomData));
                    nextConnectionLayer[1].Add(ExtractEnterance(currentRoomData));
                }
                else if (i == 0)
                {
                    currentConnectionLayer = null;
                    nextConnectionLayer[1].Add(ExtractEnterance(currentRoomData));
                }
                else if (i == _roomLevels.Count - 1)
                {
                    currentConnectionLayer[0].Add(ExtractExit(currentRoomData));
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

    public static List<List<PathUnit>> GetRoomConnectionLayers(List<List<PathUnit>[]> roomStructure)
    {
        List<List<PathUnit>> roomConnectionLayers = new();

        for (int i = 0; i < roomStructure.Count - 1; i++)
        {
            List<PathUnit> connectionLayer = new();

            connectionLayer.AddRange(roomStructure[i][0]);

            if (i + 1 < roomStructure.Count)
            {
                connectionLayer.AddRange(roomStructure[i + 1][1]);
            }

            roomConnectionLayers.Add(connectionLayer);
        }

        return roomConnectionLayers;
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

    private static PathEnd ExtractExit(BlockStructureData blockStructureData)
    {
        PathEnd exit = new PathEnd();
        exit.Connection = blockStructureData.ExitConnection;
        return exit;
    }

    private static PathEnd ExtractEnterance(BlockStructureData blockStructureData)
    {
        PathEnd enterance = new PathEnd();
        enterance.Connection = blockStructureData.EnteranceConnections[0];
        return enterance;
    }

    public BlockStructureData CreateRoom(Vector2Int roomCenterGridPosition, GameObject currentRoom)
    {
        return BlockStructure.CreateBlockStructureData(currentRoom, roomCenterGridPosition, _levelManager);
    }
}
