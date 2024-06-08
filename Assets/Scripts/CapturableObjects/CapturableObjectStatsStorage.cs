using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapturableObjectStatsStorage
{
    public float _damageMultiplier;

    public float _criticalMultiplier;

    public float _criticalHitChance;

    public CapturableObjectStatsStorage(float damageMultiplier = 1, float criticalMultiplier = 1.5f, float criticalHitChance = 0.05f) 
    { 
        _damageMultiplier = damageMultiplier;
        _criticalMultiplier = criticalMultiplier;
        _criticalHitChance = criticalHitChance;
    }
}
