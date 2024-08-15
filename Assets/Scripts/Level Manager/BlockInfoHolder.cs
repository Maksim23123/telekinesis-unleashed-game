using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BlockInfoHolder
{
    [SerializeField] private string _name;
    [SerializeField] private GameObject _blockPrefab;
    [SerializeField] private bool _upConnected, _downConnected, _rightConnected, _leftConnected, _deadEnd;
    [SerializeField] private List<string> _tags;

    // These fields are serialized but hidden in the inspector so that
    // the information won't be lost after restarting Unity.
    [SerializeField][HideInInspector] private Vector2Int _blockPosstion;
    [SerializeField][HideInInspector] private bool _neighborshipResolved;
    [SerializeField][HideInInspector] private bool _isLadderNeighbor = false;
    [SerializeField][HideInInspector] private int _generation;
    [SerializeField][HideInInspector] private GameObject _instanceInScene;

    public bool UpConnected { get => _upConnected; }
    public bool DownConnected { get => _downConnected; }
    public bool RightConnected { get => _rightConnected; }
    public bool LeftConnected { get => _leftConnected; }
    public GameObject BlockPrefab { get => _blockPrefab; }
    public Vector2Int BlockPosstion { get => _blockPosstion; set => _blockPosstion = value; }
    public bool NeighborshipResolved { get => _neighborshipResolved; set => _neighborshipResolved = value; }
    public bool IsLadderNeighbor { get => _isLadderNeighbor; set => _isLadderNeighbor = value; }
    public int Generation { get => _generation; set => _generation = value; }
    public bool DeadEnd { get => _deadEnd; set => _deadEnd = value; }
    public string Name { get => _name; }
    public List<string> Tags { get => _tags; set => _tags = value; }
    public bool IsRightLeftCoridor
    {
        get
        {
            return !UpConnected && !DownConnected && LeftConnected && RightConnected && !DeadEnd;
        }
    }
    public bool Instantiated
    {
        get => _instanceInScene != null;
    }
    public bool IsActive
    {
        get
        {
            if (_instanceInScene != null)
            {
                return _instanceInScene.activeInHierarchy;
            }
            return false;
        }

        set
        {
            if (_instanceInScene != null)
            {
                _instanceInScene.SetActive(value);
            }
            else
            {
                Debug.LogError("Trying to activate BlockInfoHolder without instance.");
            }
        }
    }

    public BlockInfoHolder(GameObject blockPrefab, Vector2Int blockPosstion)
    {
        _blockPrefab = blockPrefab;
        _blockPosstion = blockPosstion;
    }

    public void InstantiatePrefab(BlockGridSettings blockGridSettings, Transform blockContainer = null, bool active = false)
    {
        if (_instanceInScene == null)
        {
            if (blockContainer != null)
            {
                _instanceInScene = GameObject.Instantiate(_blockPrefab, blockContainer);
            }
            else
            {
                _instanceInScene = GameObject.Instantiate(_blockPrefab);
            }
            
            _instanceInScene.transform.position = new Vector2(blockGridSettings.HorizontalExpandDirectionFactor 
                * blockGridSettings.BlocksSize.x * BlockPosstion.x + blockGridSettings.PossitionBias.x
                , blockGridSettings.BlocksSize.y * BlockPosstion.y + blockGridSettings.PossitionBias.y);
            _instanceInScene.SetActive(active);
        }
        else
        {
            Debug.LogError("Trying to instantiate block prefab second time.");
        }
    }

    public void Destroy()
    {
        if (Instantiated)
        {
            GameObject.DestroyImmediate(_instanceInScene);
        }
        else
        {
            Debug.LogError("Trying to destroy unexisting instance.");
        }
    }

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

    public BlockInfoHolder HollowCopy()
    {
        BlockInfoHolder newBlockInfoHolder = new BlockInfoHolder(null, BlockPosstion);
        newBlockInfoHolder.SetConnections(GetConnections());
        newBlockInfoHolder.Tags = Tags;
        newBlockInfoHolder.IsLadderNeighbor = IsLadderNeighbor;
        newBlockInfoHolder.Generation = Generation;

        return newBlockInfoHolder;
    }
}
