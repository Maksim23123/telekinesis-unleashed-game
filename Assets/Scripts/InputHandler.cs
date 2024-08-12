using System.Net.Http.Headers;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private KeyCode _captureObjectButton;
    [SerializeField] private KeyCode _quitCapturingButton;
    [SerializeField] private KeyCode _performManipulationButton;
    [SerializeField] private KeyCode _fallThroughOneWayPlatform;
    [SerializeField] private KeyCode _stepOnLadder;
    [SerializeField] private KeyCode _pickUpItem;

    private bool _captureObjectButtonPressed;
    private bool _pickUpItemPressed;
    private bool _jumpPressed;
    private static InputHandler _instance;

    private CharacterControllerScript _characterControllerScript;
    private PlayerLadderHandler _playerLadderHandler;
    private PlayerJumpHandler _playerJumpHandler;
    private PlayerPossessableObjectManager _playerPossessableObjectManager;
    private PlayerObjectManipulation _playerObjectManipulation;
    private OneWayPlatformHandler _oneWayPlatformHandler;
    private PlayerItemsManager _playerItemsManager;

    private bool InputReceiverExists
    {
        get
        {
            return PlayerStatusInformer.PlayerGameObject != null;
        }
    }

    public static InputHandler Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject newGameObject = new GameObject("Input Handler");
                newGameObject.AddComponent<InputHandler>();
            }

            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
        PlayerStatusInformer.NewPlayerAssigned += OnNewPlayerAssigned;
    }

    private void Start()
    {
        InitReferences();
    }

    private void InitReferences()
    {
        GameObject playerGameObject = PlayerStatusInformer.PlayerGameObject;

        if (playerGameObject != null)
        {
            _characterControllerScript = playerGameObject.GetComponent<CharacterControllerScript>();
            _playerLadderHandler = playerGameObject.GetComponent<PlayerLadderHandler>();
            _playerJumpHandler = playerGameObject.GetComponent<PlayerJumpHandler>();
            _playerPossessableObjectManager = playerGameObject.GetComponent<PlayerPossessableObjectManager>();
            _playerObjectManipulation = playerGameObject.GetComponent<PlayerObjectManipulation>();
            _oneWayPlatformHandler = playerGameObject.GetComponent<OneWayPlatformHandler>();
            _playerItemsManager = playerGameObject.GetComponent<PlayerItemsManager>();
        }
    }

    private void FixedUpdate()
    {
        if (InputReceiverExists)
        {
            ProcessInput();
        }
    }

    private void OnNewPlayerAssigned(GameObject playerGameObject)
    {
        InitReferences();
    }

    private void ProcessInput()
    {
        _characterControllerScript.DirectionalFactor = Input.GetAxis("Horizontal");
        _playerLadderHandler.VerticalFactor = Input.GetAxis("Vertical");

        bool currentJumpValue = Input.GetButton("Jump");
        if (Input.GetButton("Jump"))
        {
            _jumpPressed = true;
            _playerJumpHandler.RequestJump();
            _playerLadderHandler.ExitLadderRequest();
        }
        else if (!currentJumpValue && _jumpPressed)
        {
            _jumpPressed = false;
            _playerJumpHandler.RequestJumpCanceling();
        }

        bool currentCaptureObjectButtonValue = Input.GetKey(_captureObjectButton);
        if (currentCaptureObjectButtonValue && !_captureObjectButtonPressed)
        {
            _playerPossessableObjectManager.RequestCapturing();
            _captureObjectButtonPressed = true;
        }
        else if (!currentCaptureObjectButtonValue)
            _captureObjectButtonPressed = false;

        if (Input.GetKey(_quitCapturingButton))
            _playerPossessableObjectManager.RequestQuitCapturing();
        if (Input.GetKey(_performManipulationButton))
            _playerObjectManipulation.RequestManipulation();
        if (Input.GetKey(_fallThroughOneWayPlatform))
            _oneWayPlatformHandler.FallThroughCurrentPlatforms();
        if (Input.GetKey(_stepOnLadder))
            _playerLadderHandler.PostStepOnLadderRequest();

        bool currentPickupItemValue = Input.GetKey(_pickUpItem);
        if (currentPickupItemValue && !_pickUpItemPressed)
        {
            _playerItemsManager.RequestPickUp();
            _pickUpItemPressed = true;
        }
        else if (!currentPickupItemValue)
            _pickUpItemPressed = false;
    }
}