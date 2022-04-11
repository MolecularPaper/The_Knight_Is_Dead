using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using UnityEngine;

public class AdManager : MonoBehaviour
{
    public List<AdUI> adInfos;
    public AdCollection adCollection;

    public void Start()
    {
        adCollection = new AdCollection(adInfos);
    }

    public void SetAdInfos(GameData gameData)
    {
        for (int i = 0; i < gameData.adInfos.Count; i++) {
            this.adInfos[i].SetInfo(gameData.adInfos[i]);
        }
    }
}