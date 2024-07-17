using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class RoomLevelInfo
{
    [SerializeField][Range(1, MAX_ROOM_COUNT_ON_LEVEL)] private int _minRoomCount = 1;
    [SerializeField][Range(1, MAX_ROOM_COUNT_ON_LEVEL)] private int _maxRoomCount = 1;
    [SerializeField][Min(0)] private int _betweenRoomSpaceSize = 5;

    private const int MAX_ROOM_COUNT_ON_LEVEL = 10;
    private const int CENTER_CAPTURED_SPACE_BIAS = 1;

    private RoomInfo[] _levelRoomsContainer;

    private int _horizontalPos = 0;

    private int _capturedVerticalPlace = 0;

    public int HorizontalPos { get => _horizontalPos; }
    public int CapturedVerticalPlace { get => _capturedVerticalPlace; }
    public RoomInfo[] LevelRoomsContainer { get => _levelRoomsContainer.ToArray(); }

    public void GenerateRoomLevel(GameObject[] roomsPrefabs, int verticalPos)
    {
        _levelRoomsContainer = new RoomInfo[_minRoomCount + (int)((_maxRoomCount - _minRoomCount + 1) * UnityEngine.Random.value)];
        List<GameObject> roomObjects = FillRoomObjects(roomsPrefabs);
        
        int maxCapturedPlaceBelow = FindMaxCapturedVerticalPlaceBelow(roomObjects);
        verticalPos = verticalPos + maxCapturedPlaceBelow;

        _horizontalPos = 0;

        for (int i = 0; i < _levelRoomsContainer.Length; i++)
        {
            if (roomObjects[i].TryGetComponent(out BlockStructure roomData))
            {
                int bigestSpaceOnLeft = Math.Abs(roomData.CapturedZoneInBlockGridStart.x < roomData.CapturedZoneInBlockGridEnd.x
                        ? roomData.CapturedZoneInBlockGridStart.x : roomData.CapturedZoneInBlockGridEnd.x);
                int bigestSpaceOnRight = Math.Abs(roomData.CapturedZoneInBlockGridStart.x > roomData.CapturedZoneInBlockGridEnd.x
                        ? roomData.CapturedZoneInBlockGridStart.x : roomData.CapturedZoneInBlockGridEnd.x);
                _horizontalPos += bigestSpaceOnLeft + 1;

                _levelRoomsContainer[i] = new RoomInfo(roomObjects[i], new Vector2Int(_horizontalPos, verticalPos));
                _horizontalPos += bigestSpaceOnRight;
            }
            else
            {
                Debug.Log("Unacceptable room configuration");
                return;
            }
            _horizontalPos += _betweenRoomSpaceSize;
        }

        _capturedVerticalPlace = FindMaxCapturedVerticalPlaceAbove(roomObjects) + maxCapturedPlaceBelow + CENTER_CAPTURED_SPACE_BIAS;
    }

    private List<GameObject> FillRoomObjects(GameObject[] roomsPrefabs)
    {
        List<GameObject> roomObjects = new();
        for (int i = 0; i < _levelRoomsContainer.Length; i++)
        {
            roomObjects.Add(roomsPrefabs[UnityEngine.Random.Range(0, roomsPrefabs.Length)]);
        }
        return roomObjects;
    }

    private int FindMaxCapturedVerticalPlaceBelow(List<GameObject> roomObjects)
    {
        return (int)MathF.Abs(roomObjects.Min(
            g =>
            {
                if (g.TryGetComponent(out BlockStructure roomData))
                {
                    int bigestCapturedPlaceUnderneath = roomData.CapturedZoneInBlockGridStart.y < roomData.CapturedZoneInBlockGridEnd.y
                        ? roomData.CapturedZoneInBlockGridStart.y : roomData.CapturedZoneInBlockGridEnd.y;
                    return bigestCapturedPlaceUnderneath;
                }
                return 0;
            }));
    }

    private int FindMaxCapturedVerticalPlaceAbove(List<GameObject> roomObjects)
    {
        return roomObjects.Max(g =>
        {
            if (g.TryGetComponent(out BlockStructure roomData))
            {
                bool startHasBigestY = roomData.CapturedZoneInBlockGridStart.y > roomData.CapturedZoneInBlockGridEnd.y;
                return startHasBigestY
                    ? roomData.CapturedZoneInBlockGridStart.y : roomData.CapturedZoneInBlockGridEnd.y;
            }
            return 0;
        });
    }
}
