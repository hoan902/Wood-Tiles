using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameDataMgr
{
    const string HighScorekey = "HighScoreKEY";

    public static long HighScore {
        get { return SaveManager.Instance.Data.UserProgress.HighScore; }
        set {
            if (value > SaveManager.Instance.Data.UserProgress.HighScore)
            {
                SaveManager.Instance.Data.UserProgress.HighScore = value;
            }
        }
    }

    public static long CurrentScore {

        get { return SaveManager.Instance.Data.UserProgress.CurrentScore; }
        set {
            SaveManager.Instance.Data.UserProgress.CurrentScore = value;
        }
    }


    const string Hintkey = "HintKEY";

    public static long BoosterSearch {
        get { return SaveManager.Instance.Data.GetBoosterItemCount(BOOSTER_ID.SEARCH).Num; }
        set {
            if (value < 0)
                value = 0;
            SaveManager.Instance.Data.SetBooterItem(BOOSTER_ID.SEARCH, value);
        }
    }

    public static long BoosterShuffle {
        get { return SaveManager.Instance.Data.GetBoosterItemCount(BOOSTER_ID.SHUFFLE).Num; }
        set {
            if (value < 0)
                value = 0;
            SaveManager.Instance.Data.SetBooterItem(BOOSTER_ID.SHUFFLE, value);
        }
    }

    public static long BoosterRocket {
        get { return SaveManager.Instance.Data.GetBoosterItemCount(BOOSTER_ID.ROCKET).Num; }
        set {
            if (value < 0)
                value = 0;
            SaveManager.Instance.Data.SetBooterItem(BOOSTER_ID.ROCKET, value);
        }
    }

    public static long BoosterTheme {
        get { return SaveManager.Instance.Data.GetBoosterItemCount(BOOSTER_ID.THEME).Num; }
        set {
            if (value < 0)
                value = 0;
            SaveManager.Instance.Data.SetBooterItem(BOOSTER_ID.THEME, value);
        }
    }


}
