using UnityEngine;

public abstract class PossessableObject : MonoBehaviour
{
    private PossessableObjectStatsStorage statsStorage = new PossessableObjectStatsStorage();

    public PossessableObjectStatsStorage StatsStorage { get => statsStorage; set => statsStorage = value; }

    public abstract void ProcessManipulation(Vector3 direction, float power);

    public abstract float GetContactDamage();
}
