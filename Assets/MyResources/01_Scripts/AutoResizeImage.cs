using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using System.IO;
using UnityEditor;

[ExecuteInEditMode]
public class AutoResizeImage : EditorWindow
{
    public static Object folder;

    [MenuItem("EditorUtils/Auto Resize Image")]
    static void Init()
    {
        AutoResizeImage window = (AutoResizeImage)EditorWindow.GetWindow(typeof(AutoResizeImage));
        window.Show();
        window.position = new Rect(20, 80, 550, 500);
    }

    private void OnGUI()
    {
        folder = EditorGUILayout.ObjectField("Folder", folder, typeof(Object), true);

        if (GUILayout.Button("Resize"))
        {
            StartResize();
        }

        if (GUILayout.Button("Get Folder Name"))
        {
            GetFolderName();
        }
    }

    public static void GetFolderName()
    {
        string path = AssetDatabase.GetAssetPath(folder);
        if (folder != null)
        {
            foreach (string file in Directory.GetFiles(path))
            {
                if (file.Contains(".png") && !file.Contains(".meta"))
                {
                    LoadPNG(file);
                }
            }
        }
    }

    public static void LoadPNG(string filePath)
    {

        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            Texture2D tmpTexture = new Texture2D(1, 1);
            byte[] tmpBytes = File.ReadAllBytes(filePath);
            tmpTexture.LoadImage(tmpBytes);

            tmpTexture = Resize(tmpTexture, 128, 128);

            Debug.LogError(tmpTexture.width + " " + tmpTexture.height);

            // Texture2D itemBGTex = itemBGSprite.texture;
            byte[] itemBGBytes = tmpTexture.EncodeToPNG();
            File.WriteAllBytes(filePath, itemBGBytes);
        }
    }

    public static Texture2D Resize(Texture2D texture2D, int targetX, int targetY)
    {
        RenderTexture rt = new RenderTexture(targetX, targetY, 24);
        RenderTexture.active = rt;
        Graphics.Blit(texture2D, rt);
        Texture2D result = new Texture2D(targetX, targetY);
        result.ReadPixels(new Rect(0, 0, targetX, targetY), 0, 0);
        result.Apply();
        return result;
    }


    private static void StartResize()
    {

    }

    private static int ConvertToSquare4(int size)
    {
        while (size % 4 != 0)
        {
            size++;
        }

        return size;
    }

}

#endif