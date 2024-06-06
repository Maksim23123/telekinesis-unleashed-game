using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;

public class PlayerLadderHandler : MonoBehaviour
{
    private static PlayerLadderHandler _instance;

    public static PlayerLadderHandler Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PlayerLadderHandler>();
            }

            return _instance;
        }
    }

    private float _verticalFactor;

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

    [SerializeField]
    CharacterControllerScript characterController;

    bool _stepOnLadderRequested = false;
    bool _playerOnLadder = false;


    [SerializeField]
    float _ladderRequestLifetime = 0.5f;

    [SerializeField]
    float _ladderMovementSpeed;

    [SerializeField]
    private LayerMask _ladderLayers;

    Rigidbody2D _rigidbody;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (_ladderLayers.Contains(collision.gameObject.layer) && !_playerOnLadder && _stepOnLadderRequested)
        {
            PerformStepOnLadder(collision.gameObject);
        }

    }

    public void PostStepOnLadderRequest()
    {
        if (!_playerOnLadder )
        {
            _stepOnLadderRequested = true;
            Invoke(nameof(DeleteStepOnLadderRequest), _ladderRequestLifetime);
        }
    }

    private void DeleteStepOnLadderRequest()
    {
        _stepOnLadderRequested = false;
    }

    public void ExitLadderRequest()
    {
        if (_playerOnLadder)
            ExitLadder();
    }

    private void ExitLadder()
    {
        
        characterController.BlockHorizontalMovement = false;
        characterController.UpdateGravityScale = true;

        _playerOnLadder = false;
    } 

    private void PerformStepOnLadder(GameObject ladder)
    {
        
        characterController.BlockHorizontalMovement = true;
        characterController.UpdateGravityScale = false;

        transform.position = new Vector2(ladder.transform.position.x, transform.position.y);
        _rigidbody.gravityScale = 0;

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
        Collider2D collider = Physics2D.OverlapPoint(transform.position + Vector3.down , _ladderLayers);
        
        if (collider == null)
        {
            ExitLadderRequest();
        }
    }
}
