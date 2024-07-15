using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementOperator : MonoBehaviour
{
    [SerializeField] private float _relativeHorizontalRayPossition;
    [SerializeField] private float _objectHeight; 
    [SerializeField] private CharacterControllerScript _characterController;
    [SerializeField] private LayerMask _groundLayers;
    [SerializeField] private float _maxStairSize;

    private const float SAFE_NARROWNESS_OFFSET = 0.01f;

    private Vector2 _relativeRayPosition;
    float _finalVectorSize;
    private Dictionary<string, bool> _externalMovementPermissions = new Dictionary<string, bool>();
    private MovementDirection _movementDirection;
    private bool _wayIsFree = false;
    private Dictionary<MovementDirection, int> _directionalFactor = new Dictionary<MovementDirection, int>();

    public LayerMask GroundLayers { get => _groundLayers; set => _groundLayers = value; }

    private void Start()
    {
        _movementDirection = Random.value > 0.5 ? MovementDirection.Right : MovementDirection.Left;
        Init();
    }
    
    private void Init()
    {
        InitRayParameters();
        InitDirectionalFactorDict();
    }

    private void InitRayParameters()
    {
        float positionWithObstacleTolerance = _objectHeight / 2 + _maxStairSize;
        float finalHorizontalRayPosition = positionWithObstacleTolerance + SAFE_NARROWNESS_OFFSET;
        _relativeRayPosition = new Vector2(_relativeHorizontalRayPossition, finalHorizontalRayPosition);

        _finalVectorSize = _objectHeight + _maxStairSize * 2; // Add Obstacle tolerance and fall tolerance at the same time
    }

    private void InitDirectionalFactorDict()
    {
        _directionalFactor[MovementDirection.Right] = 1;
        _directionalFactor[MovementDirection.Left] = -1;
    }

    private void FixedUpdate()
    {
        CheckWay();
        if (_wayIsFree && !_externalMovementPermissions.ContainsValue(false))
        {
            _characterController.DirectionalFactor = _directionalFactor[_movementDirection];
        }
        else if (!_wayIsFree && _movementDirection == MovementDirection.Right)
        {
            _movementDirection = MovementDirection.Left;
        }
        else if (!_wayIsFree && _movementDirection == MovementDirection.Left)
        {
            _movementDirection = MovementDirection.Right;
        }
        else
        {
            _characterController.DirectionalFactor = 0;
        }
    }

    private void CheckWay()
    {
        Vector2 objectPossition = (Vector2)gameObject.transform.position;
        Vector2 worldRayPosition ;

        if (_movementDirection == MovementDirection.Right)
        {
            worldRayPosition = _relativeRayPosition + objectPossition;
        }
        else
        {
            Vector2 opositeRayPosition = _relativeRayPosition * new Vector2(-1, 1);
            worldRayPosition = opositeRayPosition + objectPossition;
        }

        RaycastHit2D raycastHit2D = Physics2D.Raycast(worldRayPosition, Vector2.down, _finalVectorSize, GroundLayers);
        bool anyObjectHited = raycastHit2D.transform != null;
        float freePlace = Mathf.Abs(raycastHit2D.point.y - worldRayPosition.y);
        float requiredFreePlace = _objectHeight + SAFE_NARROWNESS_OFFSET;
        bool bigObstacleNotFound = freePlace >= requiredFreePlace;
        
        if (anyObjectHited && bigObstacleNotFound)
            _wayIsFree = true;
        else
            _wayIsFree = false;
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

    public void ShowUpVectors()
    {
        Vector2 mirrorRayPosition = _relativeRayPosition * new Vector2(-1, 1);
        Vector2 objectPossition = (Vector2)gameObject.transform.position;
        Vector2 worldRayPosition = _relativeRayPosition + objectPossition;
        Vector2 mirrorWorldRayPosition = mirrorRayPosition + objectPossition;

        Debug.DrawLine(worldRayPosition, worldRayPosition + Vector2.down * _finalVectorSize, Color.blue, 10f);
        Debug.DrawLine(mirrorWorldRayPosition, mirrorWorldRayPosition + Vector2.down * _finalVectorSize, Color.red, 10f);
    }
}
