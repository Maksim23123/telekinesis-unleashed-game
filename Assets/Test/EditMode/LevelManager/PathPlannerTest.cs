using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PathPlannerTest
{
    //MethodName_StateUnderTest_ExpectedBehavior
    [Test]
    public void GeneratePathPlan_AllPathEndsConnection_Connected()
    {
        try
        {
            //Arange

            GameObject gameObject = new GameObject();

            PathPlanner pathPlanner = gameObject.AddComponent<PathPlanner>();

            List<PathEnd> enterances = new List<PathEnd>();
            List<PathEnd> exits = new List<PathEnd>();
            FillWithPathEnds(ref exits);
            FillWithPathEnds(ref enterances);
            List<List<PathUnit>[]> connectionLayers = new();
            List<PathUnit>[] connectionLayer = { exits.ToList<PathUnit>(), enterances.ToList<PathUnit>() };
            connectionLayers.Add(connectionLayer);

            //Act

            HashSet<PathUnit> path = pathPlanner.GeneratePathPlan(connectionLayers);

            //Assert

            int totalPathEndsCount = enterances.Count + exits.Count;
            int pathEndsCountInGeneratedPathPlan = WalkThroughPathPlan(path).Count;
            Assert.AreEqual(totalPathEndsCount, pathEndsCountInGeneratedPathPlan);
        }
        catch (System.Exception ex)
        {
            Assert.Fail(ex.Message);
        }
    }

    public void FillWithPathEnds(ref List<PathEnd> pathEnds)
    {
        const int basePathEndsCount = 3;

        pathEnds.Clear();
        for (int i = 0; i < basePathEndsCount; i++)
        {
            pathEnds.Add(new PathEnd());
        }
    }

    public List<PathEnd> WalkThroughPathPlan(HashSet<PathUnit> path)
    {
        List<PathEnd> reachablePathEnds = new();
        List<int> idsToExamine = new List<int>
        {
            path.First().Id
        };
        HashSet<int> examindeIds = new();

        while (idsToExamine.Count > 0)
        {
            foreach (int currentId in idsToExamine.ToList())
            {
                idsToExamine.Remove(currentId);
                examindeIds.Add(currentId);
                PathUnit currentPathUnit = path.Where(x => x.Id == currentId).First();
                if (currentPathUnit is PathEnd && !reachablePathEnds.Any(x => x.Id == currentId))
                {
                    reachablePathEnds.Add((PathEnd)currentPathUnit);
                }
                else if (currentPathUnit is Triplet)
                {
                    idsToExamine.AddRange(GetTripletConnections((Triplet)currentPathUnit).Where(x => !examindeIds.Contains(x)));
                }
                PathUnit successor = path.Where(x => // The one that contains current id as back connection
                    {
                        if (x is Triplet)
                        {
                            return GetTripletConnections((Triplet)x).Contains(currentId);
                        }
                        return false;
                    })
                    .FirstOrDefault();
                if (successor != null)
                {
                    idsToExamine.Add(successor.Id);
                }
            }
        }
        return reachablePathEnds;
    }

    public int[] GetTripletConnections(Triplet triplet)
    {
        List<int> connections = new();
        connections.AddRange(triplet.BackConnections);
        return connections.ToArray();
    }
}
