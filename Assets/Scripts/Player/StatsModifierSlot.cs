using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsModifierSlot
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
}
