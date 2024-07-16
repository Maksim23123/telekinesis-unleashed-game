using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class InstanceSaveLoadManager : MonoBehaviour
{
    [SerializeField][AssetPath(typeof(GameObject))] private string _gameObjectPrefabPath;
    [SerializeField] private GameObjectIdentificatorType _onLoadDataDestributionType;
    [SerializeField][HideInInspector] public string _staticAddress; // It's public because need to be found from editor derived class

    public static readonly string GAME_OBJECT_PREFAB_PATH_KEY = "GAME_OBJECT_PREFAB_PATH";
    public static readonly string GAME_OBJECT_STATIC_ADDRESS_KEY = "GAME_OBJECT_STATIC_ADDRESS";
    private static Dictionary<string, InstanceSaveLoadManager> _staticAddresses = new Dictionary<string, InstanceSaveLoadManager>();

    public GameObjectIdentificatorType OnLoadDataDestributionType { get => _onLoadDataDestributionType; set => _onLoadDataDestributionType = value; }
    public string StaticAddress { get => _staticAddress; set => _staticAddress = value; }

    private void Awake()
    {
        SaveLoadManager.Instance.RegisterObjectDataSource(OnSaveGame);
        if (OnLoadDataDestributionType == GameObjectIdentificatorType.StaticAddress)
        {
            _staticAddresses[StaticAddress] = this;
        }
        if (_onLoadDataDestributionType == GameObjectIdentificatorType.PrefabPath)
        {
            SceneRemaker.PreRemakeActivity += DestroyItself;
        }
    }

    private void OnDestroy()
    {
        SaveLoadManager.Instance?.UnregisterObjectDataSource(OnSaveGame);
        if (OnLoadDataDestributionType == GameObjectIdentificatorType.StaticAddress
                && _staticAddresses[StaticAddress] == this)
            _staticAddresses.Remove(StaticAddress);
        else 
            SceneRemaker.PreRemakeActivity -= DestroyItself;
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

    private void OnSaveGame()
    {
        SaveLoadManager.Instance.EnrollToDataStack(GetGObjectRecords());
    }

    private IRecordable[] GetRecordables()
    {
        return GetComponents(typeof(MonoBehaviour))
            .Where(x => x is IRecordable)
            .Select(x => (IRecordable)x)
            .ToArray();
    }

    public ObjectData GetGObjectRecords()
    {
        ObjectData gObjectRecord = new ObjectData();

        if (_onLoadDataDestributionType == GameObjectIdentificatorType.PrefabPath)
            gObjectRecord.VariableValues.Add(GAME_OBJECT_PREFAB_PATH_KEY, _gameObjectPrefabPath);
        else
            gObjectRecord.VariableValues.Add(GAME_OBJECT_STATIC_ADDRESS_KEY, StaticAddress);

        IRecordable[] components = GetRecordables();

        foreach (var component in components)
        {
            gObjectRecord.ObjectDataUnits.Add(component.GetType().Name, component.GetObjectData());
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
            if (data.ObjectDataUnits.TryGetValue(component.GetType().Name, out ObjectData componentData))
            {
                component.SetObjectData(componentData);
            }
        }
    }
}
