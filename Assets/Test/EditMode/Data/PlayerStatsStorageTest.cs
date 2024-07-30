using NUnit.Framework;

public class PlayerStatsStorageTest
{
    [Test]
    [TestCase(1, 2)]
    public void PlusOperator_AddsValuesOfBothStoragesTogether_True(int firstStorageStatsValue, int secondStorageStatsValue)
    {
        PlayerStatsStorage firstStorage = GenerateStorage(firstStorageStatsValue);
        PlayerStatsStorage secondStorage = GenerateStorage(secondStorageStatsValue);
        PlayerStatsStorage expectedResult = GenerateStorage(firstStorageStatsValue + secondStorageStatsValue);

        PlayerStatsStorage additionResult = firstStorage + secondStorage;

        Assert.AreEqual(expectedResult, additionResult);

    }

    private PlayerStatsStorage GenerateStorage(int statsValue)
    {
        PlayerStatsStorage newStorage = new PlayerStatsStorage();
        newStorage.DamageMultiplier = statsValue;
        newStorage.ObjectManipulationCooldown = statsValue;
        newStorage.CaptureZoneRadius = statsValue;
        newStorage.MovementSpeed = statsValue;
        newStorage.JumpStrength = statsValue;
        newStorage.CriticalHitMultiplier = statsValue;
        newStorage.CriticalHitChance = statsValue;
        newStorage.HealthCount = statsValue;
        newStorage.Resistance = statsValue;
        newStorage.Regeneration = statsValue;
        return newStorage;
    }
}
