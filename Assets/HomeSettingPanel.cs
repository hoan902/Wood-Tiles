using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeSettingPanel : UIPanel
{
    public ToggleButton _toggleNotification;
    public ToggleButton _toggleSound;
    public ToggleButton _toggleVibration;

    private SettingData _uSetting;
    private void OnEnable()
    {
        _uSetting = SaveManager.Instance.Data.SettingData;
        _toggleNotification.Initialize(_uSetting.onNotification);
        _toggleNotification._OnToggleChanged = OnButtonNotification;

        _toggleSound.Initialize(_uSetting.OnSound());
        _toggleSound._OnToggleChanged = OnButtonSound;

        _toggleVibration.Initialize(_uSetting.onVibration);
        _toggleVibration._OnToggleChanged = OnButtonVibration;
    }

    private void OnButtonVibration(bool enable)
    {
        SaveManager.Instance.Data.SettingData.onVibration = enable;
    }

    private void OnButtonSound(bool enable)
    {
        SaveManager.Instance.Data.SettingData.onBGM = enable;
        SaveManager.Instance.Data.SettingData.onSFX = enable;
        AudioManager.Instance.ResetAudio();
    }

    private void OnButtonNotification(bool enable)
    {
        SaveManager.Instance.Data.SettingData.onNotification = enable;
    }

    public void OnButtonClose()
    {
        UIManager.Instance.ShowPage("StartPage");
    }

    public void OnButtonSignInFacebook()
    {

    }

    public void OnButtonSignInApple()
    {

    }

    public void OnButtonRestorePurchase()
    {

    }

    public void OnButtonLanguage()
    {
        if (AdsLogicController.Instance.CheckShowInterstitial(FLOW_ADS.INTER_LANGUAGE))
        {
            AdsManager.instance.ShowAdsInterstitial((success) =>
            {
                Debug.Log($"Show Interstitial PlayGame: {success}");
            });
        }
    }
}
