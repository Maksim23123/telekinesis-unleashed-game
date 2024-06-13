using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOnCreateActivityManager : MonoBehaviour
{
    private void Awake()
    {
        PlayerStatusInformer.PlayerGameObject = gameObject;
    }
}
