using UnityEngine;

public class PathCell
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
}
