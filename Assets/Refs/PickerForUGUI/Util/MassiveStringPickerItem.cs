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

	public class MassiveStringPickerItem : MassivePickerItem
	{
		MassiveStringPicker	m_Parent;
		int					m_ColumnIndex = -1;
		Text[]				m_Texts;

		public override void SetItemContents (MassivePickerScrollRect scrollRect, int itemIndex)
		{
			if( m_Parent == null )
			{
				m_Parent = scrollRect.GetComponentInParent<MassiveStringPicker>();

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

			if( m_Texts == null )
			{
				m_Texts = GetComponentsInChildren<Text>();

				if( m_Texts == null )
				{
					return;
				}
			}

			string text = m_Parent.GetItemParam( m_ColumnIndex, itemIndex );

			foreach( Text textComponent in m_Texts )
			{
				textComponent.text = text;
			}
		}
	}

}
