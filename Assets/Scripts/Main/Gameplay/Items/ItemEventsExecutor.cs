using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Class that receives ItemEvent instances and executes their actions.
/// This class is a singleton, so it should be attached to only one object in the scene.
/// </summary>
public class ItemEventsExecutor : MonoBehaviour, IRecordable
{
    private List<ItemEventSlot> _itemEventSlots = new List<ItemEventSlot>();
    private static ItemEventsExecutor _instance;
    
    public int Priority => 0;

    /// <summary>
    /// Provides access to the single instance of this class.
    /// </summary>
    public static ItemEventsExecutor Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject newGameObject = new GameObject("Item Events Executor");
                newGameObject.AddComponent<ItemEventsExecutor>();
            }

            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }

    private void FixedUpdate()
    {
        Func<ItemEvent, bool> isConditional = (ItemEvent x) => x.EventType == ItemEventType.Conditional || x.EventType == ItemEventType.ConditionalOneTime;
        Func<ItemEvent, bool> isOneTime = (ItemEvent x) => x.EventType == ItemEventType.OneTime || x.EventType == ItemEventType.ConditionalOneTime;

        foreach (var itemEventSlot in _itemEventSlots.Where(x => !(isOneTime(x.ItemEvent) && x.ItemEvent.EventExecuted)))
        {
            ItemEvent currentItemEvent = itemEventSlot.ItemEvent;
            if ((isConditional(currentItemEvent) && currentItemEvent.CheckCondition())
                    || currentItemEvent.EventType == ItemEventType.OneTime)
            {
                currentItemEvent.ExecuteItemEvent();
            }
        }
    }

    /// <summary>
    /// Registers an ItemEvent instance with this class.
    /// </summary>
    /// <param name="itemEvent">The ItemEvent instance to register.</param>
    /// <returns>The ID under which the ItemEvent instance was registered.</returns>
    public int AddItemEvent(ItemEvent itemEvent)
    {
        if (itemEvent.UnionPermission)
        {
            ItemEventSlot[] sameTypeItemEventsSlots = _itemEventSlots.Where(x => x.ItemEvent.GetType() == itemEvent.GetType()).ToArray();

            foreach (var itemEventSlotToUnion in sameTypeItemEventsSlots)
            {
                if (itemEventSlotToUnion.ItemEvent.UnionPermission && itemEventSlotToUnion.ItemEvent.Union(itemEvent))
                    return itemEventSlotToUnion.Id;
            }
        }

        int newSlotId = 0;
        while (_itemEventSlots.Any(x => x.Id == newSlotId))
            newSlotId++;
        _itemEventSlots.Add(new ItemEventSlot(newSlotId, itemEvent));

        return newSlotId;
    }

    /// <summary>
    /// Removes one or more ItemEvent instances with the specified ID.
    /// </summary>
    /// <param name="itemId">The ID of the ItemEvent instance to remove.</param>
    /// <param name="count">The number of ItemEvent instances with the same ID to remove. Default is 1.</param>
    /// <exception cref="NotImplementedException">Coming soon.</exception>
    public void RemoveItemEvent(int itemId, int count = 1)
    {
        throw new NotImplementedException();
    }

    public ObjectData GetObjectData()
    {
        ObjectData objectData = new ObjectData();
        for (int i = 0; i < _itemEventSlots.Count; i++)
        {
            objectData.ObjectDataUnits.Add(nameof(_itemEventSlots) + i, _itemEventSlots[i].GetObjectData());
        }
        return objectData;
    }

    public void SetObjectData(ObjectData objectData)
    {
        _itemEventSlots.Clear();
        int index = 0;
        while (objectData.ObjectDataUnits.TryGetValue(nameof(_itemEventSlots) + index, out ObjectData slotData))
        {
            _itemEventSlots.Add(ItemEventSlot.RemakeItemEventSlot(slotData));
            index++;
        }
    }
}
