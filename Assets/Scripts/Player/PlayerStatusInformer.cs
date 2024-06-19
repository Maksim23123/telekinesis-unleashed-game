using System;
using UnityEngine;

public static class PlayerStatusInformer
{
    private static GameObject _playerGameObject;

    public static GameObject PlayerGameObject
    {
        get => _playerGameObject;

        set
        {
            _playerGameObject = value;
            newPlayerAssigned?.Invoke(_playerGameObject);
        }
    }

    public static event Action playerDestroyed;
    public static event Action<GameObject> newPlayerAssigned;

    public static void PlayerDestroyed()
    {
        playerDestroyed?.Invoke();
    }
}
