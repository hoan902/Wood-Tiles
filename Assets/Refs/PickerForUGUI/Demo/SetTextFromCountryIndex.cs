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

public class SetTextFromCountryIndex : MonoBehaviour
{
	protected Text	m_Text;

	void Awake()
	{
		m_Text = GetComponent<Text>();
	}

	public void SetItemText( int i )
	{
		string name = ExampleCountryNameItem.CountryNames[i];
		m_Text.text = name;
	}
}
