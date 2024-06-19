using UnityEngine;

[RequireComponent(typeof(GravityScaleManager))]
public class PlayerLadderHandler : MonoBehaviour
{
    [SerializeField]
    private CharacterControllerScript _characterController;
    [SerializeField]
    private float _ladderRequestLifetime = 0.5f;
    [SerializeField]
    private float _ladderMovementSpeed;
    [SerializeField]
    private LayerMask _ladderLayers;

    private bool _stepOnLadderRequested = false;
    private bool _playerOnLadder = false;
    private Rigidbody2D _rigidbody;
    private GravityScaleRequestManager _gravityScaleRequestManager;
    private float _verticalFactor;
    private static PlayerLadderHandler _instance;

    public static PlayerLadderHandler Instance
    {
        get
        {
            return _instance;
        }
    }
    public float VerticalFactor
    {
        get
        {
            return _verticalFactor;
        }

        set
        {
            _verticalFactor = Mathf.Clamp(value, -1, 1);
        }
    }

    private void Awake()
    {
        _instance = this;
        TryGetComponent(out GravityScaleManager gravityScaleManager);
        _gravityScaleRequestManager = new GravityScaleRequestManager(gravityScaleManager, 0, 1);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (_ladderLayers.Contains(collision.gameObject.layer) && !_playerOnLadder && _stepOnLadderRequested)
        {
            PerformStepOnLadder(collision.gameObject);
        }
    }

    private void DeleteStepOnLadderRequest()
    {
        _stepOnLadderRequested = false;
    }

    private void ExitLadder()
    {
        _gravityScaleRequestManager.RequestIsActive = false;
        _characterController.BlockHorizontalMovement = false;

        _playerOnLadder = false;
    }

    private void PerformStepOnLadder(GameObject ladder)
    {

        _characterController.BlockHorizontalMovement = true;

        transform.position = new Vector2(ladder.transform.position.x, transform.position.y);
        _gravityScaleRequestManager.RequestIsActive = true;


        _stepOnLadderRequested = false;
        _playerOnLadder = true;
    }

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (_verticalFactor > 0 && !_playerOnLadder)
        {
            PostStepOnLadderRequest();
        }
        else if (_playerOnLadder)
        {
            CheckLadder();
            MovePlayer();
        }
    }

    private void MovePlayer()
    {
        _rigidbody.velocity = Vector2.zero;
        transform.Translate(_verticalFactor * _ladderMovementSpeed * Time.fixedDeltaTime * Vector2.up);
    }

    private void CheckLadder()
    {
        Collider2D collider = Physics2D.OverlapPoint(transform.position + Vector3.down, _ladderLayers);

        if (collider == null)
        {
            ExitLadderRequest();
        }
    }

    public void PostStepOnLadderRequest()
    {
        if (!_playerOnLadder)
        {
            _stepOnLadderRequested = true;
            Invoke(nameof(DeleteStepOnLadderRequest), _ladderRequestLifetime);
        }
    }

    public void ExitLadderRequest()
    {
        if (_playerOnLadder)
            ExitLadder();
    }
}
