using UnityEngine;

public class PlayerOnCreateActivityManager : MonoBehaviour
{
    private void Awake()
    {
        PlayerStatusInformer.PlayerGameObject = gameObject;
    }
}
