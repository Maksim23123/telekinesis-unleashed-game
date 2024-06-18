using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;

public class PerGObjectSaveLoadManager : MonoBehaviour
{
    GameObjectIdentificator _objectIdentificator;

    [SerializeField]
    GameObjectIdentificatorType _onLoadDataDestributionType;

    public readonly static string GAME_OBJECT_PREFAB_PATH_KEY = "GAME_OBJECT_PREFAB_PATH";
    public readonly static string GAME_OBJECT_STATIC_ADDRESS_KEY = "GAME_OBJECT_STATIC_ADDRESS";

    [HideInInspector]
    public string _staticAddress;

    private static Dictionary<string, PerGObjectSaveLoadManager> _staticAddresses = new Dictionary<string, PerGObjectSaveLoadManager>();

    public GameObjectIdentificatorType OnLoadDataDestributionType { get => _onLoadDataDestributionType; set => _onLoadDataDestributionType = value; }
    public string StaticAddress { get => _staticAddress; set => _staticAddress = value; }

    public ObjectData GetGObjectRecords()
    {
        ObjectData gObjectRecord = new ObjectData();

        if (_onLoadDataDestributionType == GameObjectIdentificatorType.PrefabPath)
            gObjectRecord.variableValues.Add(GAME_OBJECT_PREFAB_PATH_KEY, _objectIdentificator.GameObjectPrefabPath);
        else
            gObjectRecord.variableValues.Add(GAME_OBJECT_STATIC_ADDRESS_KEY, _staticAddress);

        IRecordable[] components = GetRecordables();

        foreach (var component in components)
        {
            gObjectRecord.objectDataUnits.Add(component.GetType().Name, component.GetObjectData());
        }

        return gObjectRecord;
    }

    public void SetGObjectData(ObjectData data)
    {
        // FIX: Probably, can be replaced with sorted list.
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
        SaveLoadManager.Instance.RegisterObjectDataSource(OnSaveGame);
        if (OnLoadDataDestributionType == GameObjectIdentificatorType.StaticAddress)
        {
            _staticAddresses[_staticAddress] = this;
        }
    }

    private void OnSaveGame()
    {
        SaveLoadManager.Instance.EnrollToDataStack(GetGObjectRecords());
    }

    private void OnDestroy()
    {
        SaveLoadManager.Instance?.UnregisterObjectDataSource(OnSaveGame);
        if (OnLoadDataDestributionType == GameObjectIdentificatorType.StaticAddress 
                && _staticAddresses[_staticAddress] == this)
        {
            _staticAddresses.Remove(_staticAddress);
        }
    }

    public static bool TryGetPerGObjectStaticAddressManager(string staticAddress
            , out PerGObjectSaveLoadManager perGObjectSaveLoadManager)
    {
        return _staticAddresses.TryGetValue(staticAddress, out perGObjectSaveLoadManager);
    }
}
