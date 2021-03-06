using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using UnityEngine;

public class AdManager : MonoBehaviour
{
    public static AdManager adManager;

    [SerializeField] private Transform adTap;
    [SerializeField] private GameObject adUI;
    public bool isTest;

    public BannerAd bannerAd;
    public List<RewardAd> rewardedAds;

    public void Awake()
    {
        adManager = this;
    }

    public void Start()
    {
        foreach (var rewardAd in rewardedAds) {
            CreateAdUI(rewardAd);
            rewardAd.Reset();
        }

        bannerAd.Reset();
    }

    public void SetAdInfos(GameData gameData)
    {
        if (gameData == null) return;

        for (int i = 0; i < gameData.rewardedAds.Count; i++) {
            rewardedAds[i].SetInfo(gameData.rewardedAds[i]);
            rewardedAds[i].CalculateTime();
        }
    }

    public void CreateAdUI(AdMethodExtension ad)
    {
        AdUI adUI = Instantiate(this.adUI, adTap).GetComponent<AdUI>();
        adUI.SetAdUI(ad);
    }

    public void DoubleSpeed() => Time.timeScale = 2.0f;

    public void SpeedReset() => Time.timeScale = 1.0f;
}