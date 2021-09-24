using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MissionItem : MonoBehaviour
{
    [SerializeField]
    MISSION_ID MissionID;
    [SerializeField]
    Text starText;
    [SerializeField]
    GameObject lockObject, animalObject;
    MissionState missionState;

    void OnEnable()
    {
        StartCoroutine(UpdateStateMission());
    }

    IEnumerator UpdateStateMission()
    {
        yield return new WaitForEndOfFrame();
        missionState =  MissionManager.Instance.GetMissionState(MissionID);
        if(lockObject!=null)
            lockObject.SetActive(missionState == MissionState.IsLock);
        if(animalObject!=null)
            animalObject.SetActive(missionState == MissionState.IsOpen);
        starText.text = MissionManager.Instance.GetStarMission(MissionID);
    }

    public void OnPressMissionItem()
    {
        if (missionState == MissionState.IsLock)
            return;
        MissionManager.Instance.MissionCurrentID = MissionID;
        UIManager.Instance.ShowPage("LevelPage");
       // PanelMgr.Instance.GetPanel<LevelPanel>().InitLevelGrid();
    }


}
