using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonAnimationController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

	[SerializeField] 
	private Transform buttonImg;

	public float imgHeight;

	public void OnPointerDown(PointerEventData data)
	{
		buttonImg.DOLocalMove(new Vector3(0,0,0), 0.2f);
	}

	public void OnPointerUp(PointerEventData data)
	{
		buttonImg.DOLocalMove(new Vector3(0,imgHeight,0), 0.2f);
	}
}
