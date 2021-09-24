using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.AddressableAssets;

public class MissionManager : SingletonMonoBehaviour<MissionManager>
{
    const string settingPath = "easysave2.txt";
    //public List<MissionData> Missions = new List<MissionData>();

    const string missionKey = "mission";
    const string levelKey = "level";

    [HideInInspector]
    public MISSION_ID MissionCurrentID;
    [HideInInspector]
    public int LevelCurrentID;

    void Awake()
    {
        //Missions = CreateMission(MissionsOrigin);
    }

    //List<MissionData> CreateMission(List<MissionData> origin)
    //{
    //    List<MissionData> result = new List<MissionData>();

    //    for (int i = 0; i < origin.Count; i++)
    //    {
    //        MissionData missionOrigin = origin[i];
    //        result.Add(new MissionData());

    //        int index = 0;
    //        for (int j = 0; j < missionOrigin.levels.Count; j++)
    //        {
    //            LevelData level = missionOrigin.levels[j];
    //            for (int k = 0; k < 7; k++)
    //            {
    //                LevelData newLevel = new LevelData();
    //                newLevel.moveMode = (MoveMode)k;
    //                newLevel.ID = index;
    //                newLevel.MissionID = i;
    //                newLevel.width = level.width;
    //                newLevel.height = level.height;
    //                index++;
    //                result[i].levels.Add(newLevel);
    //            }
    //        }
    //        result[i].ID = i;
    //    }
    //    return result;
    //}

    public List<LevelData> GetLevelFromCurrentMission()
    {
        //var result = Missions[MissionCurrentID].levels;
        //return result;
        var data = ResourcesManager.Instance.GetMissionByID((MISSION_ID)MissionCurrentID);
        if (data != null)
        {
            return data.levels;
        }
        else
        {
            Debug.LogError($"GetLevelFromCurrentMission error !!! {MissionCurrentID}");
            return null;
        }

    }

    public List<LevelData> GetLevelFromMissionByMissionID(MISSION_ID missionId)
    {
        //if (missionId < Missions.Count)
        //    return Missions[missionId].levels;
        //return null;

        var mission = ResourcesManager.Instance.GetMissionByID(missionId);
        if (mission != null)
        {
            return mission.levels;
        }
        else
        {
            return null;
        }
    }

    public LevelData GetCurrentLevelData()
    {
        LevelCurrentID = SaveManager.Instance.Data.UserProgress.currentLevel;
        MissionCurrentID = SaveManager.Instance.Data.UserProgress.currentMission;
        return GetLevelData(MissionCurrentID, LevelCurrentID);
    }

    public LevelData GetLevelData(MISSION_ID MissionCurrentID, int LevelCurrentID)
    {
        LevelData level = null;
        var levelList = ResourcesManager.Instance._missionData._listMissions.Find(x => x.ID == (MISSION_ID)MissionCurrentID).levels;
        level = levelList.Find(x => x.ID == LevelCurrentID);
        level.IsCustomLevel = false;

        //for custom level
        string fileName = ResourcesManager.GetLevelFileName(MissionCurrentID, LevelCurrentID);
        var textFile = FileUtils.GetTextAsset($"LevelDesigns/{fileName}");

        bool overwriteMatrix = false;
        if (textFile != null)
        {
            level.IsCustomLevel = true;
            int width = 0;
            int height = 0;
            int themeId = 0;
            bool isLoop = false;
            bool isMagnet = true;
            //add theme HoanDN
            var maxtrix = ResourcesManager.Instance.ParseLevelDesign(textFile, ref width, ref height, ref themeId, ref isLoop, ref isMagnet);
            if (maxtrix != null)
            {
                level.MatrixMove = maxtrix;
                level.Width = width;
                level.Height = height;
                //Add theme HoanDN
                level.ThemeId = themeId;
                level.IsLoop = isLoop;
                level.IsMagnet = isMagnet;
                overwriteMatrix = true;
                var debugStr = "";
                for (int i = level.Height - 1; i >= 0; i--)
                {
                    for (int j = 0; j < level.Width; j++)
                    {
                        debugStr += $"{(int)maxtrix[j, i]}\t";
                    }
                    debugStr += "\n";
                }

                //Debug.Log(debugStr);
            }
        }

        if (!overwriteMatrix)
        {
            level.IsCustomLevel = false;
           
            level.MatrixMove = new MoveMode[level.Width, level.Height];
            for (int i = 0; i < level.Width; i++)
            {
                for (int j = 0; j < level.Height; j++)
                {
                    level.MatrixMove[i, j] = level.GlobalMoveMode;
                }
            }
        }
        return level;
    }


    public void FinishLevel()
    {
        List<LevelData> levelList = GetLevelFromCurrentMission();
        if (LevelCurrentID < levelList.Count - 1)
        {
            SaveLevelState(LevelCurrentID + 1, MissionCurrentID, LevelState.IsOpen);
        }
        else
        {
            SaveMissionState((MISSION_ID)(MissionCurrentID + 1), MissionState.IsOpen);
        }
    }

    public string GetStarMission(MISSION_ID missionID)
    {
        List<LevelData> levelList = GetLevelFromMissionByMissionID(missionID);
        int total = 0;
        if (levelList == null)
            return "";
        foreach (var item in levelList)
        {
            LevelState levelState = GetLevelState(item.ID, missionID);
            switch (levelState)
            {
                case LevelState.IsLock:
                    break;
                case LevelState.IsOpen:
                    break;
                case LevelState.IsOneStar:
                case LevelState.IsTwoStar:
                case LevelState.IsThreeStar:
                    total += ((int)levelState - 1);
                    break;
                default:
                    break;
            }


        }
        return total + "/" + levelList.Count * 3;
    }

    public void NextLevel()
    {
        //List<LevelData> levelList = GetLevelFromCurrentMission();
        //if (LevelCurrentID < levelList.Count - 1)
        //{
        //    LevelCurrentID++;

        //    SaveManager.Instance.Data.UserProgress.
        //}
        //else
        //{
        //    MissionCurrentID++;
        //    LevelCurrentID = 0;
        //}
        if (SaveManager.Instance.Data.SetNextLevel())
        {
            MissionCurrentID = SaveManager.Instance.Data.UserProgress.currentMission;
            LevelCurrentID = SaveManager.Instance.Data.UserProgress.currentLevel;
        }
    }

    public bool isWin()
    {
        //if (Missions.Count - MissionCurrentID >= 2)
        //    return false;
        //if (Missions[MissionCurrentID].levels.Count - LevelCurrentID >= 2)
        //    return false;
        return true;
    }

    public void SaveLevelState(int levelID, MISSION_ID missionID, LevelState state)
    {
        var levelState = GetLevelState(levelID, missionID);
        if (state < levelState) return;
        string key = missionID + "" + levelID;
        //PlayerPrefs.SetInt(key, (int)state);

        SaveManager.Instance.Data.SaveLevelState(missionID, levelID, state);

    }

    public LevelState GetLevelState(int levelID, MISSION_ID missionID)
    {
        //string key = missionID + "" + levelID;
        //int defaultValue = levelID == 0 ? (int)LevelState.IsOpen : (int)LevelState.IsLock;
        //var value = PlayerPrefs.GetInt(key, defaultValue);
        LevelState result = LevelState.IsLock;
        var lvlData = SaveManager.Instance.Data.GetLevelState(missionID, levelID);
        if (levelID == 0)
        {
            result = (lvlData == null) ? LevelState.IsOpen : lvlData.state;
        }
        else
        {
            result = (lvlData == null) ? LevelState.IsLock : lvlData.state;
        }
        return result;
    }

    public MissionState GetMissionState(MISSION_ID MissionID)
    {
        //if ((int)MissionID == 0 || (int)MissionID == 1)
        //    return MissionState.IsOpen;
        //string key = missionKey + MissionID;
        //int defaultValue = MissionID == 0 || (int)MissionID == 1 ? (int)LevelState.IsOpen : (int)LevelState.IsLock;
        //try
        //{
        //    var value = PlayerPrefs.GetInt(key, defaultValue);
        //    return (MissionState)value;
        //}
        //catch (System.Exception)
        //{
        //    return MissionState.IsLock;
        //

        if ((int)MissionID == 0 || (int)MissionID == 1)
            return MissionState.IsOpen;
        MissionState result = MissionState.IsLock;
        var m = SaveManager.Instance.Data.GetMissionProgress(MissionID);

        result = (m != null) ? m.state : MissionState.IsLock;

        return result;

    }

    public void SaveMissionState(MISSION_ID MissionID, MissionState state)
    {
        //string key = missionKey + MissionID;
        //PlayerPrefs.SetInt(key, (int)state);

        SaveManager.Instance.Data.SetMissionProgress(MissionID, state);
    }


}
