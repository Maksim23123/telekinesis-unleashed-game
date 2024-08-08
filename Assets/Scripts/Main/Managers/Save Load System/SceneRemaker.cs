using System;
using UnityEngine;

public static class SceneRemaker
{
    public static event Action PreRemakeActivity;

    private static void UnpackGameObjectData(ObjectData gameObjectsData)
    {
        foreach (ObjectData obj in gameObjectsData.ObjectDataUnits.Values)
        {
            if (obj.VariableValues.TryGetValue(InstanceSaveLoadManager.GAME_OBJECT_PREFAB_PATH_KEY, out string prefabPath))
            {
                string resourcePath = StaticTools.GetResourcePath(prefabPath);
                GameObject prefab = Resources.Load<GameObject>(resourcePath);
                if (prefab != null)
                {
                    GameObject gameObjectInstance = GameObject.Instantiate(prefab);
                    if (gameObjectInstance.TryGetComponent(out InstanceSaveLoadManager ofObjectSaveLoadManager))
                    {
                        ofObjectSaveLoadManager.SetGObjectData(obj);
                    }
                }
            }
            else if (obj.VariableValues.TryGetValue(InstanceSaveLoadManager.GAME_OBJECT_STATIC_ADDRESS_KEY, out string staticAddress)
                    && InstanceSaveLoadManager.TryGetInstanceSaveLoadManager(staticAddress, out InstanceSaveLoadManager instanceSaveLoadManager))
            {
                instanceSaveLoadManager.SetGObjectData(obj);
            }
        }
    }

    public static void RequestRemakeScene(ObjectData gameObjectsData)
    {
        PreRemakeActivity?.Invoke();
        UnpackGameObjectData(gameObjectsData);
    }
}
