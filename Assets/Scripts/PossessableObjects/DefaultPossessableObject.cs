using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DefaultPossessableObject : PossessableObject
{
    Rigidbody2D _rigidbody;

    public override void ProcessManipulation(Vector3 direction, float power)
    {
        if (TryGetComponent(out Rigidbody2D rigidbody))
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.AddForce(direction * power, ForceMode2D.Impulse);
        }
    }

    public override float GetContactDamage()
    {
        float aditionalMultiplier = 1;

        if (Random.value < StatsStorage._criticalHitChance)
        {
            aditionalMultiplier = StatsStorage._criticalMultiplier;

            //DEBUG
            // Debug.Log("Crit");
        }
        //DEBUG
        // Debug.Log(Vector2.Distance(Vector2.zero, _rigidbody.velocity) * _rigidbody.mass * StatsStorage._damageMultiplier * aditionalMultiplier);

        return Vector2.Distance(Vector2.zero, _rigidbody.velocity) * _rigidbody.mass * StatsStorage._damageMultiplier * aditionalMultiplier;
    }

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }
}
