using UnityEngine;

public class ProjectileDamageHandler : DamageHandler
{
    private EntityHealthManager _healthManager;
    [SerializeField]
    private LayerMask _projectilesLayers;

    private void Start()
    {
        _healthManager = GetComponent<EntityHealthManager>();
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