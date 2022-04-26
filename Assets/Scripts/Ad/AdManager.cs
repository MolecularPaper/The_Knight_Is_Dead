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

    public BannerAd bannerAd;
    public List<RewardAd> rewardedAds;

    public void Awake()
    {
        adManager = this;
    }

    public void Start()
    {
        foreach (var item in rewardedAds) {
            CreateAdUI(item);
            item.Reset();
        }

        bannerAd.Reset();
    }

    public void SetAdInfos(GameData gameData)
    {
        if (gameData == null) return;

        for (int i = 0; i < gameData.rewardedAds.Count; i++) {
            rewardedAds[i].SetInfo(gameData.rewardedAds[i]);
        }
    }

    public void CreateAdUI(AdMethodExtension ad)
    {
        AdUI adUI = Instantiate(this.adUI, adTap).GetComponent<AdUI>();
        adUI.SetAdUI(ad);
    }

    public void DoubleSpeed() => Time.timeScale = 2.0f;
    public void SpeedReset() => Time.timeScale = 1.0f;

    public void GetCrystal()
    {
        PlayerCTRL playerCTRL = FindObjectOfType<PlayerCTRL>();
        Item crystal = (Item)playerCTRL["Crystal"];
        crystal.Count += 200;
    }
}