using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SceneRemaker
{
    public static event Action preRemakeActivity;
    
    public static void RequestRemakeScene(ObjectData gameObjectsData)
    {
        preRemakeActivity?.Invoke();
        UnpackGameObjectData(gameObjectsData);
    }

    private static void UnpackGameObjectData(ObjectData gameObjectsData)
    {
        foreach (ObjectData obj in gameObjectsData.objectDataUnits.Values)
        {
            if (obj.variableValues.TryGetValue(PerGObjectSaveLoadManager.GAME_OBJECT_PREFAB_PATH_KEY, out string prefabPath))
            {
                string resourcePath = StaticTools.GetResourcePath(prefabPath);
                GameObject prefab = Resources.Load<GameObject>(resourcePath);
                if (prefab != null)
                {
                    GameObject gameObjectInstance = GameObject.Instantiate(prefab);
                    if (gameObjectInstance.TryGetComponent(out PerGObjectSaveLoadManager ofObjectSaveLoadManager))
                    {
                        ofObjectSaveLoadManager.SetGObjectData(obj);
                    }
                }
            }
            else if (obj.variableValues.TryGetValue(PerGObjectSaveLoadManager.GAME_OBJECT_STATIC_ADDRESS_KEY, out string staticAddress) 
                    && PerGObjectSaveLoadManager.TryGetPerGObjectStaticAddressManager(staticAddress, out PerGObjectSaveLoadManager perGObjectSaveLoadManager))
            {
                perGObjectSaveLoadManager.SetGObjectData(obj);
            }
        }
    }
}
