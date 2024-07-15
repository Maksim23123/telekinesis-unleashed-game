using System;
using UnityEngine;

public class EntityHealthManager : MonoBehaviour, IRecordable
{
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _currentHealth;
    [SerializeField] private bool _afterDamageImortalityFrames;
    [SerializeField] private float _afterDamageImortalityTime = 0.5f;
    [Header("Save Load System related")]
    [SerializeField] private bool _triggerObjectDataUnpack = false;

    private bool _damageTakingOnCooldown = false;
    private bool _performRegenerationIteration = true;
    private float _regenerationAmount;
    private float _regenerationPool;
    private ObjectData _waitingObjectData;

    public event Action RunOutOfHealth;
    public event Action<int> HealthChanged;

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
            HealthChanged?.Invoke(_currentHealth);
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

    private void RestoreDamageTakingAbility()
    {
        _damageTakingOnCooldown = false;
    }

    public void ProcessDamage(int damage)
    {
        if (_currentHealth > 0 && !_damageTakingOnCooldown)
        {
            _currentHealth -= damage;
            if (_currentHealth < 1)
            {
                _currentHealth = 0;
                RunOutOfHealth?.Invoke();
            }

            HealthChanged?.Invoke(_currentHealth);

            if (_afterDamageImortalityFrames)
            {
                _damageTakingOnCooldown = true;
                Invoke(nameof(RestoreDamageTakingAbility), _afterDamageImortalityTime);
            }
        }
    }

    public void ProcessHeal(int amount)
    {
        _currentHealth = Mathf.Clamp(_currentHealth + amount, 0, _maxHealth);
        HealthChanged?.Invoke(_currentHealth);
    }

    public void ProcessFullHeal()
    {
        _currentHealth = _maxHealth;
        HealthChanged?.Invoke(_currentHealth);
    }

    public ObjectData GetObjectData()
    {
        ObjectData objectData = new ObjectData();
        objectData.VariableValues.Add(nameof(_currentHealth), _currentHealth.ToString());
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
        int.TryParse(objectData.VariableValues[nameof(_currentHealth)], out _currentHealth);
        HealthChanged?.Invoke(_currentHealth);
    }
}
