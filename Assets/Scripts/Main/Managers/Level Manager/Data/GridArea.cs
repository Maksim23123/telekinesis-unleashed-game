using UnityEngine;

public struct GridArea
{
    public Vector2 AreaStart { get; set; }
    public Vector2 AreaEnd { get; set; }

    public GridArea(Vector2Int areaStart, Vector2Int areaEnd)
    {
        AreaStart = areaStart;
        AreaEnd = areaEnd;
    }
}
