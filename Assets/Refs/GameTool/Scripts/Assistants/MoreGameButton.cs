using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoreGameButton : MonoBehaviour
{
    [SerializeField]
    GameObject anim;

    public int Clicked
    {
        get { return PlayerPrefs.GetInt("MoreGameClick", 0); }
        set { PlayerPrefs.SetInt("MoreGameClick", value); }
    }

    void OnEnable()
    {
        GameRemoteSettings.UpdateLinkAdsCallBack += UpdateMoreGame;

        anim.SetActive(Clicked != 1);

    }

    void OnDisable()
    {
        GameRemoteSettings.UpdateLinkAdsCallBack -= UpdateMoreGame;
    }

    void UpdateMoreGame(string link)
    {
        Clicked = 0;
    }

    public void OnPressMoreGameButton()
    {
#if UNITY_IOS
       
        Application.OpenURL(GameRemoteSettings.Instance.IOSLinkAds);

#elif UNITY_ANDROID
        Debug.Log(GameRemoteSettings.Instance.AndroidLinkAds);
        Application.OpenURL(GameRemoteSettings.Instance.AndroidLinkAds);
#endif
        Clicked = 1;
        anim.SetActive(Clicked != 1);
    }
}
