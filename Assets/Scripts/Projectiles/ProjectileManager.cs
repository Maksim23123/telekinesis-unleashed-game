using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    [SerializeField]  
    ProjectileProperties _projectileProperties;

    public void Launch(Vector2 direction)
    {
        if (TryGetComponent(out Rigidbody2D rigidbody) && _projectileProperties != null)
        {
            rigidbody.AddForce(direction * _projectileProperties._impulsPower, ForceMode2D.Impulse);
        }
    }

    private void Awake()
    {
        if (_projectileProperties != null)
        {
            Invoke(nameof(PerformSelfDestroy), _projectileProperties._lifeTime);
        }
    }

    private void PerformSelfDestroy()
    {
        Destroy(gameObject);
    }
}
