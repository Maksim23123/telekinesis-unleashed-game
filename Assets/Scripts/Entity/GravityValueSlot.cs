using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityValueSlot : IComparer<GravityValueSlot>
{
    int _slotId;

    int _priority;

    float _value;

    public GravityValueSlot(int slotId, int priority, float value)
    {
        SlotId = slotId;
        Priority = priority;
        Value = value;
    }

    public int SlotId { get => _slotId; set => _slotId = value; }
    public int Priority { get => _priority; set => _priority = value; }
    public float Value { get => _value; set => _value = value; }

    public int Compare(GravityValueSlot x, GravityValueSlot y)
    {
        if (x == null || y == null)
        {
            throw new ArgumentException("Cannot compare null values");
        }

        return x.Priority.CompareTo(y.Priority);
    }
}
