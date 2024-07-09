using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomInfo
{
    public RoomInfo(GameObject roomObjectPrefab, Vector2Int positionInGrid)
    {
        RoomObjectPrefab = roomObjectPrefab;
        PositionInGrid = positionInGrid;
    }

    public GameObject RoomObjectPrefab { get; set; }
    public Vector2Int PositionInGrid { get; set; }
}
