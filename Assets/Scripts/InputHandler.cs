using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public float HorizontalInput { get; private set; }
    public float VerticalInput { get; private set; }
    public bool JumpRequested { get; private set; }

    [SerializeField]
    private KeyCode _captureObjectButton;
    public bool CaptureRequest { get; private set; }

    [SerializeField]
    private KeyCode _quitCapturingButton;
    public bool QuitCapturingRequest { get; private set; }

    [SerializeField]
    private KeyCode _performManipulationButton;
    public bool PerformManipulationRequest { get; private set; }

    private static InputHandler _instance;

    public static InputHandler Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject instance = new GameObject("Input handler");
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
        if (Input.GetButton("Jump"))
            PlayerController.Instance.CharacterControllerScript.RequestJump();
        if (Input.GetKey(_captureObjectButton))
            PlayerObjectManager.Instance.RequestCapturing();
        if (Input.GetKey(_quitCapturingButton))
            PlayerObjectManager.Instance.RequestQuitCapturing();
        if (Input.GetKey(_performManipulationButton))
            PlayerObjectManipulation.Instance.RequestManipulation();
    }
}
