using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class StaticTools
{
    public static Vector3 GetMousePositionInWorld()
    {
        Vector3 mousePosition = Input.mousePosition;
        return Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.nearClipPlane));
    }

    public static int GetFreeId<T>(IEnumerable<T> idEnvironment, Func<T ,int> unpuckObject)
    {
        
        int[] capturedIds = idEnvironment.Select(unpuckObject).ToArray();
        if (capturedIds.Length > 0)
        {
            int freeId = capturedIds.Length;
            while (capturedIds.Contains(freeId))
            {
                freeId++;
            }
            return freeId;
        }
        return 0;
    }
}
