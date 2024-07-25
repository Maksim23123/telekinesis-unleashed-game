using System;
using System.Collections.Generic;
using UnityEngine;

public class PathCell : IComparer<PathCell>
{
    public Vector2Int Position { get; set; }
    public PathCell Parent { get; set; }
    public float F { get => G + H; }
    public float G { get; set; }
    public float H { get; set; }

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
        if (x.F == y.F)
        {
            return 1;
        }
        return x.F.CompareTo(y.F);
    }
}
