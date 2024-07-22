using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BlockInfoHolder
{
    [SerializeField]
    private string _name;
    [SerializeField]
    GameObject _block;
    [SerializeField]
    private bool _upConnected, _downConnected, _rightConnected, _leftConnected, _deadEnd;
    [SerializeField]
    private List<string> _tags;

    [HideInInspector]
    [SerializeField]
    Vector2Int _blockPosstion;

    [HideInInspector]
    [SerializeField]
    bool _neighborshipResolved;

    [HideInInspector]
    [SerializeField]
    bool _isLadderNeighbor = false;

    [HideInInspector]
    [SerializeField]
    int _generation;

    public BlockInfoHolder(GameObject block, Vector2Int blockPosstion)
    {
        _block = block;
        _blockPosstion = blockPosstion;
    }

    public bool UpConnected { get => _upConnected; }
    public bool DownConnected { get => _downConnected; }
    public bool RightConnected { get => _rightConnected; }
    public bool LeftConnected { get => _leftConnected; }
    public GameObject Block { get => _block; }
    public Vector2Int BlockPosstion { get => _blockPosstion; set => _blockPosstion = value; }
    public bool NeighborshipResolved { get => _neighborshipResolved; set => _neighborshipResolved = value; }
    public bool IsLadderNeighbor { get => _isLadderNeighbor; set => _isLadderNeighbor = value; }
    public int Generation { get => _generation; set => _generation = value; }
    public bool DeadEnd { get => _deadEnd; set => _deadEnd = value; }
    public string Name { get => _name;}
    public List<string> Tags { get => _tags; set => _tags = value; }

    /// <summary>
    /// Returns information about connections of a block in form of 
    /// bitmask in that order: UP DOWN RIGHT LEFT IS_DEAD_END
    /// </summary>
    /// <returns></returns>
    public int GetConnections()
    {
        int connections = 0;

        if (LeftConnected)
            connections |= 1 << 0;

        if (RightConnected)
            connections |= 1 << 1;

        if (DownConnected)
            connections |= 1 << 2;

        if (UpConnected)
            connections |= 1 << 3;

        if (DeadEnd)
            connections |= 1 << 4;

        string binaryRepresentation = Convert.ToString(connections, 2).PadLeft(8, '0');

        return connections;
    }

    /// <summary>
    /// Applys information about connections of a block in form of 
    /// bitmask in that order: UP DOWN RIGHT LEFT IS_DEAD_END
    /// </summary>
    /// <param name="connections"></param>
    public void SetConnections(int connections)
    {
        _leftConnected = (connections & (1 << 0)) != 0; 
        _rightConnected = (connections & (1 << 1)) != 0; 
        _downConnected = (connections & (1 << 2)) != 0; 
        _upConnected = (connections & (1 << 3)) != 0; 
        _deadEnd = (connections & (1 << 4)) != 0;
    }
}
