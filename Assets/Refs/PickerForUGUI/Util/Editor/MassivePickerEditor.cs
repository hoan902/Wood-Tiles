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
	public class MassivePickerEditor<PickerType,PickerItemComponent, ItemParamType, ItemListType> : Editor
		where PickerItemComponent : MassivePickerItem 
		where ItemListType : ItemList<ItemParamType>, new()
		where PickerType : MassivePicker<PickerItemComponent,ItemParamType,ItemListType>
	{
		public override void OnInspectorGUI ()
		{
			EditorGUI.BeginChangeCheck();
			
			base.OnInspectorGUI ();
			
			if( EditorGUI.EndChangeCheck() )
			{
				Undo.RecordObjects( GetRecordTargets(), "picker changed" );
				
				foreach( PickerType picker in GetTargetPickers() )
				{
					picker.SyncItemList(true);
				}

				PickerType targetPicker = target as PickerType;

				if( targetPicker != null )
				{
					GameObject itemSource = targetPicker.itemSource;

					if( itemSource != null )
					{
						RectTransform rectTransform = itemSource.GetComponent<RectTransform>();

						if( rectTransform != null && rectTransform.sizeDelta != targetPicker.itemSize )
						{
							rectTransform.sizeDelta = targetPicker.itemSize;
						}
					}
				}
			}

			
			RectTransform.Axis axis = GetAxis();
			RectTransform.Axis newAxis = (RectTransform.Axis)EditorGUILayout.EnumPopup( "Layout", axis );

			if( axis != newAxis )
			{
				Undo.RecordObjects( GetRecordTargets(), "picker changed" );

				foreach( PickerType picker in GetTargetPickers() )
				{
					picker.layout = newAxis;
				}
			}
		}

		RectTransform.Axis GetAxis()
		{
			int horizontal = 0;
			int vertical = 0;
			int count = 0;

			foreach( PickerType picker in GetTargetPickers() )
			{
				RectTransform.Axis layout = picker.layout;

				if( layout == RectTransform.Axis.Horizontal ) ++horizontal;
				if( layout == RectTransform.Axis.Vertical ) ++vertical;
				++count;
			}

			if( count == horizontal ) return RectTransform.Axis.Horizontal;
			if( count == vertical ) return RectTransform.Axis.Vertical;
			return (RectTransform.Axis)(-1);
		}

		Object[] GetRecordTargets()
		{
			List<Object> targets = new List<Object>();

			foreach( PickerType picker in GetTargetPickers() )
			{
				targets.AddRange( picker.GetComponentsInChildren<Component>() );
			}

			return targets.ToArray();
		}
		
		IEnumerable<PickerType> GetTargetPickers()
		{
			foreach( Object obj in targets )
			{
				PickerType picker = obj as PickerType;
				
				if( picker != null )
				{
					yield return picker;
				}
			}
		}
	}

}

