using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[Serializable]
public abstract class ItemEvent : ScriptableObject
{
    [SerializeField]
    int _eventsCount;

    [SerializeField]
    protected ItemEventType _eventType;

    [NonSerialized]
    bool _eventExecuted;

    [SerializeField]
    protected bool _unionPermission = false;

    public int eventsCount { get => _eventsCount; set => _eventsCount = value; }
    internal ItemEventType EventType { get => _eventType; set => _eventType = value; }
    public bool EventExecuted { get => _eventExecuted; set => _eventExecuted = value; }
    public bool UnionPermission { get => _unionPermission; set => _unionPermission = value; }

    public abstract void ExecuteItemEvent();

    public virtual void ReverseEventEffect()
    {
        throw new NotImplementedException();
    }

    public abstract bool Union(ItemEvent itemEvent);

    public virtual bool CheckCondition()
    {
        throw new NotImplementedException();
    }
}
