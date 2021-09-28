using DG.Tweening;
using Firebase;
using Firebase.Analytics;
using MEC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class LoadingPanel : MonoBehaviour
{
    public Text _loadingText;
    public CanvasGroup cg;

    [Header("Runtime Load Assets")]
    public List<AssetReference> _listRunTimeAssets;

    public void Fadeout()
    {
        cg.alpha = 1;
        cg.DOFade(0, 0.3f).SetEase(Ease.Linear).OnComplete(() =>
        {
            this.gameObject.SetActive(false);
        });
    }

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        InitializeGame();
    }

    public void InitializeGame()
    {
        Timing.RunCoroutine(InitializeGameCoroutine());
    }

    public IEnumerator<float> InitializeGameCoroutine()
    {
        if (_listRunTimeAssets != null && _listRunTimeAssets.Count > 0)
        {
            foreach (var asset in _listRunTimeAssets)
            {
                var op = asset.LoadAssetAsync<GameObject>();
                while (!op.IsDone)
                    yield return Timing.WaitForOneFrame;
                if (op.IsDone)
                {
                    GameObject.Instantiate(op.Result);
                }
            }
        }

        yield return Timing.WaitUntilDone(Timing.RunCoroutine(GameMaster.Instance.StartLoadGame()));

        bool waitTask = true;
        GameMaster.Instance.LoadScene(GameConst.SCENE_MAIN, (complete) =>
         {
             waitTask = false;
         });

        while (waitTask)
            yield return Timing.WaitForOneFrame;

        GameMaster.Instance.FinishLoadingProcess();

        yield return Timing.WaitForSeconds(0.1f);
        this.Fadeout();

        //MinhHN: Turn off music background
        //AudioManager.Instance.PlayMusic("bgm01");
    }

}
