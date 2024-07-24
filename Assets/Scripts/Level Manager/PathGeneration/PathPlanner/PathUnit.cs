using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class PathUnit 
{
    public int Id { get; set; }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public static Vector2Int ExtractConnectionPointPosition(PathUnit x, BlockGridSettings blockGridSettings, List<Triplet> instantiatedTriplets)
    {
        if (x is PathEnd)
        {
            return ((PathEnd)x).Connection.GetConnectionPoint(blockGridSettings);
        }
        else if (x is Triplet)
        {
            Triplet currentTriplet = (Triplet)GetById(instantiatedTriplets, x.Id);
            BlockStructure blockStructure = currentTriplet.GameObject.GetComponent<BlockStructure>();
            return blockStructure.ExitConnection.GetConnectionPoint(blockGridSettings);
        }
        else
        {
            return Vector2Int.zero;
        }
    }

    public static PathUnit GetById(IEnumerable<PathUnit> pathUnits, int id)
    {
        return pathUnits
            .Where(x => x.Id == id)
            .FirstOrDefault();
    }
}
