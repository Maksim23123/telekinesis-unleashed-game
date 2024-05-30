using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPersonalManager : MonoBehaviour
{
    [SerializeField]
    EnemyMovementOperator _enemyMovementOperator;
    [SerializeField]
    EnemyProjectileLauncher _enemyProjectileLauncher;

    const string EXTERNAL_MOVEMENT_PERMISSION_TAG = "enemyVisibility";

    private void Start()
    {
        _enemyProjectileLauncher._targetVisibilityChanged += SincShootingAndMovement;
    }

    private void SincShootingAndMovement(bool targetIsVisible)
    {
        _enemyMovementOperator.SetExternalPermission(EXTERNAL_MOVEMENT_PERMISSION_TAG, !targetIsVisible);
    }
}
