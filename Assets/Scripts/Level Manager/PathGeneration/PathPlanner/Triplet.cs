using System.Collections.Generic;
using UnityEngine;

public class Triplet : PathUnit
{
    public int[] BackConnections { get; set; } = new int[2];
    public Orientation Orientation { get; set; }
    public Placement placement { get; set; }
    public BlockStructureData BlockStructureData { get; set; }
    public Vector2Int Position { get; set; }

    public override Vector2Int ExtractConnectionPointPosition(BlockGridSettings blockGridSettings
        , List<Triplet> instantiatedTriplets)
    {
        return BlockStructureData.ExitConnection.GetConnectionPoint(blockGridSettings);
    }
}
