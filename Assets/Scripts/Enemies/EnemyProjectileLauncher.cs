using System;
using UnityEngine;

[RequireComponent(typeof(EnemyTargetManager))]
public class EnemyProjectileLauncher : MonoBehaviour
{
    [SerializeField]
    private GameObject _projectile;
    [SerializeField]
    private GameObject _target;
    [SerializeField]
    private float _shootingCooldownTime;
    [SerializeField]
    private float _maxSpread;
    private bool _shootingAllowed = true;
    private bool _targetIsVisible;

    public event Action<bool> _targetVisibilityChanged;

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

    private void Start()
    {
        EnemyTargetManager enemyTargetManager = GetComponent<EnemyTargetManager>();
        enemyTargetManager.activeTargetReAssigned += AssignTarget;
    }

    private void AssignTarget(GameObject target, bool isVisible)
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
