public class ItemEventSlot : IRecordable
{
    private int _id;
    private ItemEvent _itemEvent;

    public int Id { get => _id; }

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

    public static ItemEventSlot RemakeItemEventSlot(ObjectData objectData)
    {
        ItemEventSlot itemEventSlot = new ItemEventSlot(0, null);
        itemEventSlot.SetObjectData(objectData);
        return itemEventSlot;
    }
}
