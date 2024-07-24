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

    public abstract Vector2Int ExtractConnectionPointPosition(BlockGridSettings blockGridSettings
        , List<Triplet> instantiatedTriplets);


    public static PathUnit GetById(IEnumerable<PathUnit> pathUnits, int id)
    {
        return pathUnits
            .Where(x => x.Id == id)
            .FirstOrDefault();
    }
}
