using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class LevelFailedPanel : SingletonMonoBehaviour<LevelFailedPanel>
{

    [SerializeField]
    Text levelText, scoreText, bestScoreText, highScoreText;


    public void SetData(string level, long score, long bestScore, bool isTimeUp)
    {
        scoreText.text = score.ToString();
        bestScoreText.text = bestScore.ToString();

        highScoreText.gameObject.SetActive(SaveManager.Instance.Data.IsHighScore());
    }

    public void OnButtonRetry()
    {
        GameMaster.Instance.ReplayLastCheckPoint();
    }

}
