using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfluenceReferenceSlot
{
    private int _slotId;

    private int _itemId;

    private int _statsModifierId;

    private int _itemEventId;

    public int ItemEventId { get => _itemEventId; }
    public int SlotId { get => _slotId; set => _slotId = value; }
    public int ItemId { get => _itemId; set => _itemId = value; }
    public int StatsModifierId { get => _statsModifierId; set => _statsModifierId = value; }

    public ItemInfluenceReferenceSlot(int slotId, int itemId, int statsModifierId, int itemEventId)
    {
        _slotId = slotId;
        _itemId = itemId;
        _statsModifierId = statsModifierId;
        _itemEventId = itemEventId;
    }
}
