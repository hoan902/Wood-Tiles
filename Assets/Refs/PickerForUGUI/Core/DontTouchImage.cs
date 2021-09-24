using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Picker
{
	
	[AddComponentMenu("UI/Picker/DontTouchImage",1030), DisallowMultipleComponent]
	public class DontTouchImage : Image
	{
		public override bool IsRaycastLocationValid (Vector2 screenPoint, Camera eventCamera)
		{
			return false;
		}
	}

}