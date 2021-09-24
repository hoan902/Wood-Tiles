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

	[CustomEditor(typeof(MassiveStringPicker))]
	public class MassiveStringPickerEditor : MassivePickerEditor<MassiveStringPicker,MassivePickerItem,string,StringList>
	{

	}

}
