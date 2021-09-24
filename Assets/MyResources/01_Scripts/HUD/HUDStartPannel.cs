using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EventManager;
using QuickType.IAPOffer;
using System;
using System.Linq;

public class HUDStartPannel : UIPanel
{
    public Button buttonPlay;
    public TextMeshProUGUI _txtCurrentLevel;

    [Header("Buttons IAP Offers")]
    public RectTransform _rectIAPOffer;
    public Text _txtTimeLeft;
    public Button _btnVipOffer;
    private bool _firstTimeOpen = true;
    private IAPOfferData _currentIAPOffer = null;
    private void OnEnable()
    {
        PreInit();
    }

    private void Start()
    {
        this.RegisterListener(EventID.UpdateIAPOffer, (sender, param) => UpdateIAPOffer());
        FetchOfferConfig();
    }

    public void FetchOfferConfig()
    {
        if (SaveManager.Instance.Data.IsPurchaseAnyPack())
        {
            this._currentIAPOffer = null;
            return;
        }
        //defaut offer 19.9
        this._currentIAPOffer = ResourcesManager.Instance._listOfferItems.FirstOrDefault(x => x.OfferID == GameConst.IAP_OFFER_19_9);
        try
        {
            //try get 3.99 offer
            var config = RemoteConfigManager.Instance.GetConfigValue("IAP_VIP_80OFF_3_99");
            if (config.Source == Firebase.RemoteConfig.ValueSource.RemoteValue)
            {
                IapOfferConfig iap = IapOfferConfig.FromJson(config.StringValue);
                if (iap != null)
                {
                    bool addOfferSale = false;

                    if (iap.Enable)
                    {
                        var iapTracker = SaveManager.Instance.Data.GetIAPTracker(GameConst.IAP_OFFER_3_99);
                        if (iapTracker == null)
                        {
                            addOfferSale = true;
                        }
                        else if (iapTracker != null)
                        {
                            var curTS = TimeService.GetCurrentTimeStamp();
                            if (curTS < iapTracker.timeExpired)
                            {
                                addOfferSale = true;
                            }
                        }

                        if (addOfferSale)
                        {
                            this._currentIAPOffer = ResourcesManager.Instance._listOfferItems.FirstOrDefault(x => x.OfferID == GameConst.IAP_OFFER_3_99);
                        }
                    }


                }
            }
        }
        catch (Exception ex)
        {

        }

        if (this._currentIAPOffer != null)
        {
            SaveManager.Instance.Data.SetIAPTracker(this._currentIAPOffer.OfferID, this._currentIAPOffer.ExpiredTime);
            _btnVipOffer.image.sprite = this._currentIAPOffer._offerBackground;
            if (this._currentIAPOffer.ExpiredTime > 0)
            {
                if (!SaveManager.Instance.Data.MetaData.IsFirstTimePlay)
                {
                    OnButtonVipOffer();
                }
                else
                {
                    OnButtonPlay();
                    SaveManager.Instance.Data.MetaData.IsFirstTimePlay = false;
                    SaveManager.Instance.SaveData();
                }
            }
            else
            {
                _txtTimeLeft.text = "";
            }
        }

    }

    private void OnDestroy()
    {
        this.RemoveListener(EventID.UpdateIAPOffer, (sender, param) => UpdateIAPOffer());
    }

    public void UpdateIAPOffer()
    {
        //var vipOffer = SaveManager.Instance.Data.GetIAPHistory(GameConst.VIP_OFFER);
        //_btnVipOffer.gameObject.SetActive(vipOffer == null);
        //term turn off offer

        bool isPurchaseAnyPack = SaveManager.Instance.Data.IsPurchaseAnyPack();
        if (isPurchaseAnyPack)
        {
            _rectIAPOffer.gameObject.SetActive(false);
        }
        else
        {
            _rectIAPOffer.gameObject.SetActive(true);
            if (_currentIAPOffer != null)
            {

            }
        }

    }



    public override void PreInit(params object[] args)
    {
        base.PreInit(args);
        int currentLevel = SaveManager.Instance.Data.UserProgress.currentLevel;
        //_txtCurrentLevel.text = $"LEVEL\n{currentLevel}";


        if (!_firstTimeOpen)
        {
            if (AdsLogicController.Instance.CheckShowInterstitial(FLOW_ADS.INTER_HOME))
            {
                AdsManager.instance.ShowAdsInterstitial((complete) =>
                {
                    Debug.Log($"HUDStartPannel show interstitial: {complete}");
                });
            }
        }


        UpdateIAPOffer();
        _firstTimeOpen = false;
    }

    public void RefreshOfferTimer()
    {
        if (_currentIAPOffer != null && _currentIAPOffer.ExpiredTime > 0)
        {
            var tracker = SaveManager.Instance.Data.GetIAPTracker(_currentIAPOffer.OfferID);
            if (tracker != null)
            {
                var diff = tracker.timeExpired - TimeService.GetCurrentTimeStamp();
                if (diff >= 0)
                    _txtTimeLeft.text = TimeService.FormatTimeSpan(diff);
                else
                {
                    _txtTimeLeft.text = "";
                    _currentIAPOffer = null;
                    FetchOfferConfig();
                }

            }

        }
        else
        {
            _txtTimeLeft.text = "";
        }
    }

    public void OnButtonPlay()
    {
        GameMaster.Instance.PlayGameFromLevel();
    }

    public void OnButtonSetting()
    {
        if (AdsLogicController.Instance.CheckShowInterstitial(FLOW_ADS.INTER_SETTING))
        {
            AdsManager.instance.ShowAdsInterstitial((success) =>
            {
                Debug.Log($"Show Interstitial PlayGame: {success}");
                UIManager.Instance.ShowPage("HomeSettingPage");
            });
        }
        else
        {
            UIManager.Instance.ShowPage("HomeSettingPage");

        }

    }

    public void OnButtonVipOffer()
    {

        if (_currentIAPOffer != null)
        {
            TopLayerCanvas.Instance._iapOfferPanel.PreInit(_currentIAPOffer);
            UIManager.Instance.ShowPage("IAPOfferPage");
        }

    }

    private float timerIAP = 0f;
    private void Update()
    {
        timerIAP += Time.deltaTime;
        if (timerIAP >= 1.0f)
        {
            RefreshOfferTimer();
            timerIAP = 0f;
        }
    }
}
