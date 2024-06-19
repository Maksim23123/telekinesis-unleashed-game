using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class TargetSlot : IComparer<TargetSlot>
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

    public int Compare(TargetSlot x, TargetSlot y)
    {
        int comparisonResult = x.targetPriority.CompareTo(y.targetPriority);
        if (comparisonResult == 0)
            return 1;
        else
            return comparisonResult;
    }
}
