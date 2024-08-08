using System.Collections.Generic;
using UnityEngine;

public class Triplet : PathUnit
{
    public int[] BackConnections { get; set; } = new int[2];
    public Orientation Orientation { get; set; }
    public Placement placement { get; set; }
    public GameObject GameObject { get; set; }

    public override Vector2Int ExtractConnectionPointPosition(BlockGridSettings blockGridSettings, List<Triplet> instantiatedTriplets)
    {
        BlockStructure blockStructure = GameObject.GetComponent<BlockStructure>();
        return blockStructure.ExitConnection.GetConnectionPoint(blockGridSettings);
    }
}
