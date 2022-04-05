using UnityEngine.Events;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using TMPro;

public enum PlayAd
{
    DoubleSpeed,
    NULL,
}

public class RewardAD : MonoBehaviour
{
    public static RewardAD rewardAd;

    private RewardedAd doubleSpeedRewardedAd;
    private string adUnitId;
    private PlayAd playAd;

    [Header("Settings")]
    [SerializeField] private int doubleSpeedSceond;
    [SerializeField] private int doubleSpeedCoolTimeSceond;

    [Header("Buttons")]
    [SerializeField] private Button doubleSpeedButton;
    [SerializeField] private TextMeshProUGUI doubleSpeedText;

    public void Awake()
    {
        rewardAd = this;
        playAd = PlayAd.NULL;
    }

    public void Start()
    {
    #if UNITY_ANDROID
        adUnitId = "ca-app-pub-3940256099942544/5224354917";
    #elif UNITY_IPHONE
        adUnitId = "ca-app-pub-3940256099942544/1712485313";
    #endif

        doubleSpeedRewardedAd = CreateAndLoadRewardedAd(adUnitId);
    }

    public RewardedAd CreateAndLoadRewardedAd(string adUnitId)
    {
        RewardedAd rewardedAd = new RewardedAd(adUnitId);

        rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
        rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        rewardedAd.OnAdOpening += HandleRewardedAdOpening;
        rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        rewardedAd.OnAdClosed += HandleRewardedAdClosed;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        rewardedAd.LoadAd(request);
        return rewardedAd;
    }

    public void HandleRewardedAdLoaded(object sender, EventArgs args)
    {
        print("HandleRewardedAdLoaded event received");
    }

    public void HandleRewardedAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        print("HandleRewardedAdFailedToLoad event received with message: " + args.LoadAdError);
    }

    public void HandleRewardedAdOpening(object sender, EventArgs args)
    {
        print("HandleRewardedAdOpening event received");
    }

    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    {
        print("HandleRewardedAdFailedToShow event received with message: " + args.AdError);
    }

    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        print("HandleRewardedAdClosed event received");
        switch (playAd) {
            case PlayAd.DoubleSpeed:
                this.doubleSpeedRewardedAd = CreateAndLoadRewardedAd(adUnitId);
                break;
        }

        playAd = PlayAd.NULL;
    }

    public void HandleUserEarnedReward(object sender, Reward args)
    {
        string type = args.Type;
        double amount = args.Amount;
        print("HandleRewardedAdRewarded event received for " + amount.ToString() + " " + type);

        DoubleSpeed();
    }

    public void WatchDoubleSpeedRewardedAd()
    {
        if (playAd == PlayAd.NULL && this.doubleSpeedRewardedAd.IsLoaded()) {
            this.doubleSpeedRewardedAd.Show();
            playAd = PlayAd.DoubleSpeed;
        }
    }

    public async void DoubleSpeed()
    {
        doubleSpeedButton.interactable = false;
        Time.timeScale = 2.0f;

        int time = doubleSpeedSceond;
        do {
            if (time <= 0) {
                Time.timeScale = 1.0f;
                time += doubleSpeedCoolTimeSceond;
            }

            doubleSpeedText.text = $"{(time % 3600) / 60:D2}:{(time % 3600) % 60:D2}";
            try { await Task.Delay(1000, GameManager.gm.timerTokenSource.Token); }
            catch { return; }

            time--;
            print(time);
        } while (time >= 0);

        doubleSpeedText.text = "±§∞Ì Ω√√ª";
    }
}