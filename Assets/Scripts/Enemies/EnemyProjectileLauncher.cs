using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileLauncher : MonoBehaviour
{
    public event Action<bool> _targetVisibilityChanged; 

    [SerializeField]
    GameObject _projectile;
    [SerializeField]
    GameObject _target;
    [SerializeField]
    float _shootingCooldownTime;
    [SerializeField]
    LayerMask _notTransparentLayers;

    bool _shootingAllowed = true;

    private bool _targetIsVisible;

    public bool TargetIsVisible 
    { 
        get
        {
            return _targetIsVisible;
        } 

        private set
        {
            _targetIsVisible = value;
            _targetVisibilityChanged?.Invoke(_targetIsVisible);
        } 
    }



    // Start is called before the first frame update
    void Start()
    {
        //DEBUG
        //_target = PlayerPossessableObjectManager.Instance.gameObject;
    }

    private void FixedUpdate()
    {

        // FIX
        /*
        Vector2 direction = (_target.transform.position - transform.position).normalized;
        TestIfTargetIsVisible(direction);

        if (_shootingAllowed && TargetIsVisible)
        {
            
            Shot(direction);
            _shootingAllowed = false;
            Invoke(nameof(ResetShootingPermission), _shootingCooldownTime);
        }
        */
    }

    private void Shot(Vector2 direction)
    {
        
        GameObject newProjectile = Instantiate(_projectile, transform);

        if (newProjectile.TryGetComponent(out ProjectileManager projectileManager))
        {
            projectileManager.Launch(direction);
        }
    }
    
    private void ResetShootingPermission()
    {
        _shootingAllowed = true;
    }

    private void TestIfTargetIsVisible(Vector2 direction)
    {
        TargetIsVisible = false;
        Vector2 objectPosition = gameObject.transform.position;
        float raySize = 20;
        if (Vector2.Distance(objectPosition, _target.transform.position) <= raySize)
        {
            float rayThickness = 0.4f;
            RaycastHit2D[] hits = Physics2D.CircleCastAll(objectPosition, rayThickness / 2, direction, raySize, _notTransparentLayers);

            DebugDrawBoxCast(objectPosition, new Vector2(rayThickness, rayThickness)
                , Vector2.Angle(Vector2.right, direction), direction, raySize, Color.red);

            if (hits.Length >= 1 && hits[0].transform.gameObject == _target)
                TargetIsVisible = true;
        }
    }

    void DebugDrawBoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, Color color)
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

        // Draw the movement direction of the BoxCast
        Vector2 endPoint = origin + direction.normalized * distance;
        Debug.DrawLine(origin, endPoint, color);

        // Draw the box at the end position of the BoxCast
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
}
