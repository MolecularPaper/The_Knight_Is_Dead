using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
public class NoDeletedData
{
    public string nickName;
    public bool adDeleted;
    public float bgmVolume;
    public float seVolume;

    public NoDeletedData(string nickName)
    {
        this.nickName = nickName;
    }

    public NoDeletedData(PlayerInfo playerInfo, GameInfo gameInfo)
    {
        this.nickName = playerInfo.nickName;
        this.adDeleted = gameInfo.adDeleted;
        this.bgmVolume = gameInfo.bgmVolume;
        this.seVolume = gameInfo.seVolume;
    }
}

[Serializable]
public class GameData
{
    public List<ItemInfo> itemInfos = new List<ItemInfo>();
    public List<AbilityInfo> abilityInfos = new List<AbilityInfo>();
    public List<AdInfo> adInfos = new List<AdInfo>();

    public uint playerLevel;
    public uint playerSkillPoint;
    public ulong playerExp;

    public int highestStageIndex;
    public int stageIndex;

    public GameData(PlayerInfo playerInfo, GameInfo gameInfo, AdCollection adCollection)
    {
        foreach (var item in playerInfo.abilities) {
            this.abilityInfos.Add(new AbilityInfo(item));
        }

        foreach (var item in playerInfo.items) {
            this.itemInfos.Add(new ItemInfo(item));
        }

        foreach (var item in adCollection.ads) {
            adInfos.Add(new AdInfo(item));
        }

        this.playerLevel = playerInfo.level;
        this.playerSkillPoint = playerInfo.skillPoint;
        this.playerExp = playerInfo.exp;

        this.highestStageIndex = gameInfo.highestStageIndex;
        this.stageIndex = gameInfo.stageIndex;
    }
}

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager dataManager;

    [SerializeField] private string gameDataFileName;
    [SerializeField] private string noDeletedDataFileName;
    [SerializeField] private string titleDataFileName;
    private readonly string privateKey = "7ZWY64KY7IS47Z6I64KY7IS47J207JW86rCA652866eI7IKs67mE";

    [Space(10)]
    [SerializeField] private bool loadAdData;
    [SerializeField] private bool loadGameData;
    [SerializeField] private bool loadPlayerData;

    private void Awake()
    {
        dataManager = this;

        NoDeletedData noDeletedData = null;
        GameData gameData = null;

        try {
            noDeletedData = LoadNoDeletedData();
        }
        catch (DirectoryNotFoundException) { return; }

        try {
            gameData = LoadGameData();
        }
        catch (DirectoryNotFoundException) { }

        if (loadAdData) {
            AdManager adManager = FindObjectOfType<AdManager>();
            adManager.LoadData(gameData);
        }

        if (loadGameData) {
            GameManager gameManager = FindObjectOfType<GameManager>();
            gameManager.LoadData(gameData, noDeletedData);
        }

        if (loadPlayerData) {
            PlayerCTRL playerCTRL = FindObjectOfType<PlayerCTRL>();
            playerCTRL.LoadData(gameData, noDeletedData);
        }
    }

    private void OnApplicationQuit() => SaveGameData();

    public void SaveNoDeletedData(NoDeletedData noDeletedData) => SaveFile(noDeletedData, noDeletedDataFileName);

    public void SaveGameData()
    {
        PlayerCTRL playerCTRL = FindObjectOfType<PlayerCTRL>();
        AdCollection adCollection = AdManager.adManager.adCollection;

        GameData saveData = new GameData(playerCTRL, GameManager.gm, adCollection);
        SaveFile(saveData, gameDataFileName);
    }

    public NoDeletedData LoadNoDeletedData() => LoadData<NoDeletedData>(noDeletedDataFileName);

    public GameData LoadGameData() => LoadData<GameData>(gameDataFileName);

    public void SaveFile<T>(T saveData, string fileName)
    {
        string path = Application.persistentDataPath + $"/{fileName}.sav";
        string jsonData = JsonUtility.ToJson(saveData);
        string encryptString = Encrypt(jsonData);
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(encryptString);

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.OpenOrCreate);

        formatter.Serialize(stream, bytes);
        stream.Close();
    }

    public T LoadData<T>(string fileName)
    {
        string path = Application.persistentDataPath + $"/{fileName}.sav";

        if (File.Exists(path)) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            byte[] bytes = formatter.Deserialize(stream) as byte[];
            string encryptData = System.Text.Encoding.UTF8.GetString(bytes);
            string jsonData = Decrypt(encryptData);
            T saveData = JsonUtility.FromJson<T>(jsonData);

            stream.Close();

            return saveData;
        }
        else {
            throw new DirectoryNotFoundException();
        }
    }

    public void DeleteSaveData()
    {
        string path = Application.persistentDataPath + $"/{gameDataFileName}.sav";
        if (File.Exists(path)) File.Delete(path);
        GameManager.tokenSource.Cancel();
        SceneManager.LoadScene(gameObject.scene.name);
    }

    private string Encrypt(string data)
    {

        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(data);
        RijndaelManaged rm = CreateRijndaelManaged();
        ICryptoTransform ct = rm.CreateEncryptor();
        byte[] results = ct.TransformFinalBlock(bytes, 0, bytes.Length);
        return System.Convert.ToBase64String(results, 0, results.Length);

    }

    private string Decrypt(string data)
    {
        byte[] bytes = System.Convert.FromBase64String(data);
        RijndaelManaged rm = CreateRijndaelManaged();
        ICryptoTransform ct = rm.CreateDecryptor();
        byte[] resultArray = ct.TransformFinalBlock(bytes, 0, bytes.Length);
        return System.Text.Encoding.UTF8.GetString(resultArray);
    }


    private RijndaelManaged CreateRijndaelManaged()
    {
        byte[] keyArray = System.Text.Encoding.UTF8.GetBytes(privateKey);
        RijndaelManaged result = new RijndaelManaged();

        byte[] newKeysArray = new byte[16];
        System.Array.Copy(keyArray, 0, newKeysArray, 0, 16);

        result.Key = newKeysArray;
        result.Mode = CipherMode.ECB;
        result.Padding = PaddingMode.PKCS7;
        return result;
    }
}
