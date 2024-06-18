using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    float _lerpFactorScalar;

    [Range(0, 1)]
    [SerializeField]
    float _minorObjectFocus;

    [SerializeField]
    Transform _mainObject;

    [SerializeField]
    Transform _minorObject;

    [SerializeField]
    Camera _camera;

    private float _defaultCameraSize;

    [SerializeField]
    private float _distanceToCameraSizeFactor;

    [SerializeField]
    private float _cameraSizingTrasholdDistance;

    [SerializeField]
    private Vector2 _distanceAxisWeights = Vector2.one;

    [SerializeField]
    private float _maxDistFromMainObjectForPositioning;

    [SerializeField]
    private float _maxDistFromMainObjectForSizing;

    private void FixedUpdate()
    {
        UpdateMinorTarget();

        if (_mainObject != null && !_mainObject.IsDestroyed())
        {
            Vector2 targetPossition = _mainObject.transform.position;
            if (_minorObject != null)
            {
                Vector2 targetPositionBias = _minorObject.position - _mainObject.position;
                if (Vector2.Distance(targetPositionBias, Vector2.zero) > _maxDistFromMainObjectForPositioning)
                    targetPositionBias = targetPositionBias.normalized * _maxDistFromMainObjectForPositioning;
                targetPositionBias *= _minorObjectFocus;
                targetPossition += targetPositionBias;
                float minorMainObjectDistance = Vector2.Distance(_minorObject.position * _distanceAxisWeights, _mainObject.position * _distanceAxisWeights);
                UpdateCameraSizeByDistance(minorMainObjectDistance);
            }
            else
                UpdateCameraSizeByDistance(0);
            transform.position = Vector2.Lerp(transform.position, targetPossition, _lerpFactorScalar);
        }
    }

    private void Awake()
    {
        PlayerStatusInformer.newPlayerAssigned += OnNewTargetAssigned;
        _defaultCameraSize = _camera.orthographicSize;
    }

    private void OnNewTargetAssigned(GameObject target)
    {
        _mainObject = target.transform;
    }

    private void UpdateMinorTarget()
    {
        if (PlayerPossessableObjectManager.Instance != null && !PlayerPossessableObjectManager.Instance.IsDestroyed() 
                && PlayerPossessableObjectManager.Instance.CapturedObject != null)
        {
            _minorObject = PlayerPossessableObjectManager.Instance.CapturedObject.transform;
        }
        else
        {
            _minorObject = null;
        }
    }

    private void UpdateCameraSizeByDistance(float distance)
    {
        float newCameraSize = _defaultCameraSize + Mathf.Clamp(distance - _cameraSizingTrasholdDistance
            , 0, _maxDistFromMainObjectForSizing * 2) * _distanceToCameraSizeFactor;
        _camera.orthographicSize = newCameraSize;
    }
}
