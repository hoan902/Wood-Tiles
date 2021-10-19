using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdmobAdsController : AdsProvider
{
#if UNITY_ANDROID
    public const string STATIC_INTERSTITIAL_ID = "ca-app-pub-6626742359118351/5016362348";
    public const string REWARD_ID = "ca-app-pub-6626742359118351/2413060368";
    public const string ADS_BANNER = "ca-app-pub-6626742359118351/8955607355";
#else
    public const string STATIC_INTERSTITIAL_ID = "";
    public const string REWARD_ID = "";
    public const string ADS_BANNER = "";
#endif

    public bool IsInterstitialReady => interstitial != null ? interstitial.IsLoaded() : false;
    public bool IsRewardVideoReady => rewardBasedVideo != null ? rewardBasedVideo.IsLoaded() : false;
    public bool IsBannerAdsReady => throw new NotImplementedException();

    public event Action<bool> OnInterstitialShowComplete;
    public event Action<bool> OnInterstitialLoadComplete;
    public event Action<bool, float> OnRewardShowComplete;
    public event Action<bool> OnRewardLoadComplete;

    public bool _isCloseAds;
    private InterstitialAd interstitial = null;
    private RewardedAd rewardBasedVideo = null;
    private BannerView bannerView = null;

    public void Init()
    {
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
        //Debug.Log($"Interstitial_OnAdFailedToLoad {e.Message}");
        onLoadInterstitialComplete?.Invoke(true);
    }

    private void Interstitial_OnAdLoaded(object sender, EventArgs e)
    {
        Debug.Log("Interstitial_OnAdLoaded");
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
            Debug.Log("[Admob] Interstitial is Ready. Start show");
            interstitial.Show();
            onShowAdComplete = onComplete;
        }
        else
        {
            Debug.Log("[Admob] Interstitial Not Ready. on fail");
            onShowAdComplete(false);
            OnInterstitialShowComplete?.Invoke(false);
        }

    }


    public void HandleOnAdOpened(object sender, EventArgs args)
    {
        Debug.Log("HandleAdOpened event received");
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        _isCloseAds = true;
        Debug.Log("HandleAdClosed event received");
        if (onShowAdComplete != null)
        {
            onShowAdComplete(true);
        }

        OnInterstitialShowComplete?.Invoke(true);
        //reset
        onShowAdComplete = null;
        Debug.Log("HandleAdClosed started load new ads");
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
        this.rewardBasedVideo.LoadAd(GetRequest());

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
        Debug.Log("HandleRewardForceCloseAds event received");
        if (_OnRewardVideoValidated != null)
            _OnRewardVideoValidated(false, -1f);

        LoadRewardVideo();
    }

    public void InitReward()
    {
        this.rewardBasedVideo = new RewardedAd(REWARD_ID);

        // Called when an ad request has successfully loaded.          
        this.rewardBasedVideo.OnAdLoaded += RewardBasedVideo_OnAdLoaded;
        // Called when an ad request failed to load.                    
        this.rewardBasedVideo.OnAdFailedToLoad += RewardBasedVideo_OnAdFailedToLoad;
        // Called when an ad is shown.
        this.rewardBasedVideo.OnAdOpening += HandleRewardBasedVideoStarted;
        // Called when the user should be rewarded for interacting with the ad.
        this.rewardBasedVideo.OnUserEarnedReward += HandleRewardBasedVideoRewarded;
        // Called when the ad is closed.
        this.rewardBasedVideo.OnAdClosed += HandleRewardBasedVideoClosed;
    }

    private void RewardBasedVideo_OnAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
    {
        //Debug.Log($"RewardBasedVideo_OnAdFailedToLoad event received {e.Message}");
        _OnRewardVideoLoadComplete?.Invoke(false);
    }

    private void RewardBasedVideo_OnAdLoaded(object sender, EventArgs e)
    {
        Debug.Log("RewardBasedVideo_OnAdLoaded event received");
        _OnRewardVideoLoadComplete?.Invoke(true);
    }

    public void HandleRewardBasedVideoOnAdsComplete(object sender, EventArgs args)
    {
        Debug.Log("HandleRewardBasedVideoOnAdsComplete event received");
    }

    public void HandleRewardBasedVideoStarted(object sender, EventArgs args)
    {
        Debug.Log("HandleRewardBasedVideoStarted event received");
        //Invoke("ForceCloseRewardAds", 35f);

        // LoadingMini.Instance._isTrackingAds = true;

    }

    public void HandleRewardBasedVideoOpened(object sender, EventArgs args)
    {
        Debug.Log("HandleRewardBasedVideoOpened event received");
#if UNITY_ANDROID
        // CancelInvoke("ForceCloseRewardAds");
#endif


    }

    public void HandleRewardBasedVideoClosed(object sender, EventArgs args)
    {
        _isCloseAds = true;
        Debug.Log("HandleRewardBasedVideoClosed event received");
    }

    public void HandleRewardBasedVideoRewarded(object sender, Reward args)
    {
        string type = args.Type;
        double amount = args.Amount;
        Debug.Log(
            "HandleRewardBasedVideoRewarded event received for "
                        + amount.ToString() + " " + type);


        //_IsRewardValidated = true;
        if (_OnRewardVideoValidated != null)
        {
            Debug.Log("HandleRewardBasedVideoClosed callback");

            if (true)
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


    }

    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        Debug.Log("HandleAdLoaded event received");
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        //Debug.Log("HandleFailedToReceiveAd event received with message: + args.Message);
    }

    public void HandleOnAdBannerOpened(object sender, EventArgs args)
    {
        Debug.Log("HandleAdOpened event received");
        OnLoadedAdsBanner?.Invoke(true);
    }

    public void HandleOnAdBannerClosed(object sender, EventArgs args)
    {
        Debug.Log("HandleAdClosed event received");
    }

    public void ShowAdsBanner(Action<bool> showComplete = null)
    {
        // Load the banner with the request.
        Debug.Log("[Abmob] Show banner");
        // Create a 320x50 banner at the top of the screen.
        this.bannerView = new BannerView(ADS_BANNER, AdSize.Banner, AdPosition.Bottom);
        this.bannerView.LoadAd(GetRequest());
        OnShowedAdsBanner = showComplete;
    }

    public void DestroyAdsBanner(Action<bool> clearComplete = null)
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