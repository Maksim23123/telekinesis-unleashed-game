using System.Linq;
using UnityEngine;

public struct GridArea
{
    public Vector2Int AreaStart { get; set; }
    public Vector2Int AreaEnd { get; set; }

    public int[] XValues
    {
        get
        {
            return new int[] { AreaStart.x, AreaEnd.x };
        }
    }

    public int[] YValues
    {
        get
        {
            return new int[] { AreaStart.y, AreaEnd.y };
        }
    }

    public Vector2Int CornerWithLowestValues
    {
        get
        {
            return new Vector2Int(XValues.Min(), YValues.Min());
        }
    }

    Vector2Int CornerWithHighestValues
    {
        get
        {
            return new Vector2Int(XValues.Max(), YValues.Max());
        }
    }

    public GridArea(Vector2Int areaStart, Vector2Int areaEnd)
    {
        AreaStart = areaStart;
        AreaEnd = areaEnd;
    }

    public bool IsWithInArea(Vector2Int position)
    {
        if (position.x >= CornerWithLowestValues.x
                && position.y >= CornerWithLowestValues.y
                && position.x <= CornerWithHighestValues.x
                && position.y <= CornerWithHighestValues.y)
        {
            return true;
        }
        return false;
    }
}
