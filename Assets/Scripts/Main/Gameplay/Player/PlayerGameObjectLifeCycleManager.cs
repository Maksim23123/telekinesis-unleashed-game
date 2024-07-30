using UnityEngine;

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
