using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    [SerializeField]
    private KeyCode _captureObjectButton;
    private bool _captureObjectButtonPressed;

    [SerializeField]
    private KeyCode _quitCapturingButton;

    [SerializeField]
    private KeyCode _performManipulationButton;

    [SerializeField]
    private KeyCode _fallThroughOneWayPlatform;

    [SerializeField]
    private KeyCode _stepOnLadder;

    [SerializeField]
    private KeyCode _pickUpItem;
    private bool _pickUpItemPressed;

    private static InputHandler _instance;

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

    void Awake()
    {
        _instance = this;
    }

    private void FixedUpdate()
    {
        ProcessInput();
    }

    private void ProcessInput()
    {
        PlayerController.Instance
            .CharacterControllerScript.DirectionalFactor = Input.GetAxis("Horizontal");
        PlayerLadderHandler.Instance.VerticalFactor = Input.GetAxis("Vertical");
        if (Input.GetButton("Jump"))
        {
            PlayerController.Instance.CharacterControllerScript.RequestJump();
            PlayerLadderHandler.Instance.ExitLadderRequest();
        }

        bool currentCaptureObjectButtonValue = Input.GetKey(_captureObjectButton);
        if (currentCaptureObjectButtonValue && !_captureObjectButtonPressed)
        {
            PlayerPossessableObjectManager.Instance.RequestCapturing();
            _captureObjectButtonPressed = true;
        }
        else if (!currentCaptureObjectButtonValue)
            _captureObjectButtonPressed = false;

        if (Input.GetKey(_quitCapturingButton))
            PlayerPossessableObjectManager.Instance.RequestQuitCapturing();
        if (Input.GetKey(_performManipulationButton))
            PlayerObjectManipulation.Instance.RequestManipulation();
        if (Input.GetKey(_fallThroughOneWayPlatform))
            OneWayPlatformHandler.Instance.FallThroughCurrentPlatform();
        if (Input.GetKey(_stepOnLadder))
            PlayerLadderHandler.Instance.PostStepOnLadderRequest();

        bool currentPickupItemValue = Input.GetKey(_pickUpItem);
        if (currentPickupItemValue && !_pickUpItemPressed)
        {
            PlayerItemsManager.Instance.RequestPickUp();
            _pickUpItemPressed = true;
        }
        else if (!currentPickupItemValue)
            _pickUpItemPressed = false;
    }
}
