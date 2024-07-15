using UnityEngine;

public class StatsModifierSlot : IRecordable
{
    [SerializeField] private PlayerStatsStorage _statsModifier;
    
    private int _modifierSlotId;
    private int _count;

    public PlayerStatsStorage StatsModifier { get => _statsModifier; set => _statsModifier = value; }
    public int ModifierSlotId { get => _modifierSlotId; }
    public int Count { get => _count; set => _count = value; }

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

    public static StatsModifierSlot RemakeStatsModifierSlot(ObjectData objectData)
    {
        StatsModifierSlot statsModifierSlot = new StatsModifierSlot(1, 1, new PlayerStatsStorage());
        statsModifierSlot.SetObjectData(objectData);
        return statsModifierSlot;
    }

    public ObjectData GetObjectData()
    {
        ObjectData objectData = new ObjectData();
        objectData.VariableValues.Add(nameof(_modifierSlotId), _modifierSlotId.ToString());
        objectData.VariableValues.Add(nameof(_count), _count.ToString());
        objectData.ObjectDataUnits.Add(nameof(_statsModifier), _statsModifier.GetObjectData());
        return objectData;
    }

    public void SetObjectData(ObjectData objectData)
    {
        int.TryParse(objectData.VariableValues[nameof(_modifierSlotId)], out _modifierSlotId);
        int.TryParse(objectData.VariableValues[nameof(_count)], out _count);

        PlayerStatsStorage playerStatsStorage = new PlayerStatsStorage();
        playerStatsStorage.SetObjectData(objectData.ObjectDataUnits[nameof(_statsModifier)]);
        _statsModifier = playerStatsStorage;
    }
}
