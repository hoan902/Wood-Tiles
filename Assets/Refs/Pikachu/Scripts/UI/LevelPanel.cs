using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using DG.Tweening;
using UnityEngine.UI;
using Picker;
public class LevelPanel : MonoBehaviour
{
    [SerializeField]
    PickerScrollRect picker;

    [SerializeField]
    GameObject pageLevel, levelItem;


    int itemInPage = 12;

    MISSION_ID currentMission = MISSION_ID.CLASSIC;


    public void OnEnable()
    {
        InitLevelGrid();
    }


    public void InitLevelGrid()
    {
        picker.gameObject.SetActive(false);
        currentMission = MissionManager.Instance.MissionCurrentID;
        var items = picker.content.GetComponentsInChildren<PickerItem>();
        for (int i = 0; i < items.Length; i++)
        {

            //  foreach (Transform item  in items[i].transform)
            //  {
            //   if(item.gameObject.activeSelf)
            //        PoolManager.Pools["Pikachu"].DespawnAll();
            //    }
            Destroy(items[i].gameObject);
        }


        picker.horizontalNormalizedPosition = 0;


        var levels = MissionManager.Instance.GetLevelFromCurrentMission();

        var pageCount = (int)(levels.Count / itemInPage);
        for (int i = 0; i < pageCount + 1; i++)
        {
            var page = Instantiate(pageLevel) as GameObject;
            //   var page = PathologicalGames.PoolManager.Pools["Pikachu"].Spawn(pageLevel) as Transform;
            page.transform.SetParent(picker.content);
            page.transform.localScale = Vector3.one;
            page.transform.localPosition = Vector3.zero;


            for (int j = itemInPage * i; j < itemInPage * (i + 1); j++)
            {
                //    if (j >= itemInPage-1) return;
                var item = levels[j];
                var level = Instantiate(levelItem) as GameObject;
                // var level = PathologicalGames.PoolManager.Pools["Pikachu"].Spawn(levelItem) as Transform;
                level.GetComponent<LevelItem>().SetData(item);
                level.transform.SetParent(page.transform);
                level.transform.localScale = Vector3.one;
                level.transform.localPosition = Vector3.zero;
                if (j >= levels.Count - 1) goto EndofLoop;
            }

        }
        EndofLoop:;
        //picker.ScrollToFirst();
        picker.gameObject.SetActive(true);
    }


}
