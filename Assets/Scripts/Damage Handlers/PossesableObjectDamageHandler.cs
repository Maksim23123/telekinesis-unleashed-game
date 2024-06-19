using UnityEngine;

public class PossesableObjectDamageHandler : DamageHandler
{
    private EntityHealthManager _healthManager;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out PossessableObject capturableObject))
            _healthManager.ProcessDamage((int)Mathf.Clamp(capturableObject.GetContactDamage() - GetCurrentResistanceAmount(), 0, int.MaxValue));
    }

    private void Start()
    {
        _healthManager = GetComponent<EntityHealthManager>();
    }
}