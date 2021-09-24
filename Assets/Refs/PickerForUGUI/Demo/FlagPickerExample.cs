using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Picker;

#if UNITY_EDITOR
using UnityEditor;
#endif



public class FlagPickerExample : MonoBehaviour
{
	[SerializeField]
	GameObject				itemObject = null;

	[SerializeField]
	Sprite[]				flagIcons = null;

	[SerializeField]
	PickerScrollRect		pickerScroll = null;

	void Start()
	{
		RectTransform itemParent = pickerScroll.content;

		foreach( Sprite sprite in flagIcons )
		{
			if( sprite.name == itemObject.name )
			{
				continue;
			}

			GameObject item = (GameObject)Instantiate(itemObject);
			item.name = sprite.name;

			foreach( Image image in item.GetComponentsInChildren<Image>() )
			{
				image.sprite = sprite;
			}

			foreach( Text text in item.GetComponentsInChildren<Text>() )
			{
				text.text = sprite.name;
			}

			item.transform.SetParent(itemParent);
		}
	}
}

