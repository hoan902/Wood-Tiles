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

// readme
// 1. Picker.MassiveZoomPickerItem is expanded.
// 2. The contents of an item are set in SetItemContents().
// 3. The object to which this script was attached is assigned to 'MassiveZoomPickerScrollRect.ItemSource'.

public class ExampleZoomStringItem : Picker.MassiveZoomPickerItem
{
	protected override void Awake()
	{
        base.Awake();
		m_Texts = GetComponentsInChildren<Text>();
	}

	protected Text[] m_Texts;

	public override void SetItemContents (Picker.MassivePickerScrollRect scrollRect, int itemIndex)
	{
		if( m_Texts != null && m_Texts.Length > 0 )
		{
			string tmp = "Item" + itemIndex;

			foreach( Text text in m_Texts )
			{
				text.text = tmp;
			}
		}
	}
}
