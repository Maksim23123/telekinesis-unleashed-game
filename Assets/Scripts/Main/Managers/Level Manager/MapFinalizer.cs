
using System.Linq;
using System;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LevelManager))]
public class MapFinalizer : MonoBehaviour
{
    private LevelManager _levelManager;

    public BlockGridSettings BlockGridSettings { get => _levelManager.BlockGridSettings; }

    private Vector2Int RightBias {
        get
        {
            return new Vector2Int(1 * BlockGridSettings.HorizontalExpandDirectionFactor, 0);
        }
    }

    private Vector2Int LeftBias
    {
        get
        {
            return new Vector2Int(-1 * BlockGridSettings.HorizontalExpandDirectionFactor, 0);
        }
    }

    private List<BlockInfoHolder> LevelElements
    {
        get
        {
            return _levelManager.LevelElements;
        }
    }

    public void FinalizeMap()
    {
        Initialize();
        SealHolesInCorridors();
    }

    private void Initialize()
    {
        _levelManager = GetComponent<LevelManager>();

    }

    private void SealHolesInCorridors()
    {
        _levelManager.TryGetSuitableBlock(false, false, true, false, out BlockInfoHolder rightConnectedDeadEndPrefab, true);
        _levelManager.TryGetSuitableBlock(false, false, false, true, out BlockInfoHolder leftConnectedDeadEndPrefab, true);

        SealUnfinishedRLBlocks(rightConnectedDeadEndPrefab, leftConnectedDeadEndPrefab);

        SealGapsInLadders(rightConnectedDeadEndPrefab, leftConnectedDeadEndPrefab);
        TransformOneSidedDeadEndsIntoTwoSided();
    }

    private bool CheckNeighbor(Orientation direction, BlockInfoHolder blockInfoHolder)
    {
        Vector2Int bias;
        if (direction == Orientation.Right)
        {
            bias = RightBias;
        }
        else
        {
            bias = LeftBias;
        }

        return _levelManager.TryGetBlockInfoByPosition(blockInfoHolder.BlockPosstion + bias, out var _);
    }

    private void SealUnfinishedRLBlocks(BlockInfoHolder rightConnectedDeadEndPrefab, BlockInfoHolder leftConnectedDeadEndPrefab)
    {
        BlockInfoHolder[] corridorsToSeal = LevelElements
                    .GroupBy(blockInfoHolder => blockInfoHolder.Generation)
                    .OrderBy(generation => generation.Key)
                    .First()
                    .Where(blockInfoHolder => blockInfoHolder.IsRightLeftCoridor && (!CheckNeighbor(Orientation.Right
                        , blockInfoHolder) ^ !CheckNeighbor(Orientation.Right, blockInfoHolder)))
                    .ToArray();

        foreach (var corridor in corridorsToSeal)
        {
            _levelManager.DestroyBlock(corridor);
            if (CheckNeighbor(Orientation.Right, corridor))
                _levelManager.InstantiateBlock(corridor.BlockPosstion, rightConnectedDeadEndPrefab);
            else if (CheckNeighbor(Orientation.Left, corridor))
                _levelManager.InstantiateBlock(corridor.BlockPosstion, leftConnectedDeadEndPrefab);
        }
    }

    private void SealGapsInLadders(BlockInfoHolder rightConnectedDeadEndPrefab, BlockInfoHolder leftConnectedDeadEndPrefab)
    {
        BlockInfoHolder[] laddersToCheck = LevelElements
                    .Where(blockInfoHolder => blockInfoHolder.DownConnected || blockInfoHolder.UpConnected)
                    .ToArray();

        foreach (BlockInfoHolder ladder in laddersToCheck)
        {
            if (!CheckNeighbor(Orientation.Right, ladder))
            {
                _levelManager.InstantiateBlock(ladder.BlockPosstion + RightBias, leftConnectedDeadEndPrefab);
            }

            if (!CheckNeighbor(Orientation.Left, ladder))
            {
                _levelManager.InstantiateBlock(ladder.BlockPosstion + LeftBias, rightConnectedDeadEndPrefab);
            }
        }
    }

    private void TransformOneSidedDeadEndsIntoTwoSided()
    {
        BlockInfoHolder[] deadEndsToBecomeRLConnected = LevelElements
                    .Where(blockInfoHolder => blockInfoHolder.DeadEnd 
                        && (blockInfoHolder.RightConnected ^ blockInfoHolder.LeftConnected)) // Serch for regular dead ends
                    .ToArray();

        foreach (BlockInfoHolder regularDeadEnd in deadEndsToBecomeRLConnected)
        {
            if (_levelManager.TryGetSuitableBlock(false, false, true, true, out BlockInfoHolder twoSidedDeadEndPrefab, true)
                    && _levelManager.TryGetBlockInfoByPosition(regularDeadEnd.BlockPosstion + RightBias, out var rightNeighbor) 
                    && rightNeighbor.LeftConnected
                    && _levelManager.TryGetBlockInfoByPosition(regularDeadEnd.BlockPosstion + LeftBias, out var leftNeighbor) 
                    && leftNeighbor.RightConnected)
            {
                _levelManager.DestroyBlock(regularDeadEnd);
                _levelManager.InstantiateBlock(regularDeadEnd.BlockPosstion, twoSidedDeadEndPrefab);
            }
        }
    }
}
