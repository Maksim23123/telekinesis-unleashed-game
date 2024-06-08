using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityHealthManager : MonoBehaviour
{
    public event Action runOutOfHealth;

    public event Action<int> healthChanged;

    [SerializeField]
    int _maxHealth;

    [SerializeField]
    int _currentHealth;

    [SerializeField]
    bool _afterDamageImortalityFrames;

    [SerializeField]
    float _afterDamageImortalityTime = 0.5f;

    bool _damageTakingOnCooldown = false;

    bool _performRegenerationIteration = true;

    float _regenerationAmount;

    float _regenerationPool;

    public int CurrentHealth { get => _currentHealth; }
    public int MaxHealth 
    { 
        get => _maxHealth;

        set 
        {
            _maxHealth = Mathf.Clamp(value, 1, int.MaxValue);
            healthChanged?.Invoke(_currentHealth);
        }
    }

    public float RegenerationAmount 
    { 
        get => _regenerationAmount;

        set
        {
            _regenerationAmount = value;
            if (_regenerationAmount > 0 && _performRegenerationIteration)
                PerformRegenerationIteration();
        } 
    }

    private void Start()
    {
        PerformRegenerationIteration();
    }

    private void PerformRegenerationIteration()
    {
        _regenerationPool += RegenerationAmount;
        if (_regenerationPool >= 1)
        {
            ProcessHeal((int)Mathf.Floor(_regenerationPool));
            _regenerationPool = _regenerationPool - Mathf.Floor(_regenerationPool);
        }

        if (RegenerationAmount > 0 && _performRegenerationIteration)
            Invoke(nameof(PerformRegenerationIteration), 1);
    }

    public void ProcessDamage(int damage)
    {
        if (_currentHealth > 0 && !_damageTakingOnCooldown)
        {
            _currentHealth -= damage;
            if (_currentHealth < 1)
            {
                _currentHealth = 0;
                runOutOfHealth?.Invoke();
            }

            healthChanged?.Invoke(_currentHealth);

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
        healthChanged?.Invoke(_currentHealth);
    }

    public void ProcessFullHeal()
    {
        _currentHealth = _maxHealth;
        healthChanged?.Invoke(_currentHealth);
    }
}
