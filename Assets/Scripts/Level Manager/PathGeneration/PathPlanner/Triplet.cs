using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Triplet : PathUnit
{
    public int[] BackConnections { get; set; } = new int[2];
    public Orientation Orientation { get; set; }
    public Placement placement { get; set; }
    public GameObject GameObject { get; set; }
}
