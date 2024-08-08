
using System.Linq;
using System;
using UnityEngine;
using System.Collections.Generic;
using Codice.CM.Common;

[RequireComponent(typeof(LevelManager))]
public class MapFinalizer : MonoBehaviour
{
    private LevelManager _levelManager;

    private List<BlockInfoHolder> _levelElementsInOperationalArea;

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

    public void FinalizeArea(GridArea areaToFinalize)
    {
        Initialize();
        _levelElementsInOperationalArea = LevelElements
            .Where(blockInfoHolder => areaToFinalize.IsWithinArea(blockInfoHolder.BlockPosstion))
            .ToList();
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
        BlockInfoHolder[] corridorsToSeal = _levelElementsInOperationalArea
                    .Where(blockInfoHolder =>
                        blockInfoHolder.IsRightLeftCoridor && (!CheckNeighbor(Orientation.Right
                        , blockInfoHolder) ^ !CheckNeighbor(Orientation.Right, blockInfoHolder)))
                    .ToArray();

        foreach (var corridor in corridorsToSeal)
        {
            _levelManager.TryDestroyBlockByPosition(corridor.BlockPosstion);
            _levelElementsInOperationalArea.Remove(corridor);
            BlockInfoHolder newBlockInfoHolder = null;
            if (CheckNeighbor(Orientation.Right, corridor))
            {
                _levelManager.InstantiateBlock(corridor.BlockPosstion, rightConnectedDeadEndPrefab);
                newBlockInfoHolder = rightConnectedDeadEndPrefab.HollowCopy();
            }
            else if (CheckNeighbor(Orientation.Left, corridor))
            {
                _levelManager.InstantiateBlock(corridor.BlockPosstion, leftConnectedDeadEndPrefab);
                newBlockInfoHolder = rightConnectedDeadEndPrefab.HollowCopy();
                
            }
            newBlockInfoHolder.BlockPosstion = corridor.BlockPosstion;
            _levelElementsInOperationalArea.Add(newBlockInfoHolder);
        }
    }

    private void SealGapsInLadders(BlockInfoHolder rightConnectedDeadEndPrefab, BlockInfoHolder leftConnectedDeadEndPrefab)
    {
        BlockInfoHolder[] laddersToCheck = _levelElementsInOperationalArea
                    .Where(blockInfoHolder => blockInfoHolder.DownConnected || blockInfoHolder.UpConnected)
                    .ToArray();

        foreach (BlockInfoHolder ladder in laddersToCheck)
        {
            if (!CheckNeighbor(Orientation.Right, ladder))
            {
                Vector2Int positionForSeal = ladder.BlockPosstion + RightBias;
                _levelManager.InstantiateBlock(positionForSeal, leftConnectedDeadEndPrefab);
                BlockInfoHolder newBlockInfoHolder = leftConnectedDeadEndPrefab.HollowCopy();
                newBlockInfoHolder.BlockPosstion = positionForSeal;
                _levelElementsInOperationalArea.Add(newBlockInfoHolder);
            }

            if (!CheckNeighbor(Orientation.Left, ladder))
            {
                Vector2Int positionForSeal = ladder.BlockPosstion + LeftBias;
                _levelManager.InstantiateBlock(positionForSeal, rightConnectedDeadEndPrefab);
                BlockInfoHolder newBlockInfoHolder = rightConnectedDeadEndPrefab.HollowCopy();
                newBlockInfoHolder.BlockPosstion = positionForSeal;
                _levelElementsInOperationalArea.Add(newBlockInfoHolder);
            }
        }
    }

    private void TransformOneSidedDeadEndsIntoTwoSided()
    {
        BlockInfoHolder[] deadEndsToBecomeRLConnected = _levelElementsInOperationalArea
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
                _levelManager.TryDestroyBlockByPosition(regularDeadEnd.BlockPosstion);
                _levelElementsInOperationalArea.Remove(regularDeadEnd);

                _levelManager.InstantiateBlock(regularDeadEnd.BlockPosstion, twoSidedDeadEndPrefab);
                BlockInfoHolder newBlockInfoHolder = twoSidedDeadEndPrefab.HollowCopy();
                newBlockInfoHolder.BlockPosstion = regularDeadEnd.BlockPosstion;
                _levelElementsInOperationalArea.Add(newBlockInfoHolder);
            }
        }
    }
}
