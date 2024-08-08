using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ContentPointer
{
    [SerializeField] private Vector2 _relativePosition;

    public Vector2 RelativePosition { get => _relativePosition; set => _relativePosition = value; }
    public bool IsUsed { get; protected set; } = false;
    public Type RealType { get; protected set; }

    public virtual GameObject ToGameObject(Vector2 centerPosition)
    {
        throw new NotImplementedException();
    }

    public virtual void ActivatePointerAction(Vector2 centerPosition)
    {
        throw new NotImplementedException();
    }
}
