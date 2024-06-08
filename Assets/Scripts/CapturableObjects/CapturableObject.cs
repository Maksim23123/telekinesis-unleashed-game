using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CapturableObject : MonoBehaviour
{
    private float _damageMultiplier;

    public float DamageMultiplier { get => _damageMultiplier; set => _damageMultiplier = value; }

    public abstract void ProcessManipulation(Vector3 direction, float power);

    public abstract float GetContactDamage();
}
