using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class AdsLogicData
{
    //intersititial
    public bool EnableInterstitialHome = true;
    public bool EnableInterstitialGame = true;
    public bool EnableInterstitialCompleteGame = true;
    public bool EnableInterstitialSetting = true;
    public bool EnableInterstitialLanguage = true;
    public bool EnableInterstitialTutorial = true;
    public bool EnableInterstitialPauseGame = true;


    public int MinDurationInterstitialAndReward = GameConfig.MIN_DURATION_INTERSTITIAL_VS_REWARD;
    public int MinDurationInterstitial = GameConfig.MIN_DURATION_INTERSTITIAL;

    //rewards
    public int MaxBoosterPerDay = GameConfig.MAX_BOOSTER_ADS_PER_DAY;
    public int MaxRevive = GameConfig.MAX_REVIVE_ADS;
    public int ReviveBonus = GameConfig.REVIVE_ADS_BONUS_TIME;
    public int NextLevelBonus = GameConfig.NEXT_LEVEL_ADS_BONUS;

    public AdsLogicData()
    {
        EnableInterstitialHome = true;
        EnableInterstitialGame = true;
        EnableInterstitialCompleteGame = true;
        EnableInterstitialSetting = true;
        EnableInterstitialLanguage = true;
        EnableInterstitialTutorial = true;
        EnableInterstitialPauseGame = true;

        MinDurationInterstitialAndReward = 30;
        MinDurationInterstitial = 60;

        //rewards
        MaxBoosterPerDay = 5;
        MaxRevive = 5;
        ReviveBonus = 100;
        NextLevelBonus = 30;
    }

}

public enum FLOW_ADS
{
    NONE,
    INTER_HOME,
    INTER_GAME,
    INTER_COMPLETE_GAME,
    INTER_SETTING,
    INTER_LANGUAGE,
    INTER_TUTORIAL,
    INTER_PAUSE_GAME
}



public class AdsLogicController : SingletonMonoBehaviour<AdsLogicController>
{
    public AdsLogicData Data;

    private float _nextTimeShowInterstitial;

    public void ResetAfterWatchedReward(bool watchedSuccess)
    {
        if (watchedSuccess)
            _nextTimeShowInterstitial = Time.time + Data.MinDurationInterstitialAndReward;
    }

    public bool CheckShowInterstitial(FLOW_ADS from)
    {
        bool check = false;
        switch (from)
        {
            case FLOW_ADS.NONE:
                break;
            case FLOW_ADS.INTER_HOME:
                check = Data.EnableInterstitialHome;
                break;
            case FLOW_ADS.INTER_GAME:
                check = Data.EnableInterstitialGame;
                break;
            case FLOW_ADS.INTER_COMPLETE_GAME:
                check = Data.EnableInterstitialCompleteGame;
                break;
            case FLOW_ADS.INTER_SETTING:
                check = Data.EnableInterstitialSetting;
                break;
            case FLOW_ADS.INTER_LANGUAGE:
                check = Data.EnableInterstitialLanguage;
                break;
            case FLOW_ADS.INTER_TUTORIAL:
                check = Data.EnableInterstitialTutorial;
                break;
            case FLOW_ADS.INTER_PAUSE_GAME:
                check = Data.EnableInterstitialPauseGame;
                break;
            default:
                break;
        }

        if (check && Time.time >= _nextTimeShowInterstitial)
        {
            _nextTimeShowInterstitial = Time.time + Data.MinDurationInterstitial;
            return true;
        }
        else
            return false;
    }

    public override void Initialize()
    {
        base.Initialize();
        if (Data == null)
        {
            Data = new AdsLogicData();
        }
        _nextTimeShowInterstitial = 0;

        try
        {
            var remoteConfig = RemoteConfigManager.Instance.GetConfigValue("MIN_DURATION_INTERSTITIAL_VS_REWARD");
            if (remoteConfig.Source != Firebase.RemoteConfig.ValueSource.DefaultValue)
                Data.MinDurationInterstitialAndReward = (int)remoteConfig.LongValue;

            remoteConfig = RemoteConfigManager.Instance.GetConfigValue("MIN_DURATION_INTERSTITIAL");
            if (remoteConfig.Source != Firebase.RemoteConfig.ValueSource.DefaultValue)
                Data.MinDurationInterstitial = (int)remoteConfig.LongValue;

            remoteConfig = RemoteConfigManager.Instance.GetConfigValue("MAX_BOOSTER_ADS_PER_DAY");
            if (remoteConfig.Source != Firebase.RemoteConfig.ValueSource.DefaultValue)
                Data.MaxBoosterPerDay = (int)remoteConfig.LongValue;

            remoteConfig = RemoteConfigManager.Instance.GetConfigValue("MAX_REVIVE_ADS");
            if (remoteConfig.Source != Firebase.RemoteConfig.ValueSource.DefaultValue)
                Data.MaxRevive = (int)remoteConfig.LongValue;

            remoteConfig = RemoteConfigManager.Instance.GetConfigValue("REVIVE_ADS_BONUS_TIME");
            if (remoteConfig.Source != Firebase.RemoteConfig.ValueSource.DefaultValue)
                Data.ReviveBonus = (int)remoteConfig.LongValue;

            remoteConfig = RemoteConfigManager.Instance.GetConfigValue("NEXT_LEVEL_ADS_BONUS");
            if (remoteConfig.Source != Firebase.RemoteConfig.ValueSource.DefaultValue)
                Data.NextLevelBonus = (int)remoteConfig.LongValue;
        }
        catch(Exception ex)
        {
            Debug.LogError($"Sync Data remote config error: {ex}");
        }

     

    }

}
