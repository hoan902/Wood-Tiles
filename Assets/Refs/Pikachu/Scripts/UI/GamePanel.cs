using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventManager;
using UnityEngine.UI;

public class GamePanel : MonoBehaviour
{
    [SerializeField]
    Image timeProgress;
    [SerializeField]
    Text scoreText, resetCountText, levelText, hintText, rocketText, changeThemeText;

    [SerializeField]
    Transform arrow;

    public RectTransform _edgeObject;

    private float _timeProgressWidth;

    public Animation anim;

    private void Start()
    {
        _timeProgressWidth = timeProgress.GetComponent<RectTransform>().rect.width;
    }

    void OnEnable()
    {
        this.RegisterListener(EventID.UpdateTime, (sender, param) => UpdateTime((float)param));
        this.RegisterListener(EventID.UpdateLevel, (sender, param) => UpdateLevel((int)param));
        this.RegisterListener(EventID.UpdateScoreTotal, (sender, param) => UpdateScoreTotal((long)param));
        this.RegisterListener(EventID.UpdateBoosterShuffle, (sender, param) => UpdateShuffleCount((long)param));
        this.RegisterListener(EventID.UpdateBossterSearch, (sender, param) => UpdateSearchCount((long)param));
        this.RegisterListener(EventID.UpdateBoosterRocket, (sender, param) => UpdateRocketCount((long)param));
        this.RegisterListener(EventID.UpdateBoosterTheme, (sender, param) => UpdateChangeThemeCount((long)param));
        this.RegisterListener(EventID.ShowWinEffect, (sender, param) => PlayAnimWin());
    }

    void OnDisable()
    {
        this.RemoveListener(EventID.UpdateTime, (sender, param) => UpdateTime((float)param));
        this.RemoveListener(EventID.UpdateLevel, (sender, param) => UpdateLevel((int)param));
        this.RemoveListener(EventID.UpdateScoreTotal, (sender, param) => UpdateScoreTotal((long)param));
        this.RemoveListener(EventID.UpdateBoosterShuffle, (sender, param) => UpdateShuffleCount((long)param));
        this.RemoveListener(EventID.UpdateBoosterRocket, (sender, param) => UpdateRocketCount((long)param));
        this.RemoveListener(EventID.UpdateBoosterTheme, (sender, param) => UpdateChangeThemeCount((long)param));
        this.RemoveListener(EventID.ShowWinEffect, (sender, param) => PlayAnimWin());
    }

    void UpdateTime(float time)
    {
        timeProgress.fillAmount = time;
        _edgeObject.anchoredPosition = new Vector2(timeProgress.fillAmount * _timeProgressWidth, 0);
    }

    void UpdateScoreTotal(long score)
    {
        scoreText.text = score.ToString("D4");
    }

    void UpdateLevel(int level)
    {
        levelText.text = "Level " + level.ToString();
    }

    void UpdateShuffleCount(long count)
    {
        resetCountText.text = count.ToString();
    }

    void UpdateSearchCount(long count)
    {
        hintText.text = count.ToString();
    }

    public void UpdateRocketCount(long count)
    {
        rocketText.text = count.ToString();
    }
    
    public void UpdateChangeThemeCount(long count)
    {
        changeThemeText.text = count.ToString();
    }

    void Update()
    {
        //arrow.Rotate(new Vector3(0, 0, -5));
    }

    public void OnButtonHint()
    {
        GameController.Instance.Hint();
    }
    public void OnButtonChangeTheme()
    {
        GameController.Instance.ChangeTheme();
    }

    public void OnButtonShuffle()
    {
        GameController.Instance.Swap();
    }

    public void OnButtonRocket()
    {
        GameController.Instance.rkSpawn = gameObject;
        GameController.Instance.RandomRocket();
    }

    public void PlayAnimWin()
    {
        anim.Play("SuccessEffect");
    }
}
