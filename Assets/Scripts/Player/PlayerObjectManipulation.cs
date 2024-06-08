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

    public float ManipulationCooldown { get => _manipulationCooldown; set => _manipulationCooldown = value; }

    //Adjustable parameters
    [SerializeField]
    private float _impulsPower;

    private float _manipulationCooldown;

    //Storage parameters
    bool _manipulationAllowed = true;

    public void RequestManipulation()
    {
        if (_manipulationAllowed)
        {
            PerformManipulation();
            if (_manipulationCooldown > 0)
            {
                _manipulationAllowed = false;
                Invoke(nameof(ResetManipulation), _manipulationCooldown);
            }
        }
    }

    private void PerformManipulation()
    {
        GameObject manipulatedObject = PlayerObjectManager.Instance.CapturedObject;
        if (manipulatedObject != null && manipulatedObject.TryGetComponent(out CapturableObject capturableObject))
        {
            Vector3 mousePos = StaticTools.GetMousePositionInWorld();
            Vector3 powerVector = mousePos - manipulatedObject.transform.position;
            float minDistanceFactor = 10;
            float finalPower = _impulsPower 
                * Mathf.Clamp(Vector3.Distance(powerVector, Vector3.zero), minDistanceFactor, float.MaxValue) * 8 * Time.fixedDeltaTime;
            capturableObject.ProcessManipulation(powerVector.normalized, finalPower);
        }
    }

    private void ResetManipulation()
    {
        _manipulationAllowed = true;
    }
}
