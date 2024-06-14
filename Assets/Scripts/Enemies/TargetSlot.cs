using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSlot
{
    public int targetPriority;

    public TargetType targetType;

    public GameObject target;

    public bool targetIsVisible = true;

    public TargetSlot(int targetPriority, TargetType targetType, GameObject target)
    {
        this.targetPriority = targetPriority;
        this.targetType = targetType;
        this.target = target;
    }
}
