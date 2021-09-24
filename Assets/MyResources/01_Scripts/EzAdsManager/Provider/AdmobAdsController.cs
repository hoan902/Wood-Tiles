#if ENABLE_ADMOB
using GoogleMobileAds.Api;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdmobAdsController : AdsProvider
{

#if UNITY_ANDROID
    public const string STATIC_INTERSTITIAL_ID = "ca-app-pub-3940256099942544/1033173712";
    public const string REWARD_ID = "ca-app-pub-3940256099942544/5224354917";
    public const string ADS_BANNER = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IOS
    public const string STATIC_INTERSTITIAL_ID = "ca-app-pub-3940256099942544/4411468910";
    public const string REWARD_ID = "ca-app-pub-3940256099942544/1712485313";
    public const string ADS_BANNER = "ca-app-pub-3940256099942544/2934735716";
#else
    public const string STATIC_INTERSTITIAL_ID = "";
    public const string REWARD_ID = "";
     public const string ADS_BANNER = "";
#endif

    public bool IsInterstitialReady => interstitial != null ? interstitial.IsLoaded() : false;

    private bool _isRewardReady;
    public bool IsRewardVideoReady => rewardBasedVideo != null ? rewardBasedVideo.IsLoaded() : false;

    public bool IsBannerAdsReady => throw new NotImplementedException();

    public event Action<bool> OnInterstitialShowComplete;
    public event Action<bool> OnInterstitialLoadComplete;
    public event Action<bool, float> OnRewardShowComplete;
    public event Action<bool> OnRewardLoadComplete;

    public bool _isCloseAds;
    private InterstitialAd interstitial = null;
    private RewardBasedVideoAd rewardBasedVideo = null;
    private BannerView bannerView = null;

    public void Init()
    {
#if UNITY_ANDROID
        string appId = "ca-app-pub-5799373024772285~1456081238";
#elif UNITY_IOS
            string appId = "";
#else
            string appId = "unexpected_platform";
#endif
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize((statusInit) =>
        {
            Debug.Log($"[Admob] Init status {statusInit}");
        });

        InitInterstitial();
        InitReward();

        RequestAdsBanner();

        LoadRewardVideo();
        LoadInterstitial();
    }

#region Interstitial

    private Action<bool> onShowAdComplete = null;
    private Action<bool> onLoadInterstitialComplete = null;

    private void InitInterstitial()
    {
        Debug.Log("[Admob] called InitInterstitial!");
        // Initialize an InterstitialAd.
        interstitial = new InterstitialAd(STATIC_INTERSTITIAL_ID);
        // Called when an ad is shown.
        interstitial.OnAdOpening += HandleOnAdOpened;
        // Called when the ad is closed.
        interstitial.OnAdClosed += HandleOnAdClosed;

        interstitial.OnAdLoaded += Interstitial_OnAdLoaded;

        interstitial.OnAdFailedToLoad += Interstitial_OnAdFailedToLoad;

    }

    private void Interstitial_OnAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
    {
        MonoBehaviour.print($"Interstitial_OnAdFailedToLoad {e.Message}");
        onLoadInterstitialComplete?.Invoke(true);
    }

    private void Interstitial_OnAdLoaded(object sender, EventArgs e)
    {
        MonoBehaviour.print("Interstitial_OnAdLoaded");
        onLoadInterstitialComplete?.Invoke(true);
    }


    public void LoadInterstitial(Action<bool> loadComplete = null)
    {
        Debug.Log("[Admob] called LoadInterstitial!");
        // Load the interstitial with the request.
        interstitial.LoadAd(GetRequest());

        onLoadInterstitialComplete = loadComplete;
    }

    public void ShowInterstitial(Action<bool> onComplete)
    {
        Debug.Log("[Admob] called LoadInterstitial!");
        if (IsInterstitialReady)
        {
            MonoBehaviour.print("[Admob] Interstitial is Ready. Start show");
            interstitial.Show();
            onShowAdComplete = onComplete;
        }
        else
        {
            MonoBehaviour.print("[Admob] Interstitial Not Ready. on fail");
            onShowAdComplete(false);
            OnInterstitialShowComplete?.Invoke(false);
        }

    }


    public void HandleOnAdOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdOpened event received");
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        _isCloseAds = true;
        MonoBehaviour.print("HandleAdClosed event received");
        if (onShowAdComplete != null)
        {
            onShowAdComplete(true);
        }

        OnInterstitialShowComplete?.Invoke(true);
        //reset
        onShowAdComplete = null;
        MonoBehaviour.print("HandleAdClosed started load new ads");
        LoadInterstitial();
    }


#endregion

#region Reward Video

    // Callback when player has watched video
    private System.Action<bool, float> _OnRewardVideoValidated = null;
    private Action<bool> _OnRewardVideoLoadComplete = null;

    private bool _IsRewardValidated = false;

    public void LoadRewardVideo(Action<bool> loadComplete = null)
    {
        Debug.Log("[Admob] called LoadRewardVideo");
        this.rewardBasedVideo.LoadAd(GetRequest(), REWARD_ID);

        _OnRewardVideoLoadComplete = loadComplete;
    }

    public void ShowRewardVideo(System.Action<bool, float> onRewardVideoValidated)
    {
        Debug.Log("[Admob] called ShowAdsInterstitial!");
        if (IsRewardVideoReady)
        {
            Debug.Log("[Admob] reward ads ready ==>");
            _OnRewardVideoValidated = onRewardVideoValidated;
            _IsRewardValidated = false;
            rewardBasedVideo.Show();

        }
        else
        {
            Debug.Log("<>== reward ads not ready ==>");
            onRewardVideoValidated(false, -1f);
            OnRewardShowComplete?.Invoke(false, -1f);
        }

    }

    public void ForceCloseRewardAds()
    {
        _isCloseAds = true;
        MonoBehaviour.print("HandleRewardForceCloseAds event received");
        if (_OnRewardVideoValidated != null)
            _OnRewardVideoValidated(false, -1f);

        LoadRewardVideo();
    }

    public void InitReward()
    {
        // Get singleton reward based video ad reference.
        this.rewardBasedVideo = RewardBasedVideoAd.Instance;
        rewardBasedVideo.OnAdOpening += HandleRewardBasedVideoOpened;
        // Called when the user should be rewarded for watching a video.
        rewardBasedVideo.OnAdRewarded += HandleRewardBasedVideoRewarded;
        // Called when the ad is closed.
        rewardBasedVideo.OnAdClosed += HandleRewardBasedVideoClosed;

        rewardBasedVideo.OnAdCompleted += HandleRewardBasedVideoOnAdsComplete;

        rewardBasedVideo.OnAdStarted += HandleRewardBasedVideoStarted;

        rewardBasedVideo.OnAdLoaded += RewardBasedVideo_OnAdLoaded;

        rewardBasedVideo.OnAdFailedToLoad += RewardBasedVideo_OnAdFailedToLoad;
    }

    private void RewardBasedVideo_OnAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
    {
        MonoBehaviour.print($"RewardBasedVideo_OnAdFailedToLoad event received {e.Message}");
        _OnRewardVideoLoadComplete?.Invoke(false);
    }

    private void RewardBasedVideo_OnAdLoaded(object sender, EventArgs e)
    {
        MonoBehaviour.print("RewardBasedVideo_OnAdLoaded event received");
        _OnRewardVideoLoadComplete?.Invoke(true);
    }

    public void HandleRewardBasedVideoOnAdsComplete(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardBasedVideoOnAdsComplete event received");
    }

    public void HandleRewardBasedVideoStarted(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardBasedVideoStarted event received");
        //Invoke("ForceCloseRewardAds", 35f);

        // LoadingMini.Instance._isTrackingAds = true;

    }

    public void HandleRewardBasedVideoOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardBasedVideoOpened event received");
#if UNITY_ANDROID
        // CancelInvoke("ForceCloseRewardAds");
#endif


    }

    public void HandleRewardBasedVideoClosed(object sender, EventArgs args)
    {
        _isCloseAds = true;

        MonoBehaviour.print("HandleRewardBasedVideoClosed event received");
        if (_OnRewardVideoValidated != null)
        {
            MonoBehaviour.print("HandleRewardBasedVideoClosed callback");

            if (_IsRewardValidated)
            {
                if (_OnRewardVideoValidated != null)
                {
                    _OnRewardVideoValidated(true, 100f);
                    OnRewardShowComplete?.Invoke(true, 100f);
                }
            }
            else
            {
                if (_OnRewardVideoValidated != null)
                {
                    _OnRewardVideoValidated(false, -1f);
                    OnRewardShowComplete?.Invoke(false, -1f);
                }

            }

            _OnRewardVideoValidated = null;
        }
    }

    public void HandleRewardBasedVideoRewarded(object sender, Reward args)
    {
        string type = args.Type;
        double amount = args.Amount;
        MonoBehaviour.print(
            "HandleRewardBasedVideoRewarded event received for "
                        + amount.ToString() + " " + type);


        _IsRewardValidated = true;
    }

#endregion

#region Banner Ads

    private Action<bool> OnLoadedAdsBanner;
    private Action<bool> OnShowedAdsBanner;


    public void RequestAdsBanner(Action<bool> loadComplete = null)
    {

        // Create a 320x50 banner at the top of the screen.
        this.bannerView = new BannerView(ADS_BANNER, AdSize.Banner, AdPosition.Bottom);

        // Called when an ad request has successfully loaded.
        this.bannerView.OnAdLoaded += this.HandleOnAdLoaded;
        // Called when an ad request failed to load.
        this.bannerView.OnAdFailedToLoad += this.HandleOnAdFailedToLoad;
        // Called when an ad is clicked.
        this.bannerView.OnAdOpening += this.HandleOnAdBannerOpened;
        // Called when the user returned from the app after an ad click.
        this.bannerView.OnAdClosed += this.HandleOnAdBannerClosed;
        // Called when the ad click caused the user to leave the application.
        this.bannerView.OnAdLeavingApplication += this.HandleOnAdLeavingApplication;


    }

    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLoaded event received");
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print("HandleFailedToReceiveAd event received with message: "
                            + args.Message);
    }

    public void HandleOnAdBannerOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdOpened event received");
        OnLoadedAdsBanner?.Invoke(true);
    }

    public void HandleOnAdBannerClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdClosed event received");
    }

    public void HandleOnAdLeavingApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLeavingApplication event received");
    }



    public void ShowAdsBanner(Action<bool> showComplete = null)
    {
        // Load the banner with the request.

        // Create a 320x50 banner at the top of the screen.
        this.bannerView = new BannerView(ADS_BANNER, AdSize.Banner, AdPosition.Bottom);
        this.bannerView.LoadAd(GetRequest());

        OnShowedAdsBanner = showComplete;

    }

    public void ClearAdsBanner(Action<bool> clearComplete = null)
    {
        bannerView?.Destroy();
        clearComplete?.Invoke(true);
    }


#endregion

    private AdRequest GetRequest()
    {
        return new AdRequest.Builder().Build();
    }
    
}
#endif