using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapLoader : MonoBehaviour
{
    [SerializeField] LevelManager _levelManager;
    [SerializeField] Transform BlockContainer;
    [SerializeField] float _renderDistanceInBlocks;

    private List<BlockInfoHolder> _renderedLevelElements = new();
    private Vector2Int _previousPlayerPosition = Vector2Int.zero;

    private void FixedUpdate()
    {
        if (CheckIfPlayerMoved())
        {
            UpdateRenderedObjects();
        }
    }

    private bool CheckIfPlayerMoved()
    {
        bool output = false;
        if (PlayerStatusInformer.PlayerGameObject != null)
        {
            Vector2Int currentPlayerPosition = _levelManager.BlockGridSettings
                .WorldToGridPosition(PlayerStatusInformer.PlayerGameObject.transform.position);

            if (currentPlayerPosition != _previousPlayerPosition)
            {
                output = true;
            }
            _previousPlayerPosition = currentPlayerPosition;
        }
        return output;
    }

    private void UpdateRenderedObjects()
    {
        List<BlockInfoHolder> elementsToRender = _levelManager.LevelElements
            .Where(blockInfoHolder => Vector2Int.Distance(blockInfoHolder.BlockPosstion, _previousPlayerPosition) 
                < _renderDistanceInBlocks && !_renderedLevelElements.Contains(blockInfoHolder))
            .ToList();

        foreach (BlockInfoHolder blockInfoHolder in elementsToRender)
        {
            ActivateBlock(blockInfoHolder);
        }

        List<BlockInfoHolder> elementsToHide = _renderedLevelElements
            .Where(blockInfoHolder => Vector2Int.Distance(blockInfoHolder.BlockPosstion, _previousPlayerPosition)
                > _renderDistanceInBlocks)
            .ToList();

        foreach (BlockInfoHolder blockInfoHolder in elementsToHide)
        {
            DeactivateBlock(blockInfoHolder);
        }
    }

    private void ActivateBlock(BlockInfoHolder blockInfoHolder)
    {
        if (!blockInfoHolder.Instantiated)
        {
            blockInfoHolder.InstantiatePrefab(_levelManager.BlockGridSettings, BlockContainer, active: true);
        }
        else
        {
            blockInfoHolder.IsActive = true;
        }

        _renderedLevelElements.Add(blockInfoHolder);
    }

    private void DeactivateBlock(BlockInfoHolder blockInfoHolder)
    {
        if (blockInfoHolder.Instantiated)
        {
            blockInfoHolder.IsActive = false;
            _renderedLevelElements.Remove(blockInfoHolder);
        }
        else
        {
            _renderedLevelElements.Clear();
            UpdateRenderedObjects();
        }
    }
}
