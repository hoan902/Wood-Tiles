using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public enum ADSTYPE
{
    ADS_INTERSTITIAL,
    ADS_REWARD,
    ADS_BANNER
}

public class SimulateAdsController : AdsProvider
{
    private bool _isInterstitialReady = true;
    public bool IsInterstitialReady => _isInterstitialReady;

    private bool _isRewardReady = true;
    public bool IsRewardVideoReady => _isRewardReady;

    public bool IsBannerAdsReady => throw new NotImplementedException();

    public event Action<bool> OnInterstitialShowComplete;
    public event Action<bool> OnInterstitialLoadComplete;
    public event Action<bool, float> OnRewardShowComplete;
    public event Action<bool> OnRewardLoadComplete;

    public void Init()
    {
        //DO NOTHING
    }

    public void LoadInterstitial(Action<bool> loadComplete = null)
    {
        Debug.Log("[SimulateAds] LoadInterstitial");
        loadComplete?.Invoke(true);
    }

    public void LoadRewardVideo(Action<bool> loadComplete = null)
    {
        Debug.Log("[SimulateAds] LoadRewardVideo");
        loadComplete?.Invoke(true);
    }

    public void ShowInterstitial(Action<bool> showComplete = null)
    {
        Debug.Log("[SimulateAds] ShowInterstitial");
        TopLayerCanvas.Instance.ShowHUD(EnumHUD.HUD_SIMULATE_ADS, ADSTYPE.ADS_INTERSTITIAL);
        Timing.CallDelayed(2.0f, () =>
        {
            TopLayerCanvas.Instance.HideHUD(EnumHUD.HUD_SIMULATE_ADS);
            showComplete?.Invoke(true);
        });
    }

    public void ShowRewardVideo(Action<bool, float> showComplete = null)
    {
        Debug.Log("[SimulateAds] ShowRewardVideo");
        TopLayerCanvas.Instance.ShowHUD(EnumHUD.HUD_SIMULATE_ADS, ADSTYPE.ADS_REWARD);
        Timing.CallDelayed(3.0f, () =>
        {
            TopLayerCanvas.Instance.HideHUD(EnumHUD.HUD_SIMULATE_ADS);
            showComplete?.Invoke(true, 100f);
        });
    }

    public void RequestAdsBanner(Action<bool> loadComplete = null)
    {
        loadComplete?.Invoke(true);
    }

    public void ShowAdsBanner(Action<bool> showComplete = null)
    {
        showComplete?.Invoke(true);

        TopLayerCanvas.Instance.EnableSimuateAdsBanner(true);
    }

    public void DestroyAdsBanner(Action<bool> clearComplete = null)
    {
        clearComplete?.Invoke(true);
        TopLayerCanvas.Instance.EnableSimuateAdsBanner(false);
    }

    public void HideAdsBanner(Action<bool> hideComplete = null)
    {
        hideComplete?.Invoke(true);
        TopLayerCanvas.Instance.EnableSimuateAdsBanner(false);


    }
}
