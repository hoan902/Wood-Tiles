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
	
	[AddComponentMenu("UI/Picker/MassiveZoomPLG",1015), DisallowMultipleComponent]
	public class MassiveZoomPickerLayoutGroup : MassivePickerLayoutGroup
	{
		public Transform		zoomItemParent;
	}

}
