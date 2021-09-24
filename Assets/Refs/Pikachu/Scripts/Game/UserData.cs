using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class UserData
{
    public MetaData MetaData;
    public PlayProgress UserProgress;
    public List<MissionProgress> MissionProgress;
    public List<BoosterData> ListInvetory;
    public SettingData SettingData;
    public int VersionCode;
    public List<CheckPointData> ListCheckPointData;
    public List<IAPTracker> _listIAPTracker;
}

[Serializable]
public class IAPTracker
{
    public string IAP_ID;
    public long timeExpired;
    public bool IsPurchased;

}

[Serializable]
public class CheckPointData
{
    public int Level;
    public long InitScore;
    public long CurrentScore;
    public long HighScore;

    public void ResetDefault()
    {
        this.CurrentScore = InitScore;
    }
}

[Serializable]
public class PlayProgress
{
    public MISSION_ID currentMission;
    public int currentLevel;
    public int maxLevel;
    public long HighScore;
    public long CurrentScore;
}


[Serializable]
public class MetaData
{
    public string UserID;
    public string UserName;
    public bool IsRemoveAds;
    public bool IsFirstTimePlay = true;
}

[Serializable]
public class SettingData
{
    public bool onSFX;
    public bool onBGM;
    public bool onVibration;
    public bool onNotification;
    public string Language;

    public SettingData()
    {
        onSFX = true;
        onBGM = true;
        onVibration = true;
        onNotification = true;
        Language = "en";
    }
}

[Serializable]
public class BoosterData
{
    public BOOSTER_ID ItemID;
    public long Num;
    public string Extends;
    public long ExpiredTS;
}

[Serializable]
public enum BOOSTER_ID
{
    NONE = 0,
    SEARCH,
    SHUFFLE,
    ROCKET,
    //Add THEME HoanDN
    THEME,
}

[Serializable]
public class MissionProgress
{
    public MISSION_ID missionID;
    public MissionState state;
    public List<LevelProgress> LevelProgress;

}

[Serializable]
public class LevelProgress
{
    public LevelState state;
    public int levelID;
}