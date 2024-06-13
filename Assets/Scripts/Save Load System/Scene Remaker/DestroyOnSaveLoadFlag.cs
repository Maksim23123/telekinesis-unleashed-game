using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnSaveLoadFlag : MonoBehaviour
{
    private void Awake()
    {
        SceneRemaker.preRemakeActivity += DestroyItself;
    }

    private void DestroyItself()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        SceneRemaker.preRemakeActivity -= DestroyItself;
        PlayerStatusInformer.PlayerDestroyed();
    }
}
