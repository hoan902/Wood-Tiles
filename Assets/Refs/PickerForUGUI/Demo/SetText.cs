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

public class SetText : MonoBehaviour
{
	void Set( string text )
	{
		Text textComponent = GetComponent<Text>();

		if( textComponent != null )
		{
			textComponent.text = text;
		}
	}

	public void SetGameObjectName( GameObject obj )
	{
		Set(obj.name);
	}

	public void SetFloat( float f )
	{
		Set ( f.ToString("F2") );
	}
}

