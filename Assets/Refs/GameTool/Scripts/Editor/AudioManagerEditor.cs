using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditorInternal;
using EditorUtils;

[CustomEditor(typeof(AudioManager))]
public class AudioManagerEditor : MetaEditor
{

    AudioManager main;
    AudioManager.Sound edit = null;


    private ReorderableList listTracks;
    private ReorderableList listSound;
    bool isExpanded =false;
    protected override void OnEnable()
    {
        base.OnEnable();
        listTracks = this.GetReorderableList(mySerializedObject.FindProperty("tracks"));
        listSound = this.GetReorderableList(mySerializedObject.FindProperty("sounds"));
    }


    public override void OnInspectorGUI()
    {
        if (!metaTarget)
        {
            EditorGUILayout.HelpBox("AudioAssistant is missing", MessageType.Error);
            return;
        }
        mySerializedObject.Update();
        main = (AudioManager)metaTarget;
        Undo.RecordObject(main, "");

        if (main.tracks == null)
            main.tracks = new List<AudioManager.MusicTrack>();

        if (main.sounds == null)
            main.sounds = new List<AudioManager.Sound>();

        #region Music Tracks

        listTracks.drawHeaderCallback = rect => { 
            EditorGUI.LabelField(new Rect(rect.x+15, rect.y, 120, EditorGUIUtility.singleLineHeight), "Name");
            EditorGUI.LabelField(new Rect(rect.x + 135, rect.y, rect.width - 120, EditorGUIUtility.singleLineHeight), "Track"); 
        };
        listTracks.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = listTracks.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, 120, EditorGUIUtility.singleLineHeight),  element.FindPropertyRelative("name"), GUIContent.none);
                EditorGUI.PropertyField(new Rect(rect.x + 120, rect.y, rect.width -120, EditorGUIUtility.singleLineHeight),element.FindPropertyRelative("track"), GUIContent.none);
            };
        var property = listTracks.serializedProperty;
        GUILayout.BeginVertical("Box");
        property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, "Background Music (Track)");
        if (property.isExpanded)
        {
            this.listTracks.DoLayoutList();
        }
        GUILayout.EndVertical();
       
        mySerializedObject.ApplyModifiedProperties();


        #endregion

        #region Sounds
        EditorGUILayout.BeginVertical("Box");
        isExpanded = EditorGUILayout.Foldout(isExpanded, "Sounds Effect (Track)");
        if (isExpanded)
        {
          
            EditorGUILayout.BeginHorizontal();

            GUILayout.Space(20);
            GUILayout.Label("Edit", EditorStyles.centeredGreyMiniLabel, GUILayout.Width(40));
            GUILayout.Label("Name", EditorStyles.centeredGreyMiniLabel, GUILayout.Width(120));
            GUILayout.Label("Audio Clips", EditorStyles.centeredGreyMiniLabel, GUILayout.ExpandWidth(true));

            EditorGUILayout.EndHorizontal();

            foreach (AudioManager.Sound sound in main.sounds)
            {
                EditorGUILayout.BeginHorizontal("Box");

                if (GUILayout.Button("X", EditorStyles.miniButtonLeft, GUILayout.Width(20)))
                {
                    main.sounds.Remove(sound);
                    break;
                }
                if (GUILayout.Button("Edit", EditorStyles.miniButtonRight, GUILayout.Width(40)))
                {
                    if (edit == sound)
                        edit = null;
                    else
                        edit = sound;
                }

                sound.name = EditorGUILayout.TextField(sound.name, GUILayout.Width(120));

                if (edit == sound || sound.clips.Count == 0)
                {
                    EditorGUILayout.BeginVertical();
                    for (int i = 0; i < sound.clips.Count; i++)
                    {
                        sound.clips[i] = (AudioClip)EditorGUILayout.ObjectField(sound.clips[i], typeof(AudioClip), false, GUILayout.ExpandWidth(true));
                        if (sound.clips[i] == null)
                        {
                            sound.clips.RemoveAt(i);
                            break;
                        }
                    }
                    AudioClip new_clip = (AudioClip)EditorGUILayout.ObjectField(null, typeof(AudioClip), false, GUILayout.ExpandWidth(true));
                    if (new_clip)
                        sound.clips.Add(new_clip);
                    EditorGUILayout.EndVertical();
                }
                else
                {
                    GUILayout.Label(sound.clips.Count.ToString() + " audio clip(s)", EditorStyles.miniBoldLabel);
                }


                EditorGUILayout.EndHorizontal();
            }
        
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Add",EditorStyles.miniButtonLeft, GUILayout.Width(60)))
            {
                main.sounds.Add(new AudioManager.Sound());
                edit = main.sounds[main.sounds.Count - 1];
            }
            if (GUILayout.Button("Sort", EditorStyles.miniButtonRight, GUILayout.Width(60)))
            {
                main.sounds.Sort((AudioManager.Sound a, AudioManager.Sound b) =>
                {
                    return string.Compare(a.name, b.name);
                });
                foreach (AudioManager.Sound sound in main.sounds)
                    sound.clips.Sort((AudioClip a, AudioClip b) =>
                    {
                        return string.Compare(a.ToString(), b.ToString());
                    });
            }
            EditorGUILayout.EndHorizontal();
          
        }
     
        EditorGUILayout.EndVertical();
        #endregion
    }

    public override Object FindTarget()
    {
        return AudioManager.Instance;
    }

}
