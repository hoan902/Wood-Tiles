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

	[RequireComponent(typeof(RectTransform)), ExecuteInEditMode(),AddComponentMenu("UI/Picker/PickerItem",1002), DisallowMultipleComponent]
	public class PickerItem : UIBehaviour, IPointerClickHandler
	{
		public object		userData = null;
		public UnityEvent	onClick = new UnityEvent();
		
		[System.NonSerialized]
		public float	position = 0;

		protected PickerLayoutGroup		parentLayoutGroup = null;

		public void OnPointerClick( PointerEventData eventData )
		{
			ScrollToSelf();
			onClick.Invoke();
		}

		protected void ScrollToSelf()
		{
			PickerLayoutGroup layout = GetComponentInParent<PickerLayoutGroup>();
			
			if( layout == null )
			{
				return;
			}
			
			PickerScrollRect psr = layout.scrollRect;
			
			if( psr == null )
			{
				return;
			}
			
			psr.ScrollTo(this);
		}

		protected virtual void RebuildLayout()
		{
			PickerLayoutGroup layout = GetComponentInParent<PickerLayoutGroup>();

			if( layout != null )
			{
				layout.RebuildLayout();
			}
		}


		protected override void OnBeforeTransformParentChanged ()
		{
			base.OnBeforeTransformParentChanged ();

			if( transform.parent != null )
			{
				PickerLayoutGroup parentLayoutGroup = transform.parent.GetComponent<PickerLayoutGroup>();

				if( parentLayoutGroup != null )
				{
					parentLayoutGroup.UnregisterItem(this);
				}
			}
		}

#if UNITY_EDITOR
		protected Rect beforeRect = new Rect();
#endif		

		protected override void OnRectTransformDimensionsChange ()
		{
			base.OnRectTransformDimensionsChange();

#if UNITY_EDITOR
			
			if( !Application.isPlaying )
			{
				RectTransform transform = GetComponent<RectTransform>();
				Rect rect = transform.rect;
				
				if( beforeRect != rect )
				{
					beforeRect = rect;
					RebuildLayout();
				}
			}

#endif
		}


	}

}

