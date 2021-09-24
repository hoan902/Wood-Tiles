using System;
using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;

#region  LEVEL DATA
[Serializable]
public enum ENUM_TILE
{
    NONE = 0,
    LION,
    FOX,
    FISH,
    ALPACA,
    PIG,
    PANDA,
    DOG,
    CAT,
    SNAKE,
    ELEPHANT,
    DUCK,
    BIRD,
    WEASEL,
    JELLY_FISH,
    DRAGON,
    SQUID,
    CHAMELEON,
    HAMSTER,
    PENGUIN,
    LADY_BUG,
    FIREWORK,
    CELL_2021,
    ICE_CREAM,
    SUN_GLASSES,
    FEMALE_SWIMWEAR,
    FLIP_FLOP,
    WATERMELON,
    ID28,
    ID29,
    ID30,
    ID31,
    ID32,
    /*ID33,
    ID34,
    ID35,
    ID36,
    ID37,
    ID38,
    ID39,
    ID40,
    ID41,
    ID42,
    ID43,
    ID44,
    ID45,
    ID46,
    ID47,
    ID48,
    ID49,
    ID50,
    ID51,
    ID52,
    ID53,
    ID54,
    ID55,
    ID56,
    ID57,
    ID58,
    ID59,
    ID60,
    ID61,
    ID62,
    ID63,
    ID64,
    ID65,
    ID66,
    ID67,
    ID68,
    ID69,
    ID70,
    ID71,
    ID72,
    ID73,
    ID74,
    ID75,
    ID76,
    ID77*/
}

[Serializable]
public class TileData
{
    public ENUM_TILE Tile;
    public int Number;
}

[Serializable]
public class LevelInfoData
{
    public int Level;
    public int _time;
    public int _totalTile;
    public List<TileData> _listTileData;
    public List<RewardData> _listRewardData;

 
}

#endregion

[Serializable()]
public class GameData
{
    public List<ScoreData> scoreData = new List<ScoreData>();
}

[Serializable]
public class ScoreData
{
    public string dateTime { get; set; }
    public int score { get; set; }
    public int number { get; set; }

    public ScoreData(string date, int score, int number)
    {
        this.dateTime = date;
        this.score = score;
        this.number = number;
    }
}

public enum REWARD_ID
{
    NONE,
    REMOVE_ADS,
    BOOSTER
}

[Serializable]
public class RewardData
{
    public REWARD_ID _rewardID;
    public string _extends;
    public int _num;

    public RewardData()
    {

    }

    public RewardData(REWARD_ID id)
    {
        _rewardID = id;
        _extends = "";
        _num = 1;
    }
}
