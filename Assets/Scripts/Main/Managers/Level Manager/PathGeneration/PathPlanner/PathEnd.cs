using System.Collections.Generic;
using UnityEngine;

public class PathEnd : PathUnit
{
    public Connection Connection { get; set; }

    public override Vector2Int ExtractConnectionPointPosition(BlockGridSettings blockGridSettings, List<Triplet> instantiatedTriplets)
    {
        return Connection.GetConnectionPoint(blockGridSettings);
    }
}
