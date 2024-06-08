using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CapturableObject : MonoBehaviour
{
    private CapturableObjectStatsStorage statsStorage = new CapturableObjectStatsStorage();

    public CapturableObjectStatsStorage StatsStorage { get => statsStorage; set => statsStorage = value; }

    public abstract void ProcessManipulation(Vector3 direction, float power);

    public abstract float GetContactDamage();
}
