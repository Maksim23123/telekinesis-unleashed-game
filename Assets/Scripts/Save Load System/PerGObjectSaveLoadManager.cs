using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(GameObjectIdentificator))]
public class PerGObjectSaveLoadManager : MonoBehaviour
{
    GameObjectIdentificator _objectIdentificator;

    public readonly static string GAME_OBJECT_PREFAB_PATH_KEY = "GAME_OBJECT_PREFAB_PATH";

    public ObjectData GetGObjectRecords()
    {
        ObjectData gObjectRecord = new ObjectData();

        gObjectRecord.variableValues.Add(GAME_OBJECT_PREFAB_PATH_KEY, _objectIdentificator.GameObjectPrefabPath);

        IRecordable[] components = GetRecordables();

        foreach (var component in components)
        {
            gObjectRecord.objectDataUnits.Add(component.GetType().Name, component.GetObjectData());
        }

        return gObjectRecord;
    }

    public void SetGObjectData(ObjectData data)
    {
        IRecordable[] components = GetRecordables()
            .OrderBy(x => x.Priority)
            .Reverse()
            .ToArray();

        foreach (var component in components)
        {
            if (data.objectDataUnits.TryGetValue(component.GetType().Name, out ObjectData componentData))
            {
                component.SetObjectData(componentData);
            }
        }
    }

    private IRecordable[] GetRecordables() 
    {
        return GetComponents(typeof(MonoBehaviour))
            .Where(x => x is IRecordable)
            .Select(x => (IRecordable)x)
            .ToArray();
    }

    private void Awake()
    {
        _objectIdentificator = GetComponent<GameObjectIdentificator>();
        SaveLoadManager.RegisterObjectDataSource(OnSaveGame);
    }

    private void OnSaveGame()
    {
        SaveLoadManager.EnrollToDataStack(GetGObjectRecords());
    }
}
