using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EntityHealthManager))]
public class ProjectileDamageHandler : DamageHandler
{
    EntityHealthManager _healthManager;

    LayerMask _projectilesLayers;

    private void Start()
    {
        _healthManager = GetComponent<EntityHealthManager>();
        _projectilesLayers = LayerMask.GetMask("Projectiles");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject contactedObject = collision.gameObject;
        if (_projectilesLayers.Contains(contactedObject.layer) 
                && contactedObject.TryGetComponent(out ProjectileManager projectileManager))
        {
            _healthManager.ProcessDamage((int)Mathf.Clamp(projectileManager.ProjectileProperties._damage - GetCurrentResistanceAmount(), 0, int.MaxValue));
            projectileManager.PerformSelfDestroy();
        }
    }
}
