using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PickerDemo : MonoBehaviour
{
	[SerializeField] Picker.PickerScrollRect	scrollRect = null;
	[SerializeField] Picker.PickerLayoutGroup	layoutGroup = null;

#region Item

	int itemIndexCount = 5;
	[SerializeField] Font	itemTextFont = null;

	public void AddItem()
	{
		string name = "Item" + (itemIndexCount++);
		GameObject item = new GameObject(name);

		Picker.PickerItem pickerItem = item.AddComponent<Picker.PickerItem>();
		RectTransform rect = pickerItem.GetComponent<RectTransform>();
		rect.sizeDelta = scrollRect.layout == RectTransform.Axis.Vertical ? verticalItemSize : horizontalItemSize;

		Text text = item.AddComponent<Text>();
		text.font = itemTextFont;
		text.fontSize = 18;
		text.resizeTextForBestFit = true;
		text.color = Color.black;
		text.fontStyle = FontStyle.Bold;
		text.alignment = TextAnchor.MiddleCenter;
		text.text = name;

		item.transform.SetParent(layoutGroup.gameObject.transform);
		item.transform.localPosition = Vector3.zero;
		item.transform.localRotation = Quaternion.identity;
		item.transform.localScale = Vector3.one;

		scrollRect.UpdateLayout();
		scrollRect.ScrollTo( scrollRect.lastItemPosition, true );
	}

	public void RemoveItem()
	{
		GameObject item = scrollRect.GetSelectedItem();
		
		if( item != null )
		{
			DestroyImmediate(item);
			
			scrollRect.UpdateLayout();
			scrollRect.ScrollToNearItem(true);

			//The following is also fine instead of the above, but scrolling is seen.
			//Destroy(item);
		}
	}

	[SerializeField] Text	scrollButtonText;
	int scrollTargetItemIndex = 0;

	public void NextItem()
	{
		int childCount = layoutGroup.transform.childCount;

		if( childCount > 0 )
		{
			scrollTargetItemIndex = (scrollTargetItemIndex+1) % childCount;
		}
	}

	public void PrevItem()
	{
		int childCount = layoutGroup.transform.childCount;

		if( childCount > 0 )
		{
			scrollTargetItemIndex = (scrollTargetItemIndex+childCount-1) % childCount;
		}
	}

	public void ScrollTo()
	{
		Transform item = layoutGroup.transform.GetChild(scrollTargetItemIndex);

		if( item != null )
		{
			Picker.PickerItem pickerItem = item.GetComponent<Picker.PickerItem>();
			scrollRect.ScrollTo( pickerItem );
			//or scrollRect.ScrollTo( pickerItem.position );
		}
	}

	void Update()
	{
		Transform item = null;

		{
			int childCount = layoutGroup.transform.childCount;

			if( childCount > 0 )
			{
				scrollTargetItemIndex = Mathf.Clamp( scrollTargetItemIndex, 0, childCount-1 );
				item = layoutGroup.transform.GetChild(scrollTargetItemIndex);
			}
		}
		
		if( item != null )
		{
			scrollButtonText.text = "Scroll to " + item.name;
		}
		else
		{
			scrollButtonText.text = "No scroll";
			scrollTargetItemIndex = 0;
		}

		scrollButtonText.GetComponentInParent<UnityEngine.UI.Button>().interactable = (item != null);
	}

#endregion

#region ItemLayout

	public void SetChildPivot( float pivot )
	{
		layoutGroup.childPivot = pivot;
	}

	public void SetSpacing( float spacing )
	{
		layoutGroup.spacing = spacing;
	}

	public void SetVertical( bool on )
	{
		if( on )
		{
			scrollRect.layout = RectTransform.Axis.Vertical;
			SetItemSize(verticalItemSize);
			SetImage(RectTransform.Axis.Vertical);
		}
	}

	public void SetHorizontal( bool on )
	{
		if( on )
		{
			scrollRect.layout = RectTransform.Axis.Horizontal;
			SetItemSize(horizontalItemSize);
			SetImage(RectTransform.Axis.Horizontal);
		}
	}

	readonly Vector2 verticalItemSize = new Vector2(60,25);
	readonly Vector2 horizontalItemSize = new Vector2(20,95);

	protected void SetItemSize( Vector2 size )
	{
		foreach( RectTransform child in scrollRect.content )
		{
			child.sizeDelta = size;
		}
	}

	[SerializeField] Image glass = null;
	[SerializeField] Sprite verticalGlass = null;
	[SerializeField] Sprite horizontalGlass = null;

	protected void SetImage( RectTransform.Axis axis )
	{
		RectTransform rectTransform = glass.rectTransform;

		int index = (int)axis;
		Vector2 anchorMin = Vector2.zero, anchorMax = Vector2.zero;
		anchorMin[1-index] = 0;
		anchorMin[index] = 0.5f;
		anchorMax[1-index] = 1;
		anchorMax[index] = 0.5f;
		rectTransform.anchorMin = anchorMin;
		rectTransform.anchorMax = anchorMax;

		Vector2 size = Vector2.zero;
		size[1-index] = -20;
		size[index] = 25;
		rectTransform.sizeDelta = size;

		glass.sprite = ( axis == RectTransform.Axis.Horizontal ? verticalGlass : horizontalGlass );
	}

#endregion

#region MovementType

	public void SetInfinite( bool on )
	{
		if( on )
		{
			scrollRect.movementType = ScrollRect.MovementType.Unrestricted;
		}
	}

	public void SetElastic( bool on )
	{
		if( on )
		{
			scrollRect.movementType = ScrollRect.MovementType.Elastic;
		}
	}

	public void SetClamped( bool on )
	{
		if( on )
		{
			scrollRect.movementType = ScrollRect.MovementType.Clamped;
		}
	}

#endregion

#region Effect

	public void SetWheelEffect( bool on )
	{
		scrollRect.wheelEffect = on;
	}

	public void SetWheelPerspective( float value )
	{
		scrollRect.wheelPerspective = value;
	}

#endregion

#region Inertia

	public void SetInertiaEnabled( bool on )
	{
		scrollRect.inertia = on;
	}

	public void SetDecelerationRate( float rate )
	{
		scrollRect.decelerationRate = rate;
	}

	public void SetSlipVelocityRate( float rate )
	{
		scrollRect.slipVelocityRate = rate;
	}

#endregion

#region Other

	public void SetScrollSecondsFromClick( float seconds )
	{
		scrollRect.autoScrollSeconds = seconds;
	}

	public void Reset()
	{
		SetSlipVelocityRate(0.25f);
		SetScrollSecondsFromClick(0.65f);
	}

#endregion

}