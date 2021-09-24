using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

namespace LogWinInternal
{
	public static class LW_Prefs
	{
		static bool sPrefsLoaded = false;

		public static string FOLDER_NAME = "LogWin";
		static string FILE_PATH = "prefs";

		static bool sUpdateValuesEveryFrame = true;
		static bool sCollectStackTrace = true;
		static bool sKeepHistory = true;
		static bool sFreezeHistoryOnEnter = true;
		static bool sLimitHistorySize = true;
		static bool sDisplayCallCount = false;
		static int sMaxHistoryPerElement = 5000;
		static EventModifiers sOpenFileShortcut = EventModifiers.Shift;
		static EventModifiers sShowStacktraceShortcut = EventModifiers.Control;
		static EventModifiers sShowHistoryShortcut = EventModifiers.Alt;
		static KeyCode sBackToLogShortcut = KeyCode.Escape;
		static bool sDoubleClickOnElementOpenFile = true;
		static bool sFreezeHistoryOnSelectElement = true;
		static bool sTruncateNumberToDecimalPlaces = false;
		static int sDecimalToKeep = 2;
		static bool sPauseGameDuringFrameByFrameAnalysis = true;
		static bool sDrawColorOnLeft = false;
		static bool sAlsoOutputToDebugLog = false;
		static bool sOutputToDebugLogInBuild = false;


		static bool sSortCategoryAscending = true;
		static bool sSortCategories = false;


		//TMP VALUES --> DO NOT EXPORT
		public static bool sTMP_HistoryFrozen = false;



		public static void ResetToDefault()
		{
			sUpdateValuesEveryFrame = true;
			sCollectStackTrace = true;
			sKeepHistory = true;
			sFreezeHistoryOnEnter = true;
			sLimitHistorySize = true;
			sDisplayCallCount = false;
			sMaxHistoryPerElement = 5000;
			sOpenFileShortcut = EventModifiers.Shift;
			sShowStacktraceShortcut = EventModifiers.Control;
			sShowHistoryShortcut = EventModifiers.Alt;
			sBackToLogShortcut = KeyCode.Escape;
			sDoubleClickOnElementOpenFile = true;
			sFreezeHistoryOnSelectElement = true;
			sTruncateNumberToDecimalPlaces = false;
			sDecimalToKeep = 2;
			sPauseGameDuringFrameByFrameAnalysis = true;
			sDrawColorOnLeft = false;
			sAlsoOutputToDebugLog = false;
			sSortCategoryAscending = true;
			sSortCategories = false;
			sOutputToDebugLogInBuild = false;

			sTMP_HistoryFrozen = false;

			Export();
		}

		public static bool sortCategoryAscending
		{
			get
			{
				if (!sPrefsLoaded)
					Load();
				return sSortCategoryAscending;
			}

			set
			{
				SetPref(ref sSortCategoryAscending, value);
			}
		}
		public static bool sortCategories
		{
			get
			{
				if (!sPrefsLoaded)
					Load();
				return sSortCategories;
			}

			set
			{
				SetPref(ref sSortCategories, value);
			}
		}

		public static bool updateValuesEveryFrames
		{
			get
			{
				if (!sPrefsLoaded)
					Load();
				return sUpdateValuesEveryFrame;
			}

			set
			{
				SetPref(ref sUpdateValuesEveryFrame, value);
			}
		}

		public static int maxHistoryPerElement
		{
			get
			{
				if (!sPrefsLoaded)
					Load();
				return sMaxHistoryPerElement;
			}

			set
			{
				if (value < 0)
					value = 0;
				SetPref(ref sMaxHistoryPerElement, value);
			}
		}

		public static bool collectStackTrace
		{
			get
			{
				if (!sPrefsLoaded)
					Load();
				return sCollectStackTrace;
			}

			set
			{
				SetPref(ref sCollectStackTrace, value);
			}
		}

		public static bool keepHistory
		{
			get
			{
				if (!sPrefsLoaded)
					Load();
				return sKeepHistory;
			}

			set
			{
				SetPref(ref sKeepHistory, value);
			}
		}

		public static bool limitHistorySize
		{
			get
			{
				if (!sPrefsLoaded)
					Load();
				return sLimitHistorySize;
			}

			set
			{
				SetPref(ref sLimitHistorySize, value);
			}
		}

		public static EventModifiers openFileShortcut
		{
			get
			{
				if (!sPrefsLoaded)
					Load();
				return sOpenFileShortcut;
			}

			set
			{
				if (sOpenFileShortcut == value)
					return;
				if (!sPrefsLoaded)
					Load();
				EventModifiers _prevMod = sOpenFileShortcut;
				sOpenFileShortcut = value;
				if(sShowStacktraceShortcut == sOpenFileShortcut)
				{
					sShowStacktraceShortcut = _prevMod;
				}
				if (sShowHistoryShortcut == sOpenFileShortcut)
				{
					sShowHistoryShortcut = _prevMod;
				}
				Export();
			}
		}

		public static EventModifiers showStacktraceShortcut
		{
			get
			{
				if (!sPrefsLoaded)
					Load();
				return sShowStacktraceShortcut;
			}

			set
			{
				if (sShowStacktraceShortcut == value)
					return;
				if (!sPrefsLoaded)
					Load();
				EventModifiers _prevMod = sShowStacktraceShortcut;
				sShowStacktraceShortcut = value;
				if (sOpenFileShortcut == sShowStacktraceShortcut)
				{
					sOpenFileShortcut = _prevMod;
				}
				if (sShowHistoryShortcut == sShowStacktraceShortcut)
				{
					sShowHistoryShortcut = _prevMod;
				}
				Export();
			}
		}

		public static EventModifiers showHistoryShortcut
		{
			get
			{
				if (!sPrefsLoaded)
					Load();
				return sShowHistoryShortcut;
			}

			set
			{
				if (sShowHistoryShortcut == value)
					return;
				if (!sPrefsLoaded)
					Load();
				EventModifiers _prevMod = sShowHistoryShortcut;
				sShowHistoryShortcut = value;
				if (sOpenFileShortcut == sShowHistoryShortcut)
				{
					sOpenFileShortcut = _prevMod;
				}
				if (sShowStacktraceShortcut == sShowHistoryShortcut)
				{
					sShowStacktraceShortcut = _prevMod;
				}
				Export();
			}
		}

		public static bool displayCallCount
		{
			get
			{
				if (!sPrefsLoaded)
					Load();
				return sDisplayCallCount;
			}

			set
			{
				SetPref(ref sDisplayCallCount, value);
			}
		}

		public static bool freezeHistoryOnEnter
		{
			get
			{
				if (!sPrefsLoaded)
					Load();
				return sFreezeHistoryOnEnter;
			}

			set
			{
				SetPref(ref sFreezeHistoryOnEnter, value);
			}
		}

		public static KeyCode backToLogShortcut
		{
			get
			{
				if (!sPrefsLoaded)
					Load();
				return sBackToLogShortcut;
			}

			set
			{
				SetPref(ref sBackToLogShortcut, value);
			}
		}

		public static bool doubleClickOnElementOpenFile
		{
			get
			{
				if (!sPrefsLoaded)
					Load();
				return sDoubleClickOnElementOpenFile;
			}

			set
			{
				SetPref(ref sDoubleClickOnElementOpenFile, value);
			}
		}

		public static bool freezeHistoryOnSelectElement
		{
			get
			{
				if (!sPrefsLoaded)
					Load();
				return sFreezeHistoryOnSelectElement;
			}

			set
			{
				SetPref(ref sFreezeHistoryOnSelectElement, value);
			}
		}

		public static bool truncateNumberToDecimalPlaces
		{
			get
			{
				if (!sPrefsLoaded)
					Load();
				return sTruncateNumberToDecimalPlaces;
			}

			set
			{
				SetPref(ref sTruncateNumberToDecimalPlaces, value);
			}
		}

		public static int decimalToKeep
		{
			get
			{
				if (!sPrefsLoaded)
					Load();
				return sDecimalToKeep;
			}

			set
			{
				SetPref(ref sDecimalToKeep, value);
			}
		}

		public static bool pauseGameDuringFrameByFrameAnalysis
		{
			get
			{
				if (!sPrefsLoaded)
					Load();
				return sPauseGameDuringFrameByFrameAnalysis;
			}

			set
			{
				SetPref(ref sPauseGameDuringFrameByFrameAnalysis, value);
			}
		}

		public static bool drawColorOnLeft
		{
			get
			{
				if (!sPrefsLoaded)
					Load();
				return sDrawColorOnLeft;
			}

			set
			{
				SetPref(ref sDrawColorOnLeft, value);
			}
		}

		public static bool alsoOutputToDebugLog
		{
			get
			{
				if (!sPrefsLoaded)
					Load();
				return sAlsoOutputToDebugLog;
			}

			set
			{
				SetPref(ref sAlsoOutputToDebugLog, value);
			}
		}

		public static bool outputToDebugLogInBuild
		{
			get
			{
				if (!sPrefsLoaded)
					Load();
				return sOutputToDebugLogInBuild;
			}

			set
			{
				bool _prevState = sOutputToDebugLogInBuild;
				SetPref(ref sOutputToDebugLogInBuild, value);
				if(sOutputToDebugLogInBuild != _prevState)
				{
					if (sOutputToDebugLogInBuild)
						LW_Tools.AddDefine("LOGWIN_OUTPUT_IN_BUILD");
					else
						LW_Tools.RemoveDefine("LOGWIN_OUTPUT_IN_BUILD");
				}
			}
		}
		

		private static void SetPref(ref object pref, object newValue)
		{
			if (pref == newValue)
				return;
			if (!sPrefsLoaded)
				Load();
			pref = newValue;
			Export();
		}
		
		private static void SetPref(ref bool pref, bool newValue)
		{
			if (pref == newValue)
				return;
			if (!sPrefsLoaded)
				Load();
			pref = newValue;
			Export();
		}

		private static void SetPref(ref int pref, int newValue)
		{
			if (pref == newValue)
				return;
			if (!sPrefsLoaded)
				Load();
			pref = newValue;
			Export();
		}

		private static void SetPref(ref EventModifiers pref, EventModifiers newValue)
		{
			if (pref == newValue)
				return;
			if (!sPrefsLoaded)
				Load();
			pref = newValue;
			Export();
		}

		private static void SetPref(ref KeyCode pref, KeyCode newValue)
		{
			if (pref == newValue)
				return;
			if (!sPrefsLoaded)
				Load();
			pref = newValue;
			Export();
		}




		
		static string sPath = null;

		private static string path
		{
			get
			{
				if (sPath != null)
					return sPath;
				string _path = Application.persistentDataPath;
				try
				{
					_path.Replace("\\", "/");
					_path = _path.Substring(0, _path.LastIndexOf("/"));
					_path = _path.Substring(0, _path.LastIndexOf("/"));					
				}
				catch (Exception) { }

				_path += "/" + FOLDER_NAME + "/";
				sPath = _path;

				return sPath;
			}
		}

		public static string GetFilePath(bool createDirectory = true)
		{
			//string _path = Application.persistentDataPath.Replace("Assets", FOLDER_NAME) + "/";
			string _path = path;
			string _pathFull = _path + FILE_PATH;
			if (createDirectory)
			{
				if (!Directory.Exists(_path))
				{
					Directory.CreateDirectory(_path);
				}
			}
			return _pathFull;
		}

		static void Load()
		{
#if UNITY_EDITOR
			sPrefsLoaded = true;

			string _filePath = GetFilePath();
			if (!File.Exists(_filePath))
				return;

			StreamReader _sr = null;
			try
			{
				_sr = new StreamReader(_filePath);
				string _line = _sr.ReadLine();
				string[] _split;
				while (!string.IsNullOrEmpty(_line))
				{
					_split = _line.Split(':');
					switch (_split[0])
					{
						case "uvef":
							sUpdateValuesEveryFrame = bool.Parse(_split[1]);
							break;
						case "cst":
							sCollectStackTrace = bool.Parse(_split[1]);
							break;
						case "kh":
							sKeepHistory = bool.Parse(_split[1]);
							break;
						case "lhs":
							sLimitHistorySize = bool.Parse(_split[1]);
							break;
						case "mhpe":
							sMaxHistoryPerElement = int.Parse(_split[1]);
							break;
						case "ofs":
							sOpenFileShortcut = (EventModifiers)int.Parse(_split[1]);
							break;
						case "ssts":
							sShowStacktraceShortcut = (EventModifiers)int.Parse(_split[1]);
							break;
						case "shs":
							sShowHistoryShortcut = (EventModifiers)int.Parse(_split[1]);
							break;
						case "dcc":
							sDisplayCallCount = bool.Parse(_split[1]);
							break;
						case "fhoe":
							sFreezeHistoryOnEnter = bool.Parse(_split[1]);
							break;
						case "btls":
							sBackToLogShortcut = (KeyCode)int.Parse(_split[1]);
							break;
						case "dcoeof":
							sDoubleClickOnElementOpenFile = bool.Parse(_split[1]);
							break;
						case "fhose":
							sFreezeHistoryOnSelectElement = bool.Parse(_split[1]);
							break;
						case "tntdp":
							sTruncateNumberToDecimalPlaces = bool.Parse(_split[1]);
							break;
						case "dtk":
							sDecimalToKeep = int.Parse(_split[1]);
							break;
						case "pgdfbfa":
							sPauseGameDuringFrameByFrameAnalysis = bool.Parse(_split[1]);
							break;
						case "dcol":
							sDrawColorOnLeft = bool.Parse(_split[1]);
							break;
						case "aotdl":
							sAlsoOutputToDebugLog = bool.Parse(_split[1]);
							break;
						case "sc":
							sSortCategories = bool.Parse(_split[1]);
							break;
						case "sca":
							sSortCategoryAscending = bool.Parse(_split[1]);
							break;
						case "otdlib":
							sOutputToDebugLogInBuild = bool.Parse(_split[1]);
							break;
						default:
							UnityEngine.Debug.LogWarning("Unknown data for " + _split[0]);
							break;
					}
					_line = _sr.ReadLine();
				}
			}
			catch (Exception e) { Debug.LogError("Fail during prefs parsing:" + e.ToString()); }
			finally { if(_sr != null) _sr.Dispose(); }
#endif
		}

		static void Export()
		{
#if UNITY_EDITOR
			StreamWriter _sw = null;
			try
			{
				string _filePath = GetFilePath();
				if (File.Exists(_filePath))
					File.Delete(_filePath);

				_sw = File.CreateText(_filePath);
				_sw.WriteLine("uvef:" + sUpdateValuesEveryFrame.ToString());
				_sw.WriteLine("cst:" + sCollectStackTrace.ToString());
				_sw.WriteLine("kh:" + sKeepHistory.ToString());
				_sw.WriteLine("lhs:" + sLimitHistorySize.ToString());
				_sw.WriteLine("mhpe:" + sMaxHistoryPerElement.ToString());
				_sw.WriteLine("ofs:" + ((int)sOpenFileShortcut).ToString());
				_sw.WriteLine("ssts:" + ((int)sShowStacktraceShortcut).ToString());
				_sw.WriteLine("shs:" + ((int)sShowHistoryShortcut).ToString());
				_sw.WriteLine("dcc:" + sDisplayCallCount.ToString());
				_sw.WriteLine("fhoe:" + sFreezeHistoryOnEnter.ToString());
				_sw.WriteLine("btls:" + ((int)sBackToLogShortcut).ToString());
				_sw.WriteLine("dcoeof:" + sDoubleClickOnElementOpenFile.ToString());
				_sw.WriteLine("fhose:" + sFreezeHistoryOnSelectElement.ToString());
				_sw.WriteLine("tntdp:" + sTruncateNumberToDecimalPlaces.ToString());
				_sw.WriteLine("dtk:" + sDecimalToKeep.ToString());
				_sw.WriteLine("pgdfbfa:" + sPauseGameDuringFrameByFrameAnalysis.ToString());
				_sw.WriteLine("dcol:" + sDrawColorOnLeft.ToString());
				_sw.WriteLine("aotdl:" + sAlsoOutputToDebugLog.ToString());
				_sw.WriteLine("sc:" + sSortCategories.ToString());
				_sw.WriteLine("sca:" + sSortCategoryAscending.ToString());
				_sw.WriteLine("otdlib:" + sOutputToDebugLogInBuild.ToString());

				

			}
			catch (Exception) { }
			finally { if (_sw != null) _sw.Dispose(); }
#endif
		}
	}
}
