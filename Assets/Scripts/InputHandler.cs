using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    [SerializeField]
    private KeyCode _captureObjectButton;

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

    private static InputHandler _instance;

    public static InputHandler Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject instance = new GameObject("Input Handler");
                instance.AddComponent<InputHandler>();
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
        if (Input.GetKey(_captureObjectButton))
            PlayerPossessableObjectManager.Instance.RequestCapturing();
        if (Input.GetKey(_quitCapturingButton))
            PlayerPossessableObjectManager.Instance.RequestQuitCapturing();
        if (Input.GetKey(_performManipulationButton))
            PlayerObjectManipulation.Instance.RequestManipulation();
        if (Input.GetKey(_fallThroughOneWayPlatform))
            OneWayPlatformHandler.Instance.FallThroughCurrentPlatform();
        if (Input.GetKey(_stepOnLadder))
            PlayerLadderHandler.Instance.PostStepOnLadderRequest();
        if (Input.GetKey(_pickUpItem))
            PlayerItemsManager.Instance.RequestPickUp();
    }
}
