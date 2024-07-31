using NUnit.Framework;

public class PlayerStatsStorageTest
{
    [Test]
    [TestCase(1, 2)]
    [TestCase(5, 3)]
    public void PlusOperator_AddsValuesOfBothStoragesTogether_True(int firstStorageStatsValue, int secondStorageStatsValue)
    {
        PlayerStatsStorage firstStorage = GenerateStorage(firstStorageStatsValue);
        PlayerStatsStorage secondStorage = GenerateStorage(secondStorageStatsValue);
        PlayerStatsStorage expectedResult = GenerateStorage(firstStorageStatsValue + secondStorageStatsValue);

        PlayerStatsStorage additionResult = firstStorage + secondStorage;

        Assert.AreEqual(expectedResult, additionResult);
    }

    [Test]
    [TestCase(1, 2)]
    [TestCase(5, 3)]
    [TestCase(5, 1.5f)]
    public void MultiplierOperator_MultiplierValuesByCertainNumber_True(int storageStatsValue, float multiplier)
    {
        PlayerStatsStorage storage = GenerateStorage(storageStatsValue);
        PlayerStatsStorage expectedResult = GenerateStorage(storageStatsValue * multiplier);

        PlayerStatsStorage actualResult = storage * multiplier;

        Assert.AreEqual(expectedResult, actualResult);
    }

    private PlayerStatsStorage GenerateStorage(float statsValue)
    {
        PlayerStatsStorage newStorage = new PlayerStatsStorage();
        newStorage.DamageMultiplier = statsValue;
        newStorage.ObjectManipulationCooldown = statsValue;
        newStorage.CaptureZoneRadius = statsValue;
        newStorage.MovementSpeed = statsValue;
        newStorage.JumpStrength = statsValue;
        newStorage.CriticalHitMultiplier = statsValue;
        newStorage.CriticalHitChance = statsValue;
        newStorage.HealthCount = (int)statsValue;
        newStorage.Resistance = statsValue;
        newStorage.Regeneration = statsValue;
        return newStorage;
    }
}
