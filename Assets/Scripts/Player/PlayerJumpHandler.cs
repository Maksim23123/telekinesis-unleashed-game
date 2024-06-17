using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GravityScaleManager))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CharacterControllerScript))]
public class PlayerJumpHandler : MonoBehaviour
{
    private static PlayerJumpHandler _instance;

    public static PlayerJumpHandler Instance
    {
        get
        {
            return _instance;
        }
    }

    [SerializeField]
    float _jumpStrength, _ungroundedJumpPermDuration;

    bool _jumpInCooldown = false;

    bool _grounded = false;

    bool _ungroundedJumpPermission = false;
    bool _ungroundedJumpPermTimerStarted = false; 

    Rigidbody2D _rigidbody;

    GravityScaleRequestManager _onJumpCancelingRequest;

    public float JumpStrength { get => _jumpStrength; set => _jumpStrength = value; }

    private void Awake()
    {
        _instance = this;
        _rigidbody = GetComponent<Rigidbody2D>();
        GetComponent<CharacterControllerScript>()._groundedChanged += OnGroundedChanged;
        GravityScaleManager gravityScaleManager = GetComponent<GravityScaleManager>();
        _onJumpCancelingRequest = new GravityScaleRequestManager(gravityScaleManager, 4f, 1);
    }

    private void FixedUpdate()
    {
        if (_grounded)
        {
            _onJumpCancelingRequest.RequestIsActive = false;
            _ungroundedJumpPermission = true;
        }
        else if (!_ungroundedJumpPermTimerStarted && _ungroundedJumpPermission)
            Invoke(nameof(CancelUngroundedJumpPermission), _ungroundedJumpPermDuration);
    }

    private void CancelUngroundedJumpPermission()
    {
        _ungroundedJumpPermTimerStarted = false;
        _ungroundedJumpPermission = false;
    }

    public void RequestJump()
    {
        if ((_grounded || _ungroundedJumpPermission) && !_jumpInCooldown)
        {
            _ungroundedJumpPermission = false;
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

    void OnGroundedChanged(bool newGrounded)
    {
        _grounded = newGrounded;
    }

    public void RequestJumpCanceling()
    {
        _onJumpCancelingRequest.RequestIsActive = true;
    }
}
