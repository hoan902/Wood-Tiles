using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public enum EnumHUD
{
    NONE,
    HUD_LOADING,
    HUD_SIMULATE_ADS
}

[Serializable]
public class EnumPanel
{
    public EnumHUD enumHUD;
    public UIPanel panel;
}

public class TopLayerCanvas : SingletonMonoBehaviour<TopLayerCanvas>
{
    [Header("HUD List!!!")]
    public List<EnumPanel> _listPanels;

    public IAPOfferPanel _iapOfferPanel;

    public RectTransform bannerAds;

    public void ShowHUD(EnumHUD hud, params object[] _params)
    {
        var target = _listPanels.FirstOrDefault(x => x.enumHUD == hud);
        if (target != null)
        {
            target.panel.PreInit(_params);
            target.panel.SetVisible(true);
        }
    }

    public void HideHUD(EnumHUD hud)
    {
        var target = _listPanels.FirstOrDefault(x => x.enumHUD == hud);
        if (target != null)
        {
            target.panel.SetVisible(false);
        }

    }

    public void EnableSimuateAdsBanner(bool enable)
    {
        bannerAds.gameObject.SetActive(enable);
    }
}
