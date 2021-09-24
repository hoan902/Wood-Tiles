using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameRemoteSettings : SingletonMonoBehaviour<GameRemoteSettings>
{
    #region Default Value

    // public string VersionDesDeafault = "Updated";
    string AndroidLinkShareDefault = "https://play.google.com/store/apps/developer?id=Wazzion";
    string IOSLinkShareDeafault = "https://itunes.apple.com/app/id1243951873";
    string AndroidLinkAdsDeafault = "https://play.google.com/store/apps/developer?id=Wazzion";
    string IOSLinkAdsDeafault = "https://itunes.apple.com/app/id1243951873";

    string BannerAndroidIDDefault = "ca-app-pub-3429412909861368/8495146009";
    string FullScreenAndroidIDDefault = "ca-app-pub-3429412909861368/5451068310";

    string BannerIOS_IDDefault = "ca-app-pub-3429412909861368/6859392330";
    string FullScreenIOS_IDDefault = "ca-app-pub-3429412909861368/9812858733";

    #endregion

    #region KEY

    // Key get from server
    const string VersionAndroidKEY = "VersionAndroid";
    const string VersionIOSKEY = "VersionIOS";

    const string ANDROID_LINK_SHARE_KEY = "AndroidLinkShare";
    const string IOS_LINK_SHARE_KEY = "IOSLinkShare";
    const string ANDROID_LINK_ADS_KEY = "AndroidLinkAds";
    const string IOS_LINK_ADS_KEY = "IOSLinkAds";

    const string BannerAndroidID_KEY = "BannerAndroidID";
    const string FullScreenAndroidID_KEY = "FullScreenAndroidID";
    const string BannerIOSID_KEY = "BannerIOSID";
    const string FullScreenIOSID_KEY = "FullScreenIOSID";
    const string AdMobAndroidAppID_Key = "AdMobAndroidAppID";
    const string AdMobIOSAppID_KEY = "AdMobIOSAppID";

    const string AppIDIOSKEY = "AppID";
    const string EnableUpdateKEY = "EnableUpdate";

    #endregion


    public static Action<string, string> UpdateVersionCallBack;

    public static Action<string> UpdateLinkAdsCallBack;


    #region Value

    public string AndroidLinkAds
    {
        get { return PlayerPrefs.GetString(ANDROID_LINK_ADS_KEY, AndroidLinkAdsDeafault); }
        set { PlayerPrefs.SetString(ANDROID_LINK_ADS_KEY, value); }
    }

    public string IOSLinkAds
    {
        get { return PlayerPrefs.GetString(IOS_LINK_ADS_KEY, IOSLinkAdsDeafault); }
        set { PlayerPrefs.SetString(IOS_LINK_ADS_KEY, value); }
    }

    public string IOSLinkShare
    {
        get { return PlayerPrefs.GetString(IOS_LINK_SHARE_KEY, IOSLinkShareDeafault); }
        set { PlayerPrefs.SetString(IOS_LINK_SHARE_KEY, value); }
    }

    public string AndroidLinkShare
    {
        get { return PlayerPrefs.GetString(ANDROID_LINK_SHARE_KEY, AndroidLinkShareDefault); }
        set { PlayerPrefs.SetString(ANDROID_LINK_SHARE_KEY, value); }
    }

    public string BannerAndroidID
    {
        get { return PlayerPrefs.GetString(BannerAndroidID_KEY, BannerAndroidIDDefault); }
        set { PlayerPrefs.SetString(BannerAndroidID_KEY, value); }
    }

    public string FullScreenAndroidID
    {
        get { return PlayerPrefs.GetString(FullScreenAndroidID_KEY, FullScreenAndroidIDDefault); }
        set { PlayerPrefs.SetString(FullScreenAndroidID_KEY, value); }
    }

    public string BannerIOSID
    {
        get { return PlayerPrefs.GetString(BannerIOSID_KEY, BannerIOS_IDDefault); }
        set { PlayerPrefs.SetString(BannerIOSID_KEY, value); }
    }

    public string FullScreenIOSID
    {
        get { return PlayerPrefs.GetString(FullScreenIOSID_KEY, FullScreenIOS_IDDefault); }
        set { PlayerPrefs.SetString(FullScreenIOSID_KEY, value); }
    }

    public string AdMobAndroidAppID
    {
        get { return PlayerPrefs.GetString(AdMobAndroidAppID_Key, "ca-app-pub-3429412909861368~7143246322"); }
        set { PlayerPrefs.SetString(AdMobAndroidAppID_Key, value); }
    }

    public string AdMobIOSAppID
    {
        get { return PlayerPrefs.GetString(AdMobIOSAppID_KEY, "ca-app-pub-3429412909861368~5382659135"); }
        set { PlayerPrefs.SetString(AdMobIOSAppID_KEY, value); }
    }


    public string IOSAppID
    {
        get { return PlayerPrefs.GetString(AppIDIOSKEY, "1243951873"); }
        set { PlayerPrefs.SetString(AppIDIOSKEY, value); }
    }


    #endregion

    void Start()
    {
    //    PlayerPrefs.DeleteAll();
        RemoteSettings.Updated +=
            new RemoteSettings.UpdatedEventHandler(HandleRemoteUpdate);
    }



    void HandleRemoteUpdate()
    {
        #region ad
        BannerIOSID = RemoteSettings.GetString(BannerIOSID_KEY,BannerIOS_IDDefault);
        FullScreenIOSID = RemoteSettings.GetString(FullScreenIOSID_KEY,FullScreenIOS_IDDefault);

        BannerAndroidID = RemoteSettings.GetString(BannerAndroidID_KEY,BannerAndroidIDDefault);
        FullScreenAndroidID = RemoteSettings.GetString(FullScreenAndroidID_KEY,FullScreenAndroidIDDefault);

        AdMobAndroidAppID = RemoteSettings.GetString(AdMobAndroidAppID_Key,AdMobAndroidAppID);
        AdMobIOSAppID = RemoteSettings.GetString(AdMobIOSAppID_KEY,AdMobIOSAppID);
        #endregion

        IOSLinkAds = RemoteSettings.GetString(IOS_LINK_ADS_KEY,IOSLinkAdsDeafault);
        IOSLinkShare = RemoteSettings.GetString(IOS_LINK_SHARE_KEY,IOSLinkShareDeafault);


        AndroidLinkShare = RemoteSettings.GetString(ANDROID_LINK_SHARE_KEY,AndroidLinkShareDefault);

        IOSAppID = RemoteSettings.GetString(AppIDIOSKEY);

        bool EnableUpdate = RemoteSettings.GetBool(EnableUpdateKEY, false);





        string VersionKEY = "";
#if UNITY_IOS
        VersionKEY = VersionIOSKEY;
#elif UNITY_ANDROID
        VersionKEY = VersionAndroidKEY;
#endif

        if (Application.version != RemoteSettings.GetString(VersionKEY) && EnableUpdate)
        {

            if (UpdateVersionCallBack != null)
                UpdateVersionCallBack.Invoke(RemoteSettings.GetString(VersionKEY), "");
        }

        if (AndroidLinkAds != RemoteSettings.GetString(ANDROID_LINK_ADS_KEY))
        {
           
            if (UpdateLinkAdsCallBack != null)
                UpdateLinkAdsCallBack.Invoke(AndroidLinkAds);
        }
        AndroidLinkAds = RemoteSettings.GetString(ANDROID_LINK_ADS_KEY,AndroidLinkAdsDeafault);
    }
}
