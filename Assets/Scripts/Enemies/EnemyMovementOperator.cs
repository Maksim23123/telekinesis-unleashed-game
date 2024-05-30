using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyMovementOperator : MonoBehaviour
{
    [SerializeField]
    Vector2 _relativeRayPossition;

    [SerializeField]
    float _vectorSize;

    [SerializeField]
    CharacterControllerScript _characterController;

    [SerializeField]
    LayerMask _groundLayers;

    [SerializeField]
    float _maxStairSize;

    Dictionary<string, bool> _externalMovementPermissions = new Dictionary<string, bool>();

    MovementDirection _movementDirection;

    bool _wayIsFree = false;

    public LayerMask GroundLayers { get => _groundLayers; set => _groundLayers = value; }

    Dictionary<MovementDirection, int> _directionalFactor = new Dictionary<MovementDirection, int>();

    private void Start()
    {
        _movementDirection = Random.value > 0.5 ? MovementDirection.Right : MovementDirection.Left;
        InitDirectionalFactorDict();
    }

    private void InitDirectionalFactorDict()
    {
        _directionalFactor[MovementDirection.Right] = 1;
        _directionalFactor[MovementDirection.Left] = -1;
    }

    public void SetExternalPermission(string permissionName, bool value)
    {
        _externalMovementPermissions[permissionName] = value;
    }

    public bool RemoveExternalPermission(string permissionName)
    {
        if (_externalMovementPermissions.ContainsKey(permissionName))
        {
            _externalMovementPermissions.Remove(permissionName);
            return true;
        }
        else 
            return false; 
    }

    private void FixedUpdate()
    {
        CheckWay();
        if (_wayIsFree && !_externalMovementPermissions.ContainsValue(false))
            _characterController.DirectionalFactor = _directionalFactor[_movementDirection];
        else if (!_wayIsFree && _movementDirection == MovementDirection.Right)
            _movementDirection = MovementDirection.Left;
        else if (!_wayIsFree && _movementDirection == MovementDirection.Left)
            _movementDirection = MovementDirection.Right;
        else
            _characterController.DirectionalFactor = 0;
    }

    public void ShowUpVectors()
    {
        Vector2 mirrorRayPosition = _relativeRayPossition * new Vector2(-1, 1);
        Vector2 objectPossition = (Vector2)gameObject.transform.position;
        Vector2 worldRayPosition = _relativeRayPossition + objectPossition;
        Vector2 mirrorWorldRayPosition = mirrorRayPosition + objectPossition;

        Debug.DrawLine(worldRayPosition, worldRayPosition + Vector2.down * _vectorSize, Color.blue, 10f);
        Debug.DrawLine(mirrorWorldRayPosition, mirrorWorldRayPosition + Vector2.down * _vectorSize, Color.red, 10f);
    }

    private void CheckWay()
    {
        Vector2 objectPossition = (Vector2)gameObject.transform.position;
        if (_movementDirection == MovementDirection.Left)
        {
            Vector2 mirrorRayPosition = _relativeRayPossition * new Vector2(-1, 1);
            Vector2 mirrorWorldRayPosition = mirrorRayPosition + objectPossition;

            RaycastHit2D raycastHit2D = Physics2D.Raycast(mirrorWorldRayPosition, Vector2.down, _vectorSize, GroundLayers);
            if (raycastHit2D.transform != null && Mathf.Abs(raycastHit2D.point.y - mirrorWorldRayPosition.y) >= _vectorSize - _maxStairSize - 0.4)
                _wayIsFree = true;
            else
                _wayIsFree = false;
        }
        else if (_movementDirection == MovementDirection.Right)
        {
            Vector2 worldRayPosition = _relativeRayPossition + objectPossition;
            RaycastHit2D raycastHit2D = Physics2D.Raycast(worldRayPosition, Vector2.down, _vectorSize, GroundLayers);
            if (raycastHit2D.transform != null && Mathf.Abs(raycastHit2D.point.y - worldRayPosition.y) >= _vectorSize - _maxStairSize - 0.4)
                _wayIsFree = true;
            else
                _wayIsFree = false;
        }
    }
}
