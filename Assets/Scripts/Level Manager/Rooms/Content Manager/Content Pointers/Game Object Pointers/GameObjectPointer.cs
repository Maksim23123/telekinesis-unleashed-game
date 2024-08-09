using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameObjectPointer : MonoBehaviour
{
    public abstract ContentPointer ToRegularPointer(Vector2 centerPosition);

    public abstract void SetValuesFromPointer(ContentPointer contentPointer);
}
