using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] List<BlockInfoHolder> _blocksInfo = new List<BlockInfoHolder>();
    [SerializeField] GameObject _grid;
    [SerializeField] BlockGridSettings _blockGridSettings;
    [HideInInspector][SerializeField] List<BlockInfoHolder> _levelElements;

    public BlockGridSettings BlockGridSettings { get => _blockGridSettings; set => _blockGridSettings = value; }
    public List<BlockInfoHolder> BlocksInfo { get => _blocksInfo.ToList(); }
    public List<BlockInfoHolder> LevelElements { get => _levelElements.ToList(); }

    // Build and generate

    public void BuildPathPart(Vector2Int position, Vector2Int[] neighborsRepresentativePositions)
    {
        bool upperConnection = false;
        bool lowerConnection = false;

        if (neighborsRepresentativePositions.Any(g => g.y < position.y))
        {
            lowerConnection = true;
        }

        if (neighborsRepresentativePositions.Any(g => g.y > position.y))
        {
            upperConnection = true;
        }

        if (TryGetSuitableBlock(upperConnection, lowerConnection, true, true, out BlockInfoHolder suitableBlockPrefab))
        {
            InstantiateBlock(position, suitableBlockPrefab);
        }
    }

    public void InstantiateCustomBlock(BlockInfoHolder blockInfoHolder, Vector2Int position, bool force = false)
    {
        if (force)
        {
            InstantiateOrReplaceBlock(position, blockInfoHolder);
        }
        else
        {
            InstantiateBlock(position, blockInfoHolder);
        }
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
                    Vector2Int currentBlockPossition = startGridPosition + new Vector2Int(i, j) * signs; // find absolute position in grid than rotate it to primal direction 
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
    /*
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
    */

    public void InstantiateBlock(Vector2Int position, BlockInfoHolder blockInfo, bool ladderNeighbor = false, int generation = 0)
    {
        if (!TryGetBlockInfoByPosition(position, out var _))
        {
            GameObject currentBlock = Instantiate(blockInfo.Block, _grid.transform);
            currentBlock.transform.position = new Vector2(_blockGridSettings.HorizontalExpandDirectionFactor * _blockGridSettings.BlocksSize.x 
                * position.x + _blockGridSettings.PossitionBias.x, _blockGridSettings.BlocksSize.y * position.y + _blockGridSettings.PossitionBias.y);

            BlockInfoHolder newBlockInfoHolder = new BlockInfoHolder(currentBlock, position);
            newBlockInfoHolder.SetConnections(blockInfo.GetConnections());
            newBlockInfoHolder.IsLadderNeighbor = ladderNeighbor;

            newBlockInfoHolder.Generation = generation;

            _levelElements.Add(newBlockInfoHolder);
        }
        else
        {
            Debug.LogWarning("Trying to and block in a filled cell.");
        }
    }

    public void InstantiateOrReplaceBlock(Vector2Int position, BlockInfoHolder newBlockPrefab)
    {
        if (TryGetBlockInfoByPosition(position, out BlockInfoHolder blockToReplace))
        {
            DestroyBlock(blockToReplace);
        }
        InstantiateBlock(position, newBlockPrefab);
    }
    

    // Info and tests

    public bool TryGetSuitableBlock(bool up, bool down, bool right, bool left, out BlockInfoHolder blockInfoHolder, bool deadEnd = false)
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

    public bool CheckPosition(Vector2Int position)
    {
        return position.x >= 0 && position.y >= 0 && position.x < _blockGridSettings.MapDimensions.x && position.y < _blockGridSettings.MapDimensions.y;
    }

    public bool TryGetBlockInfoByPosition(Vector2Int position, out BlockInfoHolder blockInfoHolder)
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

    public bool TryDestroyBlockByPosition(Vector2Int position)
    {
        if (TryGetBlockInfoByPosition(position, out BlockInfoHolder blockInfoHolder))
        {
            DestroyBlock(blockInfoHolder);
            return true;
        }
        return false;
    }

    public void DestroyBlock(BlockInfoHolder block)
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
