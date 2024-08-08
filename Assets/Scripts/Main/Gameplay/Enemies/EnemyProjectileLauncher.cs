using System;
using UnityEngine;

/// <summary>
/// This class is responsible for launching projectiles.
/// It shoots if the target is visible and shooting is allowed.
/// </summary>
[RequireComponent(typeof(EnemyTargetManager))] 
public class EnemyProjectileLauncher : MonoBehaviour
{
    [SerializeField] private GameObject _projectile;
    [SerializeField] private GameObject _target;
    [SerializeField] private float _shootingCooldownTime;
    [SerializeField] private float _maxSpread;

    private bool _shootingAllowed = true;
    private bool _targetIsVisible;

    public event Action<bool> TargetVisibilityChanged;

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
                TargetVisibilityChanged?.Invoke(_targetIsVisible);
            }
        }
    }

    /// <summary>
    /// Initializes variables necessary for the class.
    /// </summary>
    private void Start()
    {
        EnemyTargetManager enemyTargetManager = GetComponent<EnemyTargetManager>();
        enemyTargetManager.ActiveTargetReAssigned += AssignTarget;
    }

    /// <summary>
    /// Allows external logic to assign a new target.
    /// </summary>
    /// <param name="target">The target to shoot at.</param>
    /// <param name="isVisible">Target visibility</param>
    private void AssignTarget(GameObject target, bool isVisible)
    {
        _target = target;
        TargetIsVisible = isVisible;
    }

    /// <summary>
    /// Periodically checks conditions for shooting and invokes shooting logic if conditions are met.
    /// </summary>
    private void FixedUpdate()
    {
        if (_target != null && _shootingAllowed && TargetIsVisible)
        {
            Vector2 direction = CalculateShootingDirection();

            Shot(direction);
            _shootingAllowed = false;
            Invoke(nameof(ResetShootingPermission), _shootingCooldownTime);
        }
    }

    /// <summary>
    /// Calculates the shooting direction based on the target and GameObject positions.
    /// </summary>
    /// <returns>Shooting direction.</returns>
    private Vector2 CalculateShootingDirection()
    {
        Vector2 direction = (_target.transform.position - transform.position).normalized;
        float currentSpread = _maxSpread * UnityEngine.Random.value - _maxSpread / 2;
        direction = Quaternion.AngleAxis(currentSpread, Vector3.forward) * direction;
        return direction;
    }


    /// <summary>
    /// Launches a projectile in the specified direction.
    /// </summary>
    /// <param name="direction">Direction to shoot.</param>
    private void Shot(Vector2 direction)
    {
        GameObject newProjectile = Instantiate(_projectile, transform);

        if (newProjectile.TryGetComponent(out ProjectileManager projectileManager))
        {
            projectileManager.Launch(direction);
        }
    }

    /// <summary>
    /// Restores shooting permission after cooldown.
    /// </summary>
    private void ResetShootingPermission()
    {
        _shootingAllowed = true;
    }
}
