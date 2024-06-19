using UnityEngine;

[RequireComponent(typeof(GravityScaleManager))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CharacterControllerScript))]
public class PlayerJumpHandler : MonoBehaviour
{
    [SerializeField]
    private float _jumpStrength, _ungroundedJumpPermDuration;

    private static PlayerJumpHandler _instance;
    private bool _jumpInCooldown = false;
    private bool _grounded = false;
    private bool _ungroundedJumpPermission = false;
    private bool _ungroundedJumpPermTimerStarted = false;
    private Rigidbody2D _rigidbody;
    private GravityScaleRequestManager _onJumpCancelingRequest;

    public float JumpStrength { get => _jumpStrength; set => _jumpStrength = value; }
    public static PlayerJumpHandler Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
        _rigidbody = GetComponent<Rigidbody2D>();
        GetComponent<CharacterControllerScript>()._groundedChanged += OnGroundedChanged;
        GravityScaleManager gravityScaleManager = GetComponent<GravityScaleManager>();
        _onJumpCancelingRequest = new GravityScaleRequestManager(gravityScaleManager, 4f, 0);
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

    private void ResetJump()
    {
        _jumpInCooldown = false;
    }

    private void OnGroundedChanged(bool newGrounded)
    {
        _grounded = newGrounded;
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

    public void RequestJumpCanceling()
    {
        _onJumpCancelingRequest.RequestIsActive = true;
    }
}
