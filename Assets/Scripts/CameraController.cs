using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    float _lerpFactorScalar;

    [SerializeField]
    Transform _mainObject;

    private void FixedUpdate()
    {
        transform.position = Vector2.Lerp(transform.position, _mainObject.transform.position, _lerpFactorScalar);
    }
}
