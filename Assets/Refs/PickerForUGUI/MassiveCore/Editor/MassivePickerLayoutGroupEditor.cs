using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Picker
{
	[CustomEditor(typeof(PickerLayoutGroup),true), CanEditMultipleObjects]
	public class MassivePickerLayoutGroupEditor : Editor
	{
		protected SerializedProperty	m_ScrollRect;
		protected SerializedProperty	m_Spacing;
		protected SerializedProperty	m_ChildPivot;
		
		protected virtual void OnEnable()
		{
			m_ScrollRect	= serializedObject.FindProperty("m_ScrollRect");
			m_Spacing		= serializedObject.FindProperty("m_Spacing");
			m_ChildPivot	= serializedObject.FindProperty("m_ChildPivot");
		}
		
		public override void OnInspectorGUI ()
		{
			serializedObject.Update();
			
			EditorGUILayout.PropertyField( m_ScrollRect );
			EditorGUILayout.PropertyField( m_Spacing );
			EditorGUILayout.PropertyField( m_ChildPivot );
			
			serializedObject.ApplyModifiedProperties();
		}
	}
}
