using UnityEngine;

/// <summary>
/// This class is derivative of <see cref="DamageHandler"/> abstract class.
/// Handles damage from possessable objects.
/// This class requires an 
/// <see cref="Rigidbody2D"/> component to be attached to the same GameObject.
/// </summary>
/// <remarks>
/// It should be added to <c>GameObject intended to take damage<c/> in order to make 
/// it process incoming damage from possessable objects.
/// </remarks>

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EntityHealthManager))]
public class PossesableObjectDamageHandler : DamageHandler
{
    private LayerMask DAMAGE_DIALER_LAYER_MASK;

    private EntityHealthManager _healthManager;

    /// <summary>
    /// Initializes variables necessary for the proper functioning of the class.
    /// </summary>
    private void Start()
    {
        _healthManager = GetComponent<EntityHealthManager>();
        DAMAGE_DIALER_LAYER_MASK = LayerMask.GetMask("Possessable Objects");
    }

    /// <summary>
    /// Checks if the GameObject collides with a Possessable Object. 
    /// If so, calculates and transfers damage to the <see cref="EntityHealthManager"/>.
    /// </summary>
    /// <param name="collision">The collision data associated with this collision event.</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        PossessableObject possessableObject = null;

        bool collisionObjectIsValidPossessableObject = DAMAGE_DIALER_LAYER_MASK.Contains(collision.gameObject.layer)
                        && collision.gameObject.TryGetComponent(out possessableObject);
        if (collisionObjectIsValidPossessableObject)
        {
            int currentDamage = (int)Mathf.Clamp(possessableObject.GetContactDamage() - GetCurrentResistanceAmount()
                , 0, int.MaxValue);
            _healthManager.ProcessDamage(currentDamage);
        }  
    }
}