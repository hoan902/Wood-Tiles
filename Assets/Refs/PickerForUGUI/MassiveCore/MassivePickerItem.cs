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

	[RequireComponent(typeof(RectTransform)), ExecuteInEditMode(),AddComponentMenu("UI/Picker/MassivePickerItem",1007), DisallowMultipleComponent]
	public class MassivePickerItem : UIBehaviour, IPointerClickHandler
	{
		public object		userData = null;
		public UnityEvent	onClick = new UnityEvent();

		public virtual void SetItemContents( MassivePickerScrollRect scrollRect,int itemIndex )
		{
			Debug.LogWarning("SetItemContens() is not overrided. It's necessary to attach the script which expanded MassivePickerItem to an item.");
		}

		[System.NonSerialized]
		public int			itemIndex = -1;

		protected override void Start()
		{
			base.Start();
			FixedAnchor();
		}

		void FixedAnchor()
		{
			rectTransform.anchorMin = rectTransform.anchorMax = new Vector2(0.5f,0.5f);
		}

#if UNITY_EDITOR

		protected override void Reset()
		{
			base.Reset();
			rectTransform.anchorMin = rectTransform.anchorMax = new Vector2(0.5f,0.5f);
		}

#endif

		public RectTransform rectTransform
		{
			get
			{
				if( m_RectTransform != null )
				{
					return m_RectTransform;
				}

				return m_RectTransform = GetComponent<RectTransform>();
			}
		}

		protected RectTransform m_RectTransform;

				
		protected MassivePickerLayoutGroup		parentLayoutGroup = null;
		
		public void OnPointerClick( PointerEventData eventData )
		{
			ScrollToSelf();
			onClick.Invoke();
		}
		
		protected void ScrollToSelf()
		{
			MassivePickerLayoutGroup layout = GetComponentInParent<MassivePickerLayoutGroup>();
			
			if( layout == null )
			{
				return;
			}
			
			MassivePickerScrollRect psr = layout.scrollRect;
			
			if( psr == null )
			{
				return;
			}
			
			psr.ScrollTo(this);
		}
	}

}

