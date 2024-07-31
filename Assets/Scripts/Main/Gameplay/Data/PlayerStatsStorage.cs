using System;
using UnityEngine;

/// <summary>
/// This class stores all important player stats in one place.
/// And also allows some manipulation on them.
/// </summary>
[Serializable] 
public class PlayerStatsStorage : IRecordable
{
    [SerializeField] private float _damageMultiplier;
    [SerializeField] private float _objectManipulationCooldown;
    [SerializeField] private float _captureZoneRadius;
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _jumpStrength;
    [SerializeField] private float _criticalHitMultiplier;
    [SerializeField] private float _criticalHitChance;
    [SerializeField] private int _healthCount;
    [SerializeField] private float _resistance;
    [SerializeField] private float _regeneration;

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

    /// <summary>
    /// Combines two stat storages.
    /// </summary>
    /// <param name="s1">First stats storage</param>
    /// <param name="s2">Second stats storage</param>
    /// <returns>New stats storage with combined values.</returns>
    public static PlayerStatsStorage operator +(PlayerStatsStorage s1, PlayerStatsStorage s2)
    {
        PlayerStatsStorage newPlayerStatsStorage = new PlayerStatsStorage();
        newPlayerStatsStorage.DamageMultiplier = s1.DamageMultiplier + s2.DamageMultiplier;
        newPlayerStatsStorage.ObjectManipulationCooldown = s1.ObjectManipulationCooldown + s2.ObjectManipulationCooldown;
        newPlayerStatsStorage.CaptureZoneRadius = s1.CaptureZoneRadius + s2.CaptureZoneRadius;
        newPlayerStatsStorage.MovementSpeed = s1.MovementSpeed + s2.MovementSpeed;
        newPlayerStatsStorage.JumpStrength = s1.JumpStrength + s2.JumpStrength;
        newPlayerStatsStorage.CriticalHitMultiplier = s1.CriticalHitMultiplier + s2.CriticalHitMultiplier;
        newPlayerStatsStorage.CriticalHitChance = s1.CriticalHitChance + s2.CriticalHitChance;
        newPlayerStatsStorage.HealthCount = s1.HealthCount + s2.HealthCount;
        newPlayerStatsStorage.Resistance = s1.Resistance + s2.Resistance;
        newPlayerStatsStorage.Regeneration = s1.Regeneration + s2.Regeneration;
        return newPlayerStatsStorage;
    }

    /// <summary>
    /// Multiplies all stats in stat storage by certain number.
    /// </summary>
    /// <param name="s1">Stats storage to perform manipulation on.</param>
    /// <param name="number">Multiplier.</param>
    /// <returns>New stats storage with modified values.</returns>
    public static PlayerStatsStorage operator *(PlayerStatsStorage s1, float number)
    {
        PlayerStatsStorage newPlayerStatsStorage = new PlayerStatsStorage();
        newPlayerStatsStorage.DamageMultiplier = s1.DamageMultiplier * number;
        newPlayerStatsStorage.ObjectManipulationCooldown = s1.ObjectManipulationCooldown * number;
        newPlayerStatsStorage.CaptureZoneRadius = s1.CaptureZoneRadius * number;
        newPlayerStatsStorage.MovementSpeed = s1.MovementSpeed * number;
        newPlayerStatsStorage.JumpStrength = s1.JumpStrength * number;
        newPlayerStatsStorage.CriticalHitMultiplier = s1.CriticalHitMultiplier * number;
        newPlayerStatsStorage.CriticalHitChance = s1.CriticalHitChance * number;
        newPlayerStatsStorage.HealthCount = (int)(s1.HealthCount * number);
        newPlayerStatsStorage.Resistance = s1.Resistance * number;
        newPlayerStatsStorage.Regeneration = s1.Regeneration * number;
        return newPlayerStatsStorage;
    }

    /// <summary>
    /// Checks if two stat storages have the same stats values.
    /// </summary>
    /// <param name="s1">First stats storage.</param>
    /// <param name="s2">Second stats storage.</param>
    /// <returns>Comparison result.</returns>
    public static bool operator ==(PlayerStatsStorage s1, PlayerStatsStorage s2)
    {
        return s1.Equals(s2);
    }

    /// <summary>
    /// Checks if there are any difference between stats values of two stat storages.
    /// </summary>
    /// <param name="s1">First stats storage.</param>
    /// <param name="s2">Second stats storage.</param>
    /// <returns>Comparison result.</returns>
    public static bool operator !=(PlayerStatsStorage s1, PlayerStatsStorage s2)
    {
        return !(s1 == s2);
    }

    /// <summary>
    /// Checks if two stat storages have the same stats values.
    /// </summary>
    /// <param name="s1">First stats storage.</param>
    /// <param name="s2">Second stats storage.</param>
    /// <returns>Comparison result.</returns>
    public override bool Equals(object obj)
    {
        if (obj is PlayerStatsStorage other)
        {
            return !(DamageMultiplier != other.DamageMultiplier
                || ObjectManipulationCooldown != other.ObjectManipulationCooldown
                || CaptureZoneRadius != other.CaptureZoneRadius
                || MovementSpeed != other.MovementSpeed
                || JumpStrength != other.JumpStrength
                || CriticalHitMultiplier != other.CriticalHitMultiplier
                || CriticalHitChance != other.CriticalHitChance
                || HealthCount != other.HealthCount
                || Resistance != other.Resistance
                || Regeneration != other.Regeneration);

        }
        return false;
    }

    public override int GetHashCode()
    {
        int hash1 = HashCode.Combine(DamageMultiplier, ObjectManipulationCooldown, CaptureZoneRadius, MovementSpeed, JumpStrength);
        int hash2 = HashCode.Combine(CriticalHitMultiplier, CriticalHitChance, HealthCount, Resistance, Regeneration);
        return HashCode.Combine(hash1, hash2);
    }

    public ObjectData GetObjectData()
    {
        ObjectData data = new ObjectData();
        data.VariableValues.Add(nameof(DamageMultiplier), DamageMultiplier.ToString());
        data.VariableValues.Add(nameof(ObjectManipulationCooldown), ObjectManipulationCooldown.ToString());
        data.VariableValues.Add(nameof(CaptureZoneRadius), CaptureZoneRadius.ToString());
        data.VariableValues.Add(nameof(MovementSpeed), MovementSpeed.ToString());
        data.VariableValues.Add(nameof(JumpStrength), JumpStrength.ToString());
        data.VariableValues.Add(nameof(CriticalHitMultiplier), CriticalHitMultiplier.ToString());
        data.VariableValues.Add(nameof(CriticalHitChance), CriticalHitChance.ToString());
        data.VariableValues.Add(nameof(HealthCount), HealthCount.ToString());
        data.VariableValues.Add(nameof(Resistance), Resistance.ToString());
        data.VariableValues.Add(nameof(Regeneration), Regeneration.ToString());
        return data;
    }

    public void SetObjectData(ObjectData objectData)
    {
        float.TryParse(objectData.VariableValues[nameof(DamageMultiplier)], out _damageMultiplier);
        float.TryParse(objectData.VariableValues[nameof(ObjectManipulationCooldown)], out _objectManipulationCooldown);
        float.TryParse(objectData.VariableValues[nameof(CaptureZoneRadius)], out _captureZoneRadius);
        float.TryParse(objectData.VariableValues[nameof(MovementSpeed)], out _movementSpeed);
        float.TryParse(objectData.VariableValues[nameof(JumpStrength)], out _jumpStrength);
        float.TryParse(objectData.VariableValues[nameof(CriticalHitMultiplier)], out _criticalHitMultiplier);
        float.TryParse(objectData.VariableValues[nameof(CriticalHitChance)], out _criticalHitChance);
        int.TryParse(objectData.VariableValues[nameof(HealthCount)], out _healthCount);
        float.TryParse(objectData.VariableValues[nameof(Resistance)], out _resistance);
        float.TryParse(objectData.VariableValues[nameof(Regeneration)], out _regeneration);
    }
}
