using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityHealthManager : MonoBehaviour, IRecordable
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

    [Header("Save Load System related")]
    [SerializeField]
    bool _triggerObjectDataUnpack = false;

    bool _damageTakingOnCooldown = false;

    bool _performRegenerationIteration = true;

    float _regenerationAmount;

    float _regenerationPool;

    ObjectData _waitingObjectData;

    public int CurrentHealth 
    { 
        get => _currentHealth;
        
        set => _currentHealth = Mathf.Clamp(value, 0, _maxHealth);
    }

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

    public int Priority { get => 0; }

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

    public ObjectData GetObjectData()
    {
        ObjectData objectData = new ObjectData();
        objectData.variableValues.Add(nameof(_currentHealth), _currentHealth.ToString());
        return objectData;
    }

    public void SetObjectData(ObjectData objectData)
    {
        if (_triggerObjectDataUnpack)
            _waitingObjectData = objectData;
        else
            UnpackObjectData(objectData);
    }

    public void TriggerWaitingObjectDataUnpacking()
    {
        if (_triggerObjectDataUnpack && _waitingObjectData != null)
        {
            UnpackObjectData(_waitingObjectData);
            _waitingObjectData = null;
        }
    }

    public void UnpackObjectData(ObjectData objectData)
    {
        int.TryParse(objectData.variableValues[nameof(_currentHealth)], out _currentHealth);
        healthChanged?.Invoke(_currentHealth);
    }
}
