using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEditor;
using UnityEngine;

public class CharacterControllerScript : MonoBehaviour
{
    private float _directionalFactor;

    public float DirectionalFactor { 
        get
        {
            return _directionalFactor;
        } 

        set
        {
            _directionalFactor = Mathf.Clamp(value, -1, 1);
        } 
    }

    public event Action<bool> _groundedChanged;

    public LayerMask GroundLayers { get => _groundLayers; private set => _groundLayers = value; }

    //Adjustable parameters
    [SerializeField]
    float _speed;
    [SerializeField]
    LayerMask _groundLayers;

    float _maxSlopeAngle = 45;

    //Storage parameters
    float _groundAngle = 0;

    bool _grounded = false;
    bool _onSlope = false;

    Rigidbody2D _rigidbody;

    GravityScaleRequestManager _gravityScaleRequestManager;

    public bool BlockHorizontalMovement { get; set; } = false;
    public bool UpdateGravityScale { get; set; } = true;
    public float Speed { get => _speed; set => _speed = value; }
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

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        TryGetComponent(out GravityScaleManager gravityScaleManager);
        _gravityScaleRequestManager = new GravityScaleRequestManager(gravityScaleManager, 0, 0);
    }

    private void FixedUpdate()
    {
        CheckGround();
        MoveCharacter(DirectionalFactor);
    }

    protected void MoveCharacter(float directionalFactor)
    {
        if (UpdateGravityScale)
        {
            if (_onSlope && Grounded)
                _gravityScaleRequestManager.RequestIsActive = true;
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
        Grounded = false;
        RaycastHit2D raycastHit = Physics2D.CircleCast(transform.position + Vector3.down * 0.58f, 0.45f, Vector2.down, 0, _groundLayers);


        if (raycastHit.collider != null
            && raycastHit.transform.gameObject != gameObject)
        {
            _groundAngle = raycastHit.transform.rotation.eulerAngles.z;
            Grounded = true;
            float angle = Mathf.Abs(_groundAngle);
            if (angle > 0 && angle < _maxSlopeAngle)
                _onSlope = true;
            else
                _onSlope = false;
        }
    }
}