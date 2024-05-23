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

    private List<GameObject> _reachableObjects = new List<GameObject>();

    [SerializeField]
    private LayerMask _capturableObjectsLayers;

    [SerializeField]
    private LayerMask _raycastTestLayers;

    //Storage parameters
    bool _captureAllowed = true;

    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Can be replaced with circle casting
        if (_capturableObjectsLayers.Contains(collision.gameObject.layer))
            _reachableObjects.Add(collision.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_reachableObjects.Contains(collision.gameObject))
            _reachableObjects.Remove(collision.gameObject);
    }

    private void FixedUpdate()
    {
        if (InputHandler.Instance.CaptureRequest && _captureAllowed)
        {
            PerformCapturing();
            _captureAllowed = false;
            Invoke("ResetCapture", 0.1f);
        }
        if (InputHandler.Instance.QuitCapturingRequest)
        {
            PerformCaptureQuiting();
        }
    }


    private void PerformCapturing()
    {
        if (TryGetAvailableCapturableObject(out GameObject firstAvailable))
        {
            PerformCaptureQuiting();
            CapturedObject = firstAvailable;
            try
            {
                CapturedObject.GetComponent<SpriteRenderer>().color = Color.red;
            }
            catch
            {
                Debug.Log("Fail");
            }
        }
            
    }

    private bool TryGetAvailableCapturableObject(out GameObject firstAvailable)
    {
        firstAvailable = null;

        Vector3 mousePos = StaticTools.GetMousePositionInWorld();

        List<GameObject> objectsByDistance = _reachableObjects
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
        CapturedObject = null;
    }

    private void ResetCapture()
    {
        _captureAllowed = true;
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
