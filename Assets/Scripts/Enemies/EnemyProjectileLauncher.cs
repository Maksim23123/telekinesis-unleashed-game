using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyTargetManager))]
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
    float _maxSpread;

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
            if (_targetIsVisible != value) 
            {
                _targetIsVisible = value;
                _targetVisibilityChanged?.Invoke(_targetIsVisible);
            }
        } 
    }



    // Start is called before the first frame update
    void Start()
    {
        EnemyTargetManager enemyTargetManager = GetComponent<EnemyTargetManager>();
        enemyTargetManager.activeTargetReassigned += AssignTarget;
    }

    void AssignTarget(GameObject target, bool isVisible)
    {
        _target = target;
        TargetIsVisible = isVisible;
    }

    private void FixedUpdate()
    {
        if (_target != null && _shootingAllowed && TargetIsVisible) 
        {
            Vector2 direction = (_target.transform.position - transform.position).normalized;
            float _currentSpread = _maxSpread * UnityEngine.Random.value - _maxSpread / 2;
            direction = Quaternion.AngleAxis(_currentSpread, Vector3.forward) * direction;

            Shot(direction);
            _shootingAllowed = false;
            Invoke(nameof(ResetShootingPermission), _shootingCooldownTime);
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
}
