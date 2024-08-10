using NUnit.Framework;

public class StatsModifierSlotTest
{
    [Test]
    public void GetUnifiedStatsModifier_PlayerStatsStorageTransformation_AllStatsMultipliedByCount()
    {
        float baseStatsValue = 6.7f;
        int count = 5;
        PlayerStatsStorage playerStatsStorage = PlayerStatsStorageTest.GenerateStorage(baseStatsValue);
        StatsModifierSlot statsModifierSlot = new StatsModifierSlot(0, count, playerStatsStorage);

        PlayerStatsStorage expectedResult = playerStatsStorage * count;

        Assert.AreEqual(expectedResult, statsModifierSlot.GetUnifiedStatsModifier());
    }
}
