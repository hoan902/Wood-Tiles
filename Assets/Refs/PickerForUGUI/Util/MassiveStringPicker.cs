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
	[AddComponentMenu("UI/Picker/MassiveStringPicker",1032)]
	public class MassiveStringPicker : MassivePicker<MassivePickerItem,string,StringList>
	{
	}

}
