using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using UnityEngine.UI;
using UnityEngine;
using GoogleMobileAds.Api;
using TMPro;

[System.Serializable]
public class AdInfo
{
    public string adName;
    public string adUnitId;
    public int compensationSecond;
    public int watingSecond;
    public int currentSecond;
    public bool canShowAd = true;

    public void SetInfo(AdInfo adInfo)
    {
        this.adName = adInfo.adName;
        this.adUnitId = adInfo.adUnitId;
        this.compensationSecond = adInfo.compensationSecond;
        this.watingSecond = adInfo.watingSecond;
        this.currentSecond = adInfo.currentSecond;
        this.canShowAd = adInfo.canShowAd;
    }
}

[System.Serializable]
public class AdUI : AdInfo
{
    [Space(10)]
    public Button button;
    public TextMeshProUGUI buttonText;
}

[System.Serializable]
public abstract class AdExtension : AdUI
{
    public AdExtension(AdUI adUI)
    {
        this.adName = adUI.adName;
        this.adUnitId = adUI.adUnitId;
        this.compensationSecond = adUI.compensationSecond;
        this.watingSecond = adUI.watingSecond;
        this.currentSecond = adUI.currentSecond;
        this.canShowAd = adUI.canShowAd;
        this.button = adUI.button;
        this.buttonText = adUI.buttonText;
        button.onClick.AddListener(() => {
            if (canShowAd) {
                ShowAd();
            }
        });
        Reset();

        if (!canShowAd) {
            CalculateTime();
        }
    }

    public abstract void Reset();

    public abstract void ShowAd();

    protected abstract void CreateAndLoadRewardedAd();

    protected void HandleRewardedAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdLoaded event received");
    }

    protected void HandleRewardedAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdFailedToLoad event received with message: "+ args.LoadAdError);
    }

    protected void HandleRewardedAdOpening(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdOpening event received");
    }

    protected void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdFailedToShow event received with message: " + args.AdError);
    }

    protected void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        this.CreateAndLoadRewardedAd();
    }

    protected void HandleUserEarnedReward(object sender, Reward args)
    {
        currentSecond = compensationSecond + watingSecond;
        CalculateTime();
    }

    public async void CalculateTime()
    {
        canShowAd = false;
        button.interactable = false;

        if(0 <= currentSecond - watingSecond) {
            Reward();
        }

        while (currentSecond > 0) {
            int displayTime = currentSecond;
            if (0 <= displayTime - watingSecond) {
                displayTime = currentSecond - watingSecond;
            }
            buttonText.text = $"{string.Format("{0:D2}", (displayTime % 3600) / 60)}:{string.Format("{0:D2}", (displayTime % 3600) % 60)}";
            await Task.Delay(1000);
            currentSecond--;
        }

        RewardEnd();
        canShowAd = true;
        button.interactable = true;
    }

    protected abstract void Reward();
    protected abstract void RewardEnd();
}

[System.Serializable]
public class AdCollection
{
    public readonly List<AdExtension> ads = new List<AdExtension>();
    public AdExtension this[string adName] {
        get {
            foreach (var item in ads) {
                if (item.adName == adName) 
                    return item;
            }
            throw new ArgumentNullException();
        }
    }

    public AdCollection(List<AdUI> adUIs)
    {
        foreach (var item in adUIs) {
            switch (item.adName) {
                case "DoubleSpeed":
                    ads.Add(new DoubleSpeedAd(item));
                    break;
                default:
                    throw new ArgumentNullException();
            }
        }
    }
}

public abstract class RewardAd : AdExtension
{
    protected RewardedAd rewardedAd;

    public RewardAd(AdUI adUI) : base(adUI) { }

    public override void Reset()
    {
        rewardedAd = new RewardedAd(adUnitId);

        // Called when an ad request has successfully loaded.
        this.rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
        // Called when an ad request failed to load.
        this.rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        // Called when an ad is shown.
        this.rewardedAd.OnAdOpening += HandleRewardedAdOpening;
        // Called when an ad request failed to show.
        this.rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        // Called when the user should be rewarded for interacting with the ad.
        this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        // Called when the ad is closed.
        this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;

        this.CreateAndLoadRewardedAd();
    }

    public override void ShowAd()
    {
        if (this.rewardedAd.IsLoaded()) {
            this.rewardedAd.Show();
        }
    }

    protected override void CreateAndLoadRewardedAd()
    {
        AdRequest request = new AdRequest.Builder().Build();
        this.rewardedAd.LoadAd(request);
    }
}

[System.Serializable]
public class DoubleSpeedAd : RewardAd
{
    public DoubleSpeedAd(AdUI adUI) : base(adUI) { }

    protected override void Reward() => Time.timeScale = 2.0f;

    protected override void RewardEnd() => Time.timeScale = 1.0f;
}