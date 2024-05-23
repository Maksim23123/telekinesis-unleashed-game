using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class PlayerObjectManipulation : MonoBehaviour
{
    //Adjustable parameters
    [SerializeField]
    private float impulsPower;
    [SerializeField]
    private KeyCode _performManipulationButton;

    //Storage parameters
    bool _manipulationAllowed = true;

    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        if (InputHandler.Instance.PerformManipulationRequest && _manipulationAllowed)
        {
            PerformManipulation();
            _manipulationAllowed = false;
            Invoke("ResetManipulation", 0.5f);
        }
    }

    private void PerformManipulation()
    {
        GameObject manipulatedObject = PlayerObjectManager.Instance.CapturedObject;
        if (manipulatedObject != null && manipulatedObject.TryGetComponent(out CapturableObject capturableObject))
        {
            Vector3 mousePos = StaticTools.GetMousePositionInWorld();
            Vector3 powerVector = mousePos - gameObject.transform.position;
            float finalPower = impulsPower 
                * Vector3.Distance(powerVector, Vector3.zero) * 8 * Time.deltaTime ;
            capturableObject.ProcessManipulation(powerVector.normalized, finalPower);
        }
    }

    private void ResetManipulation()
    {
        _manipulationAllowed = true;
    }
}
