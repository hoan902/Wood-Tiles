using MEC;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AdsManager : MonoBehaviour
{
    public static AdsManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void OnDisable()
    {
        RemoveListener();
    }

    //public List<AdsProvider> _listAdsProvider = new List<AdsProvider>();

    //public AdsProvider AdsProvider
    //{
    //    get { return (_listAdsProvider == null || _listAdsProvider.Count <= 0) ? null : _listAdsProvider[0]; }
    //}

    public void Initialize()
    {
        Debug.Log("Ads Initialize!!!");
        //TO DO ADS ads manager
        //_listAdsProvider.Clear();

//#if UNITY_EDITOR
//        AdsProvider fakeAdsProvider = new SimulateAdsController();
//        fakeAdsProvider.Init();
//        _listAdsProvider.Add(fakeAdsProvider);
//#endif


        //AdsProvider ironsourceProvider = new IronSourceController();
        //ironsourceProvider.Init();
        //_listAdsProvider.Add(ironsourceProvider);

        //AdsProvider AdmobProvider = new AdmobAdsController();
        //AdmobProvider.Init();
        //_listAdsProvider.Add(AdmobProvider);

        //RemoveListener();
        //AdsProvider.OnInterstitialLoadComplete += AdsProvider_OnInterstitialLoadComplete;
        //AdsProvider.OnInterstitialShowComplete += AdsProvider_OnInterstitialShowComplete;
        //AdsProvider.OnRewardLoadComplete += AdsProvider_OnRewardLoadComplete;
        //AdsProvider.OnRewardShowComplete += AdsProvider_OnRewardShowComplete;

        performCallWatchAds = false;
    }

    private void RemoveListener()
    {
        //AdsProvider.OnInterstitialLoadComplete -= AdsProvider_OnInterstitialLoadComplete;
        //AdsProvider.OnInterstitialShowComplete -= AdsProvider_OnInterstitialShowComplete;
        //AdsProvider.OnRewardLoadComplete -= AdsProvider_OnRewardLoadComplete;
        //AdsProvider.OnRewardShowComplete -= AdsProvider_OnRewardShowComplete;
    }

    #region Event Handler

    private void AdsProvider_OnRewardLoadComplete(bool success)
    {
        Debug.Log($"[Ads] RewardLoadComplete {success}");
        if (!success)
            EnableLoadingScreen(false);
    }

    private void AdsProvider_OnRewardShowComplete(bool success, float amount)
    {
        Debug.Log($"[Ads] RewardShowComplete {success} - {amount}");
        EnableLoadingScreen(false);
    }

    private void AdsProvider_OnInterstitialLoadComplete(bool success)
    {
        Debug.Log($"[Ads] InterstitialLoadComplete {success}");
        if (!success)
            EnableLoadingScreen(false);
    }

    private void AdsProvider_OnInterstitialShowComplete(bool success)
    {
        Debug.Log($"[Ads] InterstitialShowComplete {success}");
        EnableLoadingScreen(false);
    }

    #endregion

    #region Intertitial

    public void LoadAdsInterstitial(Action<bool> loadComplete)
    {
        //AdsProvider.LoadInterstitial((success) =>
        //{
        //    TopLayerCanvas.Instance.ShowHUD(EnumHUD.HUD_LOADING);
        //    loadComplete?.Invoke(success);
        //});
    }

    public void ShowAdsInterstitial(Action<bool> showComplete)
    {
        //if (SaveManager.Instance.Data.MetaData.IsRemoveAds)
        //{
        //    Debug.LogError("Block Show Interstital => remove Ads!!!");
        //    showComplete?.Invoke(false);
        //    return;
        //}

        //if (AdsProvider.IsInterstitialReady)
        //{
        //    Debug.Log("Start show interstital!");
        //    AdsProvider.ShowInterstitial((success) =>
        //    {
        //        TopLayerCanvas.Instance.HideHUD(EnumHUD.HUD_LOADING);
        //        showComplete?.Invoke(success);
        //        AdsProvider.LoadInterstitial();
        //    });
        //}
        //else
        //{
        //    TopLayerCanvas.Instance.HideHUD(EnumHUD.HUD_LOADING);
        //    showComplete?.Invoke(true);
        //    AdsProvider.LoadInterstitial();
        //    //Debug.Log("wait show interstital");
        //    //Timing.RunCoroutine(LoadAndShowInterstitialCoroutine(10f, (success) =>
        //    //{
        //    //    TopLayerCanvas.Instance.HideHUD(EnumHUD.HUD_LOADING);
        //    //    showComplete?.Invoke(success);
        //    //}));
        //}

        //AppLovinManager.Instance.ShowInterstitial(() => showComplete?.Invoke(true), () => showComplete?.Invoke(false));
    }

    //IEnumerator<float> LoadAndShowInterstitialCoroutine(float timeOut, Action<bool> showComplete)
    //{
    //    EnableLoadingScreen(true);
    //    bool waitCheckInternet = true;
    //    bool hasInternet = true;

    //    //NetworkDetector.instance.checkInternetConnection((reached) =>
    //    //{
    //    //    waitCheckInternet = false;
    //    //    hasInternet = reached;
    //    //});

    //    waitCheckInternet = false;

    //    while (waitCheckInternet)
    //        yield return Timing.WaitForOneFrame;

    //    if (!hasInternet)
    //    {
    //        Debug.LogError("No internet blocked load ads interstital");
    //        showComplete?.Invoke(false);
    //        yield break;
    //    }

    //    bool waitingAds = true;
    //    float waitAds = timeOut;
    //    float timerAds = 0f;
    //    AdsProvider.LoadInterstitial();
    //    while (waitingAds && timerAds <= waitAds)
    //    {
    //        timerAds += Time.deltaTime;
    //        if (AdsProvider.IsInterstitialReady)
    //        {
    //            waitingAds = false;
    //            AdsProvider.ShowInterstitial(showComplete);
    //            break;
    //        }

    //        yield return Timing.WaitForOneFrame;
    //    }

    //    if (waitingAds)
    //    {
    //        //time out!!!
    //        showComplete?.Invoke(false);
    //        yield break;
    //    }

    //    EnableLoadingScreen(false);
    //}

    #endregion

    #region RewardAds
    private bool performCallWatchAds = false;

    //public void LoadAdsReward(Action<bool> loadComplete)
    //{
    //    AdsProvider.LoadRewardVideo(loadComplete);
    //}

    public void ShowAdsReward(Action<bool, float> showComplete)
    {
        //if (AdsProvider.IsRewardVideoReady)
        //{
        //    AdsProvider.ShowRewardVideo((success, amount) =>
        //    {
        //        EnableLoadingScreen(false);
        //        AdsLogicController.Instance.ResetAfterWatchedReward(success);
        //        showComplete?.Invoke(success, amount);
        //    });
        //}
        //else
        //{
        //    Timing.RunCoroutine(LoadAndShowRewardCoroutine(10f, (success, amount) =>
        //    {
        //        EnableLoadingScreen(false);
        //        AdsLogicController.Instance.ResetAfterWatchedReward(success);
        //        showComplete?.Invoke(success, amount);
        //    }));
        //}
        //AppLovinManager.Instance.ShowRewardedAd_Modify(() => showComplete?.Invoke(true,0), () => showComplete?.Invoke(false,0));
    }

    //public void ShowAdsRewardWithNotify(Action onSuccess)
    //{
    //    if (performCallWatchAds)
    //        return;
    //    performCallWatchAds = true;
    //    TopLayerCanvas.Instance.ShowHUD(EnumHUD.HUD_LOADING);
    //    ShowAdsReward((result, f) =>
    //    {
    //        Debug.Log($"ShowAdsRewardWithNotify result {result}");
    //        if (result)
    //        {
    //            onSuccess?.Invoke();
    //        }
    //        else
    //        {
    //            //MasterCanvas.CurrentMasterCanvas.ShowNotifyHUD(LOCALIZE_ID_PREF.WATCH_ADS_FAIL.AsLocalizeString());
    //        }

    //        performCallWatchAds = false;
    //        TopLayerCanvas.Instance.HideHUD(EnumHUD.HUD_LOADING);
    //    });
    //}

    //IEnumerator<float> LoadAndShowRewardCoroutine(float timeOut, Action<bool, float> showComplete)
    //{
    //    float waitAds = timeOut;
    //    float timerAds = 0f;
    //    bool waitingAds = true;
    //    EnableLoadingScreen(true);

    //    bool waitCheckInternet = true;
    //    bool hasInternet = true;
    //    //NetworkDetector.instance.checkInternetConnection((reached) =>
    //    //{
    //    //    waitCheckInternet = false;
    //    //    hasInternet = reached;
    //    //});

    //    waitCheckInternet = false;
    //    while (waitCheckInternet)
    //        yield return Timing.WaitForOneFrame;

    //    if (!hasInternet)
    //    {
    //        Debug.LogError("No internet blocked load ads rewards");
    //        showComplete?.Invoke(false, -1);
    //        yield break;
    //    }
    //    else
    //    {
    //        AdsProvider.LoadRewardVideo();
    //        while (waitingAds && timerAds <= waitAds)
    //        {
    //            timerAds += Time.deltaTime;
    //            if (AdsProvider.IsRewardVideoReady)
    //            {
    //                waitingAds = false;
    //                AdsProvider.ShowRewardVideo(showComplete);
    //                break;
    //            }

    //            yield return Timing.WaitForOneFrame;
    //        }

    //        if (waitingAds)
    //        {
    //            showComplete?.Invoke(false, -1f);
    //        }
    //    }


    //}

    #endregion

    #region ADS Banner

    public void ShowAdsBanner(Action<bool> showComplete = null)
    {
        //if (SaveManager.Instance.Data.MetaData.IsRemoveAds)
        //{
        //    showComplete?.Invoke(false);
        //    HideAdsBanner((success) =>
        //    {

        //    });
        //    return;
        //}

        //AdsProvider.ShowAdsBanner(showComplete);
        //AppLovinManager.Instance.ShowBanner();
    }

    public void HideAdsBanner(Action<bool> hideComplete)
    {
        //AdsProvider.DestroyAdsBanner(hideComplete);
        //AppLovinManager.Instance.HideBanner();
    }

    #endregion

    private void EnableLoadingScreen(bool enable)
    {
        //if (enable)
        //    TopLayerCanvas.instance.ShowHUD(EnumHUD.HUD_LOADING, false);
        //else
        //    TopLayerCanvas.instance.HideHUD(EnumHUD.HUD_LOADING);
    }

    private void OnApplicationPause(bool pause)
    {
        //if (AdsProvider != null && AdsProvider.GetType() == typeof(IronSourceController))
        //{
        //    IronSourceController irController = (IronSourceController)AdsProvider;
        //    irController.OnApplicationPause(pause);
        //}
    }
}