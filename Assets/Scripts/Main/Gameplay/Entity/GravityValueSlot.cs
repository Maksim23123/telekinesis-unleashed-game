using System;
using System.Collections.Generic;

/// <summary>
/// Represents a slot within the <see cref="GravityScaleManager"/>.
/// Contains gravity scale value and properties to help <see cref="GravityScaleManager"/> validate slots.
/// </summary>
public class GravityValueSlot : IComparer<GravityValueSlot>
{
    private int _slotId;
    private int _priority;
    private float _value;

    public int SlotId { get => _slotId; set => _slotId = value; }
    public int Priority { get => _priority; set => _priority = value; }
    public float Value { get => _value; set => _value = value; }

    public GravityValueSlot(int slotId, int priority, float value)
    {
        SlotId = slotId;
        Priority = priority;
        Value = value;
    }

    public int Compare(GravityValueSlot x, GravityValueSlot y)
    {
        if (x == null || y == null)
        {
            throw new ArgumentException("Cannot compare null values");
        }
        int comparisonResult = x.Priority.CompareTo(y.Priority);
        if (comparisonResult == 0)
        {
            comparisonResult = x.SlotId.CompareTo(y.SlotId);
        }
        return comparisonResult;
    }
}
