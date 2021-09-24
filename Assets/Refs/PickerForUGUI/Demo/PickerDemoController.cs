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

public class PickerDemoController : MonoBehaviour
{
	void Awake()
	{
		GameObject.DontDestroyOnLoad(gameObject);
		Application.LoadLevel("CustomSkinDemo");
	}

	void OnGUI()
	{
		float scale = Camera.main.pixelHeight / 480f;

		GUI.matrix = Matrix4x4.Scale(new Vector3(scale,scale,1));

		GUILayout.BeginHorizontal();

		if( GUILayout.Button("Parameter Demo") )
		{
			Application.LoadLevel("ParameterDemo");
		}

		if( GUILayout.Button("Custom Skin Demo") )
		{
			Application.LoadLevel("CustomSkinDemo");
		}

		if( GUILayout.Button("Util Demo") )
		{
			Application.LoadLevel("UtilDemo");
		}

		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();

		if( GUILayout.Button("Object Item Demo") )
		{
			Application.LoadLevel("ObjectItemDemo");
		}

		if( GUILayout.Button("Animated Item Demo") )
		{
			Application.LoadLevel("AnimatedItemDemo");
		}

		if( GUILayout.Button("Massive Picker Demo") )
		{
			Application.LoadLevel("MassiveDemo");
		}

		GUILayout.EndHorizontal();
	}
}

