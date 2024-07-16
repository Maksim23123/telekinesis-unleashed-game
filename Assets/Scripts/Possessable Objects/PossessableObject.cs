using UnityEngine;

public abstract class PossessableObject : MonoBehaviour
{
    private PossessableObjectStatsStorage _statsStorage = new PossessableObjectStatsStorage();

    public PossessableObjectStatsStorage StatsStorage { get => _statsStorage; set => _statsStorage = value; }

    public abstract void ProcessManipulation(Vector3 direction, float power);

    public abstract float GetContactDamage();
}
