using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EntityHealthManager))]
public class PossesableObjectDamageHandler : MonoBehaviour
{
    EntityHealthManager _healthManager;
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out CapturableObject capturableObject))
        {
            _healthManager.ProcessDamage((int)capturableObject.GetContactDamage());
        }
    }

    private void Start()
    {
        _healthManager = GetComponent<EntityHealthManager>();
    }
}
