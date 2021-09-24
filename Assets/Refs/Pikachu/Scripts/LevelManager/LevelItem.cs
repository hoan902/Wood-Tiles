using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelItem : MonoBehaviour
{
    // public UnityEvent onClick = new UnityEvent();
    [SerializeField]
    GameObject start1, start2, start3;
    [SerializeField]
    Sprite lockImage, currentImage,passImage;
    [SerializeField]
    Image imageBG;

    [SerializeField]
    Text levelText;
    LevelData level;
    LevelState levelState;

    Color lockColor = new Color(240f/255f,240f/255f,240f/255f);
    Color passColor = new Color(107f/255f,242f/255f,209f/255f);
    Color currentColor = new Color(166f/255f,255f/255f,230f/255f);
    public void OnClick()
    {
        //onClick.Invoke();
        if (levelState == LevelState.IsLock)
            return;
        MissionManager.Instance.LevelCurrentID = level.ID;
        GameMaster.Instance.isPlay = true;
        GameMaster.Instance.PlayGameFromLevel();
    }

    public void SetData(LevelData level)
    {
        levelText.text = (level.ID +1).ToString("D2");
        this.level = level;
        UpdateState();
    }

    void OnEnable()
    {
        UpdateState();
    }


    void UpdateState()
    {
        if (level == null) return;
        levelState = MissionManager.Instance.GetLevelState(level.ID,level.MissionID);
       // levelState = LevelState.IsThreeStar;

        start3.SetActive(false);
        start2.SetActive(false);
        start1.SetActive(false);
       
        levelText.gameObject.SetActive(true);
        levelText.color = passColor;
        switch (levelState)
        {
            case LevelState.IsOpen:
                imageBG.overrideSprite = currentImage;
                levelText.color = currentColor;
                break;
            
            case LevelState.IsLock:
                imageBG.overrideSprite = lockImage;
                levelText.color = lockColor;
               // levelText.gameObject.SetActive(false);
 
                break;
            case LevelState.IsThreeStar:
                imageBG.overrideSprite = passImage;
                start3.SetActive(true);
                start2.SetActive(true);
                start1.SetActive(true);
                break;
            case LevelState.IsTwoStar:
                imageBG.overrideSprite = passImage;
               
                start3.SetActive(false);
                start2.SetActive(true);
                start1.SetActive(true);
                break;
            case LevelState.IsOneStar:
                imageBG.overrideSprite = passImage;
                start3.SetActive(false);
                start2.SetActive(false);
                start1.SetActive(true);
                break;
          
            default:
                break;
        }

    }

}
