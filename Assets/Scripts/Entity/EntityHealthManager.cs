using System;
using UnityEngine;

/// <summary>
/// This class tracks and manages entity health, 
/// and notifies subscribers about it state.
/// This class should be used as utility in external logic that performs manipulation
/// with enemy health.
/// </summary>
public class EntityHealthManager : MonoBehaviour, IRecordable
{
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _currentHealth;
    [SerializeField] private bool _afterDamageImortalityFrames = false;
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
            _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);
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

    /// <summary>
    /// Starts up health regeneration logic.
    /// </summary>
    private void Start()
    {
        PerformRegenerationIteration();
    }

    /// <summary>
    /// Represents on iteration of health regeneration logic.
    /// </summary>
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


    /// <summary>
    /// Allows external logic to rearange unpacking time of data provided by <c>Save Load System</c>
    /// </summary>
    public void TriggerWaitingObjectDataUnpacking()
    {
        if (_triggerObjectDataUnpack && _waitingObjectData != null)
        {
            UnpackObjectData(_waitingObjectData);
            _waitingObjectData = null;
        }
    }

    /// <summary>
    /// Allows external logic to negatively influence health amount.
    /// </summary>
    /// <param name="damage">Negative health influence amount.</param>
    public void ProcessDamage(int damage)
    {
        if (_currentHealth > 0 && !_damageTakingOnCooldown)
        {
            damage = Mathf.Clamp(damage, 0, int.MaxValue);
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

    /// <summary>
    /// Allows external logic to positively influence health amount.
    /// </summary>
    /// <param name="damage">Positive health influence amount.</param>
    public void ProcessHeal(int amount)
    {
        amount = Mathf.Clamp(amount, 0, int.MaxValue);
        _currentHealth = Mathf.Clamp(_currentHealth + amount, 0, _maxHealth);
        HealthChanged?.Invoke(_currentHealth);
    }

    /// <summary>
    /// Allows external logic to restore health to its posible maximum.
    /// </summary>
    public void ProcessFullHeal()
    {
        _currentHealth = _maxHealth;
        HealthChanged?.Invoke(_currentHealth);
    }

    /// <summary>
    /// Used in imortality frame logic.
    /// </summary>
    private void RestoreDamageTakingAbility()
    {
        _damageTakingOnCooldown = false;
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

    /// <summary>
    /// Inherits object parameters from received <see cref="ObjectData"> instance.
    /// </summary>
    /// <param name="objectData"><see cref="ObjectData"> to inherit.</param>
    private void UnpackObjectData(ObjectData objectData)
    {
        int.TryParse(objectData.VariableValues[nameof(_currentHealth)], out _currentHealth);
        HealthChanged?.Invoke(_currentHealth);
    }
}
