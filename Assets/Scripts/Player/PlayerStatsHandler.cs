using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Accessibility;

public class PlayerStatsHandler : MonoBehaviour
{
    [SerializeField]
    float _damageMultiplier = 1;
    [SerializeField]
    float _objectManipulationCooldown;
    [SerializeField]
    float _captureZoneRadius;
    [SerializeField]
    float _movementSpeed;
    [SerializeField]
    float _jumpStrength;
    [SerializeField] 
    float _criticalHitMultiplier;
    [SerializeField]
    int _healthCount;
    [SerializeField]
    float _resistance;
    [SerializeField]
    float _regenerationAmount;
    [SerializeField]
    float _regenerationRate;
    

    EntityHealthManager _healthManager;

    bool _initialized = false;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        Connect();
        ApplyStats();
        ResetPlayer();
        _initialized = true;
    }

    private void Connect()
    {
        TryGetComponent(out _healthManager);
    }

    private void ApplyStats()
    {
        ApplyHealthStat();
        PlayerObjectManager.Instance.DamageMultipier = _damageMultiplier;
        PlayerObjectManager.Instance.CaptureZoneRadius = _captureZoneRadius;
        PlayerObjectManipulation.Instance.ManipulationCooldown = _objectManipulationCooldown;
        PlayerController.Instance.CharacterControllerScript.Speed = _movementSpeed;
        PlayerController.Instance.CharacterControllerScript.JumpStrength = _jumpStrength;
    }

    private void ApplyHealthStat()
    {
        if (_healthManager != null)
        {
            _healthManager.MaxHealth = _healthCount;
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
