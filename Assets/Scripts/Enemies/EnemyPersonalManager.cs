using UnityEngine;

public class EnemyInstanceManager : MonoBehaviour
{
    [SerializeField] private EnemyMovementOperator _enemyMovementOperator;
    [SerializeField] private EnemyProjectileLauncher _enemyProjectileLauncher;

    private const string EXTERNAL_MOVEMENT_PERMISSION_TAG = "enemyVisibility";

    private void Start()
    {
        _enemyProjectileLauncher._targetVisibilityChanged += SincShootingAndMovement;
    }

    private void SincShootingAndMovement(bool targetIsVisible)
    {
        _enemyMovementOperator.SetExternalPermission(EXTERNAL_MOVEMENT_PERMISSION_TAG, !targetIsVisible);
    }
}
