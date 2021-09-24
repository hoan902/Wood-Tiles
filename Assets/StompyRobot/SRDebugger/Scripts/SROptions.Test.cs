
using System;
using System.ComponentModel;
using System.Diagnostics;
using SRDebugger;
using SRDebugger.Services;
using SRF;
using SRF.Service;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public partial class SROptions
{
    [Category("[GamePlay]")]
    [Sort(1)]
    public void WinLevel()
    {
        GameMaster.Instance.CheatWinLevel();
    }

    [Category("[GamePlay]")]
    [Sort(2)]
    public void LoseLevel()
    {
        GameMaster.Instance.CheatLoseLevel();
    }

    private int _level = 1;
    [Category("[GamePlay]")]
    [Sort(3)]
    [NumberRange(1, 500)]
    public int Level { get { return _level; } set { _level = value; } }

    [Category("[GamePlay]")]
    [Sort(4)]
    public void JumpToLevel()
    {
        var progress = SaveManager.Instance.Data.UserProgress;
        progress.currentLevel = _level;
        GameMaster.Instance.PlayGameFromLevel();
    }

    [Category("[GamePlay]")]
    [Sort(5)]
    public void AddBooster()
    {
        SaveManager.Instance.Data.SetBooterItem(BOOSTER_ID.ROCKET, SaveManager.Instance.Data.GetBoosterItemCount(BOOSTER_ID.ROCKET).Num + 1);
        SaveManager.Instance.Data.SetBooterItem(BOOSTER_ID.SEARCH, SaveManager.Instance.Data.GetBoosterItemCount(BOOSTER_ID.SEARCH).Num + 1);
        SaveManager.Instance.Data.SetBooterItem(BOOSTER_ID.SHUFFLE, SaveManager.Instance.Data.GetBoosterItemCount(BOOSTER_ID.SHUFFLE).Num + 1);
        SaveManager.Instance.Data.SetBooterItem(BOOSTER_ID.THEME, SaveManager.Instance.Data.GetBoosterItemCount(BOOSTER_ID.THEME).Num + 1);

        GameMaster.Instance.ResetAllInfos();
    }


    private bool _isGodMod = false;
    [Category("[GamePlay]")]
    [Sort(6)]
    public bool GodMod { get { return _isGodMod; } set { _isGodMod = value; } }

    private bool _iapApprove;
    [Category("IAP")]
    public bool IAPAlwaysApprove {
        get { return _iapApprove; }
        set { _iapApprove = value; }
    }

    [Category("ADS")]
    public void ShowAdsReward()
    {
        AdsManager.instance.ShowAdsReward((complete, amount) =>
        {
            Debug.Log($"ShowAdsReward {complete} - {amount}");
        });
    }

    [Category("ADS")]
    public void ShowAdsInterstitial()
    {
        AdsManager.instance.ShowAdsInterstitial((complete) =>
        {
            Debug.Log($"ShowAdsInterstitial {complete}  ");
        });
    }

    [Category("ADS")]
    public void HideAdsBanner()
    {
        AdsManager.instance.HideAdsBanner((success) =>
        {

        });
    }

    [Category("ADS")]
    public void ShowAdsBanner()
    {
        AdsManager.instance.ShowAdsBanner((complete) =>
        {
            Debug.Log($"ShowAdsBanner {complete}  ");
        });
    }

    [Category("IAP")]
    public void RestoreAllIAPPacks()
    {

    }

    [Category("IAP")]
    public void CheatFinishOffer()
    {
        var offer = SaveManager.Instance.Data.GetIAPTracker(GameConst.IAP_OFFER_3_99);
        if(offer != null)
        {
            offer.timeExpired = TimeService.GetCurrentTimeStamp() + 10;
        }
    }
}
