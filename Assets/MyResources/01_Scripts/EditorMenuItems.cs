#if UNITY_EDITOR
using System.Diagnostics;
using System.IO;
using DG.Tweening;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System;
using UnityEditor.Build.Content;
using Debug = UnityEngine.Debug;

public static class EditorMenuItems
{
    private const string BASE_MENU_PATH = "EditorUtils/Open Directory/";
    private const string SCENE_PATH = "EditorUtils/Scene/";

    [MenuItem("EditorUtils/Scenes/LoadingScene")]
    private static void OpenLoadingScene()
    {
        var pathOfFirstScene = EditorBuildSettings.scenes[0].path;
        EditorSceneManager.OpenScene(pathOfFirstScene);
    }

    [MenuItem("EditorUtils/Scenes/GameScene")]
    private static void OpenMenuScene()
    {
        var pathOfFirstScene = EditorBuildSettings.scenes[1].path;
        EditorSceneManager.OpenScene(pathOfFirstScene);
    }


    [MenuItem(BASE_MENU_PATH + "TemporaryCachePath")]
    private static void OpenTemporaryCachePath()
    {
        Process.Start(Application.temporaryCachePath);
    }

    [MenuItem(BASE_MENU_PATH + "PersistentDataPath")]
    private static void OpenPersistentDataPath()
    {
        Process.Start(Application.persistentDataPath);
    }

    [MenuItem("EditorUtils/Clear save data")]
    private static void ClearSave()
    {
        DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath);
        FileInfo[] info = dir.GetFiles("*.*");

        foreach (FileInfo f in info)
        {
            File.Delete(f.FullName);
        }

        // ClearPlayerPrefs();
    }

    [MenuItem(BASE_MENU_PATH + "Clear PlayerPrefs")]
    private static void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    [MenuItem("EditorUtils/Builds/" + "BUILD APK")]
    private static void BuildAPK()
    {
        string path = PreBuildSetup();
        BuildPipeline.BuildPlayer(GetScenePaths(), path, BuildTarget.Android, BuildOptions.None);
        ShowExplorer(path);
    }


    [MenuItem("EditorUtils/Builds/" + "BUILD AND RUN APK")]
    private static void BuildAndRunAPK()
    {
        string path = PreBuildSetup();
        // Build player.
        BuildPipeline.BuildPlayer(GetScenePaths(), path, BuildTarget.Android, BuildOptions.AutoRunPlayer);
        ShowExplorer(path);
    }

    static string PreBuildSetup()
    {
        //string keyStorePassPath = Path.GetFullPath(
        //    "Assets/../../Key/ReleaseKey/keystore.txt");
        //string keyStorePath = Path.GetFullPath(
        //    "Assets/../../Key/ReleaseKey/idle-zombie-release.keystore");

        //string pass = File.ReadAllText(keyStorePassPath);

        //PlayerSettings.Android.keystoreName = keyStorePath;
        //PlayerSettings.Android.keystorePass = pass;
        //PlayerSettings.Android.keyaliasName = "pine-entertainment";
        //PlayerSettings.Android.keyaliasPass = pass;

        // EditorWindow.GetWindow(Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor"));

        string name = $"AnimalLink_v{Application.version}";
        string path = EditorUtility.SaveFilePanel("Choose Location of Built Game", "", name, "apk");
        return path;
    }

    static string[] GetScenePaths()
    {
        string[] scenes = new string[EditorBuildSettings.scenes.Length];
        for (int i = 0; i < scenes.Length; i++)
        {
            scenes[i] = EditorBuildSettings.scenes[i].path;
        }

        return scenes;
    }

    static void ShowExplorer(string itemPath)
    {
        itemPath = itemPath.Replace(@"/", @"\"); // explorer doesn't like front slashes
        System.Diagnostics.Process.Start("explorer.exe", "/select," + itemPath);
    }
}
#endif