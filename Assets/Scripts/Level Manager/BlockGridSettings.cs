using System;
using UnityEngine;

[Serializable]
public class BlockGridSettings
{
    [SerializeField] Vector2Int _mapDimensions, _blocksSize, _possitionBias;

    [Range(-1, 1)]
    [SerializeField]
    private int _horizontalExpandDirectionFactor;

    public Vector2Int MapDimensions { get => _mapDimensions; set => _mapDimensions = value; }
    public Vector2Int BlocksSize { get => _blocksSize; set => _blocksSize = value; }
    public Vector2Int PossitionBias { get => _possitionBias; set => _possitionBias = value; }
    public int HorizontalExpandDirectionFactor 
    {
        get
        {
            if (_horizontalExpandDirectionFactor == 0)
            {
                _horizontalExpandDirectionFactor = 1;
            }

            return _horizontalExpandDirectionFactor;
        } 
    }

    public Vector2Int WorldToGridPosition(Vector2 worldPosition)
    {
        // horizontalCorrectionBias will become Vector(0, 0) if HorizontalExpandDirectionFactor is greater than 0
        Vector2 horizontalCorrectionBias = Vector2.left * Mathf.Clamp(HorizontalExpandDirectionFactor * -1, 0, 1); 

        Vector2 blockGridPosition = worldPosition - (PossitionBias - BlocksSize / 2
            * new Vector2(HorizontalExpandDirectionFactor, 1)) + horizontalCorrectionBias;

        Vector2Int gridPosition = new Vector2Int((int)Mathf.Floor(blockGridPosition.x / BlocksSize.x
                * HorizontalExpandDirectionFactor)
            , (int)Mathf.Floor(blockGridPosition.y / BlocksSize.y));

        return gridPosition;
    }

}
