using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPointer : MonoBehaviour
{
    [SerializeField] private PointerType _pointerType;
    public PointerType PointerType { get => _pointerType; set => _pointerType = value; }

    public ContentPointer ToRegularPointer(Vector2 centerPosition)
    {
        ContentPointer pointer = new ContentPointer();
        pointer.RelativePosition = (Vector2)gameObject.transform.position - centerPosition;
        pointer.PointerType = _pointerType;
        return pointer;
    }
}
