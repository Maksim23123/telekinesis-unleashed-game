using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerStatsHandler : MonoBehaviour, IRecordable
{
    [SerializeField] private PlayerStatsStorage _defaultPlayerStats;

    private PlayerStatsStorage _modifiedStats;
    private List<StatsModifierSlot> _statsModifierSlots = new List<StatsModifierSlot>();
    private EntityHealthManager _healthManager;
    private DamageHandler _damageHandler;

    public int Priority { get => 1; }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        Connect();
        UpdateModifiedStats();
        ApplyStats();
        ResetPlayer();

        if (_healthManager != null)
        {
            _healthManager.TriggerWaitingObjectDataUnpacking();
        }
    }

    private void Connect()
    {
        TryGetComponent(out _healthManager);
        TryGetComponent(out _damageHandler);
    }

    private void ApplyStats()
    {
        ApplyHealthStat();

        PossessableObjectStatsStorage objectStatsStorage = new PossessableObjectStatsStorage(_modifiedStats.DamageMultiplier
            , _modifiedStats.CriticalHitMultiplier, _modifiedStats.CriticalHitChance);
        PlayerPossessableObjectManager.Instance.ObjectStatsStorage = objectStatsStorage;

        PlayerPossessableObjectManager.Instance.CaptureZoneRadius = _modifiedStats.CaptureZoneRadius;
        PlayerObjectManipulation.Instance.ManipulationCooldown = _modifiedStats.ObjectManipulationCooldown;
        PlayerController.Instance.CharacterControllerScript.Speed = _modifiedStats.MovementSpeed;
        PlayerJumpHandler.Instance.JumpStrength = _modifiedStats.JumpStrength;

        _damageHandler.Resistance = _modifiedStats.Resistance;

        _healthManager.RegenerationAmount = _modifiedStats.Regeneration;
    }

    private void ApplyHealthStat()
    {
        if (_healthManager != null)
        {
            _healthManager.MaxHealth = _modifiedStats.HealthCount;
        }
    }

    private void ResetPlayer()
    {
        if (_healthManager != null)
        {
            _healthManager.ProcessFullHeal();
        }
    }

    private void UpdateModifiedStats()
    {
        _modifiedStats = _defaultPlayerStats;
        foreach (var statsModifier in _statsModifierSlots.Select(x => x.GetUnifiedStatsModifier()).ToArray())
        {
            _modifiedStats += statsModifier;
        }
    }

    public int AddStatsModifier(PlayerStatsStorage statsModifier)
    {
        int statsModifierId = 0;

        StatsModifierSlot[] buffer = _statsModifierSlots.Where(x => x.StatsModifier == statsModifier).Take(1).ToArray();
        StatsModifierSlot slot;

        if (buffer.Length > 0)
        {
            slot = buffer[0];
            slot.Count++;
            statsModifierId = slot.ModifierSlotId;
        }
        else
        {
            int availableId = StaticTools.GetFreeId(_statsModifierSlots, x => x.ModifierSlotId);
            slot = new StatsModifierSlot(availableId, 1, statsModifier);
            _statsModifierSlots.Add(slot);
            statsModifierId = availableId;
        }

        UpdateModifiedStats();
        ApplyStats();

        return statsModifierId;
    }

    public ObjectData GetObjectData()
    {
        ObjectData objectData = new ObjectData();
        objectData.ObjectDataUnits.Add(nameof(_defaultPlayerStats), _defaultPlayerStats.GetObjectData());

        for (int i = 0; i < _statsModifierSlots.Count; i++)
        {
            objectData.ObjectDataUnits.Add(nameof(_statsModifierSlots) + i, _statsModifierSlots[i].GetObjectData());
        }

        return objectData;
    }

    public void SetObjectData(ObjectData objectData)
    {
        _defaultPlayerStats.SetObjectData(objectData.ObjectDataUnits[nameof(_defaultPlayerStats)]);

        _statsModifierSlots = new List<StatsModifierSlot>();
        int statsModifierSlotIndex = 0;
        while (objectData.ObjectDataUnits.TryGetValue(nameof(_statsModifierSlots) + statsModifierSlotIndex, out ObjectData slotData))
        {
            _statsModifierSlots.Add(StatsModifierSlot.RemakeStatsModifierSlot(slotData));
            statsModifierSlotIndex++;
        }
    }
}
