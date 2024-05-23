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

    private void Update()
    {
        GetPlayerInput();
    }

    private void GetPlayerInput()
    {
        HorizontalInput = Input.GetAxis("Horizontal");
        VerticalInput = Input.GetAxis("Vertical");

        JumpRequested = Input.GetButton("Jump");

        CaptureRequest = Input.GetKey(_captureObjectButton);
        QuitCapturingRequest = Input.GetKey(_quitCapturingButton);

        PerformManipulationRequest = Input.GetKey(_performManipulationButton);
    }
}
