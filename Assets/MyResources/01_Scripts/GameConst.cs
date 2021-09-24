using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameConst
{
    public static long SEC_REVIVE = 90;    //Minh.HN set 110 to 60 ||13/9/2021 HoanDN set 60 to 90
    public static long ADS_NEXT_LEVEL_BONUS = 30;
    public static float SPRITE_SCALE = 2.0f;

    public static string SCENE_MAIN = "MainCloned";
    public static string VIP_OFFER = "VIP_OFFER";

    public static string IAP_OFFER_19_9 = "IAP_OFFER_19_9";
    public static string IAP_OFFER_3_99 = "IAP_OFFER_3_99";
}

public static class GameConfig
{
    public static int MIN_DURATION_INTERSTITIAL_VS_REWARD = 30;
    public static int MIN_DURATION_INTERSTITIAL = 60;
    public static int MAX_BOOSTER_ADS_PER_DAY = 5;
    public static int MAX_REVIVE_ADS = 5;
    public static int REVIVE_ADS_BONUS_TIME = 100;
    public static int NEXT_LEVEL_ADS_BONUS = 30;
}