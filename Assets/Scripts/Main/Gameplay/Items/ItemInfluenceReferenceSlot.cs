/// <summary>
/// This class stores references in the form of IDs to item influences, such as ItemEvent and StatsModifier.
/// Designed to allow item management from a single class.
/// </summary>
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
        objectData.VariableValues.Add(nameof(_slotId), _slotId.ToString());
        objectData.VariableValues.Add(nameof(_itemId), _itemId.ToString());
        objectData.VariableValues.Add(nameof(_statsModifierId), _statsModifierId.ToString());
        objectData.VariableValues.Add(nameof(_itemEventId), _itemEventId.ToString());
        return objectData;
    }

    public void SetObjectData(ObjectData objectData)
    {
        int.TryParse(objectData.VariableValues[nameof(_slotId)], out _slotId);
        int.TryParse(objectData.VariableValues[nameof(_itemId)], out _itemId);
        int.TryParse(objectData.VariableValues[nameof(_statsModifierId)], out _statsModifierId);
        int.TryParse(objectData.VariableValues[nameof(_itemEventId)], out _itemEventId);
    }

    /// <summary>
    /// Creates a new ItemInfluenceReferenceSlot instance from data received from the SaveLoad system.
    /// </summary>
    /// <param name="objectData">Data from the SaveLoad system.</param>
    /// <returns>A new ItemInfluenceReferenceSlot instance with the assigned data.</returns>
    public static ItemInfluenceReferenceSlot RemakeItemInfluenceReferenceSlot(ObjectData objectData)
    {
        ItemInfluenceReferenceSlot statsModifierSlot = new ItemInfluenceReferenceSlot(0, 0, 0, 0);
        statsModifierSlot.SetObjectData(objectData);
        return statsModifierSlot;
    }
}
