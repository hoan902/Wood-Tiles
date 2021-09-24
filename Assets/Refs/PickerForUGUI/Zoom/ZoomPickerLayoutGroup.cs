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
	
	[AddComponentMenu("UI/Picker/ZoomPickerLayoutGroup",1010), DisallowMultipleComponent]
	public class ZoomPickerLayoutGroup : PickerLayoutGroup
	{
		public Transform		zoomItemParent;
	}

}
