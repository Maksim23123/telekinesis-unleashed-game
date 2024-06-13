using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveLoadManager
{
    public static event Action _gatherDataFromDataSources;

    private static Dictionary<int, ObjectData> _dataStack = new Dictionary<int, ObjectData>();

    private static readonly string FILE_EXTENSION = "save";

    private static string _currentSaveName = "defaultSave";

    public static string CurrentSaveName { get => _currentSaveName; set => _currentSaveName = value; }

    private static int _objectDataSourceCount = 0;

    public static void SaveGame()
    {
        _gatherDataFromDataSources?.Invoke();
    }

    public static void LoadGame() 
    {
        ObjectData[] loadedData = ReadDataFromFile();
        foreach (ObjectData data in loadedData)
        {
            UnpackObjectData(data);
        }

        _gatherDataFromDataSources = null;
        _objectDataSourceCount = 0;

        SceneRemaker.RequestRemakeScene(loadedData);
    }

    public static void RegisterObjectDataSource(Action action)
    {
        _gatherDataFromDataSources += action;
        _objectDataSourceCount++;
    }

    public static int EnrollToDataStack(ObjectData objectData)
    {
        int stackPositionId = StaticTools.GetFreeId(_dataStack.Keys, x => x);
        _dataStack.Add(stackPositionId, objectData);
        CheckDataStackLoad();
        return stackPositionId;
    }

    private static void CheckDataStackLoad()
    {
        if (_dataStack.Count <= _objectDataSourceCount)
            OnDataStackFull();
    }

    private static void OnDataStackFull()
    {
        ObjectData[] finalDataColection = _dataStack.Values.ToArray();
        foreach (var data in _dataStack.Values)
        {
            UnpackObjectData(data);
        }

        WriteSaveToFile(finalDataColection);

        _dataStack.Clear();
    }
    
    //DEBUG
    private static void UnpackObjectData(ObjectData objectData)
    {
        foreach (var key in objectData.variableValues.Keys)
        {
            Debug.Log(key + " | " + objectData.variableValues[key]);
        }

        foreach (var extractedObjectData in objectData.objectDataUnits.Values)
        {
            UnpackObjectData(extractedObjectData);
        }
    }

    private static void WriteSaveToFile(ObjectData[] finalDataColection)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Path.Combine(Application.persistentDataPath, _currentSaveName + "." + FILE_EXTENSION);
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, finalDataColection);
        stream.Close();
    }

    private static ObjectData[] ReadDataFromFile()
    {
        string path = Path.Combine(Application.persistentDataPath, _currentSaveName + "." + FILE_EXTENSION);
        if (File.Exists(path))
        {
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);
                ObjectData[] receivedData = (ObjectData[])formatter.Deserialize(stream);
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
}
