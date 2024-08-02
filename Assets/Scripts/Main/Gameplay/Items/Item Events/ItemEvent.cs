using System;
using UnityEngine;

/// <summary>
/// Base class for item events.
/// Contains parameters and abstract methods for maintaining item event workflow.
/// </summary>
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

    /// <summary>
    /// Executes the action bound to this item event.
    /// </summary>
    public abstract void ExecuteItemEvent();

    /// <summary>
    /// Reverses the consequences of the execution of an action bound to this item.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public virtual void ReverseEventEffect()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Allows unifying the effects of items with the same type.
    /// </summary>
    /// <param name="itemEvent">The item event to unify with.</param>
    /// <returns>True if the union is successful, otherwise false.</returns>
    public abstract bool Union(ItemEvent itemEvent);

    /// <summary>
    /// The condition under which the item event should be executed.
    /// </summary>
    /// <returns>True if the condition is met, otherwise false.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public virtual bool CheckCondition()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Recreates the item event with its status and parameters after the save/load process.
    /// </summary>
    /// <param name="objectData">The data object containing the saved state.</param>
    /// <returns>The recreated item event.</returns>
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
