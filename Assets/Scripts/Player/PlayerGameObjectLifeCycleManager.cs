using UnityEngine;

/// <summary>
/// Manages the player's GameObject lifecycle by performing actions on its creation and destruction.
/// </summary>
public class PlayerGameObjectLifeCycleManager : MonoBehaviour
{
    private void Awake()
    {
        PlayerStatusInformer.PlayerGameObject = gameObject;
    }

    private void OnDestroy()
    {
        PlayerStatusInformer.InformPlayerDestroyed();
    }
}
