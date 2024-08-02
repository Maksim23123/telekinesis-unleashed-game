using UnityEngine;

public struct GridArea
{
    public Vector2Int AreaStart { get; set; }
    public Vector2Int AreaEnd { get; set; }

    public GridArea(Vector2Int areaStart, Vector2Int areaEnd)
    {
        AreaStart = areaStart;
        AreaEnd = areaEnd;
    }
}
