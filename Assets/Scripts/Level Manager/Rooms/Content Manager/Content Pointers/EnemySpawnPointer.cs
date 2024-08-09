using System;
using UnityEngine;

[Serializable]
public class EnemySpawnPointer : ContentPointer
{
    [SerializeField] private GameObject _enemyPrefab;

    public GameObject EnemyPrefab { get => _enemyPrefab; private set => _enemyPrefab = value; }

    public EnemySpawnPointer(GameObject enemyPrefab)
    {
        _enemyPrefab = enemyPrefab;
    }

    public override GameObject ToGameObject(Vector2 centerPosition, GameObject _defaultPointer, Transform _pointerContainer)
    {
        GameObject newPointer = GameObject.Instantiate(_defaultPointer, _pointerContainer);
        newPointer.transform.position = centerPosition + RelativePosition;
        EnemySpawnGameObjectPointer enemySpawnPointerGameObject = newPointer.AddComponent<EnemySpawnGameObjectPointer>();
        enemySpawnPointerGameObject.SetValuesFromPointer(this);
        return newPointer;
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
