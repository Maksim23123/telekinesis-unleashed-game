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

    public LayerMask GroundLayers { get => _groundLayers; private set => _groundLayers = value; }

    //Adjustable parameters
    [SerializeField]
    float _speed, _jumpStrength;
    [SerializeField]
    LayerMask _groundLayers;

    float _maxSlopeAngle = 45;

    //Storage parameters
    float _groundAngle = 0;
    float _defaultGravityScale;

    bool _grounded = false;
    bool _jumpInCooldown = false;
    bool _onSlope = false;

    public bool BlockHorizontalMovement { get; set; } = false;
    public bool UpdateGravityScale { get; set; } = true;

    Rigidbody2D _rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _defaultGravityScale = _rigidbody.gravityScale;
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
            if (_onSlope && _grounded)
            {
                _rigidbody.gravityScale = 0;
            }
            else
                _rigidbody.gravityScale = _defaultGravityScale;
        }


        float movement = directionalFactor * _speed * Time.deltaTime;

        Vector2 adaptedVector = Vector2.right;
        if (_grounded && _onSlope)
            adaptedVector = new Vector2(Mathf.Cos(Mathf.Deg2Rad * _groundAngle), Mathf.Sin(Mathf.Deg2Rad * _groundAngle));

        if (!BlockHorizontalMovement) 
            transform.Translate(movement * adaptedVector);

        _rigidbody.velocity = new Vector2(0, _rigidbody.velocity.y);
    }

    public void RequestJump()
    {
        if (_grounded && !_jumpInCooldown)
        {
            _rigidbody.velocity *= 0;
            _rigidbody.AddForce(Vector2.up * _jumpStrength * Time.deltaTime * 100, ForceMode2D.Impulse);
            _jumpInCooldown = true;
            Invoke(nameof(ResetJump), 0.05f);
        }
    }

    void ResetJump()
    {
        _jumpInCooldown = false;
    }

    protected void CheckGround()
    {
        _grounded = false;
        RaycastHit2D raycastHit = Physics2D.CircleCast(transform.position + Vector3.down * 0.58f, 0.45f, Vector2.down, 0, _groundLayers);//Physics2D.Raycast(transform.position + Vector3.down * 1.2f, Vector2.up, 1.5f, _groundLayers);


        if (raycastHit.collider != null
            && raycastHit.transform.gameObject != gameObject)
        {
            _groundAngle = raycastHit.transform.rotation.eulerAngles.z;
            _grounded = true;
            float angle = Mathf.Abs(_groundAngle);
            if (angle > 0 && angle < _maxSlopeAngle)
                _onSlope = true;
            else
                _onSlope = false;
        }
    }
}