using System.Collections.Generic;
using UnityEngine;

public class TargetSlot : IComparer<TargetSlot>
{
    public int TargetPriority { get; set; }
    public TargetType TargetType { get; set; }
    public GameObject Target { get; set; }
    public bool TargetIsVisible { get; set; }

    public TargetSlot(int targetPriority, TargetType targetType, GameObject target)
    {
        TargetPriority = targetPriority;
        TargetType = targetType;
        Target = target;
    }

    public int Compare(TargetSlot x, TargetSlot y)
    {
        int comparisonResult = x.TargetPriority.CompareTo(y.TargetPriority);
        if (comparisonResult == 0)
            return 1;
        else
            return comparisonResult;
    }
}
