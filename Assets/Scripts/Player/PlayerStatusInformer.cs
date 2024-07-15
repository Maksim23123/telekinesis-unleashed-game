using System;
using UnityEngine;

public static class PlayerStatusInformer
{
    private static GameObject _playerGameObject;

    public static event Action PlayerDestroyed;
    public static event Action<GameObject> NewPlayerAssigned;

    public static GameObject PlayerGameObject
    {
        get => _playerGameObject;

        set
        {
            _playerGameObject = value;
            NewPlayerAssigned?.Invoke(_playerGameObject);
        }
    }

    public static void InformPlayerDestroyed()
    {
        PlayerDestroyed?.Invoke();
    }
}
