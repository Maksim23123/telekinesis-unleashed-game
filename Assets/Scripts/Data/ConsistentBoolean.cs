using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsistentBoolean
{
    Queue<bool> _values;

    bool _priorityValue;

    int _poolSize;

    public bool PriorityValue { get => _priorityValue; set => _priorityValue = value; }

    public bool Value => _values.Contains(PriorityValue);

    public ConsistentBoolean(bool priorityValue, int poolSize)
    {
        PriorityValue = priorityValue;
        _poolSize = poolSize;
        _values = new Queue<bool>(poolSize);
    }

    public void AddValue(bool value)
    {
        if (_values.Count >= _poolSize)
        {
            _values.Dequeue();
        }
        _values.Enqueue(value);
        Debug.Log(_values.Count);
    }
}
