using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Accessibility;

public class PlayerStatsHandler : MonoBehaviour
{

    [SerializeField]
    private PlayerStatsStorage _defaultPlayerStats;

    EntityHealthManager _healthManager;
    DamageHandler _damageHandler;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        Connect();
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
        
        CapturableObjectStatsStorage objectStatsStorage = new CapturableObjectStatsStorage(_defaultPlayerStats.DamageMultiplier
            , _defaultPlayerStats.CriticalHitMultiplier, _defaultPlayerStats.CriticalHitChance);
        PlayerPossessableObjectManager.Instance.ObjectStatsStorage = objectStatsStorage;

        PlayerPossessableObjectManager.Instance.CaptureZoneRadius = _defaultPlayerStats.CaptureZoneRadius;
        PlayerObjectManipulation.Instance.ManipulationCooldown = _defaultPlayerStats.ObjectManipulationCooldown;
        PlayerController.Instance.CharacterControllerScript.Speed = _defaultPlayerStats.MovementSpeed;
        PlayerController.Instance.CharacterControllerScript.JumpStrength = _defaultPlayerStats.JumpStrength;

        _damageHandler.Resistance = _defaultPlayerStats.Resistance;

        _healthManager.RegenerationAmount = _defaultPlayerStats.Regeneration;
    }

    private void ApplyHealthStat()
    {
        if (_healthManager != null)
        {
            _healthManager.MaxHealth = _defaultPlayerStats.HealthCount;
        }
    }

    private void ResetPlayer()
    {
        if (_healthManager != null)
        {
            _healthManager.ProcessFullHeal();
        }
    }
}
