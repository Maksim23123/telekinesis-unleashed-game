using System;
using System.Collections.Generic;
using UnityEngine;

public class PathCell : IComparer<PathCell>
{
    public Vector2Int Position { get; set; }
    public PathCell Parent { get; set; }
    public float DistanceFromStartWeight { get; set; }
    public float DistanceFromFinishWeight { get; set; }
    public float GeneralWeight { get => DistanceFromStartWeight + DistanceFromFinishWeight; }

    public PathCell(Vector2Int position, PathCell parent)
    {
        Position = position;
        Parent = parent;
    }

    public int Compare(PathCell x, PathCell y)
    {
        if (x == null || y == null)
        {
            throw new ArgumentException("Cannot compare null values");
        }
        if (x.GeneralWeight == y.GeneralWeight)
        {
            return 1;
        }
        return x.GeneralWeight.CompareTo(y.GeneralWeight);
    }
}
