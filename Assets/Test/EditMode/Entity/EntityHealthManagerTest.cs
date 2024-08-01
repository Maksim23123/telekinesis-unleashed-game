using NUnit.Framework;
using UnityEngine;

public class EntityHealthManagerTest
{
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

        const int MIN_CURRENT_HEALTH = 0;
        Assert.IsFalse(_entityHealthManager.CurrentHealth < MIN_CURRENT_HEALTH);
    }

    [Test]
    public void MaxHealth_CanBeLessThenOne_False()
    {
        _entityHealthManager.MaxHealth = -5;

        const int MIN_MAX_HEALTH = 1;
        Assert.IsFalse(_entityHealthManager.MaxHealth < MIN_MAX_HEALTH);
    }
}
