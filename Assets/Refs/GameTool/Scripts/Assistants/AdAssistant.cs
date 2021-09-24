//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System.Linq;
//using GoogleMobileAds.Api;
//using Berry.Utils;

//public class AdAssistant : SingletonMonoBehaviour<AdAssistant>
//{
//    Action callBackFullScreen, callBackVideoReward;
//    private BannerView bannerView;
//    private InterstitialAd interstitial;
//    private RewardBasedVideoAd rewardBasedVideo;



//    AdRequest GetAdsRequest()
//    {
//        AdRequest request = new AdRequest.Builder().Build();
//        return request;
//    }


//    #region Interstiatial

//    private void HandleInterstitialClosed(object sender, EventArgs e)
//    {
//        if (callBackFullScreen != null)
//        {
//            callBackFullScreen.Invoke();
//            callBackFullScreen = null;
//        }
//    }

//    private void HandleInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs e)
//    {
//        if (callBackFullScreen != null)
//        {
//            callBackFullScreen.Invoke();
//            callBackFullScreen = null;
//        }
//    }

//    #endregion


//    private void Start()
//    {


//#if UNITY_ANDROID
//        string appId =GameRemoteSettings.Instance.AdMobAndroidAppID;
        
//#elif UNITY_IPHONE
//        string appId =GameRemoteSettings.Instance.AdMobIOSAppID;;
//#else
//        string appId = "unexpected_platform";
//#endif

//        // Initialize the Google Mobile Ads SDK.
//        MobileAds.Initialize(appId);

//        RequestBanner();

//        ShowBanner();


//        // Get singleton reward based video ad reference.
//        this.rewardBasedVideo = RewardBasedVideoAd.Instance;

//        // RewardBasedVideoAd is a singleton, so handlers should only be registered once.
//        this.rewardBasedVideo.OnAdLoaded += this.HandleRewardBasedVideoLoaded;
//        this.rewardBasedVideo.OnAdFailedToLoad += this.HandleRewardBasedVideoFailedToLoad;
//        this.rewardBasedVideo.OnAdOpening += this.HandleRewardBasedVideoOpened;
//        this.rewardBasedVideo.OnAdStarted += this.HandleRewardBasedVideoStarted;
//        this.rewardBasedVideo.OnAdRewarded += this.HandleRewardBasedVideoRewarded;
//        this.rewardBasedVideo.OnAdClosed += this.HandleRewardBasedVideoClosed;
//        this.rewardBasedVideo.OnAdLeavingApplication += this.HandleRewardBasedVideoLeftApplication;

//    }

//    private string GetAdMobIDs(AdType adType)
//    {
//#if true
//        string AdMob_Baner_Android = GameRemoteSettings.Instance.BannerAndroidID;
//        string AdMob_Baner_iOS = GameRemoteSettings.Instance.BannerIOSID;

//        string AdMob_Interstitial_Android = GameRemoteSettings.Instance.FullScreenAndroidID;
//        string AdMob_Interstitial_iOS = GameRemoteSettings.Instance.FullScreenIOSID;

//        string AdMob_Reward_Android = "ca-app-pub-3429412909861368/4927201008";
//        string AdMob_Reward_iOS = "ca-app-pub-3429412909861368/8947881654";
//#else              //TEST
//    string AdMob_Baner_Android =  "ca-app-pub-3940256099942544/6300978111" ;
//    string AdMob_Baner_iOS = "ca-app-pub-3940256099942544/6300978111" ;

//    string AdMob_Interstitial_Android = "ca-app-pub-3940256099942544/1033173712" ;

//    string AdMob_Interstitial_iOS = "ca-app-pub-3940256099942544/1033173712" ;

//    string AdMob_Reward_Android = "ca-app-pub-3940256099942544/5224354917";
//    string AdMob_Reward_iOS ="ca-app-pub-3940256099942544/1712485313"; 
//#endif

//        switch (adType)
//        {
//            case AdType.Interstitial:
//                switch (Application.platform)
//                {
//                    case RuntimePlatform.Android:
//                        return AdMob_Interstitial_Android;
//                    case RuntimePlatform.IPhonePlayer:
//                        return AdMob_Interstitial_iOS;
//                }
//                break;

//            case AdType.Banner:
//                switch (Application.platform)
//                {
//                    case RuntimePlatform.Android:
//                        return AdMob_Baner_Android;
//                    case RuntimePlatform.IPhonePlayer:
//                        return AdMob_Baner_iOS;
//                }
//                break;
//            case AdType.Reward:
//                {
//                    switch (Application.platform)
//                    {
//                        case RuntimePlatform.Android:
//                            return AdMob_Reward_Android;
//                        case RuntimePlatform.IPhonePlayer:
//                            return AdMob_Reward_iOS;
//                    }
//                }
//                break;
//            default:
//                break;
//        }
//        return "";
//    }



//    public void DestroyBanner()
//    {
//        bannerView.Destroy();
//    }





//    public  bool IsRewardVideoReady()
//    {
//#if UNITY_EDITOR
//        return true;
//#endif
//        return this.rewardBasedVideo.IsLoaded();
//    }

//    #region RewardBasedVideo callback handlers

//    public void HandleRewardBasedVideoLoaded(object sender, EventArgs args)
//    {
//        MonoBehaviour.print("HandleRewardBasedVideoLoaded event received");
//    }

//    public void HandleRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args)
//    {
//        MonoBehaviour.print(
//            "HandleRewardBasedVideoFailedToLoad event received with message: " + args.Message);
//    }

//    public void HandleRewardBasedVideoOpened(object sender, EventArgs args)
//    {
//        MonoBehaviour.print("HandleRewardBasedVideoOpened event received");
//    }

//    public void HandleRewardBasedVideoStarted(object sender, EventArgs args)
//    {
//        MonoBehaviour.print("HandleRewardBasedVideoStarted event received");
//    }

//    public void HandleRewardBasedVideoClosed(object sender, EventArgs args)
//    {
//        MonoBehaviour.print("HandleRewardBasedVideoClosed event received " + isReward);
//        if (!isReward)
//        {
//            UIManager.Instance.ShowPage("GameOverPage");    
//        }
        
//    }

//    bool isReward;

//    public void HandleRewardBasedVideoRewarded(object sender, Reward args)
//    {
//        if (callBackVideoReward != null)
//        {
//            callBackVideoReward.Invoke();
         
//            callBackVideoReward = null;
//        }
//        isReward = true;
//        string type = args.Type;
//        double amount = args.Amount;
//        MonoBehaviour.print(
//            "HandleRewardBasedVideoRewarded event received for " + amount.ToString() + " " + type);
//    }

//    public void HandleRewardBasedVideoLeftApplication(object sender, EventArgs args)
//    {
//        MonoBehaviour.print("HandleRewardBasedVideoLeftApplication event received");
//    }

//    #endregion


//    public void RequestBanner()
//    {
//        // Create a 320x50 banner at the top of the screen.
//        bannerView = new BannerView(GetAdMobIDs(AdType.Banner), AdSize.SmartBanner, AdPosition.Bottom);
//        // Create an empty ad request.

//        // Load the banner with the request.
//        bannerView.LoadAd(GetAdsRequest());
//    }

//    public void ShowBanner()
//    {
//        if (Utilities.IsInternetAvailable())
//        {
//            bannerView.Show();
//            Debug.Log("Show");
//        }
//    }

//    public void RequestFullScreen()
//    {
//        // Initialize an InterstitialAd.
//        interstitial = new InterstitialAd(GetAdMobIDs(AdType.Interstitial));
//        // Create an empty ad request.

//        // Load the interstitial with the request.
//        interstitial.LoadAd(GetAdsRequest());
//        this.interstitial.OnAdFailedToLoad += this.HandleInterstitialFailedToLoad;

//        this.interstitial.OnAdClosed += this.HandleInterstitialClosed;
//    }

//    public void ShowFullScreen()
//    {
//        isReward = false;
//#if UNITY_EDITOR
//        if (callBackFullScreen != null)
//            callBackFullScreen.Invoke();
//#else
//        //if (UnityEngine.Random.Range(0,100)>50)
//        //{
//        //    if (callBackFullScreen != null)
//        //        callBackFullScreen.Invoke();
//        //    return;
//        //}

//        Debug.Log(interstitial.IsLoaded() + "full");

//        if (interstitial.IsLoaded())
//        {

//            interstitial.Show();
//            RequestFullScreen();
//        }
//        else
//        {
//            if (callBackFullScreen != null)
//            {
//                callBackFullScreen.Invoke();
//                callBackFullScreen = null;
//            }
//        }
//#endif
//    }

//    public  void RequestVideoReward()
//    {
//        this.rewardBasedVideo.LoadAd(GetAdsRequest(), GetAdMobIDs(AdType.Reward));
//    }

//    public  void ShowVideoReward()
//    {
//        if (this.rewardBasedVideo.IsLoaded())
//        {
//            this.rewardBasedVideo.Show();
//        }
//        else
//        {
//            MonoBehaviour.print("Reward based video ad is not ready yet");
//        }
//    }

//    public  void SetCallBackFullScreen(System.Action action)
//    {
//        callBackFullScreen = action;
//    }

//    public void SetCallBackVideoReward(System.Action action)
//    {
//        callBackVideoReward = action;
//    }
//}


//public enum AdType
//{
//    Interstitial,
//    Banner,
//    Reward,
//}