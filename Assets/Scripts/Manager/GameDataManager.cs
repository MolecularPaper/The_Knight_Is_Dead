using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class GameData
{
    public List<ItemInfo> itemInfos = new List<ItemInfo>();
    public List<AbilityInfo> abilityInfos = new List<AbilityInfo>();
    public List<AdInfo> adInfos = new List<AdInfo>();
    public int highestStageIndex;
    public int stageIndex;

    public GameData(MobInfo playerInfo, GameInfo gameInfo, AdCollection adCollection)
    {
        foreach (var item in playerInfo.abilities) {
            AbilityInfo abilityInfo = new AbilityInfo();
            abilityInfo.SetAbility(item);
            this.abilityInfos.Add(abilityInfo);
        }

        foreach (var item in playerInfo.items) {
            this.itemInfos.Add(new ItemInfo(item));
        }

        foreach (var item in adCollection.ads) {
            AdInfo adInfo = new AdInfo();
            adInfo.SetInfo(item);
            adInfos.Add(adInfo);
        }

        this.highestStageIndex = gameInfo.highestStageIndex;
        this.stageIndex = gameInfo.stageIndex;
    }
}

public class GameDataManager : MonoBehaviour
{
    [SerializeField] private string saveFileName;

    public void SaveData(MobInfo playerInfo, GameInfo gameInfo, AdCollection adCollection)
    {
        string path = Application.persistentDataPath + $"/{saveFileName}.sav";
        GameData saveData = new GameData(playerInfo, gameInfo, adCollection);

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.OpenOrCreate);

        formatter.Serialize(stream, saveData);
        stream.Close();
    }

    public GameData LoadData()
    {
        string path = Application.persistentDataPath + $"/{saveFileName}.sav";

        if (File.Exists(path)) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            GameData saveData = formatter.Deserialize(stream) as GameData;
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
    }
}
