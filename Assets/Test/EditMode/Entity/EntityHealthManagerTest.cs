using NUnit.Framework;
using UnityEngine;

public class EntityHealthManagerTest
{
    const int MIN_CURRENT_HEALTH = 0;
    const int MIN_MAX_HEALTH = 1;

    private GameObject _gameObject;
    private EntityHealthManager _entityHealthManager;

    [SetUp]
    public void SetUp()
    {
        _gameObject = new GameObject();
        _entityHealthManager = _gameObject.AddComponent<EntityHealthManager>();
    }

    [Test]
    public void CurrentHealth_CanBeLessThenZero_False()
    {
        _entityHealthManager.CurrentHealth = -5;

        Assert.IsFalse(_entityHealthManager.CurrentHealth < MIN_CURRENT_HEALTH);
    }

    [Test]
    public void MaxHealth_CanBeLessThenOne_False()
    {
        _entityHealthManager.MaxHealth = -5;

        Assert.IsFalse(_entityHealthManager.MaxHealth < MIN_MAX_HEALTH);
    }

    [Test]
    [TestCase(10, 5)]
    [TestCase(7, 15)]
    [TestCase(8, -2)]
    public void ProcessDamage_IfDamagePositiveItIsSubtractedFromCurrentHealth_True(int currentHealth, int damage)
    {
        _entityHealthManager.MaxHealth = currentHealth;
        _entityHealthManager.CurrentHealth = currentHealth;

        int damageForExpectedResult = Mathf.Clamp(damage, 0, int.MaxValue);
        int expectedCurrentHealth = Mathf.Clamp(currentHealth - damageForExpectedResult, MIN_CURRENT_HEALTH, int.MaxValue);

        _entityHealthManager.ProcessDamage(damage);

        Assert.AreEqual(expectedCurrentHealth, _entityHealthManager.CurrentHealth);
    }

    [Test]
    [TestCase(10, 5)]
    [TestCase(7, 15)]
    [TestCase(8, -2)]
    public void ProcessHeal_IfHealPositiveItIsAddedToCurrentHealth_True(int currentHealth, int healAmount)
    {
        _entityHealthManager.MaxHealth = Mathf.Abs(currentHealth) * 2;
        _entityHealthManager.CurrentHealth = currentHealth;

        int healForExpectedResult = Mathf.Clamp(healAmount, 0, int.MaxValue);
        int expectedCurrentHealth = Mathf.Clamp(currentHealth + healForExpectedResult, MIN_CURRENT_HEALTH
            , _entityHealthManager.MaxHealth);

        _entityHealthManager.ProcessHeal(healAmount);

        Assert.AreEqual(expectedCurrentHealth, _entityHealthManager.CurrentHealth);
    }
}
