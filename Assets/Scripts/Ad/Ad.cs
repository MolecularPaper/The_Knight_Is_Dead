using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using UnityEngine.UI;
using UnityEngine;
using GoogleMobileAds.Api;
using TMPro;

[System.Serializable]
public class AdInfo
{
    public string adName;
    public int currentSecond;
    public bool canShowAd = true;

    public AdInfo() { }

    public AdInfo(AdInfo adInfo) => SetInfo(adInfo);

    public void SetInfo(AdInfo adInfo)
    {
        this.adName = adInfo.adName;
        this.currentSecond = adInfo.currentSecond;
        this.canShowAd = adInfo.canShowAd;
    }
}

public class AdExtension : AdInfo
{
    public string adUnitId;

    [Space(10)]
    public int compensationSecond;
    public int maxAdNestingCount;
}

[System.Serializable]
public class AdUI : AdExtension
{
    [Space(10)]
    public Button button;
    public TextMeshProUGUI buttonText;
}

[System.Serializable]
public abstract class AdMethodExtension : AdUI
{
    public AdMethodExtension(AdUI adUI)
    {
        this.adName = adUI.adName;
        this.currentSecond = adUI.currentSecond;
        this.adUnitId = adUI.adUnitId;
        this.canShowAd = adUI.canShowAd;

        this.compensationSecond = adUI.compensationSecond;
        this.maxAdNestingCount = adUI.maxAdNestingCount;

        this.button = adUI.button;
        this.buttonText = adUI.buttonText;

        button.interactable = canShowAd;
        button.onClick.AddListener(() => {
            if (GameManager.gm.adDeleted) {
                AdEnd();
            }
            else if (canShowAd) {
                GameManager.gm.PauseGame();
                ShowAd();
            }
        });
        Reset();

        CalculateTime();
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

    protected async void HandleUserEarnedReward(object sender, Reward args)
    {
        GameManager.gm.ResumeGame();
        AdEnd();

        button.interactable = false;
        await GameManager.gm.Delay(3000);
        button.interactable = true;
    }

    public void AdEnd()
    {
        if(currentSecond <= 0) {
            currentSecond = compensationSecond;
            CalculateTime();
        }
        else if(currentSecond < compensationSecond * maxAdNestingCount) {
            currentSecond += compensationSecond;
            UpdateTime();
        }
        
        if(currentSecond > compensationSecond * maxAdNestingCount) {
            canShowAd = false;
            button.interactable = false;
        }
    }

    public async void CalculateTime()
    {
        if(0 <= currentSecond) {
            Reward();
        }

        while (currentSecond >= 0) {
            UpdateTime();

            try {
                await GameManager.gm.Delay(1000); 
            }
            catch (TaskCanceledException) { 
                return; 
            }

            currentSecond--;
        }

        RewardEnd();

        canShowAd = true;
        buttonText.text = "±¤°í ½ÃÃ»";
        button.interactable = true;
    }

    private void UpdateTime() => buttonText.text = $"{string.Format("{0:D2}", (currentSecond % 3600) / 60)}:{string.Format("{0:D2}", (currentSecond % 3600) % 60)}";
    protected abstract void Reward();
    protected abstract void RewardEnd();
}

[System.Serializable]
public class AdCollection
{
    public List<AdUI> ads = new List<AdUI>();
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

public abstract class RewardAd : AdMethodExtension
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

    protected override void RewardEnd()
    {
        Debug.Log("RewardEnd");
        Time.timeScale = 1.0f;
    }
}