using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsModifierSlot : IRecordable
{
    int _modifierSlotId;

    int _count;

    [SerializeField]
    PlayerStatsStorage _statsModifier;

    public StatsModifierSlot(int modifierSlotId, int count, PlayerStatsStorage statsModifier)
    {
        _modifierSlotId = modifierSlotId;
        _count = count;
        StatsModifier = statsModifier;
    }

    public PlayerStatsStorage GetUnifiedStatsModifier()
    {
        return _statsModifier * _count;
    }

    public PlayerStatsStorage StatsModifier { get => _statsModifier; set => _statsModifier = value; }
    public int ModifierSlotId { get => _modifierSlotId; }
    public int Count { get => _count; set => _count = value; }

    public int Priority => throw new System.NotImplementedException();

    public static StatsModifierSlot RemakeStatsModifierSlot(ObjectData objectData)
    {
        StatsModifierSlot statsModifierSlot = new StatsModifierSlot(1, 1, new PlayerStatsStorage());
        statsModifierSlot.SetObjectData(objectData);
        return statsModifierSlot;

    }

    public ObjectData GetObjectData()
    {
        ObjectData objectData = new ObjectData();
        objectData.variableValues.Add(nameof(_modifierSlotId), _modifierSlotId.ToString());
        objectData.variableValues.Add(nameof(_count), _count.ToString());
        objectData.objectDataUnits.Add(nameof(_statsModifier), _statsModifier.GetObjectData());
        return objectData;
    }

    public void SetObjectData(ObjectData objectData)
    {
        int.TryParse(objectData.variableValues[nameof(_modifierSlotId)], out _modifierSlotId);
        int.TryParse(objectData.variableValues[nameof(_count)], out _count);

        PlayerStatsStorage playerStatsStorage = new PlayerStatsStorage();
        playerStatsStorage.SetObjectData(objectData.objectDataUnits[nameof(_statsModifier)]);
        _statsModifier = playerStatsStorage;
    }
}
