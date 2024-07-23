using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;

public class Connection
{
    public Vector2Int RelativePositionInBlockGrid { get; set; }

    public Orientation Orientation { get; set; }

    public ConnectionType ConnectionType { get; set; }

    public GameObject GameObject { get; set; }

    public Vector2Int SealedZoneStart { get; private set; }

    public Vector2Int SealedZoneEnd { get; private set; } 

    public bool SealedZoneParametersInitialized { get; private set; } = false;

    public BlockStructure AttachedStructure { get; set; }

    public Vector2Int GetConnectionPoint(BlockGridSettings blockGridSettings)
    {
        if (Orientation == Orientation.Left)
        {
            return blockGridSettings.WorldToGridPosition(GameObject.transform.position) 
                + Vector2Int.left * blockGridSettings.HorizontalExpandDirectionFactor * -1;
        }
        return blockGridSettings.WorldToGridPosition(GameObject.transform.position) 
            + Vector2Int.right * blockGridSettings.HorizontalExpandDirectionFactor * -1;
    }

    public void InitSealedAreaParameters(BlockGridSettings blockGridSettings, Placement expandDirection)
    {
        SealedZoneParametersInitialized = true;

        SealedZoneStart = GetConnectionPoint(blockGridSettings);
        
        Vector2Int sealedZoneEndOffset = Vector2Int.zero;
        if (Orientation == Orientation.Right)
        {
            sealedZoneEndOffset += Vector2Int.left * blockGridSettings.HorizontalExpandDirectionFactor;
        }
        else
        {
            sealedZoneEndOffset += Vector2Int.right * blockGridSettings.HorizontalExpandDirectionFactor;
        }

        if (expandDirection == Placement.Above)
        {
            sealedZoneEndOffset += Vector2Int.up;
        }
        else
        {
            sealedZoneEndOffset += Vector2Int.down;
        }
        SealedZoneEnd = SealedZoneStart + sealedZoneEndOffset;
    }
}


