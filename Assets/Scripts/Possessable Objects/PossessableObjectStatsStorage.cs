public class PossessableObjectStatsStorage
{
    public float DamageMultiplier { get; set; }
    public float CriticalMultiplier { get; set; }
    public float CriticalHitChance { get; set; }

    public PossessableObjectStatsStorage(float damageMultiplier = 1, float criticalMultiplier = 1.5f, float criticalHitChance = 0.05f)
    {
        DamageMultiplier = damageMultiplier;
        CriticalMultiplier = criticalMultiplier;
        CriticalHitChance = criticalHitChance;
    }
}
