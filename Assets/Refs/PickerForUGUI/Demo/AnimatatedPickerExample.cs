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



public class AnimatatedPickerExample : MonoBehaviour
{
	[SerializeField]
	GameObject				itemObject = null;
	
	[SerializeField]
	Sprite[]				flagIcons = null;
	
	[SerializeField]
	PickerScrollRect		pickerScroll = null;

	List<Image>			itemImages = new List<Image>();
	List<Text>			itemTexts = new List<Text>();
	
	void Awake()
	{
		RectTransform itemParent = pickerScroll.content;
		
		foreach( Sprite sprite in flagIcons )
		{
			GameObject item = itemObject;

			if( sprite.name != itemObject.name )
			{
				item = (GameObject)Instantiate(itemObject);
				item.name = sprite.name;
			}

			Image image = item.GetComponentInChildren<Image>();
			image.sprite = sprite;

			Text text = item.GetComponentInChildren<Text>();
			text.text = sprite.name;

			itemImages.Add(image);
			itemTexts.Add(text);

			item.transform.SetParent(itemParent);
		}
	}

	public void OnScroll( Vector2 normalizedPosition )
	{
		float center = pickerScroll.transform.position.x;

		float width = ((RectTransform)pickerScroll.transform).sizeDelta.x * 0.5f;
		float left = center - width;
		float right = center + width;

		for( int itemIndex = 0; itemIndex < itemImages.Count; ++itemIndex )
		{
			Image image = itemImages[itemIndex];

			float x = image.transform.position.x;

			RectTransform imageTransform = image.rectTransform;
			float imageWidthHalf = imageTransform.sizeDelta.x * 0.5f;

			if( x + imageWidthHalf < left || right < x - imageWidthHalf )
			{
				//out of content rect.
				continue;
			}

			float a = Mathf.Clamp01((minDistance - Mathf.Abs( x - center )) / minDistance);
			float size = Mathf.Lerp(minSize,maxSize,a);

			a = Mathf.Lerp(minAlpha,1f,a);

			imageTransform.sizeDelta = new Vector2(size,size);
			image.color =  new Color(1f,1f,1f,a);

			Text text = itemTexts[itemIndex];
			text.color = new Color(0,0,0,a);
			text.rectTransform.localScale = Vector2.one * (size / minSize);
		}
	}

	[SerializeField] float	minDistance;
	[SerializeField] float	minAlpha;
	[SerializeField] float	minSize;
	[SerializeField] float	maxSize;
}

