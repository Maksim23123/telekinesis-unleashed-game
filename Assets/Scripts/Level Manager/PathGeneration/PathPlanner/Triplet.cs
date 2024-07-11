using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Triplet : PathUnit
{
    public int[] BackConnections { get; set; } = new int[2];
    public TripletOrientation Orientation { get; set; }
    public TripletPlacement placement { get; set; }
}

public enum TripletOrientation
{
    Right,
    Left
}

public enum TripletPlacement
{
    Above,
    Bellow
}
