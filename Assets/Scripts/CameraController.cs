using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    float _lerpFactorScalar;

    [SerializeField]
    Transform _mainObject;

    private void FixedUpdate()
    {
        if (!_mainObject.IsDestroyed())
            transform.position = Vector2.Lerp(transform.position, _mainObject.transform.position, _lerpFactorScalar);
    }

    private void Awake()
    {
        PlayerStatusInformer.newPlayerAssigned += OnNewTargetAssigned;
    }

    private void OnNewTargetAssigned(GameObject target)
    {
        _mainObject = target.transform;
    }
}
