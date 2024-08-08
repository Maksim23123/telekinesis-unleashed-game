using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathPlanner : MonoBehaviour
{
    HashSet<PathUnit> _idRegister = new HashSet<PathUnit>();

    public HashSet<PathUnit> GeneratePathPlan(List<List<PathUnit>[]> roomStructure)
    {
        _idRegister.Clear();
        for (int i = 0; i < roomStructure.Count; i++)
        {
            List<PathUnit>[] connectionLayer = roomStructure[i];
            List<PathUnit> unionCandidats = new();
            PathUnit exitsKnot = null;
            PathUnit enterancesKnot = null;
            if (connectionLayer.Length != 2)
            {
                Debug.Log("Number of point sets within connection layer is not equal 2.");
                return null;
            }
            for (int j = 0; j < 2; j++)
            {
                unionCandidats.Clear();
                for (int k = 0; k < connectionLayer[j].Count; k++)
                {
                    connectionLayer[j][k] = RegisterId(connectionLayer[j][k]);
                }
                unionCandidats.AddRange(connectionLayer[j]);
                if (j == 0)
                {
                    enterancesKnot = CreateTriplets(Placement.Above, ref unionCandidats);
                }
                else
                {
                    exitsKnot = CreateTriplets(Placement.Bellow, ref unionCandidats);
                }
            }
            unionCandidats.Clear();
            unionCandidats.Add(exitsKnot);
            unionCandidats.Add(enterancesKnot);
            CreateTriplets(Placement.Bellow, ref unionCandidats, isKnot:true);
        }
        return _idRegister;
    }

    private PathUnit RegisterId(PathUnit pathUnit)
    {
        int freeId = StaticTools.GetFreeId(_idRegister, x => x.Id);
        pathUnit.Id = freeId;
        _idRegister.Add(pathUnit);
        return pathUnit;
    }

    private PathUnit CreateTriplets(Placement tripletPlacement, ref List<PathUnit> unionCandidats, bool isKnot = false)
    {
        PathUnit knotPoint = null;
        do
        {
            List<PathUnit> unionCandidatsCopy = unionCandidats.ToList();
            for (int i = 0; i < unionCandidatsCopy.Count; i += 2)
            {
                PathUnit firstCurrentPoint = unionCandidatsCopy[i];
                PathUnit secondCurrentPoint = null;
                if (i + 1 < unionCandidatsCopy.Count)
                {
                    secondCurrentPoint = unionCandidatsCopy[i + 1];
                }

                if (secondCurrentPoint != null)
                {
                    Triplet currentTriplet = new Triplet();
                    Orientation tripletOrientation;
                    if (0.5 > Random.value)
                    {
                        tripletOrientation = Orientation.Left;
                    }
                    else
                    {
                        tripletOrientation = Orientation.Right;
                    }
                    currentTriplet.Orientation = tripletOrientation;
                    currentTriplet.placement = tripletPlacement;
                    if (currentTriplet.Orientation == Orientation.Right != (currentTriplet.placement == Placement.Bellow) 
                            && !isKnot)
                    {
                        currentTriplet.BackConnections[0] = firstCurrentPoint.Id;
                        currentTriplet.BackConnections[1] = secondCurrentPoint.Id;
                    }
                    else
                    {
                        currentTriplet.BackConnections[0] = secondCurrentPoint.Id;
                        currentTriplet.BackConnections[1] = firstCurrentPoint.Id;
                    }
                    currentTriplet = (Triplet)RegisterId(currentTriplet);
                    unionCandidats.Remove(firstCurrentPoint);
                    unionCandidats.Remove(secondCurrentPoint);
                    unionCandidats.Add(currentTriplet);
                    knotPoint = currentTriplet;
                }
                else
                {
                    unionCandidats.Remove(firstCurrentPoint);
                    unionCandidats.Add(firstCurrentPoint);
                    knotPoint = firstCurrentPoint;
                }
            }
        } 
        while (unionCandidats.Count > 1);
        return knotPoint;
    }
}
