using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEventSlot : IRecordable
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

    public int Priority => throw new System.NotImplementedException();

    public ObjectData GetObjectData()
    {
        ObjectData objectData = new ObjectData();
        objectData.variableValues.Add(nameof(_id), _id.ToString());
        objectData.objectDataUnits.Add(nameof(_itemEvent), _itemEvent.GetObjectData());
        return objectData;
    }

    public void SetObjectData(ObjectData objectData)
    {
        int.TryParse(objectData.variableValues[nameof(_id)], out _id);
        _itemEvent = ItemEvent.RemakeItemEvent(objectData.objectDataUnits[nameof(_itemEvent)]);
    }

    public static ItemEventSlot RemakeItemEventSlot(ObjectData objectData)
    {
        ItemEventSlot itemEventSlot = new ItemEventSlot(0, null);
        itemEventSlot.SetObjectData(objectData);
        return itemEventSlot;
    }
}
