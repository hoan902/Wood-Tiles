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
//1. The script which extended MassivePickerItem is attached to GameObject of a source item.
//2. The contents of an item are set in SetItemContents of an extended script.
//ex) ExampleCountryNameItem, ExampleStringItem
//3. An item gameobject is set in MassivePickerScrollRect::ItemSource.
public class ExampleStringItem : Picker.MassivePickerItem
{
	protected override void Awake()
	{
        base.Awake();
		m_Text = GetComponent<Text>();
	}

	protected Text m_Text;

	public override void SetItemContents (Picker.MassivePickerScrollRect scrollRect, int itemIndex)
	{
		if( m_Text != null )
		{
			m_Text.text = "Item" + itemIndex;
		}
	}
}
