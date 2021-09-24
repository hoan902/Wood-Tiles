/*
'MassivePickerScrollRectEditor.cs' edited the copied from 'UnityEditor.UI/UI/ScrollRectEditor.cs' of uGUI.

License of uGUI
----------------------------------------------------------------------------------
The MIT License (MIT)

Copyright (c) 2014, Unity Technologies

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
----------------------------------------------------------------------------------
*/

using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.AnimatedValues;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Picker
{

	[CustomEditor(typeof(MassivePickerScrollRect),true)]
	[CanEditMultipleObjects]
	public class MassivePickerScrollRectEditor : Editor
	{
		protected SerializedProperty m_Content;
		protected SerializedProperty m_Horizontal;
		protected SerializedProperty m_Vertical;
		protected SerializedProperty m_MovementType;
		protected SerializedProperty m_Elasticity;
		protected SerializedProperty m_Inertia;
		protected SerializedProperty m_DecelerationRate;
		protected SerializedProperty m_ScrollSensitivity;
		protected SerializedProperty m_HorizontalScrollbar;
		protected SerializedProperty m_VerticalScrollbar;
		protected SerializedProperty m_OnSelectItem;
		protected SerializedProperty m_SlipVelocityRate;
		protected SerializedProperty m_WheelEffect;
		protected SerializedProperty m_WheelPerspective;
		protected SerializedProperty m_AutoScrollSeconds;
		protected SerializedProperty m_OnValueChanged;
		protected SerializedProperty m_InitialPosition;
		protected SerializedProperty m_InitialPositionItemIndex;
		protected AnimBool m_ShowElasticity;
		protected AnimBool m_ShowDecelerationRate;
		protected AnimBool m_ShowWheelEffect;
		protected AnimBool m_ShowInitialPositionIndex;

		protected SerializedProperty m_DeactiveItemOnAwake;
		protected SerializedProperty m_ItemSource;
		protected SerializedProperty m_ItemSize;
		protected SerializedProperty m_ItemCount;
		
		protected virtual void OnEnable()
		{
			m_Content               = serializedObject.FindProperty("m_Content");
			m_Horizontal            = serializedObject.FindProperty("m_Horizontal");
			m_Vertical              = serializedObject.FindProperty("m_Vertical");
			m_MovementType          = serializedObject.FindProperty("m_MovementType");
			m_Elasticity            = serializedObject.FindProperty("m_Elasticity");
			m_Inertia               = serializedObject.FindProperty("m_Inertia");
			m_DecelerationRate      = serializedObject.FindProperty("m_DecelerationRate");
			m_ScrollSensitivity     = serializedObject.FindProperty("m_ScrollSensitivity");
			m_HorizontalScrollbar   = serializedObject.FindProperty("m_HorizontalScrollbar");
			m_VerticalScrollbar     = serializedObject.FindProperty("m_VerticalScrollbar");
			m_OnSelectItem			= serializedObject.FindProperty("onSelectItem");
			m_SlipVelocityRate		= serializedObject.FindProperty("slipVelocityRate");
			m_WheelEffect			= serializedObject.FindProperty("m_WheelEffect");
			m_WheelPerspective		= serializedObject.FindProperty("m_Perspective");
			m_AutoScrollSeconds		= serializedObject.FindProperty("autoScrollSeconds");
			m_OnValueChanged		= serializedObject.FindProperty("m_OnValueChanged");
			m_InitialPosition		= serializedObject.FindProperty("initialPosition");
			m_InitialPositionItemIndex	= serializedObject.FindProperty("initialPositionItemIndex");

			m_DeactiveItemOnAwake	= serializedObject.FindProperty("m_DeactiveItemOnAwake");
			m_ItemSource            = serializedObject.FindProperty("m_ItemSource");
			m_ItemSize              = serializedObject.FindProperty("m_ItemSize");
			m_ItemCount             = serializedObject.FindProperty("m_ItemCount");
			
			m_ShowElasticity = new AnimBool(Repaint);
			m_ShowDecelerationRate = new AnimBool(Repaint);
			m_ShowWheelEffect = new AnimBool(Repaint);
			m_ShowInitialPositionIndex = new AnimBool(Repaint);
			SetAnimBools(true);

		}

		static Dictionary<ScrollRect.MovementType,int> movementTypeToIndex = new Dictionary<ScrollRect.MovementType, int>(){
			{ ScrollRect.MovementType.Unrestricted, 0 },
			{ ScrollRect.MovementType.Elastic, 1 },
			{ ScrollRect.MovementType.Clamped, 2 }
		};

		static string[] movementTypes = 
		{
			"Infinite",
			"Elastic",
			"Clamped",
		};
		
		protected virtual void OnDisable()
		{
			m_ShowElasticity.valueChanged.RemoveListener(Repaint);
			m_ShowDecelerationRate.valueChanged.RemoveListener(Repaint);
		}

		void SetAnimBools(bool instant)
		{
			SetAnimBool(m_ShowElasticity, !m_MovementType.hasMultipleDifferentValues && m_MovementType.enumValueIndex == (int)ScrollRect.MovementType.Elastic, instant);
			SetAnimBool(m_ShowDecelerationRate, !m_Inertia.hasMultipleDifferentValues && m_Inertia.boolValue == true, instant);
			SetAnimBool(m_ShowWheelEffect, !m_WheelEffect.hasMultipleDifferentValues && m_WheelEffect.boolValue == true, instant);
			SetAnimBool(m_ShowInitialPositionIndex, !m_InitialPosition.hasMultipleDifferentValues && m_InitialPosition.enumValueIndex == (int)InitialPosition.SelectItemIndex, instant);
		}
		
		void SetAnimBool(AnimBool a, bool value, bool instant)
		{
			if (instant)
				a.value = value;
			else
				a.target = value;
		}

		RectTransform.Axis GetLayout()
		{
			int horizontal = 0;
			int vertical = 0;
			int count = 0;

			foreach( Object component in targets )
			{
				MassivePickerScrollRect psr = component as MassivePickerScrollRect;
				if( psr == null ) continue;

				++count;

				if( psr.horizontal )
					++horizontal;
				else
					++vertical;
			}

			if( count == horizontal ) return RectTransform.Axis.Horizontal;
			if( count == vertical )   return RectTransform.Axis.Vertical;
			return (RectTransform.Axis)int.MaxValue;
		}

		int GetMovementTypeIndex()
		{
			int[] counter = new int[movementTypeToIndex.Count];
			int totalCount = 0;

			foreach( Object component in targets )
			{
				MassivePickerScrollRect psr = component as MassivePickerScrollRect;
				if( psr == null ) continue;

				++totalCount;

				int index;

				if( movementTypeToIndex.TryGetValue(psr.movementType,out index) )
				{
					++counter[index];
				}
			}

			for( int i = 0; i < counter.Length; ++i )
			{
				if( counter[i] == totalCount )
				{
					return i;
				}
			}

			return counter.Length;
		}

		public override void OnInspectorGUI()
		{
			EditorGUI.BeginChangeCheck();

			SetAnimBools(false);
			
			serializedObject.Update();

			EditorGUILayout.PropertyField( m_DeactiveItemOnAwake );
			EditorGUILayout.PropertyField( m_ItemSource );
			EditorGUILayout.PropertyField( m_ItemSize );
			EditorGUILayout.PropertyField( m_ItemCount );

			EditorGUILayout.Space();

			EditorGUILayout.PropertyField( m_Content );

			RectTransform.Axis currentLayout = GetLayout();
			RectTransform.Axis layout = (RectTransform.Axis)EditorGUILayout.EnumPopup( "Layout", (System.Enum)currentLayout );

			if( currentLayout != layout )
			{
				Undo.RecordObjects( GetRecordObjects(), "PickerScrollRect.Layout" );
				ChangeScrollRect( psr => psr.layout = layout );
				SetLayoutDirty();
			}

			int currentMovementTypeIndex = GetMovementTypeIndex();
			int movementTypeIndex = EditorGUILayout.Popup( "MovementType", currentMovementTypeIndex, movementTypes );

			if( movementTypeIndex != currentMovementTypeIndex )
			{
				m_MovementType.enumValueIndex = (int)movementTypeToIndex.First( kv => kv.Value == movementTypeIndex ).Key;
			}

			if (EditorGUILayout.BeginFadeGroup(m_ShowElasticity.faded))
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(m_Elasticity);
				EditorGUI.indentLevel--;
			}
			EditorGUILayout.EndFadeGroup();
			
			EditorGUILayout.PropertyField(m_Inertia);
			if (EditorGUILayout.BeginFadeGroup(m_ShowDecelerationRate.faded))
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(m_DecelerationRate);
				EditorGUILayout.PropertyField(m_SlipVelocityRate);
				EditorGUI.indentLevel--;
			}
			EditorGUILayout.EndFadeGroup();
			
			EditorGUILayout.PropertyField(m_ScrollSensitivity);
			EditorGUILayout.PropertyField(m_AutoScrollSeconds, new GUIContent("Scroll Sec From Click") );
			EditorGUILayout.PropertyField(m_WheelEffect);
			if( EditorGUILayout.BeginFadeGroup(m_ShowWheelEffect.faded) )
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(m_WheelPerspective);
				EditorGUI.indentLevel--;
			}
			EditorGUILayout.EndFadeGroup();

			EditorGUILayout.PropertyField(m_InitialPosition, new GUIContent("Initial Scroll Position"));

			if( EditorGUILayout.BeginFadeGroup(m_ShowInitialPositionIndex.faded) )
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(m_InitialPositionItemIndex);
				EditorGUI.indentLevel--;
			}
			EditorGUILayout.EndFadeGroup();

			EditorGUILayout.Space();
			
			EditorGUILayout.PropertyField(m_HorizontalScrollbar);
			EditorGUILayout.PropertyField(m_VerticalScrollbar);
			
			EditorGUILayout.Space();
			
			EditorGUILayout.PropertyField(m_OnSelectItem);
			EditorGUILayout.PropertyField(m_OnValueChanged, new GUIContent("On Scroll Changed"));

			serializedObject.ApplyModifiedProperties();
		}

		protected Object[] GetRecordObjects()
		{
			List<Object> objects = new List<Object>();

			foreach( Object obj in targets )
			{
				MassivePickerScrollRect psr = obj as MassivePickerScrollRect;

				if( psr != null )
				{
					objects.AddRange( psr.GetComponentsInChildren<Component>() );
				}
			}

			return objects.ToArray();
		}

		protected void SetLayoutDirty()
		{
			foreach( Object obj in GetRecordObjects() )
			{
				EditorUtility.SetDirty(obj);
			}
		}

		protected void ChangeScrollRect( System.Action<MassivePickerScrollRect> func )
		{
			foreach( Object obj in targets )
			{
				MassivePickerScrollRect psr = obj as MassivePickerScrollRect;
				
				if( psr != null )
				{
					func(psr);
				}
			}
		}
	}
}

