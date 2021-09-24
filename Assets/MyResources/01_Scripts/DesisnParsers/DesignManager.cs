using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using QuickType.LevelTile;
using QuickType.LevelReward;
using System.Linq;
using UnityEngine.AddressableAssets;

public class DesignManager : SingletonMonoBehaviour<DesignManager>
{
    public static string DESIGN_PATH = "Designs/";

    public LevelTileDesign LevelTileDesign { get; private set; }
    public LevelRewardDesign LevelRewardDesign { get; private set; }

    public IEnumerator<float> InitalizeCoroutine()
    {
        bool waitingTask = true;
        //LevelTileDesign
        Addressables.LoadAssetsAsync<TextAsset>("LevelTileDesign", (jsonFile) =>
        {
            if (jsonFile)
            {
                LevelTileDesign = LevelTileDesign.FromJson(jsonFile.text);
            }
            else
            {
                Debug.LogError("Cant Find file {LevelTileDesign}");
            }
            waitingTask = false;
        });

        while (waitingTask)
            yield return Timing.WaitForOneFrame;

        //level reward design
        waitingTask = true;
        Addressables.LoadAssetsAsync<TextAsset>("LevelRewardDesign", (jsonFile) =>
        {

            if (jsonFile)
            {
                LevelRewardDesign = LevelRewardDesign.FromJson(jsonFile.text);
            }
            else
            {
                Debug.LogError("Cant Find file {LevelRewardDesign}");
            }
            waitingTask = false;
        });

        yield return Timing.WaitForOneFrame;

    }

}

public static class DesignHelper
{
    public static Dictionary<string, int> GetLevelTileDesign(int level)
    {
        Dictionary<string, int> result = null;
        for (int i = 0; i < DesignManager.Instance.LevelTileDesign.LevelTileDesignLevelTileDesign.Count; i++)
        {
            var row = DesignManager.Instance.LevelTileDesign.LevelTileDesignLevelTileDesign[i];
            var lvl = 0;
            row.TryGetValue("LevelID", out lvl);
            if (lvl == level)
            {
                result = row;
                break;
            }

        }

        return result;
    }

    public static LevelRewardDesignElement GetLevelRewardDesign(int level)
    {
        LevelRewardDesignElement design = null;

        design = DesignManager.Instance.LevelRewardDesign.LevelRewardDesignLevelRewardDesign.FirstOrDefault(x => x.LevelId == level);
        return design;
    }

    public static LevelInfoData GetLevelInfoData(int level)
    {
        LevelInfoData result = new LevelInfoData();
        result._listTileData = new List<TileData>();
        result.Level = level;
        result._time = 180;
        var tileData = GetLevelTileDesign(level);
        if (tileData != null)
        {
            int time = tileData["Time"];
            result._time = time;
            foreach (ENUM_TILE tile in Enum.GetValues(typeof(ENUM_TILE)))
            {
                if ((int)tile == 0)
                    continue;
                var tileStr = ((int)tile).ToString();
                int countTile = 0;
                tileData.TryGetValue(tileStr, out countTile);
                if (countTile > 0)
                {
                    result._listTileData.Add(new TileData()
                    {
                        Tile = tile,
                        Number = countTile
                    });

                    result._totalTile += countTile;
                }
            }
        }

        var rewardData = GetLevelRewardDesign(level);
        if (rewardData != null)
        {
            result._listRewardData = new List<RewardData>();
            foreach (var rwd in rewardData.Rewards)
            {
                if (rwd.RewardId.Contains("SEARCH") ||
                    rwd.RewardId.Contains("SHUFFLE") ||
                    rwd.RewardId.Contains("ROCKET") ||
                    rwd.RewardId.Contains("THEME"))
                {
                    result._listRewardData.Add(new RewardData()
                    {
                        _rewardID = REWARD_ID.BOOSTER,
                        _extends = rwd.RewardId,
                        _num = rwd.Num
                    });
                }
            }
        }

        return result;
    }

    public static int CountLevelEmptyCell(this LevelData data)
    {
        int result = 0;
        for (int i = 0; i < data.Width; i++)
        {
            for (int j = 0; j < data.Height; j++)
            {
                if (data.MatrixMove[i, j] == 0)
                    ++result;
            }
        }

        return result;
    }
}