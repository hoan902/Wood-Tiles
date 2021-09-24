using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Collections.Generic;
using Lean;

public class LevelCompletePanel : SingletonMonoBehaviour<LevelCompletePanel>
{
    [SerializeField]
    Text levelText, scoreText, bestScoreText, timeLeftText, highScoreText;
    int score = 0;

    [Header("Rewards")]
    public RectTransform _rectRewardsParent;
    public RectTransform _rectRewards;

    [Header("Level Marker")]
    public List<RectTransform> _listLevelMarkers;
    //public Text _rowRewardViewPrefab;

    public RewardView _rewardViewPrefab;

    [SerializeField]
    StarView star1, star2, star3;
    //  

    public Slider _checkPointProgress;
    public TextMeshProUGUI _txtLastCheckPoint;
    public TextMeshProUGUI _txtNextCheckPoint;

    private int lastCheckPoint;
    private int NextCheckPoint;
    private List<RewardData> _listRewardData;

    private List<RewardView> _listRewardItems;

    public void SetData(int level, long score, long bestScore, long timeLeft, LevelState levelState, List<RewardData> _listRewards)
    {
        //TO DO: LOGIC CHECK PLAY MAX LEVEL !!!
        _listRewardData = _listRewards;
        levelText.text = level.ToString();
        //this.score = score;
        int _score = 0;
        int _bestScore = 0;
        long _timeLeft = 0;

        DOTween.To(() => _score, x => scoreText.text = x.ToString(), score, 1f);
        DOTween.To(() => _bestScore, x => bestScoreText.text = x.ToString(), bestScore, 1f);
        DOTween.To(() => _timeLeft, x => timeLeftText.text = $"{x.ToString()}s", timeLeft, 1f);

        star3.SetEnable(false, false);
        star2.SetEnable(false, false);
        star1.SetEnable(false, false);

        highScoreText.gameObject.SetActive(SaveManager.Instance.Data.IsHighScore());

        var currentMission = SaveManager.Instance.Data.UserProgress.currentMission;
        lastCheckPoint = ResourcesManager.Instance._missionData.GetLastCheckPoint(currentMission, level);
        NextCheckPoint = ResourcesManager.Instance._missionData.GetNextCheckPoint(currentMission, level);

        _checkPointProgress.interactable = false;
        //_checkPointProgress.maxValue = NextCheckPoint - lastCheckPoint;
        //_checkPointProgress.value = level - lastCheckPoint;
        _checkPointProgress.maxValue = 1.0f;
        var targetMarker = level - lastCheckPoint;
        targetMarker = Mathf.Clamp(targetMarker, 0, 9);
        var targetPos = _listLevelMarkers[targetMarker].anchoredPosition;

        var _targetValue = targetPos.x * 1.0f / _checkPointProgress.GetComponent<RectTransform>().rect.width;
        _checkPointProgress.value = _targetValue;



        _txtLastCheckPoint.text = lastCheckPoint.ToString();
        _txtNextCheckPoint.text = NextCheckPoint > 0 ? NextCheckPoint.ToString() : "Max";

        if (_listRewards != null && _listRewards.Count > 0)
        {
            _rectRewardsParent.gameObject.SetActive(true);
            if (_listRewardItems != null)
            {
                foreach (var item in _listRewardItems)
                {
                    LeanPool.Despawn(item.gameObject);
                }
                _listRewardItems.Clear();
            }
            else
                _listRewardItems = new List<RewardView>();

            foreach (var rwd in _listRewards)
            {
                RewardView view = LeanPool.Spawn<RewardView>(_rewardViewPrefab);
                view.Initialize(rwd);
                view.transform.SetParent(_rectRewards);
                view.transform.localScale = Vector3.one;
                _listRewardItems.Add(view);
            }
        }
        else
        {
            _rectRewardsParent.gameObject.SetActive(false);
        }

        switch (levelState)
        {
            case LevelState.IsThreeStar:

                star1.SetEnable(true, true, 0.3f);
                star2.SetEnable(true, true, 0.3f * 2);
                star3.SetEnable(true, true, 0.3f * 3);

                break;
            case LevelState.IsTwoStar:
                star1.SetEnable(true, true, 0.3f);
                star2.SetEnable(true, true, 0.3f * 2);
                star3.SetEnable(false);

                break;
            case LevelState.IsOneStar:
                star1.SetEnable(true, true, 0.3f);
                star2.SetEnable(false);
                star3.SetEnable(false);
                break;

            default:
                break;
        }
    }

    public void OnButtonNextLevel()
    {
        if (_listRewardData != null)
            SaveManager.Instance.Data.AddRewards(_listRewardData);

        GameMaster.Instance.NextLevel();
    }

    public void OnButtonNextLevelAds()
    {
        if (_listRewardData != null)
            SaveManager.Instance.Data.AddRewards(_listRewardData);

        GameMaster.Instance.NextLevelWithAds();
    }
}
