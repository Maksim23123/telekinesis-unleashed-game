using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemEventsExecutor : MonoBehaviour
{
    private static ItemEventsExecutor _instance;

    //public event Action executeEvents;

    List<ItemEventSlot> _itemEventSlots = new List<ItemEventSlot>();


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

    public void RemoveItemEvent(int itemId, int count = 1)
    {
        // CHANGE
        /*
        if (_events.TryGetValue(itemId, out ItemEvent itemEvent))
        {
            itemEvent.eventsCount -= count;

            if (itemEvent.eventsCount <= 0)
            {
                _events.Remove(itemId);
            }
        }
        */
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

    void Awake()
    {
        _instance = this;
    }
}
