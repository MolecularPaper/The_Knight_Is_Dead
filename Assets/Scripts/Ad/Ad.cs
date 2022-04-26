using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;
using GoogleMobileAds.Api;
using TMPro;

public interface IAdObservable
{
    public void Subscribe(IAdObserver observer);

    public void Unsubscribe(IAdObserver observer);

    public void AdUpdated();
}

public interface IAdObserver
{
    public void AdUpdated(AdExtension adExtension);
}

public enum AdType
{
    Rewarded,
}

[System.Serializable]
public class AdInfo
{
    public string adName;
    public int currentSecond;
    public bool canShowAd = true;
    public bool buttonEnbled;

    public AdInfo() { }

    public AdInfo(AdInfo adInfo) => SetInfo(adInfo);

    public void SetInfo(AdInfo adInfo)
    {
        this.adName = adInfo.adName;
        this.currentSecond = adInfo.currentSecond;
        this.canShowAd = adInfo.canShowAd;
    }
}

[System.Serializable]
public class AdExtension : AdInfo
{
    [Space(10)]
    [SerializeField] protected string testAdUnitID = "ca-app-pub-3940256099942544/5224354917";
    [SerializeField] protected string aosAdUnitID = "ca-app-pub-3940256099942544/5224354917";
    [SerializeField] protected string iosAdUnitID = "ca-app-pub-3940256099942544/1712485313";

    [Space(10)]
    public int compensationSecond;
    public int maxAdNestingCount;
    public bool isDelay;

    [Space(10)]
    public Sprite icon;
    public string adTitle;
    [TextArea(5, 50)]
    public string desciption;

    [Space(10)]
    [SerializeField] protected UnityEvent adStartEvent;
    [SerializeField] protected UnityEvent adEndEvent;
}

[System.Serializable]
public class AdObservable : AdExtension, IAdObservable
{
    public delegate void AdUpdateDel(AdExtension adExtension);
    public AdUpdateDel adUpdateDel;

    public void Subscribe(IAdObserver observer)
    {
        if (observer == null)
            throw new NullReferenceException();

        adUpdateDel += observer.AdUpdated;
    }

    public void Unsubscribe(IAdObserver observer)
    {
        if (observer == null)
            throw new NullReferenceException();

        adUpdateDel -= observer.AdUpdated;
    }

    public void AdUpdated()
    {
        if (adUpdateDel != null) adUpdateDel.Invoke(this);
    }
}

[System.Serializable]
public abstract class AdMethodExtension : AdObservable
{
    public abstract void Reset();

    public abstract void ShowAd();

    protected abstract void CreateAndLoadAd();

    protected void HandleUserEarnedReward(object sender, Reward args)
    {
        AdEnd();
        CreateAndLoadAd();

        buttonEnbled = true;
        AdUpdated();

        GameDataManager.dataManager.SaveGameData();
    }

    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdFailedToShow event received with message: " + args.AdError);
        GameManager.gm.ResumeGame();
    }

    public void AdEnd()
    {
        GameManager.gm.ResumeGame();

        if (currentSecond <= 0) {
            currentSecond = compensationSecond;
            CalculateTime();
            AdUpdated();
        }
        else if(currentSecond < compensationSecond * maxAdNestingCount) {
            currentSecond += compensationSecond;
            AdUpdated();
        }
        
        if(currentSecond > compensationSecond * maxAdNestingCount) {
            canShowAd = false;
            buttonEnbled = false;
            AdUpdated();
        }
    }

    public async void CalculateTime()
    {
        if (0 <= currentSecond) {
            adStartEvent.Invoke();
        }

        buttonEnbled = !isDelay;

        while (0 <= currentSecond) {
            AdUpdated();

            try {
                await GameManager.gm.Delay(1000); 
            }
            catch (TaskCanceledException) { 
                return; 
            }

            currentSecond--;
        }

        adEndEvent.Invoke();

        canShowAd = true;
        buttonEnbled = true;
    }
}

[System.Serializable]
public class RewardAd : AdMethodExtension
{
    protected RewardedAd rewardedAd;

    public override void Reset()
    {
#if UNITY_EDITOR
        string adUnitId = testAdUnitID;
#elif UNITY_ANDROID
        string adUnitId = aosAdUnitID;
#elif UNITY_IPHONE
        string adUnitId = iosAdUnitID;
#else
        string adUnitId = "unexpected_platform";
#endif

        rewardedAd = new RewardedAd(adUnitId);
        rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;

        this.CreateAndLoadAd();
    }

    public override void ShowAd()
    {
        GameManager.gm.PauseGame();
        if (GameManager.gm.AdDeleted) {
            GameManager.gm.ResumeGame();
            AdEnd();
            return;
        }

        if (rewardedAd.IsLoaded()) {
            rewardedAd.Show();
        }
    }

    protected override void CreateAndLoadAd()
    {
        AdRequest request = new AdRequest.Builder().Build();
        rewardedAd.LoadAd(request);
    }
}

[System.Serializable]
public class BannerAd
{
    [SerializeField] private string testAdUnitID = "ca-app-pub-3940256099942544/6300978111";
    [SerializeField] private string aosAdUnitID = "ca-app-pub-3940256099942544/6300978111";
    [SerializeField] private string iosAdUnitID = "ca-app-pub-3940256099942544/2934735716";
    [SerializeField] private int height;

    private BannerView bannerView;

    public void Reset()
    {
        if (GameManager.gm.AdDeleted)
            return;

        this.RequestBanner();
    }

    public void Hide()
    {
        if (bannerView != null) {
            bannerView.Destroy();
        }
    }

    private void RequestBanner()
    {
#if UNITY_EDITOR
        string adUnitId = testAdUnitID;
#elif UNITY_ANDROID
        string adUnitId = aosAdUnitID;
#elif UNITY_IPHONE
        string adUnitId = iosAdUnitID;
#else
        string adUnitId = "unexpected_platform";
#endif
        AdSize adaptiveSize = AdSize.GetPortraitAnchoredAdaptiveBannerAdSizeWithWidth(height);
        this.bannerView = new BannerView(adUnitId, adaptiveSize, AdPosition.Top);

        // Called when an ad request has successfully loaded.
        this.bannerView.OnAdLoaded += this.HandleOnAdLoaded;
        // Called when an ad request failed to load.
        this.bannerView.OnAdFailedToLoad += this.HandleOnAdFailedToLoad;
        // Called when an ad is clicked.
        this.bannerView.OnAdOpening += this.HandleOnAdOpened;
        // Called when the user returned from the app after an ad click.
        this.bannerView.OnAdClosed += this.HandleOnAdClosed;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        this.bannerView.LoadAd(request);
    }

    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLoaded event received");
        MonoBehaviour.print(String.Format("Ad Height: {0}, width: {1}",
            this.bannerView.GetHeightInPixels(),
            this.bannerView.GetWidthInPixels()));
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print("HandleFailedToReceiveAd event received with message: "+ args.LoadAdError);
    }

    public void HandleOnAdOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdOpened event received");
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdClosed event received");
    }
}