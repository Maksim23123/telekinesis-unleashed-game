public class ItemInfluenceReferenceSlot : IRecordable
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

    public ObjectData GetObjectData()
    {
        ObjectData objectData = new ObjectData();
        objectData.variableValues.Add(nameof(_slotId), _slotId.ToString());
        objectData.variableValues.Add(nameof(_itemId), _itemId.ToString());
        objectData.variableValues.Add(nameof(_statsModifierId), _statsModifierId.ToString());
        objectData.variableValues.Add(nameof(_itemEventId), _itemEventId.ToString());
        return objectData;
    }

    public void SetObjectData(ObjectData objectData)
    {
        int.TryParse(objectData.variableValues[nameof(_slotId)], out _slotId);
        int.TryParse(objectData.variableValues[nameof(_itemId)], out _itemId);
        int.TryParse(objectData.variableValues[nameof(_statsModifierId)], out _statsModifierId);
        int.TryParse(objectData.variableValues[nameof(_itemEventId)], out _itemEventId);
    }

    public static ItemInfluenceReferenceSlot RemakeItemInfluenceReferenceSlot(ObjectData objectData)
    {
        ItemInfluenceReferenceSlot statsModifierSlot = new ItemInfluenceReferenceSlot(0, 0, 0, 0);
        statsModifierSlot.SetObjectData(objectData);
        return statsModifierSlot;
    }
}
