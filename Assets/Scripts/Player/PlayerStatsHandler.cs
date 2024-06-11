using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.Accessibility;

public class PlayerStatsHandler : MonoBehaviour
{
    [SerializeField]
    private PlayerStatsStorage _defaultPlayerStats;

    private PlayerStatsStorage _modifiedStats;

    List<StatsModifierSlot> _statsModifierSlots = new List<StatsModifierSlot>();

    EntityHealthManager _healthManager;
    DamageHandler _damageHandler;

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
    }

    private void Connect()
    {
        TryGetComponent(out _healthManager);
        TryGetComponent(out _damageHandler);
    }

    private void ApplyStats()
    {
        ApplyHealthStat();
        
        CapturableObjectStatsStorage objectStatsStorage = new CapturableObjectStatsStorage(_modifiedStats.DamageMultiplier
            , _modifiedStats.CriticalHitMultiplier, _modifiedStats.CriticalHitChance);
        PlayerPossessableObjectManager.Instance.ObjectStatsStorage = objectStatsStorage;

        PlayerPossessableObjectManager.Instance.CaptureZoneRadius = _modifiedStats.CaptureZoneRadius;
        PlayerObjectManipulation.Instance.ManipulationCooldown = _modifiedStats.ObjectManipulationCooldown;
        PlayerController.Instance.CharacterControllerScript.Speed = _modifiedStats.MovementSpeed;
        PlayerController.Instance.CharacterControllerScript.JumpStrength = _modifiedStats.JumpStrength;

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
}
