using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connection
{
    public Vector2Int RelativePositionInBlockGrid { get; set; }

    public Orientation Orientation { get; set; }

    public ConnectionType ConnectionType { get; set; }

    public GameObject GameObject { get; set; }

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
}


