using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;
using System.Xml;
using UnityEditorInternal;
using UnityEditor.AnimatedValues;
using EditorUtils;

public class WazzionPanel : EditorWindow {

  

    public string editorTitle = "";
    Color selectionColor;
    Color bgColor;


    [MenuItem("Wazzion/Tools")]
    public static WazzionPanel CreateBerryPanel() {
        WazzionPanel window = GetWindow<WazzionPanel>();
        window.titleContent = new GUIContent("Wazzion Tool");
        window.Show();
        window.OnEnable();
        return window;
    }

    void OnEnable() {
        selectionColor = Color.Lerp(Color.red, Color.white, 0.7f);
        bgColor = Color.Lerp(GUI.backgroundColor, Color.black, 0.3f);
    }

   

    Color defalutColor;
    public Vector2 editorScroll, tabsScroll, levelScroll = new Vector2();
    public MetaEditor editor, editor2 = null;

    public System.Action editorRender;
    void OnGUI() {

        if (editorRender == null || editor == null) {
            editorRender = null;
            editor = null;
        }

        defalutColor = GUI.backgroundColor;
        EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        GUI.backgroundColor = bgColor;
        EditorGUILayout.BeginVertical(EditorStyles.textArea, GUILayout.Width(150), GUILayout.ExpandHeight(true));
        GUI.backgroundColor = defalutColor;
        tabsScroll = EditorGUILayout.BeginScrollView(tabsScroll);

        DrawTabs();

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();

        GUI.backgroundColor = bgColor;
        EditorGUILayout.BeginVertical(EditorStyles.textArea, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        GUI.backgroundColor = defalutColor;
        editorScroll = EditorGUILayout.BeginScrollView(editorScroll);

        if (!string.IsNullOrEmpty(editorTitle))
            DrawTitle(editorTitle);


        if (editor != null)
            editorRender.Invoke();
        else
            GUILayout.Label("Nothing selected");

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        GUILayout.Label("Game Tools Package\n(Wazzion) Copyright 2017", EditorStyles.centeredGreyMiniLabel, GUILayout.ExpandWidth(true));
    }

    void DrawTabs() {
        DrawTabTitle("General");

        if (DrawTabButton("Content")) {
            editor = new ContentManagerEditor();
            editor.onRepaint += Repaint;
            editorRender = editor.OnInspectorGUI;
        }


        if (DrawTabButton("UI")) {
            editor = new UIManagerEditor();
            editorRender = editor.OnInspectorGUI;
        }

        if (DrawTabButton("Audio")) {
            editor = new AudioManagerEditor();
            editor.onRepaint += Repaint;
            editorRender = editor.OnInspectorGUI;
        }

    }

    bool DrawTabButton(string text) {
        Color color = GUI.backgroundColor;
        if (editorTitle == text)
            GUI.backgroundColor = selectionColor;
        bool result = GUILayout.Button(text, EditorStyles.miniButton, GUILayout.ExpandWidth(true));
        GUI.backgroundColor = color;

        if (string.IsNullOrEmpty(editorTitle) || (editorTitle == text && editorRender == null))
            result = true;

        if (result) {
            EditorGUI.FocusTextInControl("");
            editorTitle = text;
        }

        return result;
    }

    void DrawTabTitle(string text) {
        GUILayout.Label(text, EditorStyles.centeredGreyMiniLabel, GUILayout.ExpandWidth(true));
    }

    void DrawTitle(string text) {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(text, EditorStyles.largeLabel, GUILayout.ExpandWidth(true));

        EditorGUILayout.EndHorizontal();
        GUILayout.Space(10);
    }



}