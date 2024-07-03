using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    List<BlockInfoHolder> _blocksInfo = new List<BlockInfoHolder>();

    [SerializeField]
    GameObject _grid;

    [SerializeField]
    BlockGridSettings _blockGridSettings;

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

    [SerializeField]
    private int _minBlocksCount;

    [SerializeField]
    int _maxLadderHeight = 4;
    [SerializeField]
    int _minLadderHeight = 2;

    [Header("Info")]

    [ReadOnly]
    [SerializeField]
    private int _blocksCount;

    [ReadOnly]
    [SerializeField]
    private int _blocksGenerationSize;



    private int _currentGeneration;

    public BlockGridSettings BlockGridSettings { get => _blockGridSettings; set => _blockGridSettings = value; }

    public void Generate()
    {
        //ClearLevel();

        Vector2Int startPosition = new Vector2Int((int)(_blockGridSettings.MapDimensions.x * _horizontalGenerationStartPosition - _horizontalGenerationStartPosition), 0);

        if (TryGetSuitableBlock(false, false, true, true, out BlockInfoHolder corridorPrefab))
        {
            _currentGeneration = 0;
            InstantiateOrReplaceBlock(startPosition, corridorPrefab);

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


    public void PerformGenrationIteration()
    {
        _blocksGenerationSize = 0;
        foreach (var block in _levelElements.Where(x => !x.NeighborshipResolved).ToList())
        {
            ResolveNeighborship(block.BlockPosstion);
        }
    }

    private void ReActivateGeneration()
    {
        PopulateWithLadders();
        RemoveDeadEnds();
    }

    // Finalizing generation

    private void PopulateWithLadders()
    {
        int maxCandidatsCount = 10;
        int laddersGap = 2;

        BlockInfoHolder[] ladderCandidats = _levelElements
            .Where(g => CheckIfRightLeftCorridor(g)
                && !g.IsLadderNeighbor && g.BlockPosstion.x < _blockGridSettings.MapDimensions.x 
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

    private void SealHolesInCorridors()
    {
        Vector2Int rightBias = new Vector2Int(1 * _blockGridSettings.HorizontalExpandDirectionFactor, 0);
        Vector2Int leftBias = new Vector2Int(-1 * _blockGridSettings.HorizontalExpandDirectionFactor, 0);

        Func<BlockInfoHolder, bool> checkRightNeighbor = x
            => TryGetBlockInfoByPosition(x.BlockPosstion + rightBias, out var _);
        Func<BlockInfoHolder, bool> checkLeftNeighbor = x
            => TryGetBlockInfoByPosition(x.BlockPosstion + leftBias, out var _);

        TryGetSuitableBlock(false, false, true, false, out BlockInfoHolder rightConnectedDeadEndPrefab, true);
        TryGetSuitableBlock(false, false, false, true, out BlockInfoHolder leftConnectedDeadEndPrefab, true);

        SealUnfinishedRLBlocks(checkRightNeighbor, checkLeftNeighbor, rightConnectedDeadEndPrefab, leftConnectedDeadEndPrefab);

        SealGapsInLadders(rightBias, leftBias, checkRightNeighbor, checkLeftNeighbor, rightConnectedDeadEndPrefab, leftConnectedDeadEndPrefab);
        TransformOneSidedDeadEndsIntoTwoSided(rightBias, leftBias);
    }

    private void TransformOneSidedDeadEndsIntoTwoSided(Vector2Int rightBias, Vector2Int leftBias)
    {
        BlockInfoHolder[] deadEndsToBecomeRLConnected = _levelElements
                    .Where(x => x.DeadEnd && (x.RightConnected ^ x.LeftConnected)) // Serch for regular dead ends
                    .ToArray();

        foreach (BlockInfoHolder regularDeadEnd in deadEndsToBecomeRLConnected)
        {
            if (TryGetSuitableBlock(false, false, true, true, out BlockInfoHolder twoSidedDeadEndPrefab, true)
                    && TryGetBlockInfoByPosition(regularDeadEnd.BlockPosstion + rightBias, out var rightNeighbor) && rightNeighbor.LeftConnected
                    && TryGetBlockInfoByPosition(regularDeadEnd.BlockPosstion + leftBias, out var leftNeighbor) && leftNeighbor.RightConnected)
            {
                DestroyBlock(regularDeadEnd);
                InstantiateBlock(regularDeadEnd.BlockPosstion, twoSidedDeadEndPrefab);
            }
        }
    }

    private void SealGapsInLadders(Vector2Int rightBias, Vector2Int leftBias, Func<BlockInfoHolder, bool> checkRightNeighbor
            , Func<BlockInfoHolder, bool> checkLeftNeighbor, BlockInfoHolder rightConnectedDeadEndPrefab, BlockInfoHolder leftConnectedDeadEndPrefab)
    {
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
    }

    private void SealUnfinishedRLBlocks(Func<BlockInfoHolder, bool> checkRightNeighbor, Func<BlockInfoHolder, bool> checkLeftNeighbor
            , BlockInfoHolder rightConnectedDeadEndPrefab, BlockInfoHolder leftConnectedDeadEndPrefab)
    {
        BlockInfoHolder[] corridorsToSeal = _levelElements
                    .GroupBy(x => x.Generation)
                    .OrderBy(x => x.Key)
                    .Where(x => x.Key == _currentGeneration)
                    .SelectMany(x => x)
                    .Where(x => CheckIfRightLeftCorridor(x) && (!checkRightNeighbor(x) ^ !checkLeftNeighbor(x)))
                    .ToArray();

        foreach (var corridor in corridorsToSeal)
        {
            DestroyBlock(corridor);
            if (checkRightNeighbor(corridor))
                InstantiateBlock(corridor.BlockPosstion, rightConnectedDeadEndPrefab);
            else
                InstantiateBlock(corridor.BlockPosstion, leftConnectedDeadEndPrefab);
        }
    }

    // Build and generate

    public void InstantiateCustomBlock(GameObject gameObject, Vector2Int position)
    {
        BlockInfoHolder customBlock = new BlockInfoHolder(gameObject, position);
        InstantiateBlock(position, customBlock);
    }

    public void FillRectWithPlaceholders(Vector2Int startGridPosition, Vector2Int endGridPosition, bool force = false)
    {
        if (TryGetSuitableBlock(false, false, false, false, out BlockInfoHolder placeholder)) {
            Vector2Int fillAreaSizes = endGridPosition - startGridPosition; // get sizes of area that will be filled
            Vector2Int signs = new Vector2Int(fillAreaSizes.x < 0 ? -1 : 1, fillAreaSizes.y < 0 ? -1 : 1); // Extract vector direction. Values on both axis from 1 to -1
            fillAreaSizes += signs; // Corect each axis of vector by 1 or -1
            for (int i = 0; i < Mathf.Abs(fillAreaSizes.x); i++)
            {
                for (int j = 0; j < Mathf.Abs(fillAreaSizes.y); j++)
                {
                    Vector2Int currentBlockPossition = startGridPosition + new Vector2Int(i, j) * signs // find absolute position in grid than rotate it to primal direction 
                        /**/; /*/+ new Vector2Int(Mathf.Clamp(_blockGridSettings.HorizontalExpandDirectionFactor, -1, 0), 0) * signs.x;/**/ // Add additional correction to mathch Horizontal expand direction
                    Debug.Log(new Vector2Int(i, j) * signs);
                    if (force)
                    {
                        InstantiateOrReplaceBlock(currentBlockPossition, placeholder);
                    }
                    else if (!TryGetBlockInfoByPosition(currentBlockPossition, out var _))
                    {
                        InstantiateBlock(currentBlockPossition, placeholder);
                    }
                }
            }
        }
        else
        {
            Debug.LogError("Block not found.");
        }
    }

    public void BuildHorizontalPath(int horizontalStartPositionInGrid, int horizontalEndPositionInGrid, int heightInGrid)
    {
        int currentHorizontalPosition = horizontalEndPositionInGrid < horizontalStartPositionInGrid ? horizontalEndPositionInGrid 
            : horizontalStartPositionInGrid; // Find start position (The smallest number)

        TryGetSuitableBlock(false, false, true, true, out BlockInfoHolder rlBlockPrefab);
        for (int i = 0; i < Mathf.Abs(horizontalEndPositionInGrid - horizontalStartPositionInGrid) + 1; i++ )
        {
            Vector2Int currentPosition = new Vector2Int(currentHorizontalPosition + i, heightInGrid);
            if (!TryGetBlockInfoByPosition(currentPosition, out BlockInfoHolder obstacle))
            {
                InstantiateBlock(currentPosition, rlBlockPrefab);
            }
            else if (obstacle.DeadEnd)
            {
                DestroyBlock(obstacle);
                InstantiateBlock(currentPosition, rlBlockPrefab);
            }
        }
    }

    public void BuildVerticalPath(int verticalStartPositionInGrid, int verticalEndPositionInGrid, int horizontalStartPosition, bool expandToRight)
    {
        int verticalDirectionFactor = 0;
        

        if (verticalStartPositionInGrid != verticalEndPositionInGrid)
        {
            verticalDirectionFactor = Mathf.Clamp(verticalEndPositionInGrid - verticalStartPositionInGrid, -1, 1); 
        }
        else
        {
            return;
        }

        int ladderHeight = Mathf.Abs(verticalEndPositionInGrid - verticalStartPositionInGrid);
        int horizExpandDirectionFactor = _blockGridSettings.HorizontalExpandDirectionFactor;
        int ladderTransitionHorizontalPosition = horizontalStartPosition + (expandToRight ? horizExpandDirectionFactor : horizExpandDirectionFactor * -1);
        int currentHorizontalPosition = horizontalStartPosition;
        int currentVerticalPosition = verticalStartPositionInGrid;

        TryGetSuitableBlock(false, false, true, true, out BlockInfoHolder ladderTransitionPrefab);

        Action<Vector2Int> addTransition = (position) =>
        {
            if (TryGetBlockInfoByPosition(position, out BlockInfoHolder obstacle))
            {
                if (obstacle != null && obstacle.DeadEnd)
                {
                    InstantiateOrReplaceBlock(position, ladderTransitionPrefab);
                }
            }
            else
            {
                InstantiateBlock(position, ladderTransitionPrefab);
            }
        };

        int iterator = 0;
        ladderHeight++;
        while (iterator < ladderHeight)
        {
            int currentLadderHeight = Mathf.Clamp(Mathf.Abs(ladderHeight - iterator), 0, _maxLadderHeight);
            if (verticalDirectionFactor < 0)
            {
                currentVerticalPosition -= currentLadderHeight + -1;
            }
            Vector2Int currentBlockPosition = new Vector2Int(currentHorizontalPosition, currentVerticalPosition);

            int itteratorIncreacement = currentLadderHeight - 1;
            if (itteratorIncreacement > 0)
            {
                TryBuildLadder(currentBlockPosition, currentLadderHeight, true);
                iterator += itteratorIncreacement;
            }
            else
            {
                iterator++;
            }

            if (verticalDirectionFactor > 0)
            {
                currentVerticalPosition += currentLadderHeight - 1;
            }

            if (currentLadderHeight == _maxLadderHeight && iterator < ladderHeight - 1)
            {
                addTransition(new Vector2Int(ladderTransitionHorizontalPosition, currentVerticalPosition));
                if (currentHorizontalPosition != horizontalStartPosition)
                {
                    currentHorizontalPosition = horizontalStartPosition;
                }
                else
                {
                    currentHorizontalPosition += 2 * (expandToRight ? horizExpandDirectionFactor : horizExpandDirectionFactor * -1);
                }
            }
            else if (iterator >= ladderHeight - 1 && currentHorizontalPosition != horizontalStartPosition)
            {
                addTransition(new Vector2Int(ladderTransitionHorizontalPosition, currentVerticalPosition));
                InstantiateOrReplaceBlock(new Vector2Int(horizontalStartPosition, currentVerticalPosition), ladderTransitionPrefab);
            }
        }
    }

    private void ResolveNeighborship(Vector2Int position)
    {
        if (TryGetBlockInfoByPosition(position, out BlockInfoHolder currentBlockInfo))
        {
            bool ladderNeighbor = false;
            if (currentBlockInfo.UpConnected || currentBlockInfo.DownConnected)
                ladderNeighbor = true;

            Vector2Int rightBias = new Vector2Int(1 * _blockGridSettings.HorizontalExpandDirectionFactor, 0);
            Vector2Int leftBias = new Vector2Int(-1 * _blockGridSettings.HorizontalExpandDirectionFactor, 0);

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

        bool positionIsOnVerticalEdge = position.x == _blockGridSettings.MapDimensions.x - 1 || position.x == 0;

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
        return (int)((_maxLadderHeight - _minLadderHeight + 1) * UnityEngine.Random.value + _minLadderHeight);
    }

    private void InstantiateBlock(Vector2Int position, BlockInfoHolder blockInfo, bool ladderNeighbor = false)
    {
        if (!TryGetBlockInfoByPosition(position, out var _))
        {
            GameObject currentBlock = Instantiate(blockInfo.Block, _grid.transform);
            currentBlock.transform.position = new Vector2(_blockGridSettings.HorizontalExpandDirectionFactor * _blockGridSettings.BlocksSize.x 
                * position.x + _blockGridSettings.PossitionBias.x, _blockGridSettings.BlocksSize.y * position.y + _blockGridSettings.PossitionBias.y);

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

    private void InstantiateOrReplaceBlock(Vector2Int position, BlockInfoHolder newBlockPrefab)
    {
        if (TryGetBlockInfoByPosition(position, out BlockInfoHolder blockToReplace))
        {
            DestroyBlock(blockToReplace);
        }
        InstantiateBlock(position, newBlockPrefab);
    }

    private bool TryBuildLadder(Vector2Int position, int ladderHeight, bool forceMod = false)
    {
        int possibleLadderHeight = CheckIfLadderPossible(position, ladderHeight);
        possibleLadderHeight = forceMod ? Mathf.Clamp(ladderHeight, _minLadderHeight, _maxLadderHeight) : possibleLadderHeight;
        if (possibleLadderHeight >= 2)
        {
            TryGetSuitableBlock(true, false, true, true, out BlockInfoHolder ladderBottomBlockInfo);
            TryGetSuitableBlock(true, true, true, true, out BlockInfoHolder ladderMidwayBlockInfo);
            TryGetSuitableBlock(false, true, true, true, out BlockInfoHolder ladderTopBlockInfo);

            InstantiateOrReplaceBlock(position, ladderBottomBlockInfo);
            InstantiateOrReplaceBlock(position + new Vector2Int(0, possibleLadderHeight - 1), ladderTopBlockInfo);

            for (int i = 1; i < possibleLadderHeight - 1; i++)
            {
                InstantiateOrReplaceBlock(position + new Vector2Int(0, i), ladderMidwayBlockInfo);
            }
            return true;
        }
        else
            return false;
    }

    // Info and tests

    private bool CheckIfRightLeftCorridor(BlockInfoHolder blockInfoHolder)
    {
        return !blockInfoHolder.UpConnected && !blockInfoHolder.DownConnected
            && blockInfoHolder.LeftConnected && blockInfoHolder.RightConnected && !blockInfoHolder.DeadEnd;
    }

    private int CheckIfLadderPossible(Vector2Int position, int ladderHeight)
    {
        int freeCells = 0;
        for (int i = 0; i < ladderHeight; i++)
        {
            Vector2Int currentCellPosition = position + new Vector2Int(0, i);
            if (TryGetBlockInfoByPosition(currentCellPosition, out BlockInfoHolder blockInfoHolder))
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
        return position.x >= 0 && position.y >= 0 && position.x < _blockGridSettings.MapDimensions.x && position.y < _blockGridSettings.MapDimensions.y;
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

    //Destroying

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
}
