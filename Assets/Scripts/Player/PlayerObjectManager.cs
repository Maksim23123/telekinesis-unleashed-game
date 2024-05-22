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
    public GameObject _CapturedObject { get; private set; }

    private List<GameObject> _reachableObjects = new List<GameObject>();

    [SerializeField]
    private LayerMask _capturableObjectsLayers;

    [SerializeField]
    private LayerMask _raycastTestLayers;

    [SerializeField]
    private KeyCode _captureObjectButton;

    [SerializeField]
    private KeyCode _quitCapturingButton;

    //Storage parameters
    bool _captureRequest = false;
    bool _quitCapturingRequest = false;
    bool _captureAllowed = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
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
        if (_captureRequest && _captureAllowed)
        {
            PerformCapturing();
            _captureAllowed = false;
            Invoke("ResetCapture", 0.1f);
        }
        if (_quitCapturingRequest)
        {
            PerformCaptureQuiting();
        }
    }

    private void Update()
    {
        GetPlayerInput();
        /*
        if (_CapturedObject != null) 
            Debug.Log(_CapturedObject.name);
        */
    }

    private void GetPlayerInput()
    {
        _captureRequest = Input.GetKey(_captureObjectButton);
        _quitCapturingRequest = Input.GetKey(_quitCapturingButton);
    }

    private void PerformCapturing()
    {
        if (TryGetAvailableCapturableObject(out GameObject firstAvailable))
        {
            PerformCaptureQuiting();
            _CapturedObject = firstAvailable;
            try
            {
                _CapturedObject.GetComponent<SpriteRenderer>().color = Color.red;
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

        Vector3 mousePos = GetMousePositionInWorld();

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
        if (_CapturedObject != null)
        {
            try
            {
                _CapturedObject.GetComponent<SpriteRenderer>().color = Color.green;
            }
            catch
            {
                Debug.Log("Fail");
            }
        }
        _CapturedObject = null;
    }

    private Vector3 GetMousePositionInWorld()
    {
        Vector3 mousePosition = Input.mousePosition;
        return Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.nearClipPlane));
    }

    private void ResetCapture()
    {
        _captureAllowed = true;
    }
}
