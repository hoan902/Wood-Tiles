using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using EventManager;
using MEC;
using System;
using UnityEngine.SceneManagement;
using Lean;
using UnityEngine.AddressableAssets;


public enum FX_ENUM
{
    NONE,
    STAR_EXPLODE
}

public class GameMaster : SingletonMonoBehaviour<GameMaster>
{
    #region Load Game
    public IEnumerator<float> StartLoadGame()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        Vibration.Init();

        Debug.Log("GameMgr initialize");
        IAPManager.instance.Initialize();
        SaveManager.Instance.LoadData();
        AdsManager.instance.Initialize();
        AudioManager.Instance.Initialize();
        AdsLogicController.Instance.Initialize();

        yield return Timing.WaitUntilDone(Timing.RunCoroutine(RemoteConfigManager.Instance.InitializeAsync()));

        yield return Timing.WaitForOneFrame;
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(DesignManager.Instance.InitalizeCoroutine()));
    }

    public void FinishLoadingProcess()
    {
        UIManager.Instance.ShowDefaultPage();
        AdsManager.instance.ShowAdsBanner();
    }

    #endregion

    int levelID;
    float timePlay;
    float timeCurrent;
    long scoreTotal;

    int levelMax;
    LevelData currentLevel;

    //int lifeCount = 3;
    MoveMode moveMode;
    LevelState levelState;

    void OnEnable()
    {
        this.RegisterListener(EventID.UpdateScore, (sender, param) => UpdateScore((long)param));
    }

    void OnDisable()
    {
        this.RemoveListener(EventID.UpdateScore, (sender, param) => UpdateScore((long)param));
    }

    void Reset()
    {
        scoreTotal = GameDataMgr.CurrentScore;
    }

    public void PlayGameFromLevel(long bonusTime = 0)
    {
        Reset();
        PlayGame(bonusTime);

    }

    public void ReplayLastCheckPoint()
    {
        Reset();
        var lastCheckPointData = SaveManager.Instance.Data.GetCurrentCheckPointData();
        if (lastCheckPointData != null)
        {
            lastCheckPointData.ResetDefault();
            GameDataMgr.CurrentScore = lastCheckPointData.InitScore;
            DialogMessage.Instance.OpenDialog("Replay", $"Replay from level {lastCheckPointData.Level}", () =>
                    {
                        SaveManager.Instance.Data.UserProgress.currentLevel = lastCheckPointData.Level;
                        this.PostEvent(EventID.UpdateScore, GameDataMgr.CurrentScore);
                        PlayGame(0);
                    });
        }
        else
        {
            Debug.LogError("sth went wrong with last check point!!!");
        }



    }

    public void ReplayCurrentLevel()
    {
        Reset();
        PlayGame(0);
    }

    public void NextLevel()
    {
        MissionManager.Instance.NextLevel();
        PlayGameFromLevel();
    }

    public void NextLevelWithAds()
    {
        long bonusTime = GameConst.ADS_NEXT_LEVEL_BONUS;
        AdsManager.instance.ShowAdsReward((success, amount) =>
        {
            if (success)
            {
                DialogMessage.Instance.OpenDialog("Congratulation", $"You got more {bonusTime}s", () =>
                {
                    MissionManager.Instance.NextLevel();
                    PlayGameFromLevel(bonusTime);
                });

            }
        });
    }

    public bool isPlay = false;

    void PlayGame(long bonusTime)
    {
        if (AdsLogicController.Instance.CheckShowInterstitial(FLOW_ADS.INTER_GAME))
        {
            AdsManager.instance.ShowAdsInterstitial((success) =>
            {

                StartGamePage(bonusTime);
            });
        }
        else
        {
            StartGamePage(bonusTime);
        }
    }

    private void StartGamePage(long bonusTime)
    {
        UIManager.Instance.ShowPage("GamePage");

        var levelData = MissionManager.Instance.GetCurrentLevelData();
        currentLevel = levelData;
        moveMode = (MoveMode)currentLevel.GlobalMoveMode;
        levelID = currentLevel.ID;
        float _timePlay = GameController.Instance.InitMap(this, currentLevel, moveMode);

        timeCurrent = timePlay = _timePlay;  //convert minute to second
        timeCurrent += bonusTime;
        isPlay = true;

        this.PostEvent(EventID.UpdateLevel, (levelID));
        this.PostEvent(EventID.UpdateBossterSearch, GameDataMgr.BoosterSearch);
        this.PostEvent(EventID.UpdateScore, scoreTotal);
        this.PostEvent(EventID.UpdateBoosterRocket, GameDataMgr.BoosterRocket);
        this.PostEvent(EventID.UpdateBoosterShuffle, GameDataMgr.BoosterShuffle);
        this.PostEvent(EventID.UpdateBoosterTheme, GameDataMgr.BoosterTheme);
    }

    public void ResetAllInfos()
    {
        this.PostEvent(EventID.UpdateLevel, (levelID));
        this.PostEvent(EventID.UpdateBossterSearch, GameDataMgr.BoosterSearch);
        this.PostEvent(EventID.UpdateScore, scoreTotal);
        this.PostEvent(EventID.UpdateBoosterRocket, GameDataMgr.BoosterRocket);
        this.PostEvent(EventID.UpdateBoosterShuffle, GameDataMgr.BoosterShuffle);
        this.PostEvent(EventID.UpdateBoosterTheme, GameDataMgr.BoosterTheme);
    }

    public void UpdateResetCount()
    {
        GameDataMgr.BoosterShuffle--;
        //if (GameDataMgr.BoosterShuffle < 0)
        //{
        //    LevelFailed(false);
        //    return;
        //}
        this.PostEvent(EventID.UpdateBoosterShuffle, GameDataMgr.BoosterShuffle);
    }
    
    public void UpdateResetCountTheme()
    {
        GameDataMgr.BoosterTheme--;
        //if (GameDataMgr.BoosterShuffle < 0)
        //{
        //    LevelFailed(false);
        //    return;
        //}
        this.PostEvent(EventID.UpdateBoosterTheme, GameDataMgr.BoosterTheme);
    }

    void Update()
    {
        if (isPlay)
        {
            timeCurrent -= Time.deltaTime;
            this.PostEvent(EventID.UpdateTime, timeCurrent / timePlay);
            if (timeCurrent <= 0)
                LevelFailed(true);
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus && isPlay)
        {
            //PauseGame(false);
        }
    }

    public void UpdateScore(long score)
    {
        scoreTotal = score;
        GameDataMgr.CurrentScore = score;
        if (GameDataMgr.CurrentScore > GameDataMgr.HighScore)
            GameDataMgr.HighScore = GameDataMgr.CurrentScore;
        this.PostEvent(EventID.UpdateScoreTotal, scoreTotal);

    }

    public void PauseGame(bool checkWithAds = true)
    {
        if (checkWithAds)
        {
            if (AdsLogicController.Instance.CheckShowInterstitial(FLOW_ADS.INTER_PAUSE_GAME))
            {
                AdsManager.instance.ShowAdsInterstitial((success) =>
                {
                    UIManager.Instance.ShowPage("PausePage");
                    isPlay = false;
                });
            }
            else
            {
                UIManager.Instance.ShowPage("PausePage");
                isPlay = false;
            }
        }
        else
        {
            UIManager.Instance.ShowPage("PausePage");
            isPlay = false;
        }

    }


    public void ResumeGame()
    {
        UIManager.Instance.ShowPage("GamePage");

        Timing.CallDelayed(0.6f, () =>
        {
            isPlay = true;
        });
    }

    public void ShowHowToPlay()
    {
        if (AdsLogicController.Instance.CheckShowInterstitial(FLOW_ADS.INTER_TUTORIAL))
        {
            AdsManager.instance.ShowAdsInterstitial((success) =>
            {
                Debug.Log($"Show Interstitial PlayGame: {success}");
                UIManager.Instance.ShowPage("HowToPlayPage");
            });
        }
        else
        {
            UIManager.Instance.ShowPage("HowToPlayPage");
        }

    }

    public void LeveFinish()
    {
        isPlay = false;
        AudioManager.Instance.Shot("completed");
        //add score time
        float scoreScale = moveMode == MoveMode.Idle ? 1 : 1.5f;
        //float scoreScale = 1;


        long totalScore = GameDataMgr.CurrentScore + (long)(timeCurrent * 10f * scoreScale);
        this.PostEvent(EventID.UpdateScore, (long)(totalScore));

        LeaderBoardMgr.Instance.PostHighScoreToLeaderBoard(scoreTotal);


        levelState = GetLevelStateFromTime();


        MissionManager.Instance.FinishLevel();


        MissionManager.Instance.SaveLevelState(currentLevel.ID, currentLevel.MissionID, levelState);


        UIManager.Instance.ShowPage("LevelCompleted");

        var listReward = GameController.Instance._levelInfoData._listRewardData;

        LevelCompletePanel.Instance.SetData((currentLevel.ID), totalScore, GameDataMgr.HighScore, (long)timeCurrent, levelState, listReward);


        if (MissionManager.Instance.isWin())
        {
            StartCoroutine(OnWin());
        }
    }

    /// <summary>
    /// Get levelstate from time play when finish
    /// </summary>
    /// <returns></returns>
    LevelState GetLevelStateFromTime()
    {
        LevelState result = LevelState.IsOpen;
        if (timeCurrent / timePlay >= 0.3f)
            result = LevelState.IsThreeStar;
        else if (timeCurrent / timePlay >= 0.2f)
            result = LevelState.IsTwoStar;
        else if (timeCurrent / timePlay >= 0f)
            result = LevelState.IsOneStar;
        return result;
    }

    /// <summary>
    /// clear all levels
    /// </summary>
    /// <returns></returns>
    IEnumerator OnWin()
    {
        isPlay = false;
        //yield return new WaitForSeconds(2f);
        //UIManager.Instance.ShowPage("WinPage");
        yield return new WaitForSeconds(0.3f);
        if (AdsLogicController.Instance.CheckShowInterstitial(FLOW_ADS.INTER_COMPLETE_GAME))
        {
            AdsManager.instance.ShowAdsInterstitial((complete) =>
            {
                Debug.Log($"ShowAdsInterstitial Finish Game {complete}");
            });
        }

    }

    public void LevelFailed(bool isTimesup)
    {
        isPlay = false;
        long secRevive = GameConst.SEC_REVIVE;
        DialogMessage.Instance.OpenDialog($"Get More Time", $"Watch a video ad to get more {secRevive}s", "Cancel", "Watch",
         () =>
           {
               AdsManager.instance.ShowAdsReward((success, amount) =>
               {
                   if (success)
                   {
                       DialogMessage.Instance.OpenDialog("Congratulation", $"+{secRevive} seconds", () =>
                      {
                          DoRevive(GameConst.SEC_REVIVE);
                      });
                   }
               });
           }
       , () =>
        {
            DoLevelFail(isTimesup);
        });
    }

    public void DoRevive(long secAdd)
    {
        timeCurrent += secAdd;
        isPlay = true;
    }

    public void DoLevelFail(bool isTimesup)
    {
        Timing.RunCoroutine(DoLevelFailCoroutine(isTimesup));
    }

    IEnumerator<float> DoLevelFailCoroutine(bool isTimesup)
    {
        AudioManager.Instance.Shot("failed");
        UIManager.Instance.ShowPage("LevelFailed");
        LevelFailedPanel.Instance.SetData((currentLevel.ID + 1).ToString(), scoreTotal, GameDataMgr.HighScore, isTimesup);
        LeaderBoardMgr.Instance.PostHighScoreToLeaderBoard(scoreTotal);
        yield return Timing.WaitForSeconds(0.3f);
        if (AdsLogicController.Instance.CheckShowInterstitial(FLOW_ADS.INTER_COMPLETE_GAME))
        {
            AdsManager.instance.ShowAdsInterstitial((complete) =>
            {
                Debug.Log($"ShowAdsInterstitial Finish Game {complete}");
            });
        }
    }

    public void DoVibratePhone()
    {
        if (SaveManager.Instance.Data.SettingData.onVibration)
        {
            Vibration.VibratePop();
        }
        //Handheld.Vibrate();
    }

    #region Cheat only
    public void CheatWinLevel()
    {
        GameController.Instance.CheatWinLevel();
    }

    public void CheatLoseLevel()
    {
        this.timeCurrent = 0;
    }
    #endregion

    public void ResetIAPItems()
    {
        this.PostEvent(EventID.UpdateIAPOffer);
    }

    #region Scene Utils

    public void LoadScene(string sceneName, Action<bool> callback)
    {
        StartCoroutine(LoadSceneAsync(sceneName, callback));
    }

    IEnumerator LoadSceneAsync(string sceneName, Action<bool> callback)
    {
        yield return null;

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        //Don't let the Scene activate until you allow it to
        asyncOperation.allowSceneActivation = false;
        Debug.Log("Pro :" + asyncOperation.progress);
        //When the load is still in progress, output the Text and progress bar
        while (!asyncOperation.isDone)
        {
            // Check if the load has finished
            if (asyncOperation.progress >= 0.9f)
            {
                asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }

        if (asyncOperation.isDone)
            callback?.Invoke(true);
    }

    #endregion

    #region Effect
    public void PlayEffect(FX_ENUM _fxEnum, Vector3 position, Transform parent)
    {
        var fx = ResourcesManager.Instance.GetEffect(_fxEnum);
        if (fx != null)
        {
            var ps = LeanPool.Spawn<AutoDespawnParticles>(fx, position, Quaternion.identity);
            ps.transform.SetParent(parent);
            ps.transform.localScale = Vector3.one;
            ps.transform.position = position;
            ps.PlayEffect();
        }
    }

    #endregion

    #region IAP
    public void PurChaseIAP(string iap_id, Action<bool> callback)
    {
        IAPManager.instance.PurchaseIAP(iap_id, callback);
    }
    #endregion
}

public static class FileUtils
{
    public static TextAsset GetTextAsset(string resourcePath)
    {
        TextAsset result = null;
        var textFile = Resources.Load<TextAsset>(resourcePath);
        if (textFile != null)
        {
            result = textFile;
        }
        return result;
    }
}

