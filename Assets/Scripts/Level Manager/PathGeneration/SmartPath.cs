using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SmartPath
{
    private LevelManager _levelManager;

    private GameObject _marker;

    public SmartPath(LevelManager levelManager)
    {
        _levelManager = levelManager ?? throw new ArgumentNullException(nameof(levelManager));
    }

    public bool AStar(Vector2Int startPosition, Vector2Int endPosition, out Vector2Int[] path)
    {
        path = new Vector2Int[0];
        List<PathCell> openList = new();
        HashSet<PathCell> closedList = new();
        bool goalFound = false;
        PathCell pathEnd = null;

        int itters = 0;

        openList.Add(new PathCell(startPosition, null));

        while (openList.Count > 0 && !goalFound && itters < 10000)
        {
            itters++;
            PathCell q = openList.OrderBy(x => x.F).First();

            Vector2 worldPosition = q.Position * _levelManager.BlockGridSettings.BlocksSize + _levelManager.BlockGridSettings.PossitionBias;

            openList.Remove(q);

            PathCell[] successors = GenerateSuccessors(q, _levelManager);

            for (int i = 0; i < successors.Length; i++)
            {
                if (successors[i].Position == endPosition)
                {
                    pathEnd = successors[i];
                    goalFound = true;
                    break;
                }

                successors[i].G = q.G + ManhattanDistance(q.Position, successors[i].Position);
                successors[i].H = ManhattanDistance(successors[i].Position, endPosition);

                if (!(openList.Any(x => x.Position == successors[i].Position && x.F < successors[i].F) ||
                        closedList.Any(x => x.Position == successors[i].Position && x.F < successors[i].F)))
                {
                    foreach (PathCell cellToRemove in openList.Where(x => x.Position == successors[i].Position).ToList())
                    {
                        openList.Remove(cellToRemove);
                    }
                    openList.Add(successors[i]);
                }
            }

            closedList.Add(q);
        }

        if (goalFound)
        {
            path = EvenUpParrents(pathEnd).ToArray();
            return true;
        }
        else
        {
            return false;
        }
    }

    private List<Vector2Int> EvenUpParrents(PathCell endCell)
    {
        List<Vector2Int> path = new();
        PathCell currentCell = endCell;
        while (currentCell != null)
        {
            path.Add(currentCell.Position);
            currentCell = currentCell.Parent;
        }
        return path;
    }

    private PathCell[] GenerateSuccessors(PathCell q, LevelManager levelManager)
    {
        PathCell[] successors = new PathCell[4];
        successors[0] = new PathCell(q.Position + Vector2Int.up, q);
        successors[1] = new PathCell(q.Position + Vector2Int.down, q);
        successors[2] = new PathCell(q.Position + Vector2Int.left, q);
        successors[3] = new PathCell(q.Position + Vector2Int.right, q);

        return successors.Where(x => !levelManager.TryGetBlockInfoByPosition(x.Position, out var _) && x.Position != x.Parent.Position)
            .OrderBy(x => UnityEngine.Random.value)
            .ToArray();
    }

    private int ManhattanDistance(Vector2Int startPosition, Vector2Int endPosition)
    {
        return Mathf.Abs(startPosition.x - endPosition.x) + Mathf.Abs(startPosition.y - endPosition.y);
    }
}
