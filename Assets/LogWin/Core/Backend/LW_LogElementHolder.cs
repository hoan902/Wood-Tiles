using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LogWinInternal
{
	public class LW_LogElementHolder
	{
		public List<LW_LogElement> mElementsHistory = new List<LW_LogElement>();
		public List<LW_LogElement> mElementsHistoryFrozen = new List<LW_LogElement>();

		const int GRAPH_HEIGHT = 50;

		public LW_LogElement mCurrentElement;

		public LW_LogElement mCurrentElementForFrame;
		int mCurElementFrameSearched = -1;


		public string mKey;
		public int mTotalElements;
		public bool mHaveGraph = false;
		public int mGraphElemCount = 100;
		int mGraphValueMin = 0;
		int mGraphValueMax = 0;
		bool mGraphMinSet = false;
		bool mGraphMaxSet = false;


		//editor stuff, to clean
		int mItemCountLayoutPhase;

		public LW_LogElementHolder(string key)
		{
			mKey = key;
		}

		public void Clear()
		{
			mCurrentElement = null;
			mCurrentElementForFrame = null;
			foreach(LW_LogElement e in mElementsHistory)
			{
				e.Clear();
			}
			foreach (LW_LogElement e in mElementsHistoryFrozen)
			{
				e.Clear();
			}
			mElementsHistoryFrozen.Clear();
			mElementsHistory.Clear();
		}

		public void RemoveElement(LW_LogElement elem)
		{
			mElementsHistory.Remove(elem);
			mElementsHistoryFrozen.Remove(elem);
		}

		public void AddElement(LW_LogElement elem)
		{
			mElementsHistory.Add(elem);

			bool _limitHistorySize = LW_Prefs.limitHistorySize || !LW_Prefs.keepHistory;
			int _maxHistoPerElem = LW_Prefs.keepHistory ? LW_Prefs.maxHistoryPerElement : 1;
			if (_limitHistorySize && _maxHistoPerElem >= 0 && mElementsHistory.Count >= _maxHistoPerElem)
			{
				if(mElementsHistory.Count >= _maxHistoPerElem)
				{
					mElementsHistory.RemoveRange(0, mElementsHistory.Count - _maxHistoPerElem);
				}
			}

			
			mCurrentElement = elem;
			mTotalElements++;

			elem.SetHolder(this);
			elem.SetCallNumber(mTotalElements);
		}

		public float GetHistoryScrollValueForElement(LW_LogElement elem)
		{
			if (elem == null)
				return 0;
			LW_LogElement[] _elems;
			if (LW_Prefs.sTMP_HistoryFrozen)
				_elems = mElementsHistoryFrozen.ToArray();
			else
				_elems = mElementsHistory.ToArray();
			if (_elems.Length == 0)
				return 0;

			int _firstElemId = _elems[0].mCallNumber;
			int _lastElemId = _elems[_elems.Length - 1].mCallNumber;
			int _range = _lastElemId - _firstElemId;
			int _curValue = elem.mCallNumber - _firstElemId;

			if (_range <= 0)
				return 0;
			return ((float)_curValue / (float)_range) * ((float)LW_LogElement.DEFAULT_HEIGHT * _range);
		}

		public void FreezeHistory()
		{
			mElementsHistoryFrozen.Clear();
			mElementsHistoryFrozen.AddRange(mElementsHistory);
		}

		LW_LogElement GetElementForFrame(int frame)
		{
			if (mElementsHistoryFrozen == null || mElementsHistoryFrozen.Count == 0)
				return null;
			for(int i = mElementsHistoryFrozen.Count - 1; i >= 0; i--)
			{
				if (mElementsHistoryFrozen[i].mFrame <= frame)
					return mElementsHistoryFrozen[i];
			}
			return null;
		}

		public LW_LogElement GetPreviousHistoryElement(LW_LogElement elem)
		{
			if (mElementsHistoryFrozen == null || mElementsHistoryFrozen.Count == 0)
				return null;
			for (int i = mElementsHistoryFrozen.Count - 1; i >= 0; i--)
			{
				if (mElementsHistoryFrozen[i] == elem)
				{
					if(i > 0)
						return mElementsHistoryFrozen[i - 1];
					return null;
				}
			}
			return null;
		}

		public LW_LogElement GetNextHistoryElement(LW_LogElement elem)
		{
			if (mElementsHistoryFrozen == null || mElementsHistoryFrozen.Count == 0)
				return null;
			if(elem == null)
			{
				return mElementsHistoryFrozen[0];
			}
			for (int i = mElementsHistoryFrozen.Count - 1; i >= 0; i--)
			{
				if (mElementsHistoryFrozen[i] == elem)
				{
					if (i < mElementsHistoryFrozen.Count - 1)
						return mElementsHistoryFrozen[i + 1];
					return null;
				}
			}
			return null;
		}

		bool HavePrevElement()
		{
			if (mElementsHistoryFrozen.Count == 0 || mCurrentElementForFrame == null)
				return false;
			return mElementsHistoryFrozen[0] != mCurrentElementForFrame;
		}

		bool HaveNextElement()
		{
			if (mElementsHistoryFrozen.Count == 0)
				return false;
			return mElementsHistoryFrozen[mElementsHistoryFrozen.Count - 1] != mCurrentElementForFrame;
		}

		Rect BeginElemDraw(bool odd, bool isSelected, bool isRealtimeFrame, int curFrame = 0)
		{
#if UNITY_EDITOR
			if (!isRealtimeFrame)
			{
				EditorGUILayout.BeginHorizontal(GUILayout.Height(LW_LogElement.DEFAULT_HEIGHT));
				bool _havePrevElem = HavePrevElement();

				Rect _rect = EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button(_havePrevElem ? LW_Tools.sPrevFrameIcon : LW_Tools.sPrevFrameNotAvailableIcon, LW_Tools.sBtnPrevNextNoBgStyle, GUILayout.Width(20), GUILayout.Height(LW_LogElement.DEFAULT_HEIGHT)))
				{
					LW_LogElement _prevElem = GetPreviousHistoryElement(mCurrentElementForFrame);
					if (_prevElem != null)
						_prevElem.JumpToFrame();
				}
				EditorGUILayout.EndHorizontal();
				if (_havePrevElem && _rect.Contains(Event.current.mousePosition))
				{
					LW_LogElement _prevElem = GetPreviousHistoryElement(mCurrentElementForFrame);
					if (_prevElem != null)
					{
						DrawNextPrevElementTooltip(_rect, curFrame, _prevElem.mFrame, false);

						//if (mCurrentElementForFrame != null)
						//	DrawNextPrevElementTooltip(_rect, curFrame, _prevElem.mFrame, false);
						//else
						//	DrawNextPrevElementTooltip(_rect, curFrame, _prevElem.mFrame, false);
					}
				}
			}
			Rect _btnRect;
			if (isSelected)
			{
				_btnRect = EditorGUILayout.BeginHorizontal(LW_Tools.sElementSelectedStyle, GUILayout.Height(LW_LogElement.DEFAULT_HEIGHT));
			}
			else
			{
				if (odd)
					_btnRect = EditorGUILayout.BeginHorizontal(LW_Tools.sElementBoxStyle0, GUILayout.Height(LW_LogElement.DEFAULT_HEIGHT));
				else
					_btnRect = EditorGUILayout.BeginHorizontal(LW_Tools.sElementBoxStyle1, GUILayout.Height(LW_LogElement.DEFAULT_HEIGHT));
			}

			return _btnRect;
#else
			return new Rect();
#endif
		}

		void DrawEmptyElem(bool odd, bool isSelected, LW_EditorBridge.eMode mode, bool isRealtimeFrame, int curFrame)
		{
#if UNITY_EDITOR
			Rect _rect = BeginElemDraw(odd, isSelected, isRealtimeFrame, curFrame);
			GUILayout.Label(mKey);
			GUILayout.FlexibleSpace();
			GUILayout.Label("NO VALUE");

			if (GUI.Button(_rect, GUIContent.none, GUIStyle.none))
			{
				if (Event.current.button == 1)
				{
					if (LW_Prefs.collectStackTrace || LW_Prefs.keepHistory)
					{
						GenericMenu _menu = new GenericMenu();
						if (LW_Prefs.keepHistory && mode != LW_EditorBridge.eMode.History)
						{
							_menu.AddItem(new GUIContent("History"), false, () => { LW_EditorBridge.RegisterEvent(LW_EditorBridge.Event.eEventType.ShowElementHistory, this); });
						}

						if (!isRealtimeFrame)
						{
							LW_LogElement _nextElem = mElementsHistoryFrozen[0];

							if (_nextElem != null)
								_menu.AddSeparator("");
							if (_nextElem != null)
								_menu.AddItem(new GUIContent("Jump to next entry"), false, () => { _nextElem.JumpToFrame(); });
						}
						_menu.AddSeparator("");
						_menu.AddItem(new GUIContent("Close"), false, () => { });
						_menu.ShowAsContext();
					}
				}
				else if (Event.current.button == 0)
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

			EndElemDraw(isRealtimeFrame, curFrame);
#endif
		}

		void EndElemDraw(bool isRealtimeFrame, int curFrame = 0)
		{
#if UNITY_EDITOR

			GUILayout.EndHorizontal();
			if (!isRealtimeFrame)
			{
				bool _haveNextElem = HaveNextElement();
				Rect _rect = EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button(_haveNextElem ? LW_Tools.sNextFrameIcon : LW_Tools.sNextFrameNotAvailableIcon, LW_Tools.sBtnPrevNextNoBgStyle, GUILayout.Width(20), GUILayout.Height(LW_LogElement.DEFAULT_HEIGHT)))
				{
					LW_LogElement _nextElem = GetNextHistoryElement(mCurrentElementForFrame);
					if (_nextElem != null)
						_nextElem.JumpToFrame();
				}
				EditorGUILayout.EndHorizontal();
				if (_haveNextElem && _rect.Contains(Event.current.mousePosition))
				{
					LW_LogElement _nextElem = GetNextHistoryElement(mCurrentElementForFrame);
					if (_nextElem != null)
					{
						DrawNextPrevElementTooltip(_rect, curFrame, _nextElem.mFrame, true);

						//if (mCurrentElementForFrame != null)
						//	DrawNextPrevElementTooltip(_rect, curFrame, _nextElem.mFrame, true);
						//else
						//	DrawNextPrevElementTooltip(_rect, curFrame, _nextElem.mFrame, true);
					}
					

				}

				EditorGUILayout.EndHorizontal();
			}
#endif
		}

		void DrawNextPrevElementTooltip(Rect rect, int curElemFrame, int otherElemFrame, bool isNext)
		{
#if UNITY_EDITOR

			GUIContent _content = null;
			int _frameDiff = (otherElemFrame - curElemFrame);
			string _toAddS = "";
			if (_frameDiff > 1 || _frameDiff < -1)
				_toAddS = "s";

			if (isNext)
				_content = new GUIContent("+" + _frameDiff + " frame" + _toAddS);
			else
			{
				if(_frameDiff == 0)
					_content = new GUIContent("-" + _frameDiff + " frame" + _toAddS);
				else
					_content = new GUIContent(_frameDiff + " frame" + _toAddS);
			}


			GUIStyle _style = LW_Tools.sElementTooltipFullGrey;
			Vector2 _widthHeight = _style.CalcSize(_content);
			float _topDecal = rect.y - _widthHeight.y;
			
			if (_topDecal < 0)
			{
				rect.y -= _topDecal;
			}
			float _x = rect.x;
			if (isNext)
				_x = rect.x + rect.width - _widthHeight.x;

			EditorGUI.LabelField(new Rect(_x, rect.y - _widthHeight.y, _widthHeight.x, _widthHeight.y), _content, _style);
#endif
		}

		void DrawHoverTooltip(Rect rect, int curFrame, LW_LogElement elem)
		{
#if UNITY_EDITOR

			GUIContent _content = null;
			int _frameDiff = (curFrame - elem.mFrame);

			if(_frameDiff != 0)
				_content = new GUIContent("Frame:" + elem.mFrame + "(" + _frameDiff + ")" + "\nEntry count:" + elem.mCallNumber);
			else
				_content = new GUIContent("Frame:" + elem.mFrame + "\nEntry count:" + elem.mCallNumber);

			GUIStyle _style = LW_Tools.sElementTooltipFullGrey;
			Vector2 _widthHeight = _style.CalcSize(_content);
			float _topDecal = rect.y - _widthHeight.y;
			if (_topDecal < 0)
			{
				rect.y -= _topDecal;
			}
			EditorGUI.LabelField(new Rect(rect.x + rect.width - _widthHeight.x, rect.y - _widthHeight.y, _widthHeight.x, _widthHeight.y), _content, _style);
#endif
		}

		void DrawGraph(Rect _rect, bool odd, bool isSelected)
		{
#if UNITY_EDITOR
			Rect _containerRect = new Rect(_rect.x, _rect.y + _rect.height, _rect.width, GRAPH_HEIGHT);
			if (isSelected)
			{
				EditorGUI.DrawRect(_containerRect, new Color(1f, 1f, 1f, 0.5f));
			}
			else
			{
				if (odd)
					EditorGUI.DrawRect(_containerRect, new Color(0, 0, 0, .05f));
			}
			GUILayout.Space(GRAPH_HEIGHT);


			int _elemToDrawCount = mGraphElemCount;
			if (_elemToDrawCount > _rect.width)
				_elemToDrawCount = (int)_rect.width;

			//if (mElementsHistory.Count < _elemToDrawCount)
			//	_elemToDrawCount = mElementsHistory.Count;


			for (int i = mElementsHistory.Count - 1; i > mElementsHistory.Count - _elemToDrawCount; i--)
			{
				if (i < 0)
					break;
				if (!mElementsHistory[i].mHaveGraph || !mElementsHistory[i].mIsNumericalValue)
					continue;

				if (!mGraphMaxSet || mGraphValueMax < (int)mElementsHistory[i].mValue)
				{
					mGraphMaxSet = true;
					mGraphValueMax = (int)mElementsHistory[i].mValue;
				}
				if (!mGraphMinSet || mGraphValueMin > (int)mElementsHistory[i].mValue)
				{
					mGraphMinSet = true;
					mGraphValueMin = (int)mElementsHistory[i].mValue;
				}
				//EditorGUI.DrawRect(_rect.x + _rect.width - i, _rect.y, 1, );
			}

			int _elementDrawn = 0;
			int _elemWidth = 1; 
			if(_elemToDrawCount != 0)
				_elemWidth = (int)_rect.width / _elemToDrawCount;
			if (_elemWidth < 1)
				_elemWidth = 1;
			float _LerpedVal = 0;

			
			for (int i = mElementsHistory.Count - 1; i > mElementsHistory.Count - _elemToDrawCount; i--)
			{
				if (i < 0)
					break;
				_elementDrawn++;
				if (!mElementsHistory[i].mIsNumericalValue)
					continue;
				_LerpedVal = Mathf.InverseLerp(mGraphValueMin, mGraphValueMax, (int)mElementsHistory[i].mValue);

				EditorGUI.DrawRect(	new Rect(_containerRect.x + _containerRect.width - (_elementDrawn * _elemWidth),
												_containerRect.y + _containerRect.height, 
												_elemWidth, 
												-(int)(_LerpedVal * (float)GRAPH_HEIGHT)), 
												mElementsHistory[i].mPastilleColor.a == 0 ? new Color(.5f,.5f,.5f,.5f) : mElementsHistory[i].mPastilleColor);
			}
#endif
		}

		public bool GUIDrawLogs(bool odd = true, bool isSelected = false, EventModifiers modifier = EventModifiers.None, LW_EditorBridge.eMode mode = LW_EditorBridge.eMode.Logs, bool isRealtimeFrame = true, int curFrame = 0)
		{
			if (isRealtimeFrame)
			{
				Rect _rect = BeginElemDraw(odd, isSelected, isRealtimeFrame, curFrame);

				mCurrentElement.GUIDrawSelf(_rect, odd, isSelected, modifier, mode);
				EndElemDraw(isRealtimeFrame, curFrame);

				if (mHaveGraph)
					DrawGraph(_rect, odd, isSelected);

				if (_rect.Contains(Event.current.mousePosition))
				{
					DrawHoverTooltip(_rect, curFrame, mCurrentElement);
				}
				return true;
			}
			else
			{
				if(curFrame != mCurElementFrameSearched)
					mCurrentElementForFrame = GetElementForFrame(curFrame);
				mCurElementFrameSearched = curFrame;
				if(mCurrentElementForFrame != null)
				{
					Rect _rect = BeginElemDraw(odd, isSelected, isRealtimeFrame, curFrame);
					mCurrentElementForFrame.GUIDrawSelf(_rect, odd, isSelected, modifier, mode, true, curFrame == mCurrentElementForFrame.mFrame, isRealtimeFrame, curFrame);
					EndElemDraw(isRealtimeFrame, curFrame);
					if (_rect.Contains(Event.current.mousePosition))
					{
						DrawHoverTooltip(_rect, curFrame, mCurrentElementForFrame);
					}
					return true;
				}
				else
				{
					DrawEmptyElem(odd, isSelected, mode, isRealtimeFrame, curFrame);
				}
			}



			return false;
		}

		public void GUIDrawHistory(Rect position, LW_LogElement selectedElem)
		{
#if UNITY_EDITOR
			LW_Tools.SetupStyles();

			bool _style0 = true;

			LW_EditorBridge.mScrollHistory = GUILayout.BeginScrollView(LW_EditorBridge.mScrollHistory);

			EventModifiers _curModifier = LW_Tools.GetCurrentModifier();

			float _lineHeight = LW_LogElement.DEFAULT_HEIGHT;//LW_Tools.sElementBoxStyle0.CalcHeight(new GUIContent("A"), 40);
			int _startI = (int)(LW_EditorBridge.mScrollHistory.y / _lineHeight);
			int _curElementId = _startI;
			int _elemCount = 0;
			float _curHeight = 0;
			bool _needSnapToLayout = Event.current.type != EventType.Layout;

			
			if (_startI < 0)
				_startI = 0;
			GUILayout.Space(_startI * _lineHeight);

			_style0 = _startI % 2 == 0;

			LW_LogElement[] _elems;
			if(LW_Prefs.sTMP_HistoryFrozen)
				_elems = mElementsHistoryFrozen.ToArray();
			else
				_elems = mElementsHistory.ToArray();

			bool _selected = false;

			for (int i = _startI; i < _elems.Length; i++)
			{
				_selected = selectedElem == _elems[i];

				Rect _rect = BeginElemDraw(_style0, _selected, true);
				_elems[i].GUIDrawSelf(_rect, _style0, _selected, _curModifier, LW_EditorBridge.eMode.History);
				EndElemDraw(true);

				_curHeight += _lineHeight;
				_curElementId = i;
				_elemCount++;
				if (_needSnapToLayout && _elemCount >= mItemCountLayoutPhase)
					break;
				if (_curHeight > (position.height + 100))	//todo : fix this disgusting 100, it is here to compensate header so we can snap to bot with scroll
					break;

				_style0 = !_style0;
			}
			if (Event.current.type == EventType.Layout)
				mItemCountLayoutPhase = _elemCount;

			GUILayout.Space((_elems.Length - _curElementId - 1) * _lineHeight);
			GUILayout.EndScrollView();
#endif
		}
	}
}

