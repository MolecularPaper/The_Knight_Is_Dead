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

    public void SetAdInfos(List<AdInfo> adInfos)
    {
        for (int i = 0; i < adInfos.Count; i++) {
            this.adInfos[i].SetInfo(adInfos[i]);
        }
    }
}