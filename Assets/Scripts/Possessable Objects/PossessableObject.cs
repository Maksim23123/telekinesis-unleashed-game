using UnityEngine;

/// <summary>
/// This class is the base abstract class for all PossessableObject classes.
/// It contains an instance of <see cref="PossessableObjectStatsStorage"/> to store the PossessableObject stats.
/// This class also provides some basic methods to handle different in-game events.
/// </summary>
public abstract class PossessableObject : MonoBehaviour
{
    private PossessableObjectStatsStorage _statsStorage = new PossessableObjectStatsStorage();

    public PossessableObjectStatsStorage StatsStorage { get => _statsStorage; set => _statsStorage = value; }

    /// <summary>
    /// Overridable method to process a player manipulation request.
    /// </summary>
    /// <param name="direction">The direction of the player input.</param>
    /// <param name="power">The power of the player input.</param>
    public abstract void ProcessManipulation(Vector3 direction, float power);

    /// <summary>
    /// Overridable method to calculate the damage of the current PossessableObject.
    /// </summary>
    /// <returns>The floating-point damage value.</returns>
    public abstract float GetContactDamage();
}
