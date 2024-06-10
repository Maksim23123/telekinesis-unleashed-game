using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerStatsStorage
{
    [SerializeField]
    float _damageMultiplier;
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
    float _criticalHitChance;
    [SerializeField]
    int _healthCount;
    [SerializeField]
    float _resistance;
    [SerializeField]
    float _regeneration;

    public float DamageMultiplier { get => _damageMultiplier; set => _damageMultiplier = value; }
    public float ObjectManipulationCooldown { get => _objectManipulationCooldown; set => _objectManipulationCooldown = value; }
    public float CaptureZoneRadius { get => _captureZoneRadius; set => _captureZoneRadius = value; }
    public float MovementSpeed { get => _movementSpeed; set => _movementSpeed = value; }
    public float JumpStrength { get => _jumpStrength; set => _jumpStrength = value; }
    public float CriticalHitMultiplier { get => _criticalHitMultiplier; set => _criticalHitMultiplier = value; }
    public float CriticalHitChance { get => _criticalHitChance; set => _criticalHitChance = value; }
    public int HealthCount { get => _healthCount; set => _healthCount = value; }
    public float Resistance { get => _resistance; set => _resistance = value; }
    public float Regeneration { get => _regeneration; set => _regeneration = value; }
}
