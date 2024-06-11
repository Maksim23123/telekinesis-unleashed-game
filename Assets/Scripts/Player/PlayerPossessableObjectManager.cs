using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPossessableObjectManager : MonoBehaviour
{
    private static PlayerPossessableObjectManager _instance;

    public static PlayerPossessableObjectManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PlayerPossessableObjectManager>();
            }

            return _instance;
        }
    }

    public GameObject CapturedObject { get; private set; }
    public float CaptureZoneRadius { get => _captureZoneRadius; set => _captureZoneRadius = value; }
    public PossessableObjectStatsStorage ObjectStatsStorage { get => _objectStatsStorage; set => _objectStatsStorage = value; }

    [SerializeField]
    private LayerMask _capturableObjectsLayers;

    private PossessableObjectStatsStorage _objectStatsStorage = new PossessableObjectStatsStorage();

    private PossessableObjectStatsStorage _defaultObjectStatsStorage = new PossessableObjectStatsStorage();

    private float _captureZoneRadius = 2.7f;

    //Storage parameters
    bool _captureAllowed = true;

    public void RequestCapturing()
    {
        if (_captureAllowed)
        {
            PerformCapturing();
            _captureAllowed = false;
            Invoke(nameof(ResetCapture), 0.1f);
        }
    }

    public void RequestQuitCapturing()
    {
        PerformCaptureQuiting();
    }

    private void PerformCapturing()
    {
        if (PlayerInteractionManager.Instance.TryGetAvailableInteractableObject(out GameObject firstAvailable, _capturableObjectsLayers, _captureZoneRadius))
        {
            PerformCaptureQuiting();
            CapturedObject = firstAvailable;
            if (CapturedObject.TryGetComponent(out PossessableObject capturableObject))
            {
                _defaultObjectStatsStorage = capturableObject.StatsStorage;
                capturableObject.StatsStorage = _objectStatsStorage;
            }


            // DEBUG
            //---
            try
            {
                CapturedObject.GetComponent<SpriteRenderer>().color = Color.red;
            }
            catch
            {
                Debug.Log("Fail");
            }
            //---
        }
    }

    private void PerformCaptureQuiting()
    {
        // DEBUG
        //---
        if (CapturedObject != null)
        {

            try
            {
                CapturedObject.GetComponent<SpriteRenderer>().color = Color.green;
            }
            catch
            {
                Debug.Log("Fail");
            }
        }
        //---

        if (CapturedObject != null && CapturedObject.TryGetComponent(out PossessableObject capturableObject))
            capturableObject.StatsStorage = _defaultObjectStatsStorage;
        CapturedObject = null;
    }

    private void ResetCapture()
    {
        _captureAllowed = true;
    }
}
