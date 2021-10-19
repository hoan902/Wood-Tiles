using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public interface AdsProvider
{
    event Action<bool> OnInterstitialShowComplete;
    event Action<bool> OnInterstitialLoadComplete;

    event Action<bool, float> OnRewardShowComplete;
    event Action<bool> OnRewardLoadComplete;

    void Init();

    void LoadInterstitial(Action<bool> loadComplete = null);
    void ShowInterstitial(Action<bool> showComplete = null);
    bool IsInterstitialReady { get; }


    void LoadRewardVideo(Action<bool> loadComplete = null);
    void ShowRewardVideo(Action<bool, float> showComplete = null);
    bool IsRewardVideoReady { get; }


    void RequestAdsBanner(Action<bool> loadComplete = null);
    bool IsBannerAdsReady { get; }
    void ShowAdsBanner(Action<bool> showComplete = null);
    void DestroyAdsBanner(Action<bool> clearComplete = null);

}
