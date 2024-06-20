using System;
using UnityEngine;

public class CharacterControllerScript : MonoBehaviour
{
    [SerializeField]
    private float _speed;
    [SerializeField]
    private LayerMask _groundLayers;

    private float _directionalFactor;
    private float _maxSlopeAngle = 46;
    private float _groundAngle = 0;
    private bool _grounded = false;
    private bool _onSlope = false;
    private Rigidbody2D _rigidbody;
    private GravityScaleRequestManager _gravityScaleRequestManager;

    public event Action<bool> _groundedChanged;

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
                _groundedChanged?.Invoke(_grounded);
            }
        }
    }

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();

        if (TryGetComponent(out GravityScaleManager gravityScaleManager))
        {
            _gravityScaleRequestManager = new GravityScaleRequestManager(gravityScaleManager, 0, 0);
        }
    }

    private void FixedUpdate()
    {
        CheckGround();
        MoveCharacter(DirectionalFactor);
    }

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

        float movement = directionalFactor * _speed * Time.deltaTime;

        Vector2 adaptedVector = Vector2.right;
        if (Grounded && _onSlope)
            adaptedVector = new Vector2(Mathf.Cos(Mathf.Deg2Rad * _groundAngle), Mathf.Sin(Mathf.Deg2Rad * _groundAngle));

        if (!BlockHorizontalMovement)
            transform.Translate(movement * adaptedVector);

        _rigidbody.velocity = new Vector2(0, _rigidbody.velocity.y);
    }

    protected void CheckGround()
    {

        RaycastHit2D raycastHit = Physics2D.CircleCast(transform.position + Vector3.down * 0.58f, 0.45f, Vector2.down, 0, _groundLayers);

        if (raycastHit.collider != null)
        {
            Grounded = true;
            _groundAngle = raycastHit.transform.rotation.eulerAngles.z;
            float angle = Mathf.Abs(_groundAngle);
            if (angle > 0 && angle <= _maxSlopeAngle)
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