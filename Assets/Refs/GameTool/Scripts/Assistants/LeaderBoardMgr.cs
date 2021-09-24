using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms;

#if UNITY_ANDROID
//using GooglePlayGames;
#endif

#if UNITY_IOS
//using UnityEngine.SocialPlatforms.GameCenter;
#endif

public class LeaderBoardMgr : SingletonMonoBehaviour<LeaderBoardMgr>
{
#if UNITY_ANDROID

    void Start()
    {
        //PlayGamesPlatform.Activate();
        //if (Utilities.IsInternetAvailable())
        //{
        //    Login();
        //}
    }

    void Login()
    {
        ////login
        //Social.localUser.Authenticate((bool success) =>
        //    {
        //        if (success)
        //        {
        //            DialogMessage.Instance.OpenDialog("Google Play Services", "You've successfully logged in", null);
        //            Debug.Log("You've successfully logged in");
        //        }
        //        else
        //        {
        //            DialogMessage.Instance.OpenDialog("Google Play Services", "Login failed for some reason", null);
        //            Debug.Log("Login failed for some reason");
        //        }
        //    });
    }

    public void PostHighScoreToLeaderBoardAndroid(long hiScore)
    {
        //if (Social.localUser.authenticated)
        //{
        //    Social.ReportScore(hiScore, GPGSIds.leaderboard_pikachu_2018__onet_connect_deluxe, (bool success) =>
        //        {
        //            if (success)
        //            {
        //                Debug.Log("post hi score sucess");
        //            }
        //            else
        //            {
        //                Debug.Log("fail post hi score");
        //            }
        //        });
        //}

    }

    public void ShowLeaderBoardAndroid()
    {
        //if (!Utilities.IsInternetAvailable())
        //{
        //    DialogMessage.Instance.OpenDialog("Google Play Services", "No Internet", null);
        //    return;
        //}
        //if (Social.localUser.authenticated)
        //{
        //    //PlayGamesPlatform.Instance.ShowLeaderboardUI(GPGSIds.leaderboard_pikachu_2018__onet_connect_deluxe);
        //}
        //else
        //{
        //    DialogMessage.Instance.OpenDialog("Google Play Services", "Not available. Try again", "Cancel", "Try", Login, null);
        //}
    }

#endif

    public void PostHighScoreToLeaderBoard(long hiScore)
    {
        //#if UNITY_ANDROID
        //        PostHighScoreToLeaderBoardAndroid(hiScore);
        //#elif UNITY_IOS
        //        ReportScoreIOS (hiScore);
        //#endif
    }

    public void ShowLeaderBoard()
    {
        //#if UNITY_ANDROID
        //        ShowLeaderBoardAndroid();
        //#elif UNITY_IOS
        //		ShowLeaderboardIOS ();
        //#endif
    }

    //#if UNITY_IOS
    //    string leaderboard_number = "taptomerge_leaderboard";
    //    string leaderboard_score = "taptomerge_score";

    //	void Start ()
    //	{
    //		AuthenticateToGameCenter ();
    //	}

    //	public void AuthenticateToGameCenter ()
    //	{
    //		Social.localUser.Authenticate (success => {
    //			if (success) {
    //				Debug.Log ("Authentication successful");
    //			} else {
    //				Debug.Log ("Authentication failed");
    //			}
    //		});
    //	}

    //	public void ReportScoreIOS (int hiScore)
    //	{
    //        Social.ReportScore (number, leaderboard_number, success => {
    //			if (success) {
    //				Debug.Log ("Reported score successfully");
    //			} else {
    //				Debug.Log ("Failed to report score");
    //			}
    //		});


    //	}

    //	//call to show leaderboard
    //	public void ShowLeaderboardIOS ()
    //	{
    //        if (Social.localUser.authenticated)
    //        {
    //            Social.ShowLeaderboardUI();
    //        }
    //        else
    //        {
    //    DialogMessage.Instance.OpenDialog("Game Center Unavailable", "Player is not signed in ", null);
    //        }
    //	}
    //#endif

}