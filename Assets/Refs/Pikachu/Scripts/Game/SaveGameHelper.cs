using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
public static class SaveGameHelper
{
    public static UserData DefaultData()
    {
        UserData data = new UserData();
        data.MetaData = new MetaData()
        {
            UserID = RandomString(10),
            UserName = "USER_01"
        };

        data.UserProgress = new PlayProgress()
        {
            currentMission = MISSION_ID.CLASSIC,
            currentLevel = 1,
            maxLevel = 1,
            HighScore = 0
        };

        data.MissionProgress = new List<MissionProgress>();
        data.SettingData = new SettingData();
        data.ListInvetory = new List<BoosterData>();
        data.ListInvetory.Add(new BoosterData()
        {
            ItemID = BOOSTER_ID.SEARCH,
            Num = 2,
            ExpiredTS = -1,
            Extends = ""
        });
        data.ListInvetory.Add(new BoosterData()
        {
            ItemID = BOOSTER_ID.ROCKET,
            Num = 2,
            ExpiredTS = -1,
            Extends = ""
        });
        data.ListInvetory.Add(new BoosterData()
        {
            ItemID = BOOSTER_ID.SHUFFLE,
            Num = 2,
            ExpiredTS = -1,
            Extends = ""
        });
        data.ListInvetory.Add(new BoosterData()
        {
            ItemID = BOOSTER_ID.THEME,
            Num = 2,
            ExpiredTS = -1,
            Extends = ""
        });

        data.ListCheckPointData = new List<CheckPointData>();
        data.ListCheckPointData.Add(new CheckPointData()
        {
            Level = 1,
            InitScore = 0,
            CurrentScore = 0,
            HighScore = 0
        });

        return data;
    }

    #region UTILS

    private static System.Random random = new System.Random();

    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public static bool IsDefault<T>(this T value) where T : struct

    {
        bool isDefault = value.Equals(default(T));
        return isDefault;
    }

    #endregion

    #region Sync Data
    public static bool SyncData(this UserData data)
    {
        bool forceSave = false;

        forceSave = data.SyncDesignData();
        forceSave = data.SyncVersionData();

        return forceSave;
    }

    private static bool SyncVersionData(this UserData data)
    {
        bool forceSave = false;
        while (data.VersionCode <= SaveManager.VERSION_CODE)
        {
            forceSave = true;
            switch (data.VersionCode)
            {
                case 2:
                    if (data._listIAPTracker == null)
                        data._listIAPTracker = new List<IAPTracker>();
                    break;
                case 1:
                    break;
                default:
                    break;
            }
            data.VersionCode++;
        }

        return forceSave;
    }

    private static bool SyncDesignData(this UserData data)
    {
        bool forceSave = false;

        foreach (var m in ResourcesManager.Instance._missionData._listMissions)
        {
            var exists = data.MissionProgress.FirstOrDefault(x => x.missionID == m.ID);
            if (exists == null)
            {
                data.MissionProgress.Add(new MissionProgress()
                {
                    missionID = m.ID,
                    state = MissionState.IsLock,
                    LevelProgress = new List<LevelProgress>()
                });
            }

        }

        return forceSave;
    }



    #endregion

    #region Mission Data

    public static MissionProgress GetMissionProgress(this UserData data, MISSION_ID id)
    {
        var m = data.MissionProgress.FirstOrDefault(x => x.missionID == id);
        if (m != null)
            return m;
        else
        {
            Debug.LogError($"GetMissionProgress error {id}");
            return null;
        }

    }

    public static bool SetMissionProgress(this UserData data, MISSION_ID id, MissionState _state)
    {
        bool success = true;
        var m = data.GetMissionProgress(id);
        if (m == null)
        {
            data.MissionProgress.Add(new MissionProgress()
            {
                missionID = id,
                state = _state,
                LevelProgress = new List<LevelProgress>()
            });

        }
        else
        {
            m.state = _state;
        }

        return success;
    }

    public static LevelProgress GetLevelState(this UserData data, MISSION_ID id, int levelID)
    {
        LevelProgress result = null;

        var m = data.GetMissionProgress(id);
        if (m != null)
        {
            var lvl = m.LevelProgress.FirstOrDefault(x => x.levelID == levelID);
            if (lvl != null)
                result = lvl;
        }

        return result;
    }

    public static bool SaveLevelState(this UserData data, MISSION_ID id, int levelID, LevelState state)
    {
        bool success = false;
        var m = data.GetMissionProgress(id);
        if (m != null)
        {
            var lvl = m.LevelProgress.FirstOrDefault(x => x.levelID == levelID);
            if (lvl == null)
            {
                m.LevelProgress.Add(new LevelProgress()
                {
                    levelID = levelID,
                    state = state
                });
            }
            else
            {
                lvl.state = state;
            }

            success = true;
        }
        else
        {
            Debug.LogError($"CANT GET MISSION {id}");
        }

        return success;
    }

    #endregion

    #region Progress Data
    public static bool SetNextLevel(this UserData data)
    {
        bool success = false;
        List<LevelData> levelList = MissionManager.Instance.GetLevelFromMissionByMissionID(data.UserProgress.currentMission);
        if (data.UserProgress.currentLevel < levelList.Count - 1)
        {
            data.UserProgress.currentLevel++;
            success = true;
        }
        else
        {
            data.UserProgress.currentMission++;
            data.UserProgress.currentLevel = 0;
            success = true;
        }

        return success;
    }

    #endregion

    #region Score
    public static int GetLastCheckPointLevel(this UserData data)
    {
        int result = 0;
        var currentMission = SaveManager.Instance.Data.UserProgress.currentMission;
        result = ResourcesManager.Instance._missionData.GetLastCheckPoint(currentMission, data.UserProgress.currentLevel);
        return result;
    }

    public static CheckPointData GetCurrentCheckPointData(this UserData data)
    {
        CheckPointData result = null;
        int lastCheckPoint = data.GetLastCheckPointLevel();
        result = data.ListCheckPointData.FirstOrDefault(x => x.Level == lastCheckPoint);

        if (result == null)
        {
            var previousCheckPoint = data.ListCheckPointData.FirstOrDefault(x => x.Level < lastCheckPoint);
            if (previousCheckPoint != null)
            {
                result = new CheckPointData()
                {
                    Level = lastCheckPoint,
                    InitScore = previousCheckPoint.CurrentScore,
                    HighScore = previousCheckPoint.HighScore
                };
                data.ListCheckPointData.Add(result);
            }

        }
        return result;

    }

    public static void SetCurrentScore(this UserData data, long score)
    {
        var cpData = data.GetCurrentCheckPointData();
        if (cpData != null)
        {
            cpData.CurrentScore = score;
            if (cpData.CurrentScore > cpData.HighScore)
            {
                cpData.HighScore = cpData.CurrentScore;
            }
        }
    }

    public static bool IsHighScore(this UserData data)
    {
        return data.UserProgress.CurrentScore >= data.UserProgress.HighScore;
    }
    #endregion

    #region Inventory
    public static BoosterData GetBoosterItemCount(this UserData data, BOOSTER_ID id)
    {
        return data.ListInvetory.FirstOrDefault(x => x.ItemID == id);
    }

    public static void SetBooterItem(this UserData data, BOOSTER_ID id, long numItem)
    {
        var booster = data.GetBoosterItemCount(id);
        if (booster != null)
            booster.Num = numItem;
    }

    #endregion

    #region Reward 
    public static bool AddReward(this UserData data, RewardData reward)
    {
        bool result = false;

        switch (reward._rewardID)
        {
            case REWARD_ID.NONE:
                break;
            case REWARD_ID.REMOVE_ADS:
                data.MetaData.IsRemoveAds = true;
                result = true;
                break;
            case REWARD_ID.BOOSTER:
                var boosterID = reward._extends;
                BOOSTER_ID booster = (BOOSTER_ID)Enum.Parse(typeof(BOOSTER_ID), boosterID.Trim());
                data.AddBooster(booster, reward._num);
                result = true;
                break;
            default:
                break;
        }


        return result;
    }

    public static bool AddRewards(this UserData data, List<RewardData> _listRewards)
    {
        bool result = false;

        foreach (var item in _listRewards)
        {
            result = data.AddReward(item);
        }

        return result;
    }

    public static bool AddBooster(this UserData data, BOOSTER_ID id, int numAdd = 1)
    {
        bool success = true;
        var currentNum = data.GetBoosterItemCount(id);
        currentNum.Num += Mathf.Abs(numAdd);
        data.SetBooterItem(id, currentNum.Num);
        return success;
    }
    public static string ToFormatedString(this RewardData data)
    {
        string result = "";
        switch (data._rewardID)
        {
            case REWARD_ID.NONE:
                break;
            case REWARD_ID.REMOVE_ADS:
                result = "Remove Ads";
                break;
            case REWARD_ID.BOOSTER:
                var boosterName = ResourcesManager.GetBoosterName((BOOSTER_ID)Enum.Parse(typeof(BOOSTER_ID), data._extends));
                result = $"+ {data._num} {boosterName}";
                break;
            default:
                break;
        }

        return result;
    }

    #endregion

    #region User Setting

    public static bool OnSound(this SettingData data)
    {
        return data.onBGM && data.onSFX;
    }

    #endregion

    #region IAP TRACKER
    public static void SetIAPTracker(this UserData data, string iapID, long duration, bool forceRefresh = false)
    {
        if (data._listIAPTracker == null)
            data._listIAPTracker = new List<IAPTracker>();

        long targetExpired = duration > 0 ? TimeService.GetCurrentTimeStamp() + duration : -1;

        var finder = data._listIAPTracker.FirstOrDefault(x => x.IAP_ID == iapID);
        if (finder != null)
        {
            if (forceRefresh)
                finder.timeExpired = targetExpired;
        }
        else
        {
            data._listIAPTracker.Add(new IAPTracker()
            {
                IAP_ID = iapID,
                timeExpired = targetExpired
            });
        }
    }

    public static bool SetPurchaseIAP(this UserData data, string offerID)
    {
        if (data._listIAPTracker == null)
            data._listIAPTracker = new List<IAPTracker>();

        var finder = data._listIAPTracker.FirstOrDefault(x => x.IAP_ID == offerID);
        if (finder != null)
        {
            finder.IsPurchased = true;
            return true;
        }

        return false;

    }

    public static bool IsPurchaseAnyPack(this UserData data)
    {
        if (data._listIAPTracker == null)
            data._listIAPTracker = new List<IAPTracker>();

        foreach (var item in data._listIAPTracker)
        {
            if (item.IsPurchased)
                return true;
        }

        return false;
    }

    public static IAPTracker GetIAPTracker(this UserData data, string iapID)
    {
        var finder = data._listIAPTracker.FirstOrDefault(x => x.IAP_ID == iapID);
        return finder;
    }

    #endregion
}

