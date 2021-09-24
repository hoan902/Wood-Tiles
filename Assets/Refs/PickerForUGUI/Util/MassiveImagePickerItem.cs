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

	public class MassiveImagePickerItem : MassivePickerItem
	{
		MassiveImagePicker	m_Parent;
		int					m_ColumnIndex = -1;
		Image				m_Image;

		public override void SetItemContents (MassivePickerScrollRect scrollRect, int itemIndex)
		{
			if( m_Parent == null )
			{
				m_Parent = scrollRect.GetComponentInParent<MassiveImagePicker>();

				if( m_Parent == null )
				{
					return;
				}
			}

			if( m_ColumnIndex < 0 )
			{
				m_ColumnIndex = m_Parent.GetColumnIndex(scrollRect);
				
				if( m_ColumnIndex < 0 )
				{
					return;
				}
			}

			if( m_Image == null )
			{
				m_Image = GetComponent<Image>();

				if( m_Image == null )
				{
					return;
				}
			}

			Sprite sprite = m_Parent.GetItemParam( m_ColumnIndex, itemIndex );
			m_Image.sprite = sprite;
		}
	}

}
