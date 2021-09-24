using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronSourceController : AdsProvider
{
    //app key
    public string appKey = "";

    public bool IsInterstitialReady => IronSource.Agent.isInterstitialReady();
    public bool IsRewardVideoReady => IronSource.Agent.isRewardedVideoAvailable();
    public bool IsBannerAdsReady => _isBannerAdsReady;

    //privte
    private bool _isInterstitialReady;
    private bool _isRewardAdsReady;
    private bool _isBannerAdsReady;

    public event Action<bool> OnInterstitialLoadComplete;
    public event Action<bool> OnInterstitialShowComplete;

    public event Action<bool> OnRewardLoadComplete;
    public event Action<bool, float> OnRewardShowComplete;

    public void OnApplicationPause(bool isPaused)
    {
        Debug.Log("unity-script: OnApplicationPause = " + isPaused);
        IronSource.Agent.onApplicationPause(isPaused);
    }

    public void Init()
    {
        _isInterstitialReady = false;
        _isRewardAdsReady = false;
        _isBannerAdsReady = false;

        AddEventListeners();
        InitIronSource();

        RequestAdsBanner((success) =>
        {
            Debug.Log($"[Ironsource] Request Ads Banner {success}");
        });
    }

    private void InitIronSource()
    {
#if UNITY_ANDROID
        string appKey = "f67adbf9";
#elif UNITY_IPHONE
        string appKey = "";
#else
        string appKey = "unexpected_platform";
#endif
        Debug.Log("unity-script: IronSource.Agent.validateIntegration");
        IronSource.Agent.validateIntegration();
        Debug.Log("unity-script: unity version" + IronSource.unityVersion());
        // SDK init
        Debug.Log("unity-script: IronSource.Agent.init");
        IronSource.Agent.init(appKey);



        IronSource.Agent.shouldTrackNetworkState(false);
        IronSource.Agent.validateIntegration();
        PreLoadAds();
    }

    public void PreLoadAds()
    {
        IronSource.Agent.loadInterstitial();
    }

    private void AddEventListeners()
    {
        IronSourceEvents.onRewardedVideoAdOpenedEvent += RewardedVideoAdOpenedEvent;
        IronSourceEvents.onRewardedVideoAdClosedEvent += RewardedVideoAdClosedEvent;
        IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += RewardedVideoAvailabilityChangedEvent;
        IronSourceEvents.onRewardedVideoAdStartedEvent += RewardedVideoAdStartedEvent;
        IronSourceEvents.onRewardedVideoAdEndedEvent += RewardedVideoAdEndedEvent;
        IronSourceEvents.onRewardedVideoAdRewardedEvent += RewardedVideoAdRewardedEvent;
        IronSourceEvents.onRewardedVideoAdShowFailedEvent += RewardedVideoAdShowFailedEvent;
        IronSourceEvents.onRewardedVideoAdClickedEvent += RewardedVideoAdClickedEvent;


        // Add Interstitial Events
        IronSourceEvents.onInterstitialAdReadyEvent += InterstitialAdReadyEvent;
        IronSourceEvents.onInterstitialAdLoadFailedEvent += InterstitialAdLoadFailedEvent;
        IronSourceEvents.onInterstitialAdShowSucceededEvent += InterstitialAdShowSucceededEvent;
        IronSourceEvents.onInterstitialAdShowFailedEvent += InterstitialAdShowFailedEvent;
        IronSourceEvents.onInterstitialAdClickedEvent += InterstitialAdClickedEvent;
        IronSourceEvents.onInterstitialAdOpenedEvent += InterstitialAdOpenedEvent;
        IronSourceEvents.onInterstitialAdClosedEvent += InterstitialAdClosedEvent;


        // Add Banner Events
        IronSourceEvents.onBannerAdLoadedEvent += BannerAdLoadedEvent;
        IronSourceEvents.onBannerAdLoadFailedEvent += BannerAdLoadFailedEvent;
        IronSourceEvents.onBannerAdClickedEvent += BannerAdClickedEvent;
        IronSourceEvents.onBannerAdScreenPresentedEvent += BannerAdScreenPresentedEvent;
        IronSourceEvents.onBannerAdScreenDismissedEvent += BannerAdScreenDismissedEvent;
        IronSourceEvents.onBannerAdLeftApplicationEvent += BannerAdLeftApplicationEvent;

    }

    private void RemoveEventListners()
    {//Add Rewarded Video Events
        IronSourceEvents.onRewardedVideoAdOpenedEvent -= RewardedVideoAdOpenedEvent;
        IronSourceEvents.onRewardedVideoAdClosedEvent -= RewardedVideoAdClosedEvent;
        IronSourceEvents.onRewardedVideoAvailabilityChangedEvent -= RewardedVideoAvailabilityChangedEvent;
        IronSourceEvents.onRewardedVideoAdStartedEvent -= RewardedVideoAdStartedEvent;
        IronSourceEvents.onRewardedVideoAdEndedEvent -= RewardedVideoAdEndedEvent;
        IronSourceEvents.onRewardedVideoAdRewardedEvent -= RewardedVideoAdRewardedEvent;
        IronSourceEvents.onRewardedVideoAdShowFailedEvent -= RewardedVideoAdShowFailedEvent;
        IronSourceEvents.onRewardedVideoAdClickedEvent -= RewardedVideoAdClickedEvent;


        // Add Interstitial Events
        IronSourceEvents.onInterstitialAdReadyEvent -= InterstitialAdReadyEvent;
        IronSourceEvents.onInterstitialAdLoadFailedEvent -= InterstitialAdLoadFailedEvent;
        IronSourceEvents.onInterstitialAdShowSucceededEvent -= InterstitialAdShowSucceededEvent;
        IronSourceEvents.onInterstitialAdShowFailedEvent -= InterstitialAdShowFailedEvent;
        IronSourceEvents.onInterstitialAdClickedEvent -= InterstitialAdClickedEvent;
        IronSourceEvents.onInterstitialAdOpenedEvent -= InterstitialAdOpenedEvent;
        IronSourceEvents.onInterstitialAdClosedEvent -= InterstitialAdClosedEvent;


        // Add Banner Events
        IronSourceEvents.onBannerAdLoadedEvent -= BannerAdLoadedEvent;
        IronSourceEvents.onBannerAdLoadFailedEvent -= BannerAdLoadFailedEvent;
        IronSourceEvents.onBannerAdClickedEvent -= BannerAdClickedEvent;
        IronSourceEvents.onBannerAdScreenPresentedEvent -= BannerAdScreenPresentedEvent;
        IronSourceEvents.onBannerAdScreenDismissedEvent -= BannerAdScreenDismissedEvent;
        IronSourceEvents.onBannerAdLeftApplicationEvent -= BannerAdLeftApplicationEvent;

    }

    #region Interstitial
    private Action<bool> onLoadInterstitialComplete = null;
    private Action<bool> onShowInterstitialComplete = null;

    public void LoadInterstitial(Action<bool> loadComplete = null)
    {
        IronSource.Agent.loadInterstitial();
        onLoadInterstitialComplete = loadComplete;
    }

    public void ShowInterstitial(Action<bool> showComplete = null)
    {
        onShowInterstitialComplete = showComplete;
        if (IronSource.Agent.isInterstitialReady())
        {
            IronSource.Agent.showInterstitial();
        }
        else
        {
            onShowInterstitialComplete?.Invoke(false);
        }

    }

    void InterstitialAdReadyEvent()
    {
        Debug.Log("unity-script: I got InterstitialAdReadyEvent");
        onLoadInterstitialComplete?.Invoke(true);
        OnInterstitialLoadComplete?.Invoke(true);
    }

    void InterstitialAdLoadFailedEvent(IronSourceError error)
    {
        Debug.Log("unity-script: I got InterstitialAdLoadFailedEvent, code: " + error.getCode() + ", description : " + error.getDescription());
        onLoadInterstitialComplete?.Invoke(false);
        OnInterstitialLoadComplete?.Invoke(false);
    }

    void InterstitialAdShowSucceededEvent()
    {
        Debug.Log("unity-script: I got InterstitialAdShowSucceededEvent");
        onShowInterstitialComplete?.Invoke(true);
        OnInterstitialShowComplete?.Invoke(true);
    }

    void InterstitialAdShowFailedEvent(IronSourceError error)
    {
        Debug.Log("unity-script: I got InterstitialAdShowFailedEvent, code :  " + error.getCode() + ", description : " + error.getDescription());
        onShowInterstitialComplete?.Invoke(false);
        OnInterstitialShowComplete?.Invoke(false);
    }

    void InterstitialAdClickedEvent()
    {
        Debug.Log("unity-script: I got InterstitialAdClickedEvent");
    }

    void InterstitialAdOpenedEvent()
    {
        Debug.Log("unity-script: I got InterstitialAdOpenedEvent");
        onShowInterstitialComplete?.Invoke(true);
        OnInterstitialShowComplete?.Invoke(true);
    }

    void InterstitialAdClosedEvent()
    {
        Debug.Log("unity-script: I got InterstitialAdClosedEvent");
    }

    #endregion

    #region Reward Ads
    private Action<bool> onRewardVideoLoadComplete = null;
    private System.Action<bool, float> onRewardVideoValidated = null;
    private bool isRewardAdsValidated = false;

    public void LoadRewardVideo(Action<bool> loadComplete = null)
    {
        onRewardVideoLoadComplete = loadComplete;
    }

    public void ShowRewardVideo(Action<bool, float> showComplete = null)
    {
        onRewardVideoValidated = showComplete;
        isRewardAdsValidated = false;
        if (IronSource.Agent.isRewardedVideoAvailable())
        {
            IronSource.Agent.showRewardedVideo();
        }
    }

    void RewardedVideoAvailabilityChangedEvent(bool canShowAd)
    {
        Debug.Log("unity-script: I got RewardedVideoAvailabilityChangedEvent, value = " + canShowAd);
        onRewardVideoLoadComplete?.Invoke(canShowAd);
        OnRewardLoadComplete?.Invoke(canShowAd);

    }

    void RewardedVideoAdOpenedEvent()
    {
        Debug.Log("unity-script: I got RewardedVideoAdOpenedEvent");
    }

    void RewardedVideoAdRewardedEvent(IronSourcePlacement ssp)
    {
        Debug.Log("unity-script: I got RewardedVideoAdRewardedEvent, amount = " + ssp.getRewardAmount() + " name = " + ssp.getRewardName());
        isRewardAdsValidated = true;
    }

    void RewardedVideoAdClosedEvent()
    {
        Debug.Log("unity-script: I got RewardedVideoAdClosedEvent");
        onRewardVideoValidated?.Invoke(isRewardAdsValidated, 100f);
        OnRewardShowComplete?.Invoke(isRewardAdsValidated, 100f);
    }

    void RewardedVideoAdStartedEvent()
    {
        Debug.Log("unity-script: I got RewardedVideoAdStartedEvent");
    }

    void RewardedVideoAdEndedEvent()
    {
        Debug.Log("unity-script: I got RewardedVideoAdEndedEvent");
    }

    void RewardedVideoAdShowFailedEvent(IronSourceError error)
    {
        Debug.Log("unity-script: I got RewardedVideoAdShowFailedEvent, code :  " + error.getCode() + ", description : " + error.getDescription());
    }

    void RewardedVideoAdClickedEvent(IronSourcePlacement ssp)
    {
        Debug.Log("unity-script: I got RewardedVideoAdClickedEvent, name = " + ssp.getRewardName());
    }
    #endregion

    #region Banner Ads
    private Action<bool> onLoadBannerComplete = null;
    private Action<bool> onShowBannerComplete = null;

    public void RequestAdsBanner(Action<bool> loadComplete = null)
    {
        IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, IronSourceBannerPosition.BOTTOM);
        onLoadBannerComplete = loadComplete;
    }

    public void ShowAdsBanner(Action<bool> showComplete = null)
    {
        Debug.Log("[IronSource] Called ShowAdsBanner");
        IronSource.Agent.displayBanner();
        onShowBannerComplete = showComplete;
    }

    public void DestroyAdsBanner(Action<bool> clearComplete = null)
    {
        Debug.Log("[IronSource] Called DestroyAdsBanner");
        IronSource.Agent.destroyBanner();
    }

    public void HideAdsBanner(Action<bool> hideComplete = null)
    {
        Debug.Log("[IronSource] Called HideAdsBanner");
        IronSource.Agent.hideBanner();
    }

    void BannerAdLoadedEvent()
    {
        Debug.Log("unity-script: I got BannerAdLoadedEvent");
        onLoadBannerComplete?.Invoke(true);
    }

    void BannerAdLoadFailedEvent(IronSourceError error)
    {
        Debug.Log("unity-script: I got BannerAdLoadFailedEvent, code: " + error.getCode() + ", description : " + error.getDescription());
        onLoadBannerComplete?.Invoke(false);
    }

    void BannerAdClickedEvent()
    {
        Debug.Log("unity-script: I got BannerAdClickedEvent");
    }

    void BannerAdScreenPresentedEvent()
    {
        Debug.Log("unity-script: I got BannerAdScreenPresentedEvent");
        onShowBannerComplete?.Invoke(true);
    }

    void BannerAdScreenDismissedEvent()
    {
        Debug.Log("unity-script: I got BannerAdScreenDismissedEvent");
    }

    void BannerAdLeftApplicationEvent()
    {
        Debug.Log("unity-script: I got BannerAdLeftApplicationEvent");
    }

    #endregion

}
