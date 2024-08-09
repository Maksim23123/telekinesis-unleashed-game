using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class ContentPointer
{
    [SerializeField] private Vector2 _relativePosition;

    public Vector2 RelativePosition { get => _relativePosition; set => _relativePosition = value; }
    public bool IsUsed { get; protected set; } = false;

    public abstract GameObject ToGameObject(Vector2 centerPosition, GameObject _defaultPointer, Transform pointerContainer);

    public abstract void ActivatePointerAction(Vector2 centerPosition);
}
