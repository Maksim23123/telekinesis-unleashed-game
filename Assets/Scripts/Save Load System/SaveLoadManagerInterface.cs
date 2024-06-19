using UnityEngine;


public class SaveLoadManagerInterface : MonoBehaviour
{
    public void RequestSaveGame()
    {
        SaveLoadManager.Instance.SaveGame();
    }

    public void RequestLoadGame()
    {
        if (Application.isPlaying)
            SaveLoadManager.Instance.LoadGame();
    }
}
