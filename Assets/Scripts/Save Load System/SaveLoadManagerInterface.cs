using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class SaveLoadManagerInterface : MonoBehaviour
{
    public void RequestSaveGame()
    {
        SaveLoadManager.SaveGame();
    }

    public void RequestLoadGame()
    {
        SaveLoadManager.LoadGame();
    }
}
