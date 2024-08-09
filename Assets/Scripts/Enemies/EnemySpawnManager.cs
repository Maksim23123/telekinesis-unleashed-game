using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
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
}
