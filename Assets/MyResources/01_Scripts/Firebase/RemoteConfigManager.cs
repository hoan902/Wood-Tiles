using Firebase.RemoteConfig;
using MEC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteConfigManager : SingletonMonoBehaviour<RemoteConfigManager>
{
    public IEnumerator<float> InitializeAsync()
    {
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
