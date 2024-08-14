using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockStructureData
{
    public GameObject StructurePrefab { get; set; }
    public BlockStructure BlockStructure { get; set; }
    public Vector2Int GridPosition { get; set; }
    public Connection ExitConnection { get; set; }
    public List<Connection> EnteranceConnections { get; private set; } = new();
}
