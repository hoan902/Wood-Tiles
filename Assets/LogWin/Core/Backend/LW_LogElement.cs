using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace LogWinInternal
{
	public class LW_LogElement
	{
		public enum eLogType
		{
			log,
			warning,
			error
		}

		public class ComparerId : IComparer<LW_LogElement>
		{
			public int Compare(LW_LogElement x, LW_LogElement y)
			{
				return x.mId.CompareTo(y.mId);
			}
		}


		public LW_LogCategory mParentCategory;
		public LW_LogElementHolder mHolder;

		public List<LW_LogElement> mElementsHistoryFrozen = new List<LW_LogElement>();

		public const int DEFAULT_HEIGHT = 20;

		public int mFrame;
		public float mTime;
		public object mValue;
		public LW_StackTrace mStackTrace;
		public int mCallNumber = 0;
		bool mIsDecimalFromat;
		public bool mIsNumericalValue;
		public bool mHaveGraph = false;
		public eLogType mLogType;

		public long mId;
		

		public Color mPastilleColor = new Color(0,0,0,0);

		public static long sElemCount = 0;
		public static long sElemTotal = 0;

		public void SetNeedGraph(int elemCount)
		{
			mHaveGraph = true;
			if(mHolder != null)
			{
				mHolder.mHaveGraph = true;
				mHolder.mGraphElemCount = elemCount;
			}
		}

		public void RemoveFromHolder()
		{
			if(mHolder != null)
				mHolder.RemoveElement(this);
		}

		public void Clear()
		{
			mElementsHistoryFrozen.Clear();
			mStackTrace = null;
			mHolder = null;
			mParentCategory = null;
		}

		public void SetHistory(List<LW_LogElement> elemList)
		{
			mElementsHistoryFrozen.AddRange(elemList);
		}

		public void SetHistory(Queue<LW_LogElement> elemList)
		{
			mElementsHistoryFrozen.AddRange(elemList);
		}

		void PreventWarning()
		{
#if !UNITY_EDITOR
			if(mIsDecimalFromat){}
#endif
		}

		public LW_LogElement(object value, LW_LogCategory parentCategory, eLogType logType = eLogType.log)
		{
			mId = sElemTotal;
			sElemTotal++;
			sElemCount++;

			//Logwin_Internal.PushNewLog(this);

			if (LW_Prefs.collectStackTrace)
			{
				mStackTrace = LW_StackTrace.GenerateStackTrace(4);
			}
			Init(value, parentCategory, logType);
		}


		void Init(object value, LW_LogCategory parentCategory, eLogType logType)
		{
			mValue = value;
			mFrame = Time.frameCount;
			mTime = Time.time;
			mLogType = logType;
			mParentCategory = parentCategory;
			mIsDecimalFromat = value is float || value is double;
			mIsNumericalValue = LW_Tools.IsNumericType(value.GetType());
		}

		public void SetPastilleColor(Color pastilleColor)
		{
			mPastilleColor = pastilleColor;
		}

		public void SetHolder(LW_LogElementHolder holder)
		{
			mHolder = holder;
		}

		public void SetCallNumber(int nb)
		{
			mCallNumber = nb;
		}

		~LW_LogElement()
		{
			sElemCount--;
		}

		void DrawHeader()
		{
#if UNITY_EDITOR
			if (mHolder != null)
			{
				Rect _boxRect = EditorGUILayout.BeginHorizontal("box");
				if (LW_Prefs.drawColorOnLeft)
				{
					if (mPastilleColor.a != 0)
						EditorGUI.DrawRect(new Rect(_boxRect.x + 1, _boxRect.y + 1, 9, _boxRect.height - 2), mPastilleColor);
					EditorGUILayout.Space();
				}

				GUILayout.Label(mHolder.mKey);
				GUILayout.FlexibleSpace();
				GUILayout.Label(mValue.ToString());
				if (!LW_Prefs.drawColorOnLeft)
				{
					if (mPastilleColor.a != 0)
						EditorGUI.DrawRect(new Rect(_boxRect.x + _boxRect.width - 10, _boxRect.y + 1, 9, _boxRect.height - 2), mPastilleColor);

					EditorGUILayout.Space();
				}
				GUILayout.EndHorizontal();
			}
#endif
		}

		public void GUIDrawStack(Rect position)
		{
#if UNITY_EDITOR
			ProcessStackIfNeeded();

			GUILayout.BeginVertical();

			DrawHeader();

			if (mStackTrace != null)
				mStackTrace.GUIDraw(position, this);
			GUILayout.EndVertical();
#endif
		}

		public void GUIDrawHistory(Rect position)
		{
#if UNITY_EDITOR
			ProcessStackIfNeeded();

			GUILayout.BeginVertical();

			DrawHeader();

			if (mHolder != null)
				mHolder.GUIDrawHistory(position, this);
			GUILayout.EndVertical();
#endif
		}
		/*
		public static Rect GUIDraw(Rect rect, bool odd = true, bool isSelected = false, EventModifiers modifier = EventModifiers.None, LW_EditorBridge.eMode mode = LW_EditorBridge.eMode.Logs, bool showNew = false, bool isNew = false, bool isRealtimeFrame = true, int curFrame = -1)
		{
			return GUIDrawSelf(rect, odd, isSelected, modifier, mode, showNew, isNew, isRealtimeFrame, curFrame);
		}
		*/
		public Rect GUIDrawSelf(Rect rect, bool odd = true, bool isSelected = false, EventModifiers modifier = EventModifiers.None, LW_EditorBridge.eMode mode = LW_EditorBridge.eMode.Logs, bool showNew = false, bool isNew = false, bool isRealtimeFrame = true, int curFrame = -1)
		{
#if UNITY_EDITOR
			EventType _curFrameEvent = Event.current.type;

			switch (mLogType)
			{
				case eLogType.log:
					
					break;
				case eLogType.warning:
					EditorGUI.LabelField(new Rect(rect.x + rect.width/2f - rect.height / 2f, rect.y, rect.height, rect.height), LW_Tools.sWarningIcon);
					break;
				case eLogType.error:
					EditorGUI.LabelField(new Rect(rect.x + rect.width / 2f - rect.height / 2f, rect.y, rect.height, rect.height), LW_Tools.sErrorIcon);
					break;
			}

			
			switch (mode)
			{
				case LW_EditorBridge.eMode.Logs:
					if (LW_Prefs.drawColorOnLeft)
					{
						if (mPastilleColor.a != 0)
							EditorGUI.DrawRect(new Rect(rect.x, rect.y, 10, rect.height), mPastilleColor);

						EditorGUILayout.Space();
					}

					if (LW_Prefs.displayCallCount)
					{
						if(!isSelected)
							GUILayout.Label(mHolder.mKey + " (" + (mCallNumber).ToString() + ")");
						else
							GUILayout.Label(mHolder.mKey + " (" + (mCallNumber).ToString() + ")", LW_Tools.sSelectedElementLabel);
					}
					else
					{
						if (!isSelected)
							GUILayout.Label(mHolder.mKey);
						else
							GUILayout.Label(mHolder.mKey, LW_Tools.sSelectedElementLabel);
					}
					GUILayout.FlexibleSpace();
					if (LW_Prefs.truncateNumberToDecimalPlaces && mIsDecimalFromat)
					{
						if (!isSelected)
							GUILayout.Label(LW_Tools.TruncateDecimal(mValue, LW_Prefs.decimalToKeep).ToString());
						else
							GUILayout.Label(LW_Tools.TruncateDecimal(mValue, LW_Prefs.decimalToKeep).ToString(), LW_Tools.sSelectedElementLabel);
					}
					else
					{
						if (!isSelected)
							GUILayout.Label(mValue.ToString());
						else
							GUILayout.Label(mValue.ToString(), LW_Tools.sSelectedElementLabel);
					}
					if (showNew)
					{
						if (!isNew)
						{
							Rect _rect = EditorGUILayout.BeginHorizontal();
							GUILayout.Label(LW_Tools.sIconNotNew);
							//GUILayout.Label(isNew ? LW_Tools.sIconNew : LW_Tools.sIconNotNew);
							EditorGUILayout.EndHorizontal();
							if (_rect.Contains(Event.current.mousePosition))
							{
								Rect _infoRect = new Rect(rect);
								_infoRect.width = _rect.x - _infoRect.x;
								EditorGUI.LabelField(_infoRect, "", LW_Tools.sElementBoxFullGrey);
								EditorGUI.HelpBox(_infoRect, "Not updated this frame", MessageType.Warning);
								//EditorGUI.HelpBox(_infoRect, GUI.tooltip, isNew ? MessageType.Info : MessageType.Warning);

								if (Event.current.type == EventType.MouseDown)
								{
									JumpToFrame();
								}
							}
						}

					}
					if (!LW_Prefs.drawColorOnLeft)
					{
						if (mPastilleColor.a != 0)
							EditorGUI.DrawRect(new Rect(rect.x + rect.width - 10, rect.y, 10, rect.height), mPastilleColor);

						EditorGUILayout.Space();
					}
					break;
				case LW_EditorBridge.eMode.History:
					if (LW_Prefs.drawColorOnLeft)
					{
						if (mPastilleColor.a != 0)
							EditorGUI.DrawRect(new Rect(rect.x, rect.y, 10, rect.height), mPastilleColor);

						EditorGUILayout.Space();
					}

					if (!isSelected)
						GUILayout.Label("id:" + mCallNumber);
					else
						GUILayout.Label("id:" + mCallNumber, LW_Tools.sSelectedElementLabel);
					GUILayout.FlexibleSpace();
					if (!isSelected)
						GUILayout.Label("Frame:" + mFrame);
					else
						GUILayout.Label("Frame:" + mFrame, LW_Tools.sSelectedElementLabel);
					GUILayout.FlexibleSpace();
					if (!isSelected)
						GUILayout.Label(mValue.ToString());
					else
						GUILayout.Label(mValue.ToString(), LW_Tools.sSelectedElementLabel);

					if (!LW_Prefs.drawColorOnLeft)
					{
						if (mPastilleColor.a != 0)
							EditorGUI.DrawRect(new Rect(rect.x + rect.width - 10, rect.y, 10, rect.height), mPastilleColor);

						EditorGUILayout.Space();
					}
					break;
				default:
					GUILayout.Label(mHolder.mKey);
					GUILayout.FlexibleSpace();
					GUILayout.Label(mValue.ToString());
					break;
			}


			if (LW_Prefs.doubleClickOnElementOpenFile && LW_Prefs.collectStackTrace && rect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown && Event.current.clickCount > 1) //doucle click
			{
				OpenFile();
			}
			else
			{
				if (GUI.Button(rect, GUIContent.none, GUIStyle.none))
				{
					if (Event.current.button == 1)      //right click
					{
						if (LW_Prefs.collectStackTrace || LW_Prefs.keepHistory)
						{
							GenericMenu _menu = new GenericMenu();
							if (LW_Prefs.collectStackTrace)
							{
								_menu.AddItem(new GUIContent("Open File"), false, () => { OpenFile(); });
								_menu.AddItem(new GUIContent("StackTrace"), false, () => { ShowStackTrace(); });
							}

							if (LW_Prefs.keepHistory && mode != LW_EditorBridge.eMode.History)
							{
								_menu.AddItem(new GUIContent("History"), false, () => { ShowHistory(); });
							}

							if (!isRealtimeFrame)
							{
								bool _curFrameIsLogFrame = mFrame == curFrame;
								LW_LogElement _prevElem = mHolder.GetPreviousHistoryElement(this);
								LW_LogElement _nextElem = mHolder.GetNextHistoryElement(this);

								if (_prevElem != null || _nextElem != null || !_curFrameIsLogFrame)
									_menu.AddSeparator("");

								if (!_curFrameIsLogFrame)
									_menu.AddItem(new GUIContent("Jump to creation frame"), false, () => { JumpToFrame(); });
								if (_prevElem != null)
									_menu.AddItem(new GUIContent("Jump to previous entry"), false, () => { _prevElem.JumpToFrame(); });
								if (_nextElem != null)
									_menu.AddItem(new GUIContent("Jump to next entry"), false, () => { _nextElem.JumpToFrame(); });
							}
							_menu.AddSeparator("");
							_menu.AddItem(new GUIContent("Close"), false, () => { });
							_menu.ShowAsContext();
						}
					}
					else if (Event.current.button == 0) //left click
					{
						if (LW_Prefs.openFileShortcut == modifier)
						{
							mHolder.mCurrentElement.OpenFile();
						}
						else if (LW_Prefs.showHistoryShortcut == modifier && mode != LW_EditorBridge.eMode.History && LW_Prefs.keepHistory)
						{
							mHolder.mCurrentElement.ShowHistory();
						}
						else if (LW_Prefs.showStacktraceShortcut == modifier)
						{
							mHolder.mCurrentElement.ShowStackTrace();
						}
						else
						{
							if (!isSelected)
								LW_EditorBridge.RegisterEvent(LW_EditorBridge.Event.eEventType.ElementSelected, this);
							else
							{
								if (mode != LW_EditorBridge.eMode.History)
									LW_EditorBridge.RegisterEvent(LW_EditorBridge.Event.eEventType.ElementUnselected, this);
							}
						}
					}
				}
			}

			return rect;
#else
			return new Rect();
#endif
		}

		public void OpenFile()
		{
			ProcessStackIfNeeded();

			mStackTrace.OpenFile();

			//AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(mFilePath), mLine);
			//UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(mFilePath, mLine);
		}

		public void JumpToFrame()
		{
			mHolder.mCurrentElementForFrame = this;
			LW_EditorBridge.RegisterEvent(LW_EditorBridge.Event.eEventType.JumpToFrame, this);
		}

		public void ShowHistory()
		{
			LW_EditorBridge.RegisterEvent(LW_EditorBridge.Event.eEventType.ShowElementHistory, this);
		}

		public void ShowStackTrace()
		{
			ProcessStackIfNeeded();
			LW_EditorBridge.RegisterEvent(LW_EditorBridge.Event.eEventType.ShowElementStackTrace, this);
		}

		
		void ProcessStackIfNeeded()
		{
			if (mStackTrace != null && mStackTrace.mProcessed == false)
				mStackTrace.Process();
		}
	}
}
