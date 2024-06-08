using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using Unity.VisualScripting;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerObjectManager : MonoBehaviour
{
    private static PlayerObjectManager _instance;

    public static PlayerObjectManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PlayerObjectManager>();
            }

            return _instance;
        }
    }

    public GameObject CapturedObject { get; private set; }
    public float DamageMultipier { get => _damageMultipier; set => _damageMultipier = value; }

    [SerializeField]
    private LayerMask _capturableObjectsLayers;

    [SerializeField]
    private LayerMask _raycastTestLayers;

    private float _damageMultipier = 1f;

    private float _defaultDamageMultiplier = 1f;

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
        if (TryGetAvailableCapturableObject(out GameObject firstAvailable))
        {
            PerformCaptureQuiting();
            CapturedObject = firstAvailable;
            if (CapturedObject.TryGetComponent(out CapturableObject capturableObject))
            {
                _defaultDamageMultiplier = capturableObject.DamageMultiplier;
                capturableObject.DamageMultiplier = _damageMultipier;
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

    private bool TryGetAvailableCapturableObject(out GameObject firstAvailable)
    {
        firstAvailable = null;

        Vector3 mousePos = StaticTools.GetMousePositionInWorld();

        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, 2.7f, Vector2.down, 0, _capturableObjectsLayers);

        GameObject[] reachableObjects = hits.Select(x => x.transform.gameObject).ToArray();

        List<GameObject> objectsByDistance = reachableObjects
            .OrderBy(x => Vector3.Distance(x.transform.position, mousePos)).ToList();

        if (objectsByDistance.Count > 0 && PerformFinalObjectTest(objectsByDistance[0]))
        {
            firstAvailable = objectsByDistance[0];
            return true;
        }

        return false;
    }

    private bool PerformFinalObjectTest(GameObject testedObject)
    {
        Vector2 directionTowardsItem = (testedObject.transform.position - gameObject.transform.position).normalized;
        RaycastHit2D raycastHit = Physics2D.Raycast(gameObject.transform.position, directionTowardsItem, 3, _raycastTestLayers);
        if (raycastHit.transform.gameObject == testedObject.gameObject)
            return true;
        else
            return false;
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

        if (CapturedObject != null && CapturedObject.TryGetComponent(out CapturableObject capturableObject))
            capturableObject.DamageMultiplier = _defaultDamageMultiplier;
        CapturedObject = null;
    }

    private void ResetCapture()
    {
        _captureAllowed = true;
    }
}
