using Firebase;
using Firebase.Analytics;
using Firebase.RemoteConfig;
using MEC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteConfigManager : SingletonMonoBehaviour<RemoteConfigManager>
{
    public IEnumerator<float> InitializeAsync()
    {
        /*FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                FirebaseApp app = Firebase.FirebaseApp.DefaultInstance;
                Debug.LogError("[Analytics Firebase] Initailize success");
                // Set a flag here to indicate whether Firebase is ready to use by your app.
            }
            else
            {
                      Debug.LogError(System.String.Format("Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });*/
        var op = FirebaseRemoteConfig.DefaultInstance.FetchAndActivateAsync();
        while (!op.IsCompleted)
        {
            if (op.IsFaulted || op.IsCanceled)
            {
                Debug.LogError("[RemoteConfigManager] Initailize Error!!!");
                yield break;
            }
            yield return Timing.WaitForOneFrame;
        }

        if(op.IsCompleted)
        {
            Debug.LogError("[Firebase Remote] Fetched successfully!");
        }

    }

    public ConfigValue GetConfigValue(string key)
    {
        return FirebaseRemoteConfig.DefaultInstance.GetValue(key);
    }

}
