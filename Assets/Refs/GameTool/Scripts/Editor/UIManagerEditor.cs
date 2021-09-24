using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;
using UnityEditorInternal;
using EditorUtils;


[CustomEditor(typeof(UIManager))]
public class UIManagerEditor : MetaEditor
{

    UIManager main;
    UIManager.Page edit = null;

    private ReorderableList listUIModules;
    private ReorderableList listPages;

    protected override void OnEnable()
    {
        base.OnEnable();
        listUIModules = this.GetReorderableList(mySerializedObject.FindProperty("UImodules"));
        listPages = this.GetReorderableList(mySerializedObject.FindProperty("pages"));
    }

    public override void OnInspectorGUI()
    {
        if (!metaTarget)
            return;
        main = (UIManager)metaTarget;

        Undo.RecordObject(main, "");
        Color defColor = GUI.color;

        if (main.UImodules == null)
            main.UImodules = new List<Transform>();

        if (main.pages == null)
            main.pages = new List<UIManager.Page>();

        #region UI Modules


        mySerializedObject.Update();
        listUIModules.drawHeaderCallback = rect => { EditorGUI.LabelField(rect, "Canvas parent"); };
        listUIModules.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                EditorGUI.PropertyField(rect, listUIModules.serializedProperty.GetArrayElementAtIndex(index), GUIContent.none, true);
            };
        listUIModules.DoLayoutList();
        main.ArraysConvertation();
        if (listUIModules == null || listUIModules.count <= 0)
        {
            EditorGUILayout.HelpBox("Drag Canvas parent to get Panel", MessageType.Info);
        }
        if (main.panels == null || main.panels.Count <= 0)
        {
            EditorGUILayout.HelpBox("Canvas must have UIPanel", MessageType.Info);
        }


        mySerializedObject.ApplyModifiedProperties();

        #endregion

        #region Pages


        GUILayout.Space(20);
     
        EditorGUILayout.BeginVertical("Box");
        GUI.color = Color.red;
        GUILayout.Label("Pages", EditorStyles.centeredGreyMiniLabel, GUILayout.ExpandWidth(true));
        GUI.color = defColor;
        foreach (UIManager.Page page in main.pages)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal("Box");
            if (GUILayout.Button("X", EditorStyles.miniButtonLeft, GUILayout.Width(20)))
            {
                main.pages.Remove(page);
                break;
            }

            if (edit == page)
            {

                if (GUILayout.Button("Hide", EditorStyles.miniButtonRight, GUILayout.Width(35)))
                {
                    edit = null;
                }
            }
            else
            {
                if (GUILayout.Button("Edit", EditorStyles.miniButtonRight, GUILayout.Width(35)))
                {
                    edit = page;
                }
            }

            page.name = EditorGUILayout.TextField(page.name, GUILayout.ExpandWidth(true));

            //   GUILayout.FlexibleSpace();
            UIManager.Page default_page = main.pages.Find(x => x.default_page);

            if (default_page == null)
            {
                default_page = page;
                page.default_page = true;
            }

            if (page.default_page && default_page != page)
                page.default_page = false;


            if (page.default_page)
            {
                GUI.color = Color.red;
                GUILayout.Label("DEFAULT", GUILayout.Width(80));
                GUI.color = defColor;
            }
            else
                if (GUILayout.Button("Make default", EditorStyles.miniButtonRight, GUILayout.Width(80)))
                {
                    default_page.default_page = false;
                    default_page = page;
                    page.default_page = true;
                }

            EditorGUILayout.EndHorizontal();
            if (string.IsNullOrEmpty(page.name))
                EditorGUILayout.HelpBox("Fill page name", MessageType.Warning);
            EditorGUILayout.EndVertical();


            if (edit == page)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(40);
                EditorGUILayout.BeginVertical("Box");


                if (!AudioManager.Instance)
                    EditorGUILayout.HelpBox("AudioAssistant is missing", MessageType.Error, true);
                else if (AudioManager.Instance.tracks.Count > 0)
                {
                    List<string> tracks = new List<string>();
                    //  tracks.Add("-");
                    tracks.Add("None");
                    tracks.AddRange(AudioManager.Instance.tracks.Select(x => x.name).ToList());
                    int selected = -1;
                    selected = tracks.FindIndex(x => x == page.soundtrack);
                    if (selected == -1)
                        selected = 0;

                    selected = EditorGUILayout.Popup("Soundtrack", selected, tracks.ToArray());

                    page.soundtrack = tracks[selected];
                }

                if (false)
                    EditorGUILayout.HelpBox("AdAssistant is missing", MessageType.Error, true);
                else {
                    EditorGUILayout.BeginHorizontal();
//                   page.adType = (AdType)EditorGUILayout.EnumPopup("Ads", page.adType);
                    page.show_ads = EditorGUILayout.Toggle(page.show_ads, GUILayout.Width(20));
                    GUILayout.Label("Show Ads", GUILayout.Width(100));



                    EditorGUILayout.EndHorizontal();
                }
                //bool active = GUI.enabled;
                //GUI.enabled = false;
                //EditorGUILayout.BeginHorizontal();
                //EditorGUILayout.Toggle(false, GUILayout.Width(20));
                //GUILayout.Label("Show Ads", GUILayout.Width(100));
                //EditorGUILayout.EndHorizontal();
                //GUI.enabled = active;

                //EditorGUILayout.BeginHorizontal();
                //page.setTimeScale = EditorGUILayout.Toggle(page.setTimeScale, GUILayout.Width(20));
                //GUILayout.Label("Time Scale", GUILayout.Width(100));
                //if (page.setTimeScale)
                //    page.timeScale = EditorGUILayout.Slider(page.timeScale, 0, 1);
                //EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Panel Name", EditorStyles.boldLabel, GUILayout.Width(150));
                GUI.color = Color.green;
                GUILayout.Label("Show", EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
                GUI.color = Color.yellow;
                GUILayout.Label("Ignor", EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
                GUI.color = Color.cyan;
                GUILayout.Label("Hide", EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
                GUI.color = defColor;
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginVertical();
                Dictionary<UIPanel, int> mask = new Dictionary<UIPanel, int>();
                foreach (UIPanel panel in main.panels)
                {
                    if (!mask.ContainsKey(panel))
                        mask.Add(panel, -1);
                    if (page.panels.Contains(panel))
                        mask[panel] = 1;
                    else if (page.ignoring_panels.Contains(panel))
                        mask[panel] = 0;
                }

                foreach (UIPanel panel in main.panels)
                {
                    EditorGUILayout.BeginHorizontal();
                    switch (mask[panel])
                    {
                        case -1: GUI.color = Color.cyan; break;
                        case 0: GUI.color = Color.yellow; break;
                        case 1: GUI.color = Color.green; break;
                    }
                    EditorGUILayout.LabelField(panel.name, GUILayout.Width(150));
                    GUI.color = defColor;

                    if (EditorGUILayout.Toggle(mask[panel] == 1))
                        mask[panel] = 1;
                    GUILayout.FlexibleSpace();
                    if (EditorGUILayout.Toggle(mask[panel] == 0))
                        mask[panel] = 0;
                    GUILayout.FlexibleSpace();
                    if (EditorGUILayout.Toggle(mask[panel] == -1))
                        mask[panel] = -1;
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();

                page.panels.Clear();
                page.ignoring_panels.Clear();
                foreach (KeyValuePair<UIPanel, int> pair in mask)
                {
                    if (pair.Value == 1)
                        page.panels.Add(pair.Key);
                    else if (pair.Value == 0)
                        page.ignoring_panels.Add(pair.Key);
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }

        }
        GUI.color = Color.green;
        if (GUILayout.Button("Create Page", GUILayout.Width(100)))
            main.pages.Add(new UIManager.Page());

        GUI.color = defColor;
        EditorGUILayout.EndVertical();
        #endregion

        GUI.color = defColor;
        mySerializedObject.ApplyModifiedProperties();

    }

    public override Object FindTarget()
    {
        return UIManager.Instance;
    }



}

