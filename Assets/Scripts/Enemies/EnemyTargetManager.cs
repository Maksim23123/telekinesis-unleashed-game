using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyTargetManager : MonoBehaviour
{

    [SerializeField]
    private LayerMask _notTransparentLayers;

    [SerializeField]
    private float switchToMinorDistance;
    private EnemyProjectileLauncher _launcher;
    private SortedSet<TargetSlot> _targetSlots = new SortedSet<TargetSlot>(new TargetSlot(0, TargetType.Main, null));
    private TargetSlot previousMostPriorSlot;

    public event Action<GameObject, bool> activeTargetReAssigned;

    private void Start()
    {
        PlayerStatusInformer.newPlayerAssigned += UpdateTargets;
    }

    private void FixedUpdate()
    {
        if (PlayerStatusInformer.PlayerGameObject != null)
            UpdateTargets(PlayerStatusInformer.PlayerGameObject);
    }

    private void Awake()
    {
        TryGetComponent(out _launcher);
    }

    private void AssignTarget()
    {
        TargetSlot mostPriorSlot = _targetSlots.Max; 

        if (mostPriorSlot != null && (previousMostPriorSlot == null
                || previousMostPriorSlot.target != mostPriorSlot.target
                || previousMostPriorSlot.targetIsVisible != mostPriorSlot.targetIsVisible))
        {
            previousMostPriorSlot = mostPriorSlot;
            activeTargetReAssigned?.Invoke(mostPriorSlot.target, mostPriorSlot.targetIsVisible);
        }
    }

    private void UpdateTargets(GameObject mainTarget)
    {
        _targetSlots.Clear();
        _targetSlots.Add(new TargetSlot(1, TargetType.Main, mainTarget));

        RecalculateTargetsPriority();
        AssignTarget();
    }

    private void RecalculateTargetsPriority()
    {
        List<TargetSlot> slotsList = _targetSlots.ToList();
        _targetSlots.Clear();
        foreach (TargetSlot slot in slotsList) 
        {
            _targetSlots.Add(CalculatePriorityForTarget(slot));
        }
    }

    private TargetSlot CalculatePriorityForTarget(TargetSlot targetSlot)
    {
        targetSlot.targetIsVisible = true;
        if (targetSlot.targetType == TargetType.Main && TestIfTargetIsVisible(targetSlot))
        {
            targetSlot.targetPriority = 1;
        }
        else if (targetSlot.targetType == TargetType.Minor && TestIfTargetIsVisible(targetSlot))
        {
            targetSlot.targetPriority = 2;
        }
        else
        {
            targetSlot.targetPriority = 0;
            targetSlot.targetIsVisible = false;
        }

        return targetSlot;
    }

    private bool TestIfTargetIsVisible(TargetSlot targetSlot)
    {
        bool isVisible = false;
        GameObject target = targetSlot.target;
        Vector2 objectPosition = gameObject.transform.position;
        Vector2 direction = (target.transform.position - transform.position).normalized;
        float raySize = 20;
        if (targetSlot.targetType == TargetType.Minor)
        {
            raySize = switchToMinorDistance;
        }
        if (Vector2.Distance(objectPosition, target.transform.position) <= raySize)
        {
            float rayThickness = 0.4f;
            RaycastHit2D[] hits = Physics2D.CircleCastAll(objectPosition, rayThickness / 2, direction, raySize, _notTransparentLayers);

            DebugDrawBoxCast(objectPosition, new Vector2(rayThickness, rayThickness)
                , Vector2.Angle(Vector2.right, direction), direction, raySize, Color.red);

            if (hits.Length >= 1 && hits[0].transform.gameObject == target)
            {
                isVisible = true;
            }

        }
        return isVisible;
    }

    private void DebugDrawBoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, Color color)
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

    private void OnDestroy()
    {
        PlayerStatusInformer.newPlayerAssigned -= UpdateTargets;
    }
}
