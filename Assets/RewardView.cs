using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardView : MonoBehaviour
{
    public Image _icon;
    public Text _number;

    public bool SetNativeSize;

    public void Initialize(RewardData data)
    {
        _icon.sprite = ResourcesManager.Instance.GetSpriteResource(data._extends.ToUpper().Trim());
        //Debug.LogError(_icon.sprite.name);
        _icon.SetNativeSize();

        //HoanDN icon scale is 0.6 fixed to 1
        
        /*_icon.rectTransform.sizeDelta = Vector2.one * 120;
        if (SetNativeSize || data._extends.Contains("ROCKET"))   //Bug: icon rocket is small in level complete page
        {
            _icon.SetNativeSize();
        }*/
        _number.text = data._num.ToString();

        if (data._rewardID == REWARD_ID.REMOVE_ADS)
        {
            _number.text = "No Ads";
        }
    }
}
