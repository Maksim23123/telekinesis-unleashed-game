using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(GameObjectIdentificator))]
public class PerGObjectSaveLoadManager : MonoBehaviour
{
    GameObjectIdentificator _objectIdentificator;

    public readonly static string GAME_OBJECT_PREFAB_PATH_KEY = "GAME_OBJECT_PREFAB_PATH_KEY";

    public ObjectData GetGObjectRecords()
    {
        ObjectData gObjectRecord = new ObjectData();

        gObjectRecord.variableValues.Add(GAME_OBJECT_PREFAB_PATH_KEY, _objectIdentificator.GameObjectPrefabPath);

        IRecordable[] components = GetComponents(typeof(MonoBehaviour))
            .Where(x => x is IRecordable)
            .Select(x => (IRecordable)x)
            .ToArray();

        foreach (var component in components)
        {
            gObjectRecord.objectDataUnits.Add(component.GetType().Name, component.GetObjectData());
        }

        return gObjectRecord;
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
