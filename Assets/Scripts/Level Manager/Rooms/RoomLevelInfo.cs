using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class RoomLevelInfo
{
    private const int _maxRoomCountOnLevel = 10;

    [SerializeField][Range(1, _maxRoomCountOnLevel)] private int _minRoomCount = 1;
    [SerializeField][Range(1, _maxRoomCountOnLevel)] private int _maxRoomCount = 1;
    [SerializeField][Min(0)] private int _betweenRoomSpaceSize = 5;

    private RoomInfo[] _roomInfos;

    private int _horizontalPos = 0;

    private int _capturedVerticalPlace = 0;

    public int HorizontalPos { get => _horizontalPos; }
    public int CapturedVerticalPlace { get => _capturedVerticalPlace; }
    public RoomInfo[] RoomInfos { get => _roomInfos.ToArray(); }

    public void GenerateRooms(GameObject[] roomsPrefabs, int verticalPos)
    {
        _roomInfos = new RoomInfo[_minRoomCount + (int)(((_maxRoomCount - _minRoomCount) + 1) * UnityEngine.Random.value)];
        List<GameObject> roomObjects = new();
        for (int i = 0; i < _roomInfos.Length; i++)
        {
            roomObjects.Add(roomsPrefabs[UnityEngine.Random.Range(0, roomsPrefabs.Length)]);
        }
        _capturedVerticalPlace = roomObjects.Max(g =>
        {
            if (g.TryGetComponent(out RoomData roomData))
            {
                return roomData.CapturedZoneInBlockGridStart.y > roomData.CapturedZoneInBlockGridEnd.y
                    ? roomData.CapturedZoneInBlockGridStart.y : roomData.CapturedZoneInBlockGridEnd.y;
            }
            return 0;
        });

        int verticalPosOffset = (int)MathF.Abs(roomObjects.Min(
            g =>
            {
                if (g.TryGetComponent(out RoomData roomData))
                {
                    int bigestCapturedPlaceUnderneath = roomData.CapturedZoneInBlockGridStart.y < roomData.CapturedZoneInBlockGridEnd.y
                        ? roomData.CapturedZoneInBlockGridStart.y : roomData.CapturedZoneInBlockGridEnd.y;
                    return bigestCapturedPlaceUnderneath;
                }
                return 0;
            }));
        verticalPos = verticalPos + verticalPosOffset;

        _horizontalPos = 0;

        for (int i = 0; i < _roomInfos.Length; i++)
        {
            if (roomObjects[i].TryGetComponent(out RoomData roomData))
            {
                int bigestSpaceOnLeft = Math.Abs(roomData.CapturedZoneInBlockGridStart.x < roomData.CapturedZoneInBlockGridEnd.x
                        ? roomData.CapturedZoneInBlockGridStart.x : roomData.CapturedZoneInBlockGridEnd.x);
                int bigestSpaceOnRight = Math.Abs(roomData.CapturedZoneInBlockGridStart.x > roomData.CapturedZoneInBlockGridEnd.x
                        ? roomData.CapturedZoneInBlockGridStart.x : roomData.CapturedZoneInBlockGridEnd.x);
                _horizontalPos += bigestSpaceOnLeft + 1;

                _roomInfos[i] = new RoomInfo(roomObjects[i], new Vector2Int(_horizontalPos, verticalPos));
                _horizontalPos += bigestSpaceOnRight;
            }
            else
            {
                _roomInfos[i] = null;
            }
            _horizontalPos += _betweenRoomSpaceSize;
        }

        _capturedVerticalPlace += verticalPosOffset + 1;
    }
}
