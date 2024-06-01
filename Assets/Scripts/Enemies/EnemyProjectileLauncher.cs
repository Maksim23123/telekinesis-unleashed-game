using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileLauncher : MonoBehaviour
{
    public event Action<bool> _targetVisibilityChanged; 

    [SerializeField]
    GameObject _projectile;
    [SerializeField]
    GameObject _target;
    [SerializeField]
    float _shootingCooldownTime;
    [SerializeField]
    LayerMask _notTransparentLayers;

    bool _shootingAllowed = true;

    private bool _targetIsVisible;

    public bool TargetIsVisible 
    { 
        get
        {
            return _targetIsVisible;
        } 

        private set
        {
            _targetIsVisible = value;
            _targetVisibilityChanged?.Invoke(_targetIsVisible);
        } 
    }



    // Start is called before the first frame update
    void Start()
    {
        _target = PlayerObjectManager.Instance.gameObject;
    }

    private void FixedUpdate()
    {
        Vector2 direction = (_target.transform.position - transform.position).normalized;
        TestIfTargetIsVisible(direction);

        if (_shootingAllowed && TargetIsVisible)
        {
            
            Shot(direction);
            _shootingAllowed = false;
            Invoke("ResetShootingPermission", _shootingCooldownTime);
        }
    }

    private void Shot(Vector2 direction)
    {
        
        GameObject newProjectile = Instantiate(_projectile, transform);

        if (newProjectile.TryGetComponent(out ProjectileManager projectileManager))
        {
            projectileManager.Launch(direction);
        }
    }
    
    private void ResetShootingPermission()
    {
        _shootingAllowed = true;
    }

    private void TestIfTargetIsVisible(Vector2 direction)
    {
        RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, direction, 20, _notTransparentLayers);
        if (raycastHit.transform != null && raycastHit.transform.gameObject == _target)
            TargetIsVisible = true;
        else
            TargetIsVisible = false;
    }
}
