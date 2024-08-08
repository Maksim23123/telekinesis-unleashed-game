/// <summary>
/// This class stores an ItemEvent instance and the necessary additional information about it.
/// Designed to be used in ItemEventExecutor.
/// </summary>
public class ItemEventSlot : IRecordable
{
    private int _id;
    private ItemEvent _itemEvent;

    /// <summary>
    /// Unique indentificator of current slot.
    /// </summary>
    public int Id { get => _id; }

    /// <summary>
    /// Gets the ItemEvent instance associated with this slot.
    /// </summary>
    public ItemEvent ItemEvent { get => _itemEvent; }

    public ItemEventSlot(int id, ItemEvent itemEvent)
    {
        _id = id;
        _itemEvent = itemEvent;
    }

    public ObjectData GetObjectData()
    {
        ObjectData objectData = new ObjectData();
        objectData.VariableValues.Add(nameof(_id), _id.ToString());
        objectData.ObjectDataUnits.Add(nameof(_itemEvent), _itemEvent.GetObjectData());
        return objectData;
    }

    public void SetObjectData(ObjectData objectData)
    {
        int.TryParse(objectData.VariableValues[nameof(_id)], out _id);
        _itemEvent = ItemEvent.RemakeItemEvent(objectData.ObjectDataUnits[nameof(_itemEvent)]);
    }

    /// <summary>
    /// Creates a new ItemEventSlot instance from data received from the SaveLoad system.
    /// </summary>
    /// <param name="objectData">Data from the SaveLoad system.</param>
    /// <returns>A new ItemEventSlot instance with the assigned data.</returns>
    public static ItemEventSlot RemakeItemEventSlot(ObjectData objectData)
    {
        ItemEventSlot itemEventSlot = new ItemEventSlot(0, null);
        itemEventSlot.SetObjectData(objectData);
        return itemEventSlot;
    }
}
