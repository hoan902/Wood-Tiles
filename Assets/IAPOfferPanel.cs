using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

[Serializable]
public class IAPOfferData
{
    public string OfferID;
    public string OfferTitle;
    public List<RewardData> _listRewards;
    public float PercentOff;
    public long ExpiredTime;

    public string android_iap_id;
    public string ios_iap_id;
    public bool IsSaleOff;

    [Header("Visual")]
    public Sprite _offerBackground;
}

public class IAPOfferPanel : UIPanel
{
    public RewardView _offerItemPrefab;
    public Text _txtOldPrice;
    public Text _txtCurrentPrice;
    public Text _txtPercentOff;
    public Text _txtTimeLeft;

    public RectTransform _rectRewards;
    public GameObject _saleIcon;

    private bool isInited = false;
    private List<RewardView> _listRewardViews;
    private IAPOfferData Data;
    private float timer = 0f;

    private IAPTracker _iapTracker;


    public override void PreInit(params object[] args)
    {
        base.PreInit(args);

        if (_listRewardViews == null)
            _listRewardViews = new List<RewardView>();


        Data = (IAPOfferData)(args[0]);
        _iapTracker = SaveManager.Instance.Data.GetIAPTracker(Data.OfferID);

        if (Data != null)
        {
            _txtPercentOff.text = $"{Data.PercentOff}%\n OFF";
            _txtCurrentPrice.text = IAPManager.instance.GetProductPrice(Data.android_iap_id);
            decimal oldPrice = IAPManager.instance.GetProductPriceValue(Data.android_iap_id) * 100 / (decimal)(100 - Data.PercentOff);

            Debug.LogError($"old price is {oldPrice}");
            string oldStringText = $"{IAPManager.FormatPriceString(oldPrice)}";

            _txtOldPrice.text = oldStringText;

            UpdateTimeLeft();

            if (Data._listRewards != null && Data._listRewards.Count > 0 && (_listRewardViews == null || _listRewardViews.Count <= 0))
            {
                foreach (var item in Data._listRewards)
                {
                    var offerItem = Lean.LeanPool.Spawn<RewardView>(_offerItemPrefab, Vector3.zero, Quaternion.identity);
                    if (offerItem != null)
                    {
                        offerItem.transform.SetParent(_rectRewards);
                        offerItem.transform.localPosition = Vector3.zero;
                        offerItem.transform.localRotation = Quaternion.identity;
                        offerItem.transform.localScale = Vector3.one;
                        offerItem.Initialize(item);
                        _listRewardViews.Add(offerItem);
                    }
                }
            }

            if (Data.IsSaleOff)
            {
                _saleIcon.gameObject.SetActive(true);
                _txtOldPrice.gameObject.SetActive(true);
                _txtTimeLeft.gameObject.SetActive(true);
            }
            else
            {
                _saleIcon.gameObject.SetActive(false);
                _txtOldPrice.gameObject.SetActive(false);
                _txtTimeLeft.gameObject.SetActive(false);
            }
        }
    }

    private void Update()
    {
        if (Data != null)
        {
            timer += Time.deltaTime;
            if (timer >= 1.0f)
            {
                UpdateTimeLeft();
                timer = 0f;
            }
        }
    }

    public void UpdateTimeLeft()
    {
        if (_iapTracker != null && _iapTracker.timeExpired > 0)
        {
            var diff = _iapTracker.timeExpired - TimeService.GetCurrentTimeStamp();
            if (diff > 0)
            {
                _txtTimeLeft.text = TimeService.FormatTimeSpan(diff);
            }
            else
            {
                _txtTimeLeft.text = TimeService.FormatTimeSpan(0);
            }
        }
        else
        {
            _txtTimeLeft.text = "";
        }
    }

    public void OnPurchase()
    {
        //TODO : LOGIC IAP
        GameMaster.Instance.PurChaseIAP(Data.android_iap_id, (finish) =>
         {
             if (finish)
             {
                 SaveManager.Instance.Data.SetPurchaseIAP(Data.OfferID);
                 var success = SaveManager.Instance.Data.AddRewards(Data._listRewards);
                 if (success)
                 {
                     DialogMessage.Instance.OpenDialog("Congratulation", $"Purchase complete", () =>
                     {
                         OnButtonClose();
                     });
                 }

                 GameMaster.Instance.ResetIAPItems();
             }
             else
             {
                 OnButtonClose();
             }
         });


    }

    public void OnButtonClose()
    {
        UIManager.Instance.ShowPage("StartPage");
    }
}
