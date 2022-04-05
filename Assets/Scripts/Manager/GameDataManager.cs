using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameDataManager : MonoBehaviour
{
    [SerializeField] private string saveFileName;

    public void SaveData(PlayerData playerData, GameData gameData)
    {
        string path = Application.persistentDataPath + $"/{saveFileName}.sav";

        Debug.Log(path);

        GameSaveData saveData = new GameSaveData(playerData, gameData);

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.OpenOrCreate);

        formatter.Serialize(stream, saveData);
        stream.Close();
    }

    public GameSaveData LoadData()
    {
        string path = Application.persistentDataPath + $"/{saveFileName}.sav";

        if (File.Exists(path)) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            GameSaveData saveData = formatter.Deserialize(stream) as GameSaveData;
            stream.Close();

            return saveData;
        }
        else {
            throw new DirectoryNotFoundException();
        }
    }

    public void DeleteSaveData()
    {
        string path = Application.persistentDataPath + $"/{saveFileName}.sav";
        if (File.Exists(path)) File.Delete(path);
        SceneManager.LoadScene(gameObject.scene.name);
        GameManager.gm.CancelEntityToken();
    }
}
