using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    List<BlockInfoHolder> _blocksInfo = new List<BlockInfoHolder>();

    [SerializeField]
    GameObject _grid;

    [SerializeField]
    Vector2Int _mapDimensions, _blocksSize, _possitionBias;

    [HideInInspector]
    [SerializeField]
    List<BlockInfoHolder> _levelElements;

    [Range(0, 1)]
    [SerializeField]
    private float _horizontalGenerationStartPosition;

    [SerializeField]
    private int _minGenerationIterationsCount;

    [SerializeField]
    private int _maxGenerationIterationsCount;

    // ATTENTION, DEBUG
    //----------
    [Range(-1, 1)]
    [SerializeField]
    private int _horizontalExpandDirectionFactor;
    //----------

    [SerializeField]
    private int _minBlocksCount;

    [Header("Info")]

    [ReadOnly]
    [SerializeField]
    private int _blocksCount;

    [ReadOnly]
    [SerializeField]
    private int _blocksGenerationSize;

    [HideInInspector]
    private int _currentGeneration;

    public void Generate()
    {
        ClearLevel();

        if (_horizontalExpandDirectionFactor == 0)
        {
            _horizontalExpandDirectionFactor = 1;
        }

        Vector2Int startPosition = new Vector2Int((int)(_mapDimensions.x * _horizontalGenerationStartPosition - _horizontalGenerationStartPosition), 0);

        if (TryGetSuitableBlock(false, false, true, true, out BlockInfoHolder corridorPrefab))
        {
            _currentGeneration = 0;
            InstantiateBlock(startPosition, corridorPrefab);
            
            for (int i = 0; (i < _minGenerationIterationsCount || _blocksCount < _minBlocksCount) && i < _maxGenerationIterationsCount; i++)
            {
                _currentGeneration++;
                PerformGenrationIteration();
                if (_blocksGenerationSize == 0 && _blocksCount < _minBlocksCount)
                {
                    ReActivateGeneration();
                }
            }

            SealHolesInCorridors();
        }
    }

    private bool CheckIfRightLeftCorridor(BlockInfoHolder blockInfoHolder)
    {
        return !blockInfoHolder.UpConnected && !blockInfoHolder.DownConnected 
            && blockInfoHolder.LeftConnected && blockInfoHolder.RightConnected && !blockInfoHolder.DeadEnd;
    }

    private void SealHolesInCorridors()
    {
        Vector2Int rightBias = new Vector2Int(1 * _horizontalExpandDirectionFactor, 0);
        Vector2Int leftBias = new Vector2Int(-1 * _horizontalExpandDirectionFactor, 0);

        Func<BlockInfoHolder, bool> checkRightNeighbor = x 
            => TryGetBlockInfoByPosition(x.BlockPosstion + rightBias, out var _);
        Func<BlockInfoHolder, bool> checkLeftNeighbor = x
            => TryGetBlockInfoByPosition(x.BlockPosstion + leftBias, out var _);

        BlockInfoHolder[] corridorsToSeal = _levelElements
            .GroupBy(x => x.Generation)
            .OrderBy(x => x.Key)
            .Where(x => x.Key == _currentGeneration)
            .SelectMany(x => x)
            .Where(x => CheckIfRightLeftCorridor(x) && (!checkRightNeighbor(x) ^ !checkLeftNeighbor(x)))
            .ToArray();

        TryGetSuitableBlock(false, false, true, false, out BlockInfoHolder rightConnectedDeadEndPrefab, true);
        TryGetSuitableBlock(false, false, false, true, out BlockInfoHolder leftConnectedDeadEndPrefab, true);

        foreach (var corridor in corridorsToSeal)
        {
            DestroyBlock(corridor);
            if (checkRightNeighbor(corridor))
                InstantiateBlock(corridor.BlockPosstion, rightConnectedDeadEndPrefab);
            else 
                InstantiateBlock(corridor.BlockPosstion, leftConnectedDeadEndPrefab);
        }

        BlockInfoHolder[] laddersToCheck = _levelElements
            .Where(x => x.DownConnected || x.UpConnected)
            .ToArray();

        

        foreach (BlockInfoHolder ladder in laddersToCheck)
        {
            if (!checkRightNeighbor(ladder))
            {
                InstantiateBlock(ladder.BlockPosstion + rightBias, leftConnectedDeadEndPrefab);
            }

            if (!checkLeftNeighbor(ladder))
            {
                InstantiateBlock(ladder.BlockPosstion + leftBias, rightConnectedDeadEndPrefab);
            }
        }

        BlockInfoHolder[] deadEndsToBecomeRLConnected = _levelElements
            .Where(x => x.DeadEnd && (x.RightConnected ^ x.LeftConnected)) // Serch for regular dead ends
            .ToArray();

        foreach (BlockInfoHolder regularDeadEnd in deadEndsToBecomeRLConnected)
        {
            if (TryGetSuitableBlock(false, false, true, true, out BlockInfoHolder twoSidedDeadEndPrefab, true)
                    && TryGetBlockInfoByPosition(regularDeadEnd.BlockPosstion + rightBias, out var rightNeighbor) && rightNeighbor.LeftConnected
                    && TryGetBlockInfoByPosition(regularDeadEnd.BlockPosstion + leftBias, out var leftNeighbor) && leftNeighbor.RightConnected)
            {
                DestroyBlock (regularDeadEnd);
                InstantiateBlock(regularDeadEnd.BlockPosstion, twoSidedDeadEndPrefab);
            }
        }
    }

    private void ReActivateGeneration()
    {
        PopulateWithLadders();
        RemoveDeadEnds();
    }

    private void PopulateWithLadders()
    {
        int maxCandidatsCount = 10;
        int laddersGap = 2;

        BlockInfoHolder[] ladderCandidats = _levelElements
            .Where(g => CheckIfRightLeftCorridor(g)
                && !g.IsLadderNeighbor && g.BlockPosstion.x < _mapDimensions.x 
                && g.BlockPosstion.x > 0 && g.BlockPosstion.x % (laddersGap + 1) == 0)
            .OrderBy(x => x.BlockPosstion.y)
            .Reverse()
            .Take(maxCandidatsCount)
            .ToArray();

        foreach (var ladderCandidat in ladderCandidats)
        {
            DestroyBlock(ladderCandidat);
            if (!TryBuildLadder(ladderCandidat.BlockPosstion, GenerateLadderHeight()) && TryGetSuitableBlock(false, false, true, true, out BlockInfoHolder rlBlockReference))
            {
                InstantiateBlock(ladderCandidat.BlockPosstion, rlBlockReference);
            }
        }
    }

    private void RemoveDeadEnds()
    {
        BlockInfoHolder[] deadEnds = _levelElements
            .Where(x => (x.RightConnected && !x.LeftConnected) || (!x.RightConnected && x.LeftConnected))
            .ToArray();

        foreach (BlockInfoHolder block in deadEnds)
        {
            Vector2Int blockPosition = block.BlockPosstion;
            bool rightConnNecessary = block.RightConnected;
            bool leftConnNecessary = block.LeftConnected;
            DestroyBlock(block);
            TryGetSuitableBlock(false, false, true, true, out BlockInfoHolder freeWayBlockPrefab);
            GenerateBlock(blockPosition ,rightConnNecessary, leftConnNecessary);
        }
    }

    private void DestroyBlock(BlockInfoHolder block)
    {
        if (_levelElements.Contains(block))
        {
            DestroyImmediate(block.Block);
            _levelElements.Remove(block);
        }
    }

    public void ClearLevel()
    {
        foreach (GameObject levelElement in _levelElements.Select(x => x.Block))
        {
            DestroyImmediate(levelElement);
        }
        _levelElements.Clear();
    }

    public void PerformGenrationIteration()
    {
        _blocksGenerationSize = 0;
        foreach (var block in _levelElements.Where(x => !x.NeighborshipResolved).ToList())
        {
            ResolveNeighborship(block.BlockPosstion);
        }
    }

    public void ResolveNeighborship(Vector2Int position)
    {
        if (TryGetBlockInfoByPosition(position, out BlockInfoHolder currentBlockInfo))
        {
            bool ladderNeighbor = false;
            if (currentBlockInfo.UpConnected || currentBlockInfo.DownConnected)
                ladderNeighbor = true;

            Vector2Int rightBias = new Vector2Int(1 * _horizontalExpandDirectionFactor, 0);
            Vector2Int leftBias = new Vector2Int(-1 * _horizontalExpandDirectionFactor, 0);

            Vector2Int neighborPosition;
            if (currentBlockInfo.RightConnected)
            {
                neighborPosition = position + rightBias;
                if (CheckPosition(neighborPosition) && !TryGetBlockInfoByPosition(neighborPosition, out BlockInfoHolder neighbor))
                {
                    GenerateBlock(neighborPosition, false, true, ladderNeighbor);
                }
            }

            if (currentBlockInfo.LeftConnected)
            {
                neighborPosition = position + leftBias;
                if (CheckPosition(neighborPosition) && !TryGetBlockInfoByPosition(neighborPosition, out BlockInfoHolder neighbor))
                {
                    GenerateBlock(neighborPosition, true, false, ladderNeighbor);
                }
            }

            currentBlockInfo.NeighborshipResolved = true;
        }
    }

    private void GenerateBlock(Vector2Int position, bool rightConnectionNecessary = false, bool leftConnectionNecessary = false, bool ladderNeighbor = true)
    {
        float ladderProbability = 0.1f;

        float coridorEndProbability = 0.25f;

        bool generationDone = false;

        bool positionIsOnVerticalEdge = position.x == _mapDimensions.x - 1 || position.x == 0;

        if (!positionIsOnVerticalEdge && !ladderNeighbor && UnityEngine.Random.value < ladderProbability)
        {
            generationDone = TryBuildLadder(position, GenerateLadderHeight());
        }

        if (!generationDone && (UnityEngine.Random.value < coridorEndProbability || positionIsOnVerticalEdge)
                && TryGetSuitableBlock(false, false, rightConnectionNecessary, leftConnectionNecessary, out BlockInfoHolder coridorEndInfo, true))
        {
            InstantiateBlock(position, coridorEndInfo, ladderNeighbor);
            generationDone = true;
        }

        if (!generationDone && TryGetSuitableBlock(false, false, true, true, out BlockInfoHolder blockInfo))
        {
            InstantiateBlock(position, blockInfo, ladderNeighbor);
        }
    }


    private int GenerateLadderHeight()
    {
        int maxLadderHeight = 4;
        int minLadderHeight = 2;
        return (int)((maxLadderHeight - minLadderHeight + 1) * UnityEngine.Random.value + minLadderHeight);
    }

    private void InstantiateBlock(Vector2Int position, BlockInfoHolder blockInfo, bool ladderNeighbor = false)
    {
        if (!TryGetBlockInfoByPosition(position, out var _))
        {
            GameObject currentBlock = Instantiate(blockInfo.Block, _grid.transform);
            currentBlock.transform.position = new Vector2(_horizontalExpandDirectionFactor * _blocksSize.x * position.x + _possitionBias.x
                , _blocksSize.y * position.y + _possitionBias.y);

            BlockInfoHolder newBlockInfoHolder = new BlockInfoHolder(currentBlock, position);
            newBlockInfoHolder.SetConnections(blockInfo.GetConnections());
            newBlockInfoHolder.IsLadderNeighbor = ladderNeighbor;

            newBlockInfoHolder.Generation = _currentGeneration;

            _levelElements.Add(newBlockInfoHolder);

            _blocksCount = _levelElements.Count;
            _blocksGenerationSize++;
        }
        else
        {
            Debug.LogError("Trying to and block in a filled cell.");
        }
    }

    private bool TryBuildLadder(Vector2Int position, int ladderHeight)
    {
        int possibleLadderHeight = CheckIfLadderPossible(position, ladderHeight);
        if (possibleLadderHeight >= 2)
        {
            TryGetSuitableBlock(true, false, true, true, out BlockInfoHolder ladderBottomBlockInfo);
            TryGetSuitableBlock(true, true, true, true, out BlockInfoHolder ladderMidwayBlockInfo);
            TryGetSuitableBlock(false, true, true, true, out BlockInfoHolder ladderTopBlockInfo);

            InstantiateBlock(position, ladderBottomBlockInfo);
            InstantiateBlock(position + new Vector2Int(0, possibleLadderHeight - 1), ladderTopBlockInfo);

            for (int i = 1; i < possibleLadderHeight - 1; i++)
            {
                InstantiateBlock(position + new Vector2Int(0, i), ladderMidwayBlockInfo);
            }
            return true;
        }
        else
            return false;
    }

    private int CheckIfLadderPossible(Vector2Int position, int ladderHeight)
    {
        int freeCells = 0;
        for (int i = 0; i < ladderHeight; i++)
        {
            Vector2Int currentCellPosition = position + new Vector2Int(0, i);
            if (!CheckPosition(currentCellPosition) || TryGetBlockInfoByPosition(currentCellPosition, out BlockInfoHolder blockInfoHolder))
                break;
            freeCells++;
        }
        return freeCells;
    }

    private bool TryGetSuitableBlock(bool up, bool down, bool right, bool left, out BlockInfoHolder blockInfoHolder, bool deadEnd = false)
    {
        blockInfoHolder = null;
        List<BlockInfoHolder> suitableBlocks = _blocksInfo.Where(x => x.UpConnected == up
            && x.DownConnected == down && x.LeftConnected == left && x.RightConnected == right && x.DeadEnd == deadEnd).ToList();
        if (suitableBlocks.Count > 0)
        {
            blockInfoHolder = suitableBlocks.First();
            return true;
        }
        return false;
    }

    private bool CheckPosition(Vector2Int position)
    {
        return position.x >= 0 && position.y >= 0 && position.x < _mapDimensions.x && position.y < _mapDimensions.y;
    }

    private bool TryGetBlockInfoByPosition(Vector2Int position, out BlockInfoHolder blockInfoHolder)
    {
        BlockInfoHolder[] blocksInfoByPossition = _levelElements.Where(x => x.BlockPosstion == position).ToArray();
        blockInfoHolder = null;
        if (blocksInfoByPossition.Length == 1)
        {
            blockInfoHolder = blocksInfoByPossition[0];
            return true;
        }
        else if (blocksInfoByPossition.Length > 1)
        {
            Debug.LogError("Multiple blocks can't exist at the same position");
        }
        return false;
    }
}
