using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEventSlot
{
    int _id;

    ItemEvent _itemEvent;

    public ItemEventSlot(int id, ItemEvent itemEvent)
    {
        _id = id;
        _itemEvent = itemEvent;
    }

    public int Id { get => _id; }

    public ItemEvent ItemEvent { get => _itemEvent; }
}
