#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityToolbarExtender;


[InitializeOnLoad]
public class ToolbarExtension
{
    static ToolbarExtension()
    {
        ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
    }

    static void OnToolbarGUI()
    {
        GUILayout.FlexibleSpace();

        if (GUILayout.Button(new GUIContent(">>", "Start Game")))
        {
#if UNITY_EDITOR
            if (EditorApplication.isPlaying == true)
            {
                EditorApplication.isPlaying = false;
                return;
            }

            if (SaveCurrentScene())
            {
                var firstScene = EditorBuildSettings.scenes[0].path;
                EditorSceneManager.OpenScene(firstScene);
                EditorApplication.isPlaying = true;
            }
#endif
        }
    }

    private static bool SaveCurrentScene()
    {
        var currentScene = EditorSceneManager.GetActiveScene();
        if (currentScene.isDirty)
            return EditorSceneManager.SaveScene(currentScene, currentScene.path);

        return true;
    }
}

#endif