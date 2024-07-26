using System;
using UnityEngine;

/// <summary>
/// This class responsible for horizontal movement of GameObject.
/// Should be used as utilite for controling horizontal of each GameObject it attached to.
/// </summary>
public class CharacterControllerScript : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private LayerMask _groundLayers;
    private const float GROUND_CHECK_RELATIVE_VERTICAL_POSITION = 0.555f;
    private const float GROUND_CHECK_RADIUS = 0.475f;

    private float _directionalFactor;
    private float _maxSlopeAngle = 46;
    private float _groundAngle = 0;
    private bool _grounded = false;
    private bool _onSlope = false;
    private Rigidbody2D _rigidbody;
    private GravityScaleRequestManager _gravityScaleRequestManager;

    public event Action<bool> GroundedChanged;

    public bool BlockHorizontalMovement { get; set; } = false;
    public float Speed { get => _speed; set => _speed = value; }
    public LayerMask GroundLayers { get => _groundLayers; private set => _groundLayers = value; }
    public float DirectionalFactor
    {
        get
        {
            return _directionalFactor;
        }

        set
        {
            _directionalFactor = Mathf.Clamp(value, -1, 1);
        }
    }
    public bool Grounded
    {
        get => _grounded;
        set
        {
            if (value != _grounded)
            {
                _grounded = value;
                GroundedChanged?.Invoke(_grounded);
            }
        }
    }

    /// <summary>
    /// Initializes variables necessary for the class.
    /// </summary>
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();

        if (TryGetComponent(out GravityScaleManager gravityScaleManager))
        {
            _gravityScaleRequestManager = new GravityScaleRequestManager(gravityScaleManager, 0, 0);
        }
    }

    /// <summary>
    /// Periodicaly invokes main class logic.
    /// </summary>
    private void FixedUpdate()
    {
        CheckGround();
        MoveCharacter(DirectionalFactor);
    }


    /// <summary>
    /// Moves GameObject acording to directional factor
    /// </summary>
    /// <param name="directionalFactor"> indicates in which direction GameObject should move.</param>
    protected void MoveCharacter(float directionalFactor)
    {
        if (_gravityScaleRequestManager != null)
        {
            if (_onSlope && Grounded)
            {
                _gravityScaleRequestManager.RequestIsActive = true;
            }
            else
                _gravityScaleRequestManager.RequestIsActive = false;
        }

        float movementAmount = directionalFactor * _speed * Time.deltaTime;

        Vector2 adaptedDirection = Vector2.right;
        if (Grounded && _onSlope)
            adaptedDirection = new Vector2(Mathf.Cos(Mathf.Deg2Rad * _groundAngle), Mathf.Sin(Mathf.Deg2Rad * _groundAngle));

        if (!BlockHorizontalMovement)
            transform.Translate(movementAmount * adaptedDirection);

        _rigidbody.velocity = new Vector2(0, _rigidbody.velocity.y);
    }
    
    /// <summary>
    /// Performs circle cast to check if there is any surface that can be considered ground underneath GameObject.
    /// </summary>
    protected void CheckGround()
    {
        RaycastHit2D raycastHit = Physics2D.CircleCast(transform.position + Vector3.down * GROUND_CHECK_RELATIVE_VERTICAL_POSITION
            , GROUND_CHECK_RADIUS, Vector2.down, 0, _groundLayers);

        if (raycastHit.collider != null)
        {
            Grounded = true;
            _groundAngle = Vector3.Angle(Vector3.up, raycastHit.transform.rotation * Vector3.up) * (raycastHit.transform.rotation.eulerAngles.z < 180 ? 1 : -1);
            float angleSize = Mathf.Abs(_groundAngle);
            if (angleSize > 0 && angleSize <= _maxSlopeAngle)
            {
                _onSlope = true;
            }
            else
            {
                _onSlope = false;
            }
        }
        else
        {
            Grounded = false;
            if (_gravityScaleRequestManager != null)
                _gravityScaleRequestManager.RequestIsActive = false;
        }
    }
}
