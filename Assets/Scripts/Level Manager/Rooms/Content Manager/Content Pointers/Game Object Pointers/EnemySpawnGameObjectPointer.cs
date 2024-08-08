using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnGameObjectPointer : GameObjectPointer
{
    [SerializeField] GameObject _enemyPrefab;

    public override ContentPointer ToRegularPointer(Vector2 centerPosition)
    {
        EnemySpawnPointer pointer = new EnemySpawnPointer(_enemyPrefab);
        pointer.RelativePosition = (Vector2)gameObject.transform.position - centerPosition;
        return pointer;
    }
}
