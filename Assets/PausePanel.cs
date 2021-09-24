using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausePanel : UIPanel
{

    public ToggleButton _toggleVibration;
    public ToggleButton _toggleSound;

    private SettingData _uSetting;

    private void OnEnable()
    {
        _uSetting = SaveManager.Instance.Data.SettingData;
        _toggleSound.Initialize(_uSetting.OnSound());
        _toggleSound._OnToggleChanged = OnButtonSound;

        _toggleVibration.Initialize(_uSetting.onVibration);
        _toggleVibration._OnToggleChanged = OnButtonVibration;

        GameMaster.Instance.isPlay = false;
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

    public void OnButtonHome()
    {
        UIManager.Instance.ShowPage("StartPage");
    }

    public void OnButtonRetry()
    {
        //GameMaster.Instance.ReplayCurrentLevel();
        GameMaster.Instance.ReplayLastCheckPoint();
    }

    public void OnButtonResume()
    {
        GameMaster.Instance.ResumeGame();
    }

    public void OnButtonHowToPlay()
    {
        GameMaster.Instance.ShowHowToPlay();
    }

    public void OnButtonLanguage()
    {

    }
}
