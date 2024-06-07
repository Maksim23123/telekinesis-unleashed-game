using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class PlayerObjectManipulation : MonoBehaviour
{
    private static PlayerObjectManipulation _instance;

    public static PlayerObjectManipulation Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PlayerObjectManipulation>();
            }

            return _instance;
        }
    }

    //Adjustable parameters
    [SerializeField]
    private float impulsPower;

    //Storage parameters
    bool _manipulationAllowed = true;

    public void RequestManipulation()
    {
        if (_manipulationAllowed)
        {
            PerformManipulation();
            _manipulationAllowed = false;
            Invoke(nameof(ResetManipulation), 0.5f);
        }
    }

    private void PerformManipulation()
    {
        GameObject manipulatedObject = PlayerObjectManager.Instance.CapturedObject;
        if (manipulatedObject != null && manipulatedObject.TryGetComponent(out CapturableObject capturableObject))
        {
            Vector3 mousePos = StaticTools.GetMousePositionInWorld();
            Vector3 powerVector = mousePos - manipulatedObject.transform.position;
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
