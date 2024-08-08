using System;
using UnityEngine;

/// <summary>
/// Gathers information about the player object and its status.
/// Other classes can subscribe to this class to get a reference to the player GameObject.
/// </summary>
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

    /// <summary>
    /// Notifies that the player GameObject is being destroyed.
    /// </summary>
    public static void InformPlayerDestroyed()
    {
        PlayerDestroyed?.Invoke();
    }
}
