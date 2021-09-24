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

	[CustomEditor(typeof(ZoomPickerLayoutGroup),true)]
	public class ZoomPickerLayoutGroupEditor : PickerLayoutGroupEditor
	{
		protected SerializedProperty	m_ZoomItemParent;
		
		protected override void OnEnable()
		{
			base.OnEnable();
			m_ZoomItemParent	= serializedObject.FindProperty("zoomItemParent");
		}
		
		public override void OnInspectorGUI ()
		{
			EditorGUILayout.PropertyField( m_ZoomItemParent );
			base.OnInspectorGUI ();
		}
	}

}
