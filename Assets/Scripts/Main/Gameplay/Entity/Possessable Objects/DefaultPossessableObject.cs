using UnityEngine;

/// <summary>
/// Class to be added as a componnent to PossessableObjects.
/// Defines basic PossessableObject behaviour.
/// Requires <see cref="Rigidbody2D"> to be attached to the same GameObject.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class DefaultPossessableObject : PossessableObject
{
    private Rigidbody2D _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Gives an impuls to a PossessableObject in a certain direction.
    /// </summary>
    /// <param name="direction">The direction of the impuls.</param>
    /// <param name="power">The power of the impuls.</param>
    public override void ProcessManipulation(Vector3 direction, float power)
    {
        if (TryGetComponent(out Rigidbody2D rigidbody))
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.AddForce(direction * power, ForceMode2D.Impulse);
        }
    }

    /// <summary>
    /// Calculates the damage of the current PossessableObject.
    /// </summary>
    /// <returns>The floating-point damage value.</returns>
    public override float GetContactDamage()
    {
        float aditionalMultiplier = 1;

        if (Random.value < StatsStorage.CriticalHitChance)
        {
            aditionalMultiplier = StatsStorage.CriticalMultiplier;
        }
        return Vector2.Distance(Vector2.zero, _rigidbody.velocity) * _rigidbody.mass * StatsStorage.DamageMultiplier * aditionalMultiplier;
    }
}
