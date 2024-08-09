using System;
using UnityEngine;

[Serializable]
public class EnemySpawnPointer : ContentPointer
{
    [SerializeField] private GameObject _enemyPrefab;

    public EnemySpawnPointer(GameObject enemyPrefab)
    {
        _enemyPrefab = enemyPrefab;
    }

    public override GameObject ToGameObject(Vector2 centerPosition)
    {
        throw new NotImplementedException();
    }

    public override void ActivatePointerAction(Vector2 centerPosition)
    {
        if (!IsUsed)
        {
            GameObject.Instantiate(_enemyPrefab, centerPosition + RelativePosition, Quaternion.identity);
            IsUsed = true;
        }
    }
}
