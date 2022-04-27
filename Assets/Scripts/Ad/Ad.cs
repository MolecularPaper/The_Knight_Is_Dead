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
    [SerializeField] protected string aosAdUnitID = "ca-app-pub-3940256099942544/5224354917";
    [SerializeField] protected string iosAdUnitID = "ca-app-pub-3940256099942544/1712485313";
    
    protected string testAosAdUnitID = "ca-app-pub-3940256099942544/5224354917";
    protected string testIosAdUnitID = "ca-app-pub-3940256099942544/5224354917";

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
    public abstract void ShowAd();

    public abstract void CreateAndLoadAd();

    protected void HandleUserEarnedReward(object sender, Reward args)
    {
        GameManager.gm.mainThreadEvent.Enqueue(GameManager.gm.ResumeGame);

        CreateAndLoadAd();
        AdEnd();

        buttonEnbled = true;
        AdUpdated();

        GameDataManager.dataManager.SaveGameData();
    }

    public void AdEnd()
    {
        if (currentSecond <= 0) {
            currentSecond = compensationSecond;
            CalculateTime();
            GameManager.gm.mainThreadEvent.Enqueue(AdUpdated);
        }
        else if(currentSecond < compensationSecond * maxAdNestingCount) {
            currentSecond += compensationSecond;
            GameManager.gm.mainThreadEvent.Enqueue(AdUpdated);
        }
        
        if(currentSecond > compensationSecond * maxAdNestingCount) {
            canShowAd = false;
            buttonEnbled = false;
            GameManager.gm.mainThreadEvent.Enqueue(AdUpdated);
        }
    }

    public async void CalculateTime()
    {
        if (0 <= currentSecond) {
            adStartEvent.Invoke();
        }

        buttonEnbled = !isDelay;

        while (0 <= currentSecond) {
            GameManager.gm.mainThreadEvent.Enqueue(AdUpdated);

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

    public override void ShowAd()
    {
        GameManager.gm.PauseGame();
        if (GameManager.gm.AdDeleted) {
            GameManager.gm.ResumeGame();
            AdEnd();
        }
        else if (rewardedAd.IsLoaded()) {
            rewardedAd.Show();
        }
        else {
            GameManager.gm.ResumeGame();
            return;
        }
    }

    public override void CreateAndLoadAd()
    {
        string adUnitId = "";

        if (AdManager.adManager.isTest) {
#if UNITY_EDITOR
            adUnitId = testAosAdUnitID;
#elif UNITY_ANDROID
            adUnitId = testAosAdUnitID;
#elif UNITY_IPHONE
            adUnitId = "unexpected_platform";
#else
            return
#endif
        }
        else {
#if UNITY_EDITOR
            Debug.LogError("테스트가 활성화 되있지 않습니다. 확인해주세요.");
            adUnitId = testAosAdUnitID;
#elif UNITY_ANDROID
            adUnitId = aosAdUnitID;
#elif UNITY_IPHONE
            adUnitId = iosAdUnitID;
#else
            adUnitId = "unexpected_platform";
#endif
        }

        RewardedAd rewardedAd = new RewardedAd(adUnitId);

        rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        rewardedAd.LoadAd(request);
        this.rewardedAd = rewardedAd;
    }
}

[System.Serializable]
public class BannerAd
{
    [SerializeField] private string aosAdUnitID = "ca-app-pub-3940256099942544/6300978111";
    [SerializeField] private string iosAdUnitID = "ca-app-pub-3940256099942544/2934735716";

    private string testAosAdUnitID = "ca-app-pub-3940256099942544/6300978111";
    private string testIosAdUnitID = "ca-app-pub-3940256099942544/2934735716";

    [Space(10)]
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
        string adUnitId = "";

        if (AdManager.adManager.isTest) {
#if UNITY_EDITOR
            adUnitId = testAosAdUnitID;
#elif UNITY_ANDROID
            adUnitId = testAosAdUnitID;
#elif UNITY_IPHONE
            adUnitId = "unexpected_platform";
#else
            return
#endif
        }
        else {
#if UNITY_EDITOR
            Debug.LogError("테스트가 활성화 되있지 않습니다. 확인해주세요.");
            adUnitId = testAosAdUnitID;
#elif UNITY_ANDROID
            adUnitId = aosAdUnitID;
#elif UNITY_IPHONE
            adUnitId = iosAdUnitID;
#else
            adUnitId = "unexpected_platform";
#endif
        }

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