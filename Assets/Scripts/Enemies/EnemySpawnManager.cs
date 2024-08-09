using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    [SerializeField] GameObject _enemyPrefab;

    private Dictionary<RoomContentManager, List<ContentPointer>> _roomToEnemySpawnPointers = new();

    private Dictionary<RoomContentManager, List<GameObject>> _roomToEnemiesGameObjects = new();

    private static EnemySpawnManager _instance;

    public static EnemySpawnManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject newGameObject = new GameObject("Input Handler");
                newGameObject.AddComponent<EnemySpawnManager>();
            }

            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }

    public void AddEnemySpawnPoint(RoomContentManager pointerContainer, ContentPointer pointer)
    {
        if (!_roomToEnemySpawnPointers.ContainsKey(pointerContainer))
        {
            _roomToEnemySpawnPointers[pointerContainer] = new List<ContentPointer>();
        }
        _roomToEnemySpawnPointers[pointerContainer].Add(pointer);
    }

    public void ActivateEnemies(RoomContentManager pointerContainer)
    {
        if (_roomToEnemySpawnPointers.ContainsKey(pointerContainer))
        {
            if (_roomToEnemiesGameObjects.ContainsKey(pointerContainer))
            {
                foreach (GameObject enemyObject in _roomToEnemiesGameObjects[pointerContainer].ToList())
                {

                    if (enemyObject != null)
                    {
                        enemyObject.SetActive(true);
                    }
                    else
                    {
                        _roomToEnemiesGameObjects[pointerContainer].Remove(enemyObject);
                    }
                }
            }
            else
            {
                _roomToEnemiesGameObjects[pointerContainer] = new();
                foreach (ContentPointer pointer in _roomToEnemySpawnPointers[pointerContainer])
                {
                    GameObject newEnemy = Instantiate(_enemyPrefab, pointer.WorldPosition, Quaternion.identity);
                    _roomToEnemiesGameObjects[pointerContainer].Add(newEnemy);
                }
            }
        }
    }

    public void DeactivateEnemies(RoomContentManager pointerContainer)
    {
        if (_roomToEnemiesGameObjects.ContainsKey(pointerContainer))
        {
            foreach (GameObject enemyObject in _roomToEnemiesGameObjects[pointerContainer].ToList())
            {
                if (enemyObject != null)
                {
                    enemyObject.SetActive(false);
                }
                else
                {
                    _roomToEnemiesGameObjects[pointerContainer].Remove(enemyObject);
                }
            }
        }
    }
}
