using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// This class tracks potential targets nearby. It also check target visibility.
/// </summary>
/// <remarks>
/// Sends all the information about targets to <c>ActivateTargetReAssigned</c> event subscribers.
/// </remarks>
public class EnemyTargetManager : MonoBehaviour
{
    [SerializeField] private LayerMask _notTransparentLayers;
    [SerializeField] private float switchToMinorDistance;

    private List<TargetSlot> _targetSlots = new();
    private TargetSlot previousMostPriorSlot;

    public event Action<GameObject, bool> ActiveTargetReAssigned;

    /// <summary>
    /// Subscribes to NewPlayerAssigned event, as the player object is the main target.
    /// </summary>
    private void Start()
    {
        PlayerStatusInformer.NewPlayerAssigned += UpdateTargets;
    }

    /// <summary>
    /// Periodically updates target references.
    /// </summary>
    private void FixedUpdate()
    {
        if (PlayerStatusInformer.PlayerGameObject != null)
            UpdateTargets(PlayerStatusInformer.PlayerGameObject);
    }

    /// <summary>
    /// Chooses the target with the highest priority and informs subscribers.
    /// </summary>
    private void AssignTarget()
    {
        TargetSlot mostPriorSlot = _targetSlots
            .OrderBy(targetSlot => targetSlot.TargetPriority)
            .FirstOrDefault();

        bool currentTargetDifferentFromPrevious = (previousMostPriorSlot == null
                        || previousMostPriorSlot.Target != mostPriorSlot.Target
                        || previousMostPriorSlot.TargetIsVisible != mostPriorSlot.TargetIsVisible);
        if (mostPriorSlot != null && currentTargetDifferentFromPrevious)
        {
            previousMostPriorSlot = mostPriorSlot;
            ActiveTargetReAssigned?.Invoke(mostPriorSlot.Target, mostPriorSlot.TargetIsVisible);
        }
    }

    /// <summary>
    /// Updates available targets in the targetSlots list.
    /// </summary>
    /// <param name="mainTarget">The main target, usually the Player GameObject.</param>
    private void UpdateTargets(GameObject mainTarget)
    {
        _targetSlots.Clear();
        _targetSlots.Add(new TargetSlot(1, TargetType.Main, mainTarget));

        RecalculateTargetsPriority();
        AssignTarget();
    }

    /// <summary>
    /// Recalculates priority for all the targets in the targetSlots list.
    /// </summary>
    private void RecalculateTargetsPriority()
    {
        List<TargetSlot> targetSlotsCopy = _targetSlots.ToList();
        _targetSlots.Clear();
        foreach (TargetSlot slot in targetSlotsCopy) 
        {
            _targetSlots.Add(CalculatePriorityForTarget(slot));
        }
    }

    /// <summary>
    /// Calculates priority for the specified target.
    /// </summary>
    /// <param name="targetSlot">The specified target.</param>
    /// <returns>Copy of the specified target with a new priority value.</returns>
    private TargetSlot CalculatePriorityForTarget(TargetSlot targetSlot)
    {
        targetSlot.TargetIsVisible = true;
        if (targetSlot.TargetType == TargetType.Main && TestIfTargetIsVisible(targetSlot))
        {
            targetSlot.TargetPriority = 1;
        }
        else if (targetSlot.TargetType == TargetType.Minor && TestIfTargetIsVisible(targetSlot))
        {
            targetSlot.TargetPriority = 2;
        }
        else
        {
            targetSlot.TargetPriority = 0;
            targetSlot.TargetIsVisible = false;
        }

        return targetSlot;
    }

    /// <summary>
    /// Performs a circle cast to measure target visibility.
    /// </summary>
    /// <param name="targetSlot">The specified target.</param>
    /// <returns>Target visibility value.</returns>
    private bool TestIfTargetIsVisible(TargetSlot targetSlot)
    {
        bool isVisible = false;
        GameObject target = targetSlot.Target;

        Vector2 objectPosition = gameObject.transform.position;
        Vector2 direction = (target.transform.position - transform.position).normalized;

        float raySize = 20;
        if (targetSlot.TargetType == TargetType.Minor)
        {
            raySize = switchToMinorDistance;
        }

        if (Vector2.Distance(objectPosition, target.transform.position) <= raySize)
        {
            float rayThickness = 0.4f;
            RaycastHit2D[] hits = Physics2D.CircleCastAll(objectPosition, rayThickness / 2, direction, raySize, _notTransparentLayers);

            DebugDrawCircleCast(objectPosition, new Vector2(rayThickness, rayThickness)
                , Vector2.Angle(Vector2.right, direction), direction, raySize, Color.red);

            if (hits.Length >= 1 && hits[0].transform.gameObject == target)
            {
                isVisible = true;
            }

        }
        return isVisible;
    }

    /// <summary>
    /// Debug feature to show circle cast in Scene view.
    /// </summary>
    /// <param name="color">Color of the displayed cast.</param>
    /// <remarks>
    /// All parameters (except "color") should be transferred from the circle cast it displays.
    /// </remarks>
    private void DebugDrawCircleCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, Color color)
    {
        Vector2[] corners = new Vector2[4];
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        corners[0] = origin + (Vector2)(rotation * new Vector2(-size.x / 2, -size.y / 2));
        corners[1] = origin + (Vector2)(rotation * new Vector2(size.x / 2, -size.y / 2));
        corners[2] = origin + (Vector2)(rotation * new Vector2(size.x / 2, size.y / 2));
        corners[3] = origin + (Vector2)(rotation * new Vector2(-size.x / 2, size.y / 2));

        Debug.DrawLine(corners[0], corners[1], color);
        Debug.DrawLine(corners[1], corners[2], color);
        Debug.DrawLine(corners[2], corners[3], color);
        Debug.DrawLine(corners[3], corners[0], color);

        Vector2 endPoint = origin + direction.normalized * distance;
        Debug.DrawLine(origin, endPoint, color);

        Vector2 endOrigin = endPoint;
        Vector2[] endCorners = new Vector2[4];

        endCorners[0] = endOrigin + (Vector2)(rotation * new Vector2(-size.x / 2, -size.y / 2));
        endCorners[1] = endOrigin + (Vector2)(rotation * new Vector2(size.x / 2, -size.y / 2));
        endCorners[2] = endOrigin + (Vector2)(rotation * new Vector2(size.x / 2, size.y / 2));
        endCorners[3] = endOrigin + (Vector2)(rotation * new Vector2(-size.x / 2, size.y / 2));

        Debug.DrawLine(endCorners[0], endCorners[1], color);
        Debug.DrawLine(endCorners[1], endCorners[2], color);
        Debug.DrawLine(endCorners[2], endCorners[3], color);
        Debug.DrawLine(endCorners[3], endCorners[0], color);
    }

    /// <summary>
    /// Automatically unsubscribes from all subscriptions on destroy.
    /// </summary>
    private void OnDestroy()
    {
        PlayerStatusInformer.NewPlayerAssigned -= UpdateTargets;
    }
}
