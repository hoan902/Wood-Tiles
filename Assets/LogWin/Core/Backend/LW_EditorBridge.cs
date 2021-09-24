using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LogWinInternal
{
	public static class LW_EditorBridge
	{
		public static Queue<Event> sEvents = new Queue<Event>();

		public static Vector2 mScrollHistory;

		public enum eMode
		{
			Logs = 0,
			History,
			StackTrace,
			Options,

			COUNT
		}

		public class Event
		{
			public enum eEventType
			{
				ElementSelected,
				ElementUnselected,
				ShowElementStackTrace,
				ShowElementHistory,
				ShowLogs,
				ShowOptions,
				JumpToFrame
			}

			public Event(eEventType eventType, object linkedObject)
			{
				mEventType = eventType;
				mLinkedItem = linkedObject;
			}

			public eEventType mEventType;
			public object mLinkedItem;
		}

		public static void RegisterEvent(Event.eEventType eventType, object linkedObject = null)
		{
			sEvents.Enqueue(new Event(eventType, linkedObject));
		}

		public static Event GetEvent()
		{
			if (sEvents.Count <= 0)
				return null;
			return sEvents.Dequeue();
		}

	}
}

