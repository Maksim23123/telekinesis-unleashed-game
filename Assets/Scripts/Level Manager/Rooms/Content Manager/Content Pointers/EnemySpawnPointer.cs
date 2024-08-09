using System;
using UnityEngine;

[Serializable]
public class EnemySpawnPointer : ContentPointer
{
    public override GameObject ToGameObject(Vector2 centerPosition, GameObject _defaultPointer, Transform _pointerContainer)
    {
        GameObject newPointer = GameObject.Instantiate(_defaultPointer, _pointerContainer);
        newPointer.transform.position = centerPosition + RelativePosition;
        EnemySpawnGameObjectPointer enemySpawnPointerGameObject = newPointer.AddComponent<EnemySpawnGameObjectPointer>();
        return newPointer;
    }

    public override void ActivatePointerAction(Vector2 centerPosition)
    {
        Debug.Log("Summoning enemies");
    }

    public override void PerformPointerActionCleanUp()
    {
        Debug.Log("Hiding enemies");
    }
}
