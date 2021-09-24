using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimulateAdsPanel : UIPanel
{
    public Text _txtAdstype;
    public override void PreInit(params object[] args)
    {
        base.PreInit(args);
        ADSTYPE type = (ADSTYPE)args[0];
        _txtAdstype.text = type.ToString();
    }
}
