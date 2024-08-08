using UnityEngine;

/// <summary>
/// This class is derivative of <see cref="DamageHandler"/> abstract class.
/// Handles damage from projectiles.
/// This class requires an 
/// <see cref="Rigidbody2D"/> component to be attached to the same GameObject.
/// </summary>
/// <remarks>
/// It should be added to <c>GameObject intended to take damage<c/> in order to make 
/// it process incoming damage from projectiles.
/// </remarks>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EntityHealthManager))]
public class ProjectileDamageHandler : DamageHandler
{
    [SerializeField] private LayerMask _projectilesLayers;
    private EntityHealthManager _healthManager;
    
    /// <summary>
    /// Initializes variables necessary for the proper functioning of the class.
    /// </summary>
    private void Start()
    {
        _healthManager = GetComponent<EntityHealthManager>();
    }

    /// <summary>
    /// Checks if the GameObject collides with a projectiles. 
    /// If so, calculates and transfers damage to the <see cref="EntityHealthManager"/>.
    /// </summary>
    /// <param name="collision">The collision data associated with this collision event.</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject collidedObject = collision.gameObject;
        ProjectileManager projectileManager = null;
        bool collidedObjectIsValidProjectile = _projectilesLayers.Contains(collidedObject.layer)
                        && collidedObject.TryGetComponent(out projectileManager);
        if (collidedObjectIsValidProjectile)
        {
            int currentDamage = (int)Mathf.Clamp(projectileManager.ProjectileProperties.Damage - GetCurrentResistanceAmount()
                , 0, int.MaxValue);
            _healthManager.ProcessDamage(currentDamage);
            projectileManager.PerformSelfDestroy();
        }
    }
}