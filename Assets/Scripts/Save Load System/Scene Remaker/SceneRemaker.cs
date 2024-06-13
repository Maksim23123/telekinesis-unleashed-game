using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class SceneRemaker
{
    public static event Action preRemakeActivity;
    
    public static void RequestRemakeScene(ObjectData[] gameObjectsData)
    {
        preRemakeActivity?.Invoke();
        UnpackGameObjectData(gameObjectsData);
    }

    private static void UnpackGameObjectData(ObjectData[] gameObjectsData)
    {
        foreach (ObjectData obj in gameObjectsData)
        {
            if (obj.variableValues.TryGetValue(PerGObjectSaveLoadManager.GAME_OBJECT_PREFAB_PATH_KEY, out string prefabPath))
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                if (prefab != null)
                {
                    GameObject gameObjectInstance = GameObject.Instantiate(prefab);
                    if (gameObjectInstance.TryGetComponent(out PerGObjectSaveLoadManager ofObjectSaveLoadManager))
                    {
                        ofObjectSaveLoadManager.SetGObjectData(obj);
                    }
                }
            }
        }
    }
}
