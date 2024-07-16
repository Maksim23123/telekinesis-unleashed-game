using System;
using UnityEngine;

[Serializable] 
public abstract class ItemEvent : ScriptableObject, IRecordable
{
    [NonSerialized] private int _eventsCount;
    [NonSerialized] private bool _eventExecuted;
    [SerializeField] private bool _unionPermission = false;
    [SerializeField][AssetPath(typeof(ItemEvent))] private string _itemEventPath;
    [SerializeField] protected ItemEventType _eventType;

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

    public static ItemEvent RemakeItemEvent(ObjectData objectData)
    {
        string resourcePath = StaticTools.GetResourcePath(objectData.VariableValues[nameof(_itemEventPath)]);
        ItemEvent itemEvent = Resources.Load<ItemEvent>(resourcePath);
        itemEvent.SetObjectData(objectData);
        return itemEvent;
    }

    public virtual ObjectData GetObjectData()
    {
        ObjectData objectData = new ObjectData();
        objectData.VariableValues.Add(nameof(_eventsCount), _eventsCount.ToString());
        objectData.VariableValues.Add(nameof(_eventExecuted), _eventExecuted.ToString());
        objectData.VariableValues.Add(nameof(_itemEventPath), _itemEventPath);
        return objectData;
    }

    public virtual void SetObjectData(ObjectData objectData)
    {
        int.TryParse(objectData.VariableValues[nameof(_eventsCount)], out _eventsCount);
        bool.TryParse(objectData.VariableValues[nameof(_eventExecuted)], out _eventExecuted);
    }


}
