using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityHealthManager : MonoBehaviour
{
    public event Action onRunOutOfHealth;

    [SerializeField]
    int _maxHealth;

    [SerializeField]
    int _currentHealth;

    [SerializeField]
    bool _afterDamageImortalityFrames;

    [SerializeField]
    float _afterDamageImortalityTime = 0.5f;

    bool _damageTakingOnCooldown = false;

    public void ProcessDamage(int damage)
    {
        if (_currentHealth > 0 && !_damageTakingOnCooldown)
        {
            _currentHealth -= damage;
            if (_currentHealth < 1)
            {
                _currentHealth = 0;
                onRunOutOfHealth?.Invoke();
            }

            if (_afterDamageImortalityFrames)
            {
                _damageTakingOnCooldown = true;
                Invoke(nameof(RestoreDamageTakingAbility), _afterDamageImortalityTime);
            }
        }
    }

    private void RestoreDamageTakingAbility()
    {
        _damageTakingOnCooldown = false;
    }

    public void ProcessHeal(int amount)
    {
        _currentHealth = Mathf.Clamp(_currentHealth + amount, 0, _maxHealth);
    }

    public void ProcessFullHeal()
    {
        _currentHealth = _maxHealth;
    }
}
