using UnityEngine;
using System.Collections;

public class PickerGotoButton : MonoBehaviour 
{
	[SerializeField] int m_Index;
	[SerializeField] Picker.MassivePickerScrollRect		m_ScrollRect;

	public void OnClick()
	{
		m_ScrollRect.ScrollAt( m_Index );
	}
}
