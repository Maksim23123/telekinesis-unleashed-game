using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EntityHealthManager))]
public class ProjectileDamageHandler : MonoBehaviour
{
    EntityHealthManager _healthManager;

    LayerMask _projectilesLayers;

    private void Start()
    {
        if (_healthManager == null)
        {
            _healthManager = GetComponent<EntityHealthManager>();
        }
        _projectilesLayers = LayerMask.GetMask("Projectiles");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject contactedObject = collision.gameObject;
        if (_projectilesLayers.Contains(contactedObject.layer) 
                && contactedObject.TryGetComponent(out ProjectileManager projectileManager))
        {
            _healthManager.ProcessDamage((int)projectileManager.ProjectileProperties._damage);
            projectileManager.PerformSelfDestroy();
        }
    }
}
