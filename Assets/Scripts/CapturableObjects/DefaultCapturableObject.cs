using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DefaultCapturableObject : CapturableObject
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
        return Vector2.Distance(Vector2.zero, _rigidbody.velocity) * _rigidbody.mass;
    }

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }
}
