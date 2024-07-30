using UnityEngine;

/// <summary>
/// Represents a slot for a target with related properties used in <see cref="EnemyTargetManager"/>.
/// This class is intended to be stored and used within the <see cref="EnemyTargetManager"/>.
/// </summary>
public class TargetSlot
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
}
