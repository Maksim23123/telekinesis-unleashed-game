using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ContentPointer
{
    [SerializeField] private Vector2 _relativePosition;

    [SerializeField] private PointerType _pointerType;

    public Vector2 RelativePosition { get => _relativePosition; set => _relativePosition = value; }
    public Vector2 WorldPosition { get; set; }
    public bool IsUsed { get; protected set; } = false;
    public PointerType PointerType { get => _pointerType; set => _pointerType = value; }

    public GameObject ToGameObject(Vector2 centerPosition, GameObject _defaultPointer, Transform _pointerContainer)
    {
        GameObject newPointer = GameObject.Instantiate(_defaultPointer, _pointerContainer);
        newPointer.transform.position = centerPosition + RelativePosition;
        GameObjectPointer enemySpawnGameObjectPointer = newPointer.AddComponent<GameObjectPointer>();
        enemySpawnGameObjectPointer.PointerType = _pointerType;
        return newPointer;
    }

    public void InitWorldPosition(Vector2 containerPosition)
    {
        WorldPosition = containerPosition + RelativePosition;
    }

    public void ActivatePointerAction(Vector2 centerPosition)
    {
        Debug.Log("Summoning enemies");
    }

    public void PerformPointerActionCleanUp()
    {
        Debug.Log("Hiding enemies");
    }
}
