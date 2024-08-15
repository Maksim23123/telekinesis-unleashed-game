using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] List<BlockInfoHolder> _blocksInfo = new List<BlockInfoHolder>();
    [SerializeField] BlockGridSettings _blockGridSettings;
    [HideInInspector][SerializeField] List<BlockInfoHolder> _levelElements;

    public BlockGridSettings BlockGridSettings { get => _blockGridSettings; set => _blockGridSettings = value; }
    public List<BlockInfoHolder> BlocksInfo { get => _blocksInfo.ToList(); }
    public List<BlockInfoHolder> LevelElements { get => _levelElements.ToList(); }

    // Build and generate

    public void AddPathPart(Vector2Int position, Vector2Int[] neighborsRepresentativePositions)
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
            AddBlock(position, suitableBlockPrefab);
        }
    }

    public GameObject AddCustomBlock(BlockInfoHolder blockInfoHolder, Vector2Int position, bool force = false)
    {
        if (force)
        {
            return AddOrReplaceBlock(position, blockInfoHolder);
        }
        else
        {
            return AddBlock(position, blockInfoHolder);
        }
    }

    public void FillRect(Vector2Int startGridPosition, Vector2Int endGridPosition, BlockInfoHolder blockInfoHolder, bool force = false)
    {
        if (force)
        {
            ExecuteForArea(startGridPosition, endGridPosition, position => AddOrReplaceBlock(position, blockInfoHolder));
        }
        else
        {
            ExecuteForArea(startGridPosition, endGridPosition, position =>
            {
                if (!TryGetBlockInfoByPosition(position, out var _))
                {
                    AddBlock(position, blockInfoHolder);
                }
            });
        }
    }

    public void FillRectWithPlaceholders(Vector2Int startGridPosition, Vector2Int endGridPosition, bool force = false)
    {
        if (TryGetSuitableBlock(false, false, false, false, out BlockInfoHolder placeholder)) {
            FillRect(startGridPosition, endGridPosition, placeholder, force);
        }
        else
        {
            Debug.LogError("Block not found.");
        }
    }

    public GameObject AddBlock(Vector2Int position, BlockInfoHolder blockInfo, bool ladderNeighbor = false, int generation = 0)
    {
        if (!TryGetBlockInfoByPosition(position, out var _))
        {
            
            BlockInfoHolder newBlockInfoHolder = new BlockInfoHolder(blockInfo.BlockPrefab, position);
            newBlockInfoHolder.SetConnections(blockInfo.GetConnections());
            newBlockInfoHolder.Tags = blockInfo.Tags;
            newBlockInfoHolder.IsLadderNeighbor = ladderNeighbor;

            newBlockInfoHolder.Generation = generation;

            _levelElements.Add(newBlockInfoHolder);
            return null;
        }
        else
        {
            Debug.LogWarning("Trying to add block in a filled cell: " + position);
            return null;
        }
    }

    public GameObject AddOrReplaceBlock(Vector2Int position, BlockInfoHolder newBlockPrefab)
    {
        if (TryGetBlockInfoByPosition(position, out BlockInfoHolder blockToReplace))
        {
            DestroyBlock(blockToReplace);
        }
        return AddBlock(position, newBlockPrefab);
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

    public bool TryGetSuitableBlock(string tag, out BlockInfoHolder blockInfoHolder, bool deadEnd = false)
    {
        blockInfoHolder = null;
        List<BlockInfoHolder> suitableBlocks = _blocksInfo.Where(x => x.Tags.Contains(tag)).ToList();
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

    public void DestroyBlocksInArea(Vector2Int startGridPosition, Vector2Int endGridPosition)
    {
        ExecuteForArea(startGridPosition, endGridPosition, position => TryDestroyBlockByPosition(position));
    }

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
            if (block.Instantiated)
            {
                block.Destroy();
            }
            
            _levelElements.Remove(block);
        }
    }

    public void ClearLevel()
    {
        foreach (BlockInfoHolder levelElement in _levelElements)
        {
            if (levelElement.Instantiated)
            {
                levelElement.Destroy();
            }
        }
        _levelElements.Clear();
    }

    //General
    public void ExecuteForArea(Vector2Int startGridPosition, Vector2Int endGridPosition, Action<Vector2Int> action)
    {
        Vector2Int affectedAreaSizes = endGridPosition - startGridPosition; // get sizes of area that will be filled
        Vector2Int signs = new Vector2Int(affectedAreaSizes.x < 0 ? -1 : 1, affectedAreaSizes.y < 0 ? -1 : 1); // Extract vector direction. Values on both axis from 1 to -1
        affectedAreaSizes += signs; // Corect each axis of vector by 1 or -1
        for (int i = 0; i < Mathf.Abs(affectedAreaSizes.x); i++)
        {
            for (int j = 0; j < Mathf.Abs(affectedAreaSizes.y); j++)
            {
                Vector2Int currentBlockPossition = startGridPosition + new Vector2Int(i, j) * signs; // find absolute position in grid than rotate it to primal direction 
                action(currentBlockPossition);
            }
        }
    }
}
