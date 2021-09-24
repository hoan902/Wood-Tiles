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

public class SwitchFontStyle : MonoBehaviour
{
	GameObject	selected = null;

	public void OnSelected( GameObject item )
	{
		if( selected == item )
		{
			return;
		}

		if( selected != null )
		{
			Text text = selected.GetComponentInChildren<Text>();
			if( text != null ) text.fontStyle = FontStyle.Italic;
		}

		if( item != null )
		{
			Text text = item.GetComponentInChildren<Text>();
			if( text != null ) text.fontStyle = FontStyle.BoldAndItalic;
		}

		selected = item;
	}
}

