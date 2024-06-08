using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHandler : MonoBehaviour
{
    private float _resistance;

    public float Resistance { get => _resistance; set => _resistance = value; }

    protected float GetCurrentResistanceAmount()
    {
        return Random.value * Resistance;
    }
}
