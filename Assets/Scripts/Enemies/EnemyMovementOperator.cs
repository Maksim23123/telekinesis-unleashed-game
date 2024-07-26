using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the movement of an Enemy GameObject.
/// </summary>
[RequireComponent(typeof(CharacterControllerScript))]
public class EnemyMovementOperator : MonoBehaviour
{
    [SerializeField] private float _rorizontalRayGameObjectDistance;
    [SerializeField] private float _objectHeight; 
    [SerializeField] private LayerMask _groundLayers;
    [SerializeField] private float _maxStairSize;

    private const float SAFE_NARROWNESS_OFFSET = 0.01f;

    private CharacterControllerScript _characterController;
    private Vector2 _relativeRayPosition;
    float _finalVectorSize;
    private Dictionary<string, bool> _externalMovementPermissions = new Dictionary<string, bool>();
    private MovementDirection _movementDirection;
    private bool _wayIsFree = false;
    private Dictionary<MovementDirection, int> _directionalFactor = new Dictionary<MovementDirection, int>();

    public LayerMask GroundLayers { get => _groundLayers; set => _groundLayers = value; }

    /// <summary>
    /// Initializes necessary variables for the class.
    /// </summary>
    private void Start()
    {
        _movementDirection = Random.value > 0.5 ? MovementDirection.Right : MovementDirection.Left;
        _characterController = GetComponent<CharacterControllerScript>();
        Initialize();
    }

    /// <summary>
    /// Encapsulates complex initialization logic.
    /// </summary>
    private void Initialize()
    {
        InitRayParameters();
        InitDirectionalFactorDict();
    }

    /// <summary>
    /// Calculates and initializes ray parameters for path checking.
    /// </summary>
    private void InitRayParameters()
    {
        float positionWithObstacleTolerance = _objectHeight / 2 + _maxStairSize;
        float finalHorizontalRayPosition = positionWithObstacleTolerance + SAFE_NARROWNESS_OFFSET;

        _relativeRayPosition = new Vector2(_rorizontalRayGameObjectDistance, finalHorizontalRayPosition);
        // \/ Add Obstacle tolerance and fall tolerance at the same time because those are equal values
        _finalVectorSize = _objectHeight + _maxStairSize * 2; 
    }

    /// <summary>
    /// Initializes a dictionary with directional factors indicating movement direction.
    /// </summary>
    private void InitDirectionalFactorDict()
    {
        _directionalFactor[MovementDirection.Right] = 1;
        _directionalFactor[MovementDirection.Left] = -1;
    }

    /// <summary>
    /// Validates the path and changes movement direction based on the outcome.
    /// </summary>
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

    /// <summary>
    /// Performs raycast to validate the path ahead of the GameObject.
    /// </summary>
    /// <remarks>
    /// Front of the GameObject is based on the current movement direction.
    /// </remarks>
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

    /// <summary>
    /// Allows external logic to control GameObject movement.
    /// </summary>
    /// <param name="permissionName">Tag of permission.</param>
    /// <param name="value">Indicates if permission allows movement.</param>
    public void SetExternalPermission(string permissionName, bool value)
    {
        _externalMovementPermissions[permissionName] = value;
    }

    /// <summary>
    /// Removes a movement permission with a specified tag.
    /// </summary>
    /// <param name="permissionName">Tag of permission.</param>
    /// <returns>True if permission was removed; otherwise false.</returns>
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

    /// <summary>
    /// Debug feature to display vector representation in the Scene view.
    /// </summary>
    public void ShowUpVectors()
    {
        Vector2 mirrorRayPosition = _relativeRayPosition * new Vector2(-1, 1);
        Vector2 objectPossition = (Vector2)gameObject.transform.position;
        Vector2 worldRayPosition = _relativeRayPosition + objectPossition;
        Vector2 mirroredWorldRayPosition = mirrorRayPosition + objectPossition;

        Debug.DrawLine(worldRayPosition, worldRayPosition + Vector2.down * _finalVectorSize, Color.blue, 10f);
        Debug.DrawLine(mirroredWorldRayPosition, mirroredWorldRayPosition + Vector2.down * _finalVectorSize, Color.red, 10f);
    }
}
