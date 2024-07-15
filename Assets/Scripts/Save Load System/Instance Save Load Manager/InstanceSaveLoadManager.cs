using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class InstanceSaveLoadManager : MonoBehaviour
{
    [AssetPath(typeof(GameObject))]
    [SerializeField]
    private string _gameObjectPrefabPath;
    [SerializeField]
    private GameObjectIdentificatorType _onLoadDataDestributionType;

    public static readonly string GAME_OBJECT_PREFAB_PATH_KEY = "GAME_OBJECT_PREFAB_PATH";
    public static readonly string GAME_OBJECT_STATIC_ADDRESS_KEY = "GAME_OBJECT_STATIC_ADDRESS";
    //private GameObjectIdentificator _objectIdentificator;
    [HideInInspector]
    public string _staticAddress;
    private static Dictionary<string, InstanceSaveLoadManager> _staticAddresses = new Dictionary<string, InstanceSaveLoadManager>();

    public GameObjectIdentificatorType OnLoadDataDestributionType { get => _onLoadDataDestributionType; set => _onLoadDataDestributionType = value; }
    public string StaticAddress { get => _staticAddress; set => _staticAddress = value; }

    private IRecordable[] GetRecordables()
    {
        return GetComponents(typeof(MonoBehaviour))
            .Where(x => x is IRecordable)
            .Select(x => (IRecordable)x)
            .ToArray();
    }

    private void Awake()
    {
        //_objectIdentificator = GetComponent<GameObjectIdentificator>();
        SaveLoadManager.Instance.RegisterObjectDataSource(OnSaveGame);
        if (OnLoadDataDestributionType == GameObjectIdentificatorType.StaticAddress)
        {
            _staticAddresses[_staticAddress] = this;
        }
        if (_onLoadDataDestributionType == GameObjectIdentificatorType.PrefabPath)
        {
            SceneRemaker._preRemakeActivity += DestroyItself;
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
            _staticAddresses.Remove(_staticAddress);
        else 
            SceneRemaker._preRemakeActivity -= DestroyItself;
        PlayerStatusInformer.InformPlayerDestroyed();
    }

    private void DestroyItself()
    {
        Destroy(gameObject);
    }

    public static bool TryGetInstanceSaveLoadManager(string staticAddress
            , out InstanceSaveLoadManager instanceSaveLoadManager)
    {
        return _staticAddresses.TryGetValue(staticAddress, out instanceSaveLoadManager);
    }

    public ObjectData GetGObjectRecords()
    {
        ObjectData gObjectRecord = new ObjectData();

        if (_onLoadDataDestributionType == GameObjectIdentificatorType.PrefabPath)
            gObjectRecord.variableValues.Add(GAME_OBJECT_PREFAB_PATH_KEY, _gameObjectPrefabPath);
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
}
