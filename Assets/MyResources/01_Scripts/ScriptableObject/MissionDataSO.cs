using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[Serializable]
public enum MISSION_ID
{
    CLASSIC = 0,
    EXTEND_01,
    EXTEND_02,
    EXTEND_03,
    EXTEND_04,
    EXTEND_05,
    EXTEND_06
}

[CreateAssetMenu(fileName = "MissionData", menuName = "ScriptableObjects/MissionData", order = 1)]
public class MissionDataSO : ScriptableObject
{
    public List<MissionData> _listMissions;

    public int GetNextCheckPoint(MISSION_ID id, int currentLevel)
    {
        int result = -1;

        var mission = _listMissions.FirstOrDefault(x => x.ID == id);
        if (mission != null)
        {
            var lvl = mission.levels.FirstOrDefault(x => x.IsCheckPoint && x.ID > currentLevel);
            if (lvl != null)
                result = lvl.ID;
        }

        return result;
    }

    public int GetLastCheckPoint(MISSION_ID id, int currentLevel)
    {
        int result = 1;

        var mission = _listMissions.FirstOrDefault(x => x.ID == id);
        if (mission != null)
        {
            var lvl = mission.levels.LastOrDefault(x => x.IsCheckPoint && x.ID <= currentLevel);
            if (lvl != null)
                result = lvl.ID;
        }

        return result;
    }
}


