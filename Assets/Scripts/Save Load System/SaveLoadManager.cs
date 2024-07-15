using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    private static SaveLoadManager _instance;

    public static SaveLoadManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SaveLoadManager>();
            }

            return _instance;
        }
    }

    public event Action _gatherDataFromDataSources;

    private ObjectData _dataStack = new ObjectData();
    static readonly string FILE_EXTENSION = "save";
    private string _currentSaveName = "defaultSave";
    private int _objectDataSourceCount = 0;

    public string CurrentSaveName { get => _currentSaveName; set => _currentSaveName = value; }

    private void CheckDataStackLoad()
    {
        int dataSourcesCount = _gatherDataFromDataSources?.GetInvocationList().Length ?? 0;
        if (_dataStack.ObjectDataUnits.Count >= dataSourcesCount)
            OnDataStackFull();
    }

    private void OnDataStackFull()
    {
        WriteSaveToFile();

        _dataStack.ObjectDataUnits.Clear();
    }

    // DEBUG
    private void UnpackObjectData(ObjectData objectData)
    {
        foreach (var key in objectData.VariableValues.Keys)
        {
            Debug.Log(key + " | " + objectData.VariableValues[key]);
        }

        foreach (var extractedObjectData in objectData.ObjectDataUnits.Values)
        {
            UnpackObjectData(extractedObjectData);
        }
    }

    private void WriteSaveToFile()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Path.Combine(Application.persistentDataPath, _currentSaveName + "." + FILE_EXTENSION);
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, _dataStack);
        stream.Close();
    }

    private ObjectData ReadDataFromFile()
    {
        string path = Path.Combine(Application.persistentDataPath, _currentSaveName + "." + FILE_EXTENSION);
        if (File.Exists(path))
        {
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);
                ObjectData receivedData = (ObjectData)formatter.Deserialize(stream);

                stream.Close();
                return receivedData;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                return null;
            }
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }

    public void SaveGame()
    {
        _gatherDataFromDataSources?.Invoke();
    }

    public void LoadGame()
    {
        ObjectData loadedData = ReadDataFromFile();

        _gatherDataFromDataSources = null;
        _objectDataSourceCount = 0;

        SceneRemaker.RequestRemakeScene(loadedData);
    }

    public void RegisterObjectDataSource(Action action)
    {
        _gatherDataFromDataSources += action;
        _objectDataSourceCount++;
    }

    public void UnregisterObjectDataSource(Action action)
    {
        _gatherDataFromDataSources -= action;
    }

    public int EnrollToDataStack(ObjectData objectData)
    {
        int stackPositionId;
        stackPositionId = StaticTools.GetFreeId(_dataStack.ObjectDataUnits.Keys.ToArray(), x => int.Parse(x));
        _dataStack.ObjectDataUnits.Add(stackPositionId.ToString(), objectData);
        CheckDataStackLoad();
        return stackPositionId;
    }
}
