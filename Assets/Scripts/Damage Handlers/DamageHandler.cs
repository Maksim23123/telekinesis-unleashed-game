using UnityEngine;

/// <summary>
/// Abstract class for handling damage. This class requires an 
/// <see cref="EntityHealthManager"/> component to be attached to the same GameObject.
/// </summary>
/// <remarks>
/// The <c>DamageHandler</c> class provides a base implementation for damage handling,
/// including a resistance property that can be modified. It also includes a method to 
/// calculate a random resistance amount based on the current resistance value.
/// </remarks>
[RequireComponent(typeof(EntityHealthManager))]
public abstract class DamageHandler : MonoBehaviour
{
    private float _resistance;

    public float Resistance { get => _resistance; set => _resistance = value; }

    /// <summary>
    /// Calculates a random resistance amount based on the current resistance value.
    /// </summary>
    /// <returns>A random resistance amount.</returns>
    protected float GetCurrentResistanceAmount()
    {
        return Random.value * Resistance;
    }
}
