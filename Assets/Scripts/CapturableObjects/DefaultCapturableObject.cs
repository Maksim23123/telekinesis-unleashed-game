using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using UnityEngine;

public class DefaultCapturableObject : CapturableObject
{
    public override void ProcessManipulation(Vector3 direction, float power)
    {
        if (TryGetComponent(out Rigidbody2D rigidbody))
        {
            rigidbody.AddForce(direction * power, ForceMode2D.Impulse);
        }
    }
}
