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
	[System.Serializable]
	public class ImageList : ItemList<Sprite>{}

	[AddComponentMenu("UI/Picker/ImagePicker",1031)]
	public class ImagePicker : Picker<PickerItem,Sprite,ImageList>
	{
		protected override void SetParameter (PickerItem item, Sprite param)
		{
			Image image = item.GetComponent<Image>();
		
			if( image == null )
			{
				image = item.gameObject.AddComponent<Image>();
			}

			image.sprite = param;
		}
	}

}
