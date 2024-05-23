using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticTools
{
    public static Vector3 GetMousePositionInWorld()
    {
        Vector3 mousePosition = Input.mousePosition;
        return Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.nearClipPlane));
    }
}
