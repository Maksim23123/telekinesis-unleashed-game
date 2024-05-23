using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Can be singletone
public class PlayerController : MonoBehaviour
{
    //Adjustable parameters
    [SerializeField]
    float _speed, _jumpStrength;
    [SerializeField]
    LayerMask _groundLayers;

    //Storage parameters
    float _horizontalInput = 0f;
    float _verticalInput = 0f;
    float _groundAngle = 0;
    float _defaultGravityScale;

    bool _jumpRequested = false;
    bool _grounded = false;
    bool _jumpInCooldown = false;
    bool _onSlope = false;

    Rigidbody2D _rigidbody;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _defaultGravityScale = _rigidbody.gravityScale;
    }

    private void FixedUpdate()
    {
        CheckGround();
        MovePlayer();
    }

    private void Update()
    {
        GetPlayerInput();
    }

    void GetPlayerInput()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");

        _jumpRequested = Input.GetButton("Jump");
    }

    void MovePlayer()
    {
        if (_onSlope && _grounded)
        {
            _rigidbody.gravityScale = 0;
            _rigidbody.velocity = new Vector2(0, _rigidbody.velocity.y);
        }
        else 
            _rigidbody.gravityScale = _defaultGravityScale;

        float movement = _horizontalInput * _speed * Time.deltaTime;

        Vector2 adaptedVector = Vector2.right;
        if (_grounded && _onSlope)
            adaptedVector = new Vector2(Mathf.Cos(Mathf.Deg2Rad * _groundAngle), Mathf.Sin(Mathf.Deg2Rad * _groundAngle));

        transform.Translate(movement * adaptedVector);
        ProcessJump();
    }

    void ProcessJump()
    {
        if (_jumpRequested && _grounded && !_jumpInCooldown)
        {
            _rigidbody.AddForce(Vector2.up * _jumpStrength * Time.deltaTime * 100, ForceMode2D.Impulse);
            _jumpInCooldown = true;
            Invoke("ResetJump", 0.05f);
        }
    }

    void ResetJump()
    {
        _jumpInCooldown = false;
    }

    void CheckGround()
    {
        _grounded = false;
        RaycastHit2D raycastHit = Physics2D.Raycast(transform.position + Vector3.down * 1.2f, Vector2.up, 1.5f, _groundLayers);

        if (raycastHit.collider != null 
            && raycastHit.transform.gameObject != gameObject)
        {
            _groundAngle = raycastHit.transform.rotation.eulerAngles.z;
            _grounded = true;
            float angle = Mathf.Abs(_groundAngle);
            if (angle > 0 &&  angle < 45)
                _onSlope = true;
            else
                _onSlope = false;
        }
    }
}
