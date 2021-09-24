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

	public class InfiniteImage : Image
	{
		protected override void Start()
		{
			if( Application.isPlaying )
			{
				parent = transform.parent.GetComponent<RectTransform>();
				scrollRect = parent.GetComponent<PickerScrollRect>();
				self = (RectTransform)transform;

				int layout = (int)scrollRect.layout;
				Vector2 sizeDelta = self.sizeDelta;
				sizeDelta[layout] += self.rect.size[layout];
				self.sizeDelta = sizeDelta;

				content = scrollRect.content;
			}
		}

		PickerScrollRect scrollRect;
		RectTransform parent;
		RectTransform self;
		RectTransform content;

		void LateUpdate()
		{
			if( Application.isPlaying )
			{
				int layout = (int)scrollRect.layout;
				Vector2 position = content.anchoredPosition;
				position[layout] %= sprite.rect.size[layout];
				self.anchoredPosition = position;
			}
		}
	}


}