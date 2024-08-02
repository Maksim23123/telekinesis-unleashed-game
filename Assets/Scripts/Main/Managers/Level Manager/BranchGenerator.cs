using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BranchGenerator : MonoBehaviour
{
    [SerializeField] LevelManager _levelManager;
    [Range(0, 1)][SerializeField] private float _horizontalGenerationStartPosition;
    [SerializeField] private int _minGenerationIterationsCount;
    [SerializeField] private int _maxGenerationIterationsCount;
    [SerializeField] private int _minBlocksCount;
    [SerializeField] int _maxLadderHeight = 4;
    [SerializeField] int _minLadderHeight = 2;

    [Header("Info")]

    [ReadOnly][SerializeField] private int _blocksCount;
    [ReadOnly][SerializeField] private int _blocksGenerationSize;

    private int _currentGeneration;
    private GridArea _operationalArea;
    private List<BlockInfoHolder> _levelElementsInOperationalArea;

    private BlockGridSettings BlockGridSettings
    {
        get
        {
            return _levelManager.BlockGridSettings;
        }
    }

    private List<BlockInfoHolder> LevelElements
    {
        get
        {
            return _levelManager.LevelElements;
        }
    }

    public void GenerateBranchesInArea(GridArea gridArea)
    {
        _operationalArea = gridArea;
        _levelElementsInOperationalArea = LevelElements
            .Where(blockInfoHolder => _operationalArea.IsWithinArea(blockInfoHolder.BlockPosstion))
            .ToList();
        if (_levelElementsInOperationalArea.Count > 0)
        {
            _currentGeneration = 0;

            for (int i = 0; (i < _minGenerationIterationsCount || _blocksCount < _minBlocksCount) 
                    && i < _maxGenerationIterationsCount; i++)
            {
                _currentGeneration++;
                PerformGenrationIteration();
                if (_blocksGenerationSize == 0 && _blocksCount < _minBlocksCount)
                {
                    ReActivateGeneration();
                }
            }
        }
        else
        {
            Debug.LogError("No base level elements to start brunch generation.");
        }
    }

    public void PerformGenrationIteration()
    {
        _blocksGenerationSize = 0;
        foreach (var block in _levelElementsInOperationalArea
                .Where(blockInfoHolder => !blockInfoHolder.NeighborshipResolved)
                .ToList())
        {
            ResolveNeighborship(block.BlockPosstion);
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

        BlockInfoHolder[] ladderCandidats = _levelElementsInOperationalArea
            .Where(blockInfoHolder => blockInfoHolder.IsRightLeftCoridor
                && !blockInfoHolder.IsLadderNeighbor
                && blockInfoHolder.BlockPosstion.x % (laddersGap + 1) == 0)
            .OrderBy(x => x.BlockPosstion.y)
            .Reverse()
            .Take(maxCandidatsCount)
            .ToArray();

        foreach (var ladderCandidat in ladderCandidats)
        {
            _levelManager.DestroyBlock(ladderCandidat);
            if (!TryBuildLadder(ladderCandidat.BlockPosstion, GenerateLadderHeight()) 
                    && _levelManager.TryGetSuitableBlock(false, false, true, true, out BlockInfoHolder rlBlockReference))
            {
                InstantiateBlock(ladderCandidat.BlockPosstion, rlBlockReference);
            }
        }
    }

    private void RemoveDeadEnds()
    {
        BlockInfoHolder[] deadEnds = _levelElementsInOperationalArea
            .Where(blockInfoHolder => (blockInfoHolder.RightConnected && !blockInfoHolder.LeftConnected) 
            || (!blockInfoHolder.RightConnected && blockInfoHolder.LeftConnected))
            .ToArray();

        foreach (BlockInfoHolder block in deadEnds)
        {
            Vector2Int blockPosition = block.BlockPosstion;
            bool rightConnNecessary = block.RightConnected;
            bool leftConnNecessary = block.LeftConnected;
            _levelManager.DestroyBlock(block);
            _levelManager.TryGetSuitableBlock(false, false, true, true, out BlockInfoHolder freeWayBlockPrefab);
            GenerateBlock(blockPosition, rightConnNecessary, leftConnNecessary);
        }
    }

    private void ResolveNeighborship(Vector2Int position)
    {
        if (_levelManager.TryGetBlockInfoByPosition(position, out BlockInfoHolder currentBlockInfo))
        {
            bool ladderNeighbor = false;
            if (currentBlockInfo.UpConnected || currentBlockInfo.DownConnected)
                ladderNeighbor = true;

            Vector2Int rightBias = new Vector2Int(1 * BlockGridSettings.HorizontalExpandDirectionFactor, 0);
            Vector2Int leftBias = new Vector2Int(-1 * BlockGridSettings.HorizontalExpandDirectionFactor, 0);

            Vector2Int neighborPosition;
            if (currentBlockInfo.RightConnected)
            {
                neighborPosition = position + rightBias;
                if (_operationalArea.IsWithinArea(neighborPosition) && !_levelManager.TryGetBlockInfoByPosition(neighborPosition, out BlockInfoHolder neighbor))
                {
                    GenerateBlock(neighborPosition, false, true, ladderNeighbor);
                }
            }

            if (currentBlockInfo.LeftConnected)
            {
                neighborPosition = position + leftBias;
                if (_operationalArea.IsWithinArea(neighborPosition) && !_levelManager.TryGetBlockInfoByPosition(neighborPosition, out BlockInfoHolder neighbor))
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

        bool positionIsOnVerticalEdge = position.x == BlockGridSettings.MapDimensions.x - 1 || position.x == 0;

        if (!positionIsOnVerticalEdge && !ladderNeighbor && UnityEngine.Random.value < ladderProbability)
        {
            generationDone = TryBuildLadder(position, GenerateLadderHeight());
        }

        if (!generationDone && (UnityEngine.Random.value < coridorEndProbability || positionIsOnVerticalEdge)
                && _levelManager.TryGetSuitableBlock(false, false, rightConnectionNecessary, leftConnectionNecessary, out BlockInfoHolder coridorEndInfo, true))
        {
            InstantiateBlock(position, coridorEndInfo, ladderNeighbor);
            generationDone = true;
        }

        if (!generationDone && _levelManager.TryGetSuitableBlock(false, false, true, true, out BlockInfoHolder blockInfo))
        {
            InstantiateBlock(position, blockInfo, ladderNeighbor);
        }
    }

    private bool TryBuildLadder(Vector2Int position, int ladderHeight, bool forceMod = false)
    {
        int possibleLadderHeight =  CheckIfLadderPossible(position, ladderHeight);
        possibleLadderHeight = forceMod ? Mathf.Clamp(ladderHeight, _minLadderHeight, _maxLadderHeight) : possibleLadderHeight;
        if (possibleLadderHeight >= 2)
        {
            _levelManager.TryGetSuitableBlock(true, false, true, true, out BlockInfoHolder ladderBottomBlockInfo);
            _levelManager.TryGetSuitableBlock(true, true, true, true, out BlockInfoHolder ladderMidwayBlockInfo);
            _levelManager.TryGetSuitableBlock(false, true, true, true, out BlockInfoHolder ladderTopBlockInfo);

            _levelManager.InstantiateOrReplaceBlock(position, ladderBottomBlockInfo);
            _levelManager.InstantiateOrReplaceBlock(position + new Vector2Int(0, possibleLadderHeight - 1), ladderTopBlockInfo);

            for (int i = 1; i < possibleLadderHeight - 1; i++)
            {
                _levelManager.InstantiateOrReplaceBlock(position + new Vector2Int(0, i), ladderMidwayBlockInfo);
            }
            return true;
        }
        else
            return false;
    }

    private int GenerateLadderHeight()
    {
        return (int)((_maxLadderHeight - _minLadderHeight + 1) * UnityEngine.Random.value + _minLadderHeight);
    }

    public void InstantiateBlock(Vector2Int position, BlockInfoHolder blockInfo, bool ladderNeighbor = false)
    {
        _levelManager.InstantiateBlock(position, blockInfo, ladderNeighbor, _currentGeneration);

        BlockInfoHolder newBlockInfoHolder = blockInfo.HollowCopy();
        newBlockInfoHolder.BlockPosstion = position;
        _levelElementsInOperationalArea.Add(newBlockInfoHolder);

        _blocksCount = LevelElements.Count;
        _blocksGenerationSize++;
    }

    private int CheckIfLadderPossible(Vector2Int position, int ladderHeight)
    {
        int freeCells = 0;
        for (int i = 0; i < ladderHeight; i++)
        {
            Vector2Int currentCellPosition = position + new Vector2Int(0, i);
            if (!_operationalArea.IsWithinArea(currentCellPosition)
                    || _levelManager.TryGetBlockInfoByPosition(currentCellPosition, out BlockInfoHolder blockInfoHolder))
                break;
            freeCells++;
        }
        return freeCells;
    }
}
