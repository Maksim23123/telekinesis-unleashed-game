using UnityEngine;


/// <summary>
/// Manages complex relationships between components attached to a GameObject 
/// that is considered an Enemy.
/// </summary>
public class EnemyInstanceManager : MonoBehaviour
{
    [SerializeField] private EnemyMovementOperator _enemyMovementOperator;
    [SerializeField] private EnemyProjectileLauncher _enemyProjectileLauncher;

    private const string EXTERNAL_MOVEMENT_PERMISSION_TAG = "enemyVisibility";

    /// <summary>
    /// Initializes listeners for events that trigger relationship state updates.
    /// </summary>
    private void Start()
    {
        _enemyProjectileLauncher.TargetVisibilityChanged += SincShootingAndMovement;
    }

    /// <summary>
    /// Synchronizes the active state between <see cref="EnemyMovementOperator"/> and
    /// <see cref="EnemyProjectileLauncher"/>.
    /// </summary>
    /// <remarks>
    /// Transfers state from <see cref="EnemyProjectileLauncher"/> to <see cref="EnemyMovementOperator"/>.
    /// The active state of <see cref="EnemyProjectileLauncher"/> is considered inactive for <see cref="EnemyMovementOperator"/>.
    /// </remarks>
    /// <param name="targetIsVisible">Indicates whether the target is visible.</param>
    private void SincShootingAndMovement(bool targetIsVisible)
    {
        _enemyMovementOperator.SetExternalPermission(EXTERNAL_MOVEMENT_PERMISSION_TAG, !targetIsVisible);
    }
}
