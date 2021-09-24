using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveManager : SingletonMonoBehaviour<SaveManager>
{
    public static string KEY_USER_DATA = "USER_DATA";
    public static int VERSION_CODE = 2;

    public UserData Data { get; private set; }

    public override void Initialize()
    {
        base.Initialize();
    }

    public void LoadData()
    {
        var dataStr = SaveSystem.GetString(KEY_USER_DATA);
        if (string.IsNullOrEmpty(dataStr))
        {
            Data = SaveGameHelper.DefaultData();
        }
        else
        {
            Data = JsonUtility.FromJson<UserData>(dataStr);
        }

        if(Data.SyncData())
        {
            SaveData();
        }

        
    }

    public void SaveData()
    {
        SaveSystem.SetString(KEY_USER_DATA, JsonUtility.ToJson(Data));
        SaveSystem.SaveToDisk();
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            SaveData();
        }
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }
}


