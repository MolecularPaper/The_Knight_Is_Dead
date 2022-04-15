using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using UnityEngine;

public class AdManager : MonoBehaviour
{
    public static AdManager adManager;

    public List<AdUI> adExtensions;
    public AdCollection adCollection;

    public void Awake()
    {
        adManager = this;
    }

    public void LoadData(GameData gameData)
    {
        if(gameData != null) {
            SetAdInfos(gameData);
        }
    }

    public void Start()
    {
        adCollection = new AdCollection(adExtensions);
    }

    public void SetAdInfos(GameData gameData)
    {
        for (int i = 0; i < gameData.adInfos.Count; i++) {
            this.adExtensions[i].SetInfo(gameData.adInfos[i]);
        }
    }
}