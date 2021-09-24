#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LogWinInternal;

//icon list https://unitylist.com/p/5c3/Unity-editor-icons
//if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown && Event.current.clickCount > 1)	//double click

namespace LogWinInternal
{
	public class LW_MainWindow : EditorWindow
	{
		public static LW_MainWindow sInstance;

		static bool CAN_HAVE_UNFREEZE_HISTO = false;
		const int FOOTER_HEIGHT = 18;
		const int TOOLBAR_WIDTH_MIN_EXTENDED_MODE = 202;

		Logwin_Internal mLogSaved = null;

		Vector2 mLogScrollPos;
		Vector2 mOptionsScrollPos;

		GUIContent mPlayIcon;
		GUIContent mPauseIcon;
		GUIContent mOptionsIcon;

		LW_LogElement mSelectedLogElement;
		LW_LogElementHolder mSelectedHolder;

		string[] mModeSelectorOptions;
		string[] mModeSelectorOptionsShort;

		string[] mShortcutModSelectorOptionsStr;
		EventModifiers[] mShortcutModSelectorOptions;

		LW_EditorBridge.eMode mMode = LW_EditorBridge.eMode.Logs;

		bool mIsAtCurrentFrame = true;
		int mCurrentFrameDisplayed = -1;
		int mFrameFreezedNumber = -1;
		int mFrameCount = 0;

#if !UNITY_5
		LW_MainWindow()
		{
			EditorApplication.playModeStateChanged += HandlerPlayModeStateChanged;
			EditorApplication.pauseStateChanged += HandlerPauseModeStateChange;
		}

		void HandlerPauseModeStateChange(PauseState state)
		{
			if (!EditorApplication.isPlaying)
				return;
			switch (state)
			{
				case PauseState.Paused:
					SetIsAtCurFrame(false, false);
					break;
				case PauseState.Unpaused:
					SetIsAtCurFrame(true, false);
					break;
			}
		}

		void HandlerPlayModeStateChanged(PlayModeStateChange state)
		{
			//when playmode start : clear all remaining data / reset frame freeze and go back to main window
			if(state == PlayModeStateChange.ExitingEditMode)
			{
				if (mLogSaved != null)
					mLogSaved.ClearSavedDico();
				mLogSaved = null;
				Logwin_Internal.ClearAll();
				SetIsAtCurFrame(true, false);
				mMode = LW_EditorBridge.eMode.Logs;
			}

			//when playmode exit : freeze frames / re-init display and save data to member variable to keep them
			if(state == PlayModeStateChange.EnteredEditMode)
			{
				if (mIsAtCurrentFrame)
				{
					mFrameFreezedNumber = mFrameCount;
					SetIsAtCurFrame(false, false);
				}
				LW_Tools.SetupStyles(true);
				mLogSaved = Logwin_Internal.SaveValuesForEditMode();
			}
		}
#endif


		[MenuItem("Window/LogWin")]
		public static void OpenWindow()
		{
			sInstance = (LW_MainWindow)EditorWindow.GetWindow(typeof(LW_MainWindow));
			sInstance.titleContent = new GUIContent("LogWin");
		}

		
		void OnInspectorUpdate()
		{
			if(!LW_Prefs.updateValuesEveryFrames)
				Repaint();
		}


		private void Update()
		{
			ProcessEvents();
			if (LW_Prefs.updateValuesEveryFrames)
				Repaint();
		}

		void Init()
		{
			if(mModeSelectorOptions == null)
			{
				mModeSelectorOptions = new string[(int)LW_EditorBridge.eMode.COUNT];
				for(int i = 0; i < mModeSelectorOptions.Length; i++)
				{
					mModeSelectorOptions[i] = ((LW_EditorBridge.eMode)i).ToString();
				}
			}
			if(mModeSelectorOptionsShort == null)
			{
				mModeSelectorOptionsShort = new string[] { LW_EditorBridge.eMode.Logs.ToString(), LW_EditorBridge.eMode.History.ToString(), LW_EditorBridge.eMode.StackTrace.ToString() };

			}
			if (mShortcutModSelectorOptions == null || mShortcutModSelectorOptionsStr == null)
			{
				mShortcutModSelectorOptionsStr = new string[5];
				mShortcutModSelectorOptions = new EventModifiers[5];
				mShortcutModSelectorOptions[LW_Tools.GetShortcutId(EventModifiers.Shift)] = EventModifiers.Shift;
				mShortcutModSelectorOptions[LW_Tools.GetShortcutId(EventModifiers.Control)] = EventModifiers.Control;
				mShortcutModSelectorOptions[LW_Tools.GetShortcutId(EventModifiers.Alt)] = EventModifiers.Alt;
				mShortcutModSelectorOptions[LW_Tools.GetShortcutId(EventModifiers.Command)] = EventModifiers.Command;
				mShortcutModSelectorOptions[LW_Tools.GetShortcutId(EventModifiers.None)] = EventModifiers.None;

				mShortcutModSelectorOptionsStr[0] = mShortcutModSelectorOptions[0].ToString();
				mShortcutModSelectorOptionsStr[1] = mShortcutModSelectorOptions[1].ToString();
				mShortcutModSelectorOptionsStr[2] = mShortcutModSelectorOptions[2].ToString();
				mShortcutModSelectorOptionsStr[3] = mShortcutModSelectorOptions[3].ToString();
				mShortcutModSelectorOptionsStr[4] = mShortcutModSelectorOptions[4].ToString();
			}

			if (mPlayIcon == null)
			{
				mPlayIcon = EditorGUIUtility.IconContent("PlayButton");
			}
			if (mPauseIcon == null)
			{
				mPauseIcon = EditorGUIUtility.IconContent("PauseButton");
			}
			if (mOptionsIcon == null)
			{
				mOptionsIcon = EditorGUIUtility.IconContent("SettingsIcon");
			}
		}

		void SetSelectedLogElement(LW_LogElement logElement)
		{
			mSelectedLogElement = logElement;
		}

		void SetSelectedHolder(LW_LogElementHolder holder)
		{
			mSelectedHolder = holder;
		}

		//process events stack 
		//(event stack is necesary to avoid interface modification during gui layout/repaint and to pass data between game and editor solutions)
		void ProcessEvents()
		{
			LW_EditorBridge.Event _lastEvent = LW_EditorBridge.GetEvent();
			int _security = 0;

			LW_LogElement _elem;
			LW_LogElementHolder _holder;

			while (_lastEvent != null && _security < 1000)
			{
				_elem = null;
				_holder = null;
				if (_lastEvent.mLinkedItem is LW_LogElement)
					_elem = _lastEvent.mLinkedItem as LW_LogElement;
				if (_lastEvent.mLinkedItem is LW_LogElementHolder)
					_holder = _lastEvent.mLinkedItem as LW_LogElementHolder;
				switch (_lastEvent.mEventType)
				{
					case LW_EditorBridge.Event.eEventType.ElementSelected:
						SetSelectedHolder(_holder);
						SetSelectedLogElement(_elem);

						//if we are in history window, we want the element feed to stop
						if(LW_EditorBridge.eMode.History == mMode && LW_Prefs.freezeHistoryOnSelectElement)
						{
							if (!LW_Prefs.sTMP_HistoryFrozen)
							{
								if(_elem != null)
									_elem.mHolder.FreezeHistory();
								if(_holder != null)
									_holder.FreezeHistory();
							}
							LW_Prefs.sTMP_HistoryFrozen = true;
						}
						break;
					case LW_EditorBridge.Event.eEventType.ElementUnselected:
						SetSelectedLogElement(null);
						break;
					case LW_EditorBridge.Event.eEventType.ShowElementStackTrace:
						OnModeSwitched(_elem);
						mMode = LW_EditorBridge.eMode.StackTrace;
						break;
					case LW_EditorBridge.Event.eEventType.ShowElementHistory:
						SetSelectedHolder(_holder);
						SetSelectedLogElement(_elem);

						OnModeSwitched(_elem);
						mMode = LW_EditorBridge.eMode.History;
						LW_Prefs.sTMP_HistoryFrozen = LW_Prefs.freezeHistoryOnEnter;
						break;
					case LW_EditorBridge.Event.eEventType.ShowLogs:
						mMode = LW_EditorBridge.eMode.Logs;
						break;
					case LW_EditorBridge.Event.eEventType.ShowOptions:
						OnModeSwitched(_elem);
						mMode = LW_EditorBridge.eMode.Options;
						break;
					case LW_EditorBridge.Event.eEventType.JumpToFrame:
						mCurrentFrameDisplayed = _elem.mFrame;
						break;
				}
				_lastEvent = LW_EditorBridge.GetEvent();
				_security++;
			}
		}

		void OnModeSwitched(LW_LogElement elem)
		{
			if (elem == null)
				return;
			if (mMode == LW_EditorBridge.eMode.Logs)
			{
				if (mIsAtCurrentFrame)
				{
					FreezeHisto(elem.mHolder.mCurrentElement);
					SetSelectedLogElement(elem.mHolder.mCurrentElement);
				}
				else
				{
					//LW_EditorBridge.mScrollHistory = new Vector2(0, _elem.mHolder.mCurrentElementForFrame.mHolder.GetHistoryScrollValueForElement(_elem.mHolder.mCurrentElementForFrame));
					SetSelectedLogElement(elem.mHolder.mCurrentElementForFrame);
				}
			}
			else
			{
				if (mIsAtCurrentFrame)
				{
					SetSelectedLogElement(elem);
				}
				else
				{
					SetSelectedLogElement(elem.mHolder.mCurrentElementForFrame);
				}
			}
		}

		void FreezeHisto(LW_LogElement elem)
		{
			if (elem != null)
			{
				elem.mHolder.FreezeHistory();
				LW_EditorBridge.mScrollHistory = new Vector2(0, elem.mHolder.GetHistoryScrollValueForElement(elem));
			}
		}

		void RegisterSwitchToModeEvent(LW_EditorBridge.eMode mode)
		{
			if (mode == LW_EditorBridge.eMode.History)
			{
				if (mSelectedLogElement != null)
				{
					mSelectedLogElement.ShowHistory();
				}
				else if(mSelectedHolder != null)
				{
					LW_EditorBridge.RegisterEvent(LW_EditorBridge.Event.eEventType.ShowElementHistory, mSelectedHolder);
				}
				else
				{
					LW_EditorBridge.RegisterEvent(LW_EditorBridge.Event.eEventType.ShowElementHistory, null);
				}
			}
			else if (mode == LW_EditorBridge.eMode.StackTrace)
			{
				if (mSelectedLogElement != null)
				{
					mSelectedLogElement.ShowStackTrace();
				}
				else
				{
					LW_EditorBridge.RegisterEvent(LW_EditorBridge.Event.eEventType.ShowElementStackTrace, mSelectedLogElement);
				}
			}
			else if (mode == LW_EditorBridge.eMode.Logs)
			{
				LW_EditorBridge.RegisterEvent(LW_EditorBridge.Event.eEventType.ShowLogs, mSelectedLogElement);
			}
			else if (mode == LW_EditorBridge.eMode.Options)
			{
				LW_EditorBridge.RegisterEvent(LW_EditorBridge.Event.eEventType.ShowOptions, mSelectedLogElement);
			}
			else
			{
				//mMode = mode;
			}
		}

		void DrawToolbarTop()
		{
			EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
			if (Event.current.keyCode == LW_Prefs.backToLogShortcut)
			{
				RegisterSwitchToModeEvent(LW_EditorBridge.eMode.Logs);
			}

			//draw toolbar with multiple btns (Logs/histo/stack)
			if (position.width > TOOLBAR_WIDTH_MIN_EXTENDED_MODE)
			{
				for (int i = 0; i < mModeSelectorOptions.Length; i++)
				{
					if (i == (int)LW_EditorBridge.eMode.Options)
						continue;
					if (!LW_Prefs.keepHistory && i == (int)LW_EditorBridge.eMode.History)
					{
						if (GUILayout.Button(mModeSelectorOptions[i], LW_Tools.sToolbarBtnDisabledStyle))
						{

						}
						continue;
					}
					if (!LW_Prefs.collectStackTrace && i == (int)LW_EditorBridge.eMode.StackTrace)
					{
						if (GUILayout.Button(mModeSelectorOptions[i], LW_Tools.sToolbarBtnDisabledStyle))
						{

						}
						continue;
					}

					if (GUILayout.Button(mModeSelectorOptions[i], (int)mMode == i ? LW_Tools.sToolbarBtnSelectedStyle : LW_Tools.sToolbarBtnStyle))
					{
						if ((LW_EditorBridge.eMode)i != mMode)
						{
							RegisterSwitchToModeEvent((LW_EditorBridge.eMode)i);
						}
					}
				}
			}
			//draw toolbar with droptdown (more compact) (Logs/histo/stack)
			else
			{
				if (GUILayout.Button("<", LW_Tools.sToolbarBtnStyle))
				{
					RegisterSwitchToModeEvent(LW_EditorBridge.eMode.Logs);
				}

				LW_EditorBridge.eMode _newMode = (LW_EditorBridge.eMode)EditorGUILayout.Popup(
					(int)mMode,
					//mModeSelectorOptionsShort,
					mModeSelectorOptions,
					EditorStyles.toolbarPopup,
					 GUILayout.Width(82), GUILayout.Height(15));

				if (_newMode != mMode)
				{
					RegisterSwitchToModeEvent(_newMode);
				}
			}


			if (CAN_HAVE_UNFREEZE_HISTO && mSelectedLogElement != null && mSelectedLogElement.mHolder != null && mMode == LW_EditorBridge.eMode.History)
			{
				GUIContent _btnIcon;
				//GUIStyle _style;
				if (LW_Prefs.sTMP_HistoryFrozen)
				{
					_btnIcon = mPlayIcon;
					//_style = mSmallBtnPlayStyle;
				}
				else
				{
					_btnIcon = mPauseIcon;
					//_style = mSmallBtnPauseStyle;
				}

				if (GUILayout.Button(_btnIcon, LW_Tools.sToolbarBtnStyle, GUILayout.Width(22), GUILayout.Height(22)))
				{
					LW_Prefs.sTMP_HistoryFrozen = !LW_Prefs.sTMP_HistoryFrozen;
					if (LW_Prefs.sTMP_HistoryFrozen)
						mSelectedLogElement.mHolder.FreezeHistory();
				}
			}

			GUILayout.FlexibleSpace();
			
			if (mMode == LW_EditorBridge.eMode.Logs)
			{
				if (GUILayout.Button(LW_Tools.sSortAlphabeticalIcon, LW_Tools.sToolbarSortBtnStyle))
				{
					if (!LW_Prefs.sortCategories)
						LW_Prefs.sortCategories = true;

					LW_Prefs.sortCategoryAscending = !LW_Prefs.sortCategoryAscending;
					if(mLogSaved == null)
					{
						Logwin_Internal.SortCategoriesAlphabetical(LW_Prefs.sortCategoryAscending, ref Logwin_Internal.sCatDico);
					}
					else
					{
						Logwin_Internal.SortCategoriesAlphabetical(LW_Prefs.sortCategoryAscending, ref mLogSaved.mCatDicoSaved);
					}
				}
			}
			
			if (GUILayout.Button(mOptionsIcon, mMode == LW_EditorBridge.eMode.Options ? LW_Tools.sToolbarOptionBtnStyleSelected : LW_Tools.sToolbarOptionBtnStyleUnselected))
			{
				RegisterSwitchToModeEvent(LW_EditorBridge.eMode.Options);
			}

			EditorGUILayout.EndHorizontal();
		}

		void DrawLogs()
		{
			mLogScrollPos = EditorGUILayout.BeginScrollView(mLogScrollPos);
			if (mLogSaved == null)
			{
				foreach (KeyValuePair<string, LW_LogCategory> cat in Logwin_Internal.GetCatDico())
				{
					cat.Value.GUIDraw(mSelectedLogElement, mSelectedHolder, LW_EditorBridge.eMode.Logs, mIsAtCurrentFrame, mCurrentFrameDisplayed);
				}
			}
			else
			{
				foreach (KeyValuePair<string, LW_LogCategory> cat in mLogSaved.mCatDicoSaved)
				{
					cat.Value.GUIDraw(mSelectedLogElement, mSelectedHolder, LW_EditorBridge.eMode.Logs, mIsAtCurrentFrame, mCurrentFrameDisplayed);
				}
			}

			EditorGUILayout.EndScrollView();
		}

		void DrawHistory()
		{
			if (!LW_Prefs.keepHistory)
			{
				EditorGUI.LabelField(new Rect(0, position.height / 2f, position.width, 60), "You need to enable history in the settings");
			}
			else
			{
				if (mSelectedLogElement != null)
				{
					mSelectedLogElement.GUIDrawHistory(position);
				}
				else if (mSelectedHolder != null)
				{
					mSelectedHolder.GUIDrawHistory(position, null);
				}
				else
				{
					EditorGUI.LabelField(new Rect(0, position.height / 2f, position.width, 60), "Please select a log entry");
				}
			}
		}

		void DrawStackTrace()
		{
			if (mSelectedLogElement != null)
			{
				mSelectedLogElement.GUIDrawStack(position);
			}
			else
			{
				EditorGUI.LabelField(new Rect(0, position.height / 2f, position.width, 60), "Please select a log entry");
			}
		}

		void DrawOptions()
		{
			mOptionsScrollPos = EditorGUILayout.BeginScrollView(mOptionsScrollPos);
			LW_Prefs.updateValuesEveryFrames = EditorGUILayout.ToggleLeft(new GUIContent("Update window every frame", "Disable this option if you have performances issues or if you want the values to be displayed slower."), LW_Prefs.updateValuesEveryFrames);
			LW_Prefs.displayCallCount = EditorGUILayout.ToggleLeft(new GUIContent("Display call count", "Enable to show the call count for each log"), LW_Prefs.displayCallCount);
			LW_Prefs.collectStackTrace = EditorGUILayout.ToggleLeft(new GUIContent("Collect StackTrace", "If this option is disabled, you will not be able to open the file that called a log, and will not be able to see the stacktrace of the log entry. Disable this feature if you have performance issues with Logwin"), LW_Prefs.collectStackTrace);
			LW_Prefs.pauseGameDuringFrameByFrameAnalysis = EditorGUILayout.ToggleLeft(new GUIContent("Pause game during frame by frame analysis", "Pause the game when you tick the button \"current\""), LW_Prefs.pauseGameDuringFrameByFrameAnalysis);


			LW_Prefs.drawColorOnLeft = EditorGUILayout.ToggleLeft(new GUIContent("Color id displayed on left of log", "ON: color displayed on left of entry\nOFF: color displayed on right of entry"), LW_Prefs.drawColorOnLeft);

			bool _prevSort = LW_Prefs.sortCategories;
			LW_Prefs.sortCategories = EditorGUILayout.ToggleLeft(new GUIContent("Sort categories alphabetically", "ON: category will be sorted alphabetically\nOFF: categories will be sorted in log order (first top)\nIf you use the sort button (on left of the options button), this setting will be turned on automatically"), LW_Prefs.sortCategories);
			if(_prevSort != LW_Prefs.sortCategories)
			{
				if (LW_Prefs.sortCategories)
				{
					if(mLogSaved == null)
						Logwin_Internal.SortCategoriesAlphabetical(LW_Prefs.sortCategoryAscending, ref Logwin_Internal.sCatDico);
					else
						Logwin_Internal.SortCategoriesAlphabetical(LW_Prefs.sortCategoryAscending, ref mLogSaved.mCatDicoSaved);
				}
				else
				{
					if (mLogSaved == null)
						Logwin_Internal.SortCategoriesByApparition(ref Logwin_Internal.sCatDico);
					else
						Logwin_Internal.SortCategoriesByApparition(ref mLogSaved.mCatDicoSaved);
				}
			}


			LW_Prefs.truncateNumberToDecimalPlaces = EditorGUILayout.ToggleLeft(new GUIContent("Truncate Floating point number to decimal place", "Truncate Floating point number to decimal place"), LW_Prefs.truncateNumberToDecimalPlaces);

			if (LW_Prefs.truncateNumberToDecimalPlaces)
			{
				LW_Prefs.decimalToKeep = EditorGUILayout.IntSlider(new GUIContent("Decimal to keep", "If set to 3, 3.1415926 will be displayed as 3.141"), LW_Prefs.decimalToKeep, 0, 6);
			}


			LW_Prefs.alsoOutputToDebugLog = EditorGUILayout.ToggleLeft(new GUIContent("Output to Debug.Log() in editor", "C'mon, don't do that..."), LW_Prefs.alsoOutputToDebugLog);
			LW_Prefs.outputToDebugLogInBuild = EditorGUILayout.ToggleLeft(new GUIContent("Output to Debug.Log() in build", "If enabled Logwin.Log() will call Debug.Log() outside the editor (in build) [Thanks to BigPixels for the feature request]"), LW_Prefs.outputToDebugLogInBuild);


			GUILayout.BeginVertical("History", LW_Tools.sBoxWithTitleStyle);
			GUILayout.Space(20);
			LW_Prefs.keepHistory = EditorGUILayout.ToggleLeft(new GUIContent("Keep History", "If this option is disabled, you will not be able to see the log history"), LW_Prefs.keepHistory);
			if (LW_Prefs.keepHistory)
			{
				if (CAN_HAVE_UNFREEZE_HISTO)
					LW_Prefs.freezeHistoryOnEnter = EditorGUILayout.ToggleLeft(new GUIContent("Freeze history on enter history mode", "If this option is selected, when you enter the history, the feed will be stoped and you will not see the new entry on the history"), LW_Prefs.freezeHistoryOnEnter);

				//LW_Prefs.freezeHistoryOnSelectElement = EditorGUILayout.ToggleLeft(new GUIContent("Freeze history on select history element", "If this option is selected, the feed will be stoped and you will not see the new entry on the history when you select an element in history mode"), LW_Prefs.freezeHistoryOnSelectElement);

				LW_Prefs.limitHistorySize = EditorGUILayout.ToggleLeft(new GUIContent("Limit history size", "Enable this to limit the history size"), LW_Prefs.limitHistorySize);
				if (LW_Prefs.limitHistorySize)
				{
					LW_Prefs.maxHistoryPerElement = EditorGUILayout.IntField(new GUIContent("Max History per log", "Set the maximum history you want to keep with each log"), LW_Prefs.maxHistoryPerElement);
				}
			}
			EditorGUILayout.EndVertical();


			GUILayout.BeginVertical("Shortcuts", LW_Tools.sBoxWithTitleStyle);
			GUILayout.Space(20);

			GUILayout.BeginHorizontal();
			GUILayout.Label("Open File:");
			GUILayout.FlexibleSpace();
			GUILayout.Label("Click +");
			LW_Prefs.openFileShortcut = mShortcutModSelectorOptions[EditorGUILayout.Popup(
				LW_Tools.GetShortcutId(LW_Prefs.openFileShortcut),
				mShortcutModSelectorOptionsStr, GUILayout.MaxWidth(80))];
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("StackTrace:");
			GUILayout.FlexibleSpace();
			GUILayout.Label("Click +");
			LW_Prefs.showStacktraceShortcut = mShortcutModSelectorOptions[EditorGUILayout.Popup(
				LW_Tools.GetShortcutId(LW_Prefs.showStacktraceShortcut),
				mShortcutModSelectorOptionsStr, GUILayout.MaxWidth(80))];
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("History:");
			GUILayout.FlexibleSpace();
			GUILayout.Label("Click +");
			LW_Prefs.showHistoryShortcut = mShortcutModSelectorOptions[EditorGUILayout.Popup(
				LW_Tools.GetShortcutId(LW_Prefs.showHistoryShortcut),
				mShortcutModSelectorOptionsStr, GUILayout.MaxWidth(80))];
			GUILayout.EndHorizontal();

			GUILayout.Label("Back to logs tab: Escape");

			LW_Prefs.doubleClickOnElementOpenFile = EditorGUILayout.ToggleLeft(new GUIContent("Double click on log to open file", "If this option is selected, you can double click on a log entry to open the file at correct line"), LW_Prefs.doubleClickOnElementOpenFile);

			GUILayout.EndVertical();


			GUILayout.Space(10);
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Read the doc"))
			{
				Help.BrowseURL("http://www.memory-leaks.org/logwin");
			}
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Reset to default values"))
			{
				if(EditorUtility.DisplayDialog("Reset Logwin prefs?", "Are you sure you want to revert Logwin preferences to their default value?", "Yep", "Nope"))
				{
					LW_Prefs.ResetToDefault();
				}
				
			}
			//GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			EditorGUILayout.EndScrollView();
		}

		void OnGUI()
		{
			if (sInstance == null)
				OpenWindow();
			Init();
			LW_Tools.SetupStyles();
			
			if(mMode != LW_EditorBridge.eMode.Options)
				EditorGUILayout.BeginVertical(GUILayout.MaxHeight(position.height - FOOTER_HEIGHT));
			else
				EditorGUILayout.BeginVertical(GUILayout.MaxHeight(position.height));

			DrawToolbarTop();

			switch (mMode)
			{
				case LW_EditorBridge.eMode.Logs:
					DrawLogs();
					break;

				case LW_EditorBridge.eMode.History:
					DrawHistory();
					break;

				case LW_EditorBridge.eMode.StackTrace:
					DrawStackTrace();
					break;

				case LW_EditorBridge.eMode.Options:
					DrawOptions();
					break;
			}

			EditorGUILayout.EndVertical();
			if (mMode == LW_EditorBridge.eMode.Logs)
			{
				EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
			}
			else
			{
				EditorGUILayout.BeginHorizontal();
			}


			
			if (Application.isPlaying)
			{
				if (mIsAtCurrentFrame)
				{
					mCurrentFrameDisplayed = Time.frameCount;
				}
				mFrameCount = Time.frameCount;
			}

			if (mMode == LW_EditorBridge.eMode.Logs)
			{
				GUILayout.FlexibleSpace();
				if (mIsAtCurrentFrame)
				{
					GUILayout.Label("frame:" + mCurrentFrameDisplayed);
				}
				else
				{
					float _prevLabelWidth = EditorGUIUtility.labelWidth;
					EditorGUIUtility.labelWidth = 40;
					int _newFrame = EditorGUILayout.IntField("frame", mCurrentFrameDisplayed);
					EditorGUIUtility.labelWidth = _prevLabelWidth;
					if (_newFrame > mFrameFreezedNumber)
						_newFrame = mFrameFreezedNumber;
					if (_newFrame < 0)
						_newFrame = 0;
					mCurrentFrameDisplayed = _newFrame;
				}
				if (GUILayout.Button(mCurrentFrameDisplayed > 0 ? LW_Tools.sPrevFrameIcon : LW_Tools.sPrevFrameNotAvailableIcon, LW_Tools.sToolbarNoPaddingBtnStyle, GUILayout.Width(20)))
				{
					if (mIsAtCurrentFrame)
					{
						SetIsAtCurFrame(false);
					}
					mCurrentFrameDisplayed--;
				}
				if (GUILayout.Button(mCurrentFrameDisplayed < mFrameFreezedNumber ? LW_Tools.sNextFrameIcon : LW_Tools.sNextFrameNotAvailableIcon, LW_Tools.sToolbarNoPaddingBtnStyle, GUILayout.Width(20)))
				{
					if (!mIsAtCurrentFrame)
					{
						mCurrentFrameDisplayed++;
						if (mCurrentFrameDisplayed > mFrameFreezedNumber)
						{
							mCurrentFrameDisplayed = mFrameFreezedNumber;
						}
					}
				}
				if (GUILayout.Button("Current", mIsAtCurrentFrame ? LW_Tools.sToolbarBtnSelectedStyle : LW_Tools.sToolbarBtnStyle))
				{
					SetIsAtCurFrame(!mIsAtCurrentFrame);
				}
			}
			EditorGUILayout.EndHorizontal();


			if (mMode == LW_EditorBridge.eMode.Logs || mMode == LW_EditorBridge.eMode.History)
			{
				string _actionStr = LW_Tools.GetCurShortcutActionStr(mMode);
				if (!string.IsNullOrEmpty(_actionStr))
				{
					EditorGUI.LabelField(new Rect(0, position.height - FOOTER_HEIGHT, position.width, FOOTER_HEIGHT), _actionStr);
				}
			}
			//if (Event.current.type != EventType.Layout && Event.current.type != EventType.Repaint)
			//	ProcessEvents();
		}

		void SetIsAtCurFrame(bool isAtCurFrame, bool canAffectPlayPause = true)
		{
			if(canAffectPlayPause && LW_Prefs.pauseGameDuringFrameByFrameAnalysis && EditorApplication.isPlaying)
				EditorApplication.isPaused = !isAtCurFrame;

			mIsAtCurrentFrame = isAtCurFrame;
			if (!mIsAtCurrentFrame)
			{
				if (Application.isPlaying)
				{
					mFrameFreezedNumber = Time.frameCount;
				}

				if (mLogSaved == null)
				{
					foreach (KeyValuePair<string, LW_LogCategory> cat in Logwin_Internal.GetCatDico())
					{
						cat.Value.FreezeAllHisto();
					}
				}
				else
				{
					foreach (KeyValuePair<string, LW_LogCategory> cat in mLogSaved.mCatDicoSaved)
					{
						cat.Value.GUIDraw(mSelectedLogElement, mSelectedHolder, LW_EditorBridge.eMode.Logs, mIsAtCurrentFrame, mCurrentFrameDisplayed);
					}
				}
				if (Application.isPlaying)
				{
					mCurrentFrameDisplayed = Time.frameCount;
				}
			}
		}

	}
}
#endif
	  