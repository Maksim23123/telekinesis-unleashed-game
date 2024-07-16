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
                    connectionLayer[j][k] = registerId(connectionLayer[j][k]);
                }
                unionCandidats.AddRange(connectionLayer[j]);
                if (j == 0)
                {
                    enterancesKnot = CreateTriplet(TripletPlacement.Above, ref unionCandidats);
                }
                else
                {
                    exitsKnot = CreateTriplet(TripletPlacement.Above, ref unionCandidats);
                }
            }
            unionCandidats.Clear();
            unionCandidats.Add(enterancesKnot);
            unionCandidats.Add(exitsKnot);
            CreateTriplet(TripletPlacement.Above, ref unionCandidats);
        }
        return _idRegister;
    }

    private PathUnit registerId(PathUnit pathUnit)
    {
        int freeId = StaticTools.GetFreeId(_idRegister, x => x.Id);
        pathUnit.Id = freeId;
        _idRegister.Add(pathUnit);
        return pathUnit;
    }

    private PathUnit CreateTriplet(TripletPlacement tripletPlacement, ref List<PathUnit> unionCandidats)
    {
        PathUnit knotPoint = null;
        while (unionCandidats.Count > 1)
        {
            List<PathUnit> unionCandidatsCopy = unionCandidats.ToList();
            for (int i = 0; i < unionCandidats.Count; i += 2)
            {
                PathUnit fCurrentPoint = unionCandidats[i];
                PathUnit sCurrentPoint = null;
                if (i + 1 < unionCandidats.Count)
                {
                    sCurrentPoint = unionCandidats[i + 1];
                }
                if (sCurrentPoint != null)
                {
                    Triplet currentTriplet = new Triplet();
                    Orientation tripletOrientation;
                    if (i + 1 > unionCandidatsCopy.Count / 2)
                    {
                        tripletOrientation = Orientation.Left;
                    }
                    else
                    {
                        tripletOrientation = Orientation.Right;
                    }
                    currentTriplet.Orientation = tripletOrientation;
                    currentTriplet.placement = tripletPlacement;
                    if (currentTriplet.Orientation == Orientation.Right != (currentTriplet.placement == TripletPlacement.Bellow))
                    {
                        currentTriplet.BackConnections[0] = fCurrentPoint.Id;
                        currentTriplet.BackConnections[1] = sCurrentPoint.Id;
                    }
                    else
                    {
                        currentTriplet.BackConnections[0] = sCurrentPoint.Id;
                        currentTriplet.BackConnections[1] = fCurrentPoint.Id;
                    }
                    currentTriplet = (Triplet)registerId(currentTriplet);
                    unionCandidats.Remove(fCurrentPoint);
                    unionCandidats.Remove(sCurrentPoint);
                    unionCandidats.Add(currentTriplet);
                    knotPoint = currentTriplet;
                }
                else
                {
                    knotPoint = fCurrentPoint;
                }
            }
        }
        return knotPoint;
    }
}
