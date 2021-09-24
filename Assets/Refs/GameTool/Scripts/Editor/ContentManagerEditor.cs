using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditorInternal;
using EditorUtils;

[CustomEditor(typeof(ContentMgr))]
public class ContentManagerEditor : MetaEditor
{

    ContentMgr main;
   
    private ReorderableList listTracks;

  //  bool isExpanded =false;
    protected override void OnEnable()
    {
        base.OnEnable();
        listTracks = this.GetReorderableList(mySerializedObject.FindProperty("contents"));

    }


    public override void OnInspectorGUI()
    {
        if (!metaTarget)
        {
            EditorGUILayout.HelpBox("ContentMgr is missing", MessageType.Error);
            return;
        }
        mySerializedObject.Update();
        main = (ContentMgr)metaTarget;
        Undo.RecordObject(main, "");

       
     

        #region Music Tracks

        listTracks.drawHeaderCallback = rect => { 
            EditorGUI.LabelField(new Rect(rect.x+15, rect.y, 120, EditorGUIUtility.singleLineHeight), "Item Type");
            EditorGUI.LabelField(new Rect(rect.x + 135, rect.y, rect.width - 120, EditorGUIUtility.singleLineHeight), "Prefab"); 
        };
        listTracks.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = listTracks.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, 120, EditorGUIUtility.singleLineHeight),  element.FindPropertyRelative("itemType"), GUIContent.none);
                EditorGUI.PropertyField(new Rect(rect.x + 120, rect.y, rect.width -120, EditorGUIUtility.singleLineHeight),element.FindPropertyRelative("go"), GUIContent.none);
            };
        var property = listTracks.serializedProperty;
        GUILayout.BeginVertical("Box");
     //   property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, "Background Music (Track)");
    //    if (property.isExpanded)
        {
            this.listTracks.DoLayoutList();
        }
        GUILayout.EndVertical();
       
        mySerializedObject.ApplyModifiedProperties();


        #endregion


    }

    public override Object FindTarget()
    {
        return ContentMgr.Instance;
    }

}
