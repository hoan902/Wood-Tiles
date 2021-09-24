using System.Collections.Generic;
using UnityEngine;
using LogWinInternal;
using System;

#if UNITY_EDITOR
using UnityEditor;

public class Logwin {


	/// <summary>
	/// Display a log in the Logwin window
	/// </summary>
	/// <param name="key">All the values with the same key in the same category will be merged, and only the latest log will be displayed</param>
	/// <param name="value">The value to display</param>
	/// <param name="options">Additionnals parameters</param>
	public static void Log(string key, object value, params LogwinParam[] options)
	{
		LogInternal(key, value, null, LW_LogElement.eLogType.log, options);
	}

	/// <summary>
	/// Display a log in the Logwin window
	/// </summary>
	/// <param name="key">All the values with the same key in the same category will be merged, and only the latest log will be displayed</param>
	/// <param name="value">The value to display</param>
	/// <param name="categoryKey">(optionnal)Used to organize your entries in different categories</param>
	/// <param name="options">Additionnals parameters</param>
	public static void Log(string key, object value, string categoryKey = "Uncategorized", params LogwinParam[] options)
	{
		LogInternal(key, value, categoryKey, LW_LogElement.eLogType.log, options);
	}

	/// <summary>
	/// Display a warning log in the Logwin window
	/// </summary>
	/// <param name="key">All the values with the same key in the same category will be merged, and only the latest log will be displayed</param>
	/// <param name="value">The value to display</param>
	/// <param name="options">Additionnals parameters</param>
	public static void LogWarning(string key, object value, params LogwinParam[] options)
	{
		LogInternal(key, value, null, LW_LogElement.eLogType.warning, options);
	}

	/// <summary>
	/// Display a warning log in the Logwin window
	/// </summary>
	/// <param name="key">All the values with the same key in the same category will be merged, and only the latest log will be displayed</param>
	/// <param name="value">The value to display</param>
	/// <param name="categoryKey">(optionnal)Used to organize your entries in different categories</param>
	/// <param name="options">Additionnals parameters</param>
	public static void LogWarning(string key, object value, string categoryKey = "Uncategorized", params LogwinParam[] options)
	{
		LogInternal(key, value, categoryKey, LW_LogElement.eLogType.warning, options);
	}

	/// <summary>
	/// Display an error log in the Logwin window
	/// </summary>
	/// <param name="key">All the values with the same key in the same category will be merged, and only the latest log will be displayed</param>
	/// <param name="value">The value to display</param>
	/// <param name="options">Additionnals parameters</param>
	public static void LogError(string key, object value, params LogwinParam[] options)
	{
		LogInternal(key, value, null, LW_LogElement.eLogType.error, options);
	}

	/// <summary>
	/// Display an error log in the Logwin window
	/// </summary>
	/// <param name="key">All the values with the same key in the same category will be merged, and only the latest log will be displayed</param>
	/// <param name="value">The value to display</param>
	/// <param name="categoryKey">(optionnal)Used to organize your entries in different categories</param>
	/// <param name="options">Additionnals parameters</param>
	public static void LogError(string key, object value, string categoryKey = "Uncategorized", params LogwinParam[] options)
	{
		LogInternal(key, value, categoryKey, LW_LogElement.eLogType.error, options);
	}
	

	private static void LogInternal(string key, object value, string categoryKey, LW_LogElement.eLogType logType, params LogwinParam[] options)
	{
		if (key == null)
		{
			Debug.LogWarning("LogWin : you need to set a valid key (aka log name)");
			return;
		}
		if (value == null) 
		{
			value = "null";
		}
		if (categoryKey == null)
		{
			categoryKey = "Uncategorized";
		}
		LW_LogCategory _cat = null;
		if (!Logwin_Internal.sCatDico.ContainsKey(categoryKey))
		{
			_cat = new LW_LogCategory(categoryKey);
			Logwin_Internal.sCatDico.Add(categoryKey, _cat);
			if(LW_Prefs.sortCategories)
				Logwin_Internal.SortCategoriesAlphabetical(LW_Prefs.sortCategoryAscending, ref Logwin_Internal.sCatDico);
		}
		else
		{
			_cat = Logwin_Internal.sCatDico[categoryKey];
		}

		LW_LogElement _newElem = new LW_LogElement(value, _cat, logType);
		_cat.AddLog(_newElem, key);

		if (options != null)
		{
			for (int i = 0; i < options.Length; i++)
			{
				options[i].ApplyParam(_newElem);
			}
		}

		if (LW_Prefs.alsoOutputToDebugLog)
		{
			switch (logType)
			{
				case LW_LogElement.eLogType.log:
					Debug.Log(categoryKey + ":" + key + ":" + value.ToString());
					break;
				case LW_LogElement.eLogType.warning:
					Debug.LogWarning(categoryKey + ":" + key + ":" + value.ToString());
					break;
				case LW_LogElement.eLogType.error:
					Debug.LogError(categoryKey + ":" + key + ":" + value.ToString());
					break;
			}
		}
	}
}
#else
public class Logwin {
	protected static Dictionary<string, LW_LogCategory> sCatDico = new Dictionary<string, LW_LogCategory>();

	public static void Log(string key, object value, params LogwinParam[] options)
	{
		LogInternal(key, value, null, LW_LogElement.eLogType.log, options);
	}

	public static void Log(string key, object value, string categoryKey = "Uncategorized", params LogwinParam[] options)
	{
		LogInternal(key, value, categoryKey, LW_LogElement.eLogType.log, options);
	}

	public static void LogWarning(string key, object value, params LogwinParam[] options)
	{
		LogInternal(key, value, null, LW_LogElement.eLogType.warning, options);
	}

	public static void LogWarning(string key, object value, string categoryKey = "Uncategorized", params LogwinParam[] options)
	{
		LogInternal(key, value, categoryKey, LW_LogElement.eLogType.warning, options);
	}

	public static void LogError(string key, object value, params LogwinParam[] options)
	{
		LogInternal(key, value, null, LW_LogElement.eLogType.error, options);
	}

	public static void LogError(string key, object value, string categoryKey = "Uncategorized", params LogwinParam[] options)
	{
		LogInternal(key, value, categoryKey, LW_LogElement.eLogType.error, options);
	}
	
	private static void LogInternal(string key, object value, string categoryKey, LW_LogElement.eLogType logType, params LogwinParam[] options)
	{
#if LOGWIN_OUTPUT_IN_BUILD
		switch (logType)
		{
			case LW_LogElement.eLogType.log:
				Debug.Log(categoryKey + ":" + key + ":" + value.ToString());
				break;
			case LW_LogElement.eLogType.warning:
				Debug.LogWarning(categoryKey + ":" + key + ":" + value.ToString());
				break;
			case LW_LogElement.eLogType.error:
				Debug.LogError(categoryKey + ":" + key + ":" + value.ToString());
				break;
		}
#endif
	}
}
#endif

public class LogwinParam
{
	/// <summary>
	/// Pause the game on log
	/// </summary>
	/// <param name="pause">Pause the game if true, else do nothing</param>
	/// <returns></returns>
	public static LogwinParam Pause(bool pause = true)
	{
		LogwinParam param = new LogwinParam();
		param.mCondition = pause;
		param.mParamType = eParamType.Pause;
		return param;
	}

	/// <summary>
	/// Add a small color rectangle on the left or right of the entry
	/// </summary>
	/// <param name="color">The color to display</param>
	/// <returns></returns>
	public static LogwinParam Color(Color color)
	{
		LogwinParam param = new LogwinParam();
		param.mColor = color;
		param.mCondition = true;
		param.mParamType = eParamType.Color;
		return param;
	}

	/// <summary>
	/// WARNING : BETA, NOT SAFE TO USE FOR NOW, ONLY FOR INT ATM
	/// </summary>
	/// <param name="elementsDisplayedCount"> Max elements displayed (may be modified if entry width sub 1 pixel)</param>
	/// <returns></returns>
	public static LogwinParam Graph(int elementsDisplayedCount = 100)
	{
		LogwinParam param = new LogwinParam();
		param.mCondition = true;
		param.mParamType = eParamType.Graph;
		param.mGraphElemCount = elementsDisplayedCount;
		return param;
	}

	public void ApplyParam(LW_LogElement elem)
	{
		if (!mCondition)
			return;
		switch (mParamType)
		{
			case eParamType.Pause:
#if UNITY_EDITOR
				EditorApplication.isPaused = true;
#endif
				break;
			case eParamType.Color:
				elem.SetPastilleColor(mColor);
				break;
			case eParamType.Graph:
				elem.SetNeedGraph(mGraphElemCount);
				break;
		}
	}

	eParamType mParamType;
	Color mColor;
	bool mCondition = true;
	int mGraphElemCount = 0;

	public enum eParamType
	{
		Color,
		Pause,
		Graph
	}
}

namespace LogWinInternal
{
	public class Logwin_Internal : Logwin
	{
		public static Dictionary<string, LW_LogCategory> sCatDico = new Dictionary<string, LW_LogCategory>();
		public Dictionary<string, LW_LogCategory> mCatDicoSaved = new Dictionary<string, LW_LogCategory>();

		/*
		protected static List<LW_LogElement> sGlobalHistory = new List<LW_LogElement>();
		public static List<LW_LogElement> sGlobalHistoryFrozen = new List<LW_LogElement>();


		public static void PushNewLog(LW_LogElement elem)
		{
			int _globalHistoLimit = 500;
			sGlobalHistory.Add(elem);
			if(sGlobalHistory.Count > _globalHistoLimit)
			{
				List<LW_LogElement> _removed = sGlobalHistory.GetRange(0, sGlobalHistory.Count - _globalHistoLimit);
				for(int i = 0; i < _removed.Count; i++)
				{
					_removed[i].RemoveFromHolder();
					_removed[i].Clear();
				}
				sGlobalHistory.RemoveRange(0, sGlobalHistory.Count - _globalHistoLimit);
				
			}
		}

		public static void GenerateGlobalHisto(bool force = false)
		{
			if (!force && sGlobalHistoryFrozen.Count > 0 && sGlobalHistoryFrozen[sGlobalHistoryFrozen.Count - 1].mId == LW_LogElement.sElemTotal - 1)
				return;

			sGlobalHistoryFrozen.Clear();
			Dictionary<string, LW_LogElementHolder> _holders;
			foreach (KeyValuePair<string, LW_LogCategory> cat in sCatDico)
			{
				_holders = cat.Value.GetHolders();
				foreach (KeyValuePair<string, LW_LogElementHolder> holder in _holders)
				{
					sGlobalHistoryFrozen.AddRange(holder.Value.mElementsHistory);
				}
			}
			LW_LogElement.ComparerId _comparer = new LW_LogElement.ComparerId();
			sGlobalHistoryFrozen.Sort(_comparer);
		}
		*/

		public static void SortCategoriesByApparition(ref Dictionary<string, LW_LogCategory> dicoToSort)
		{
			List<KeyValuePair<string, LW_LogCategory>> _tmpList = new List<KeyValuePair<string, LW_LogCategory>>();
			foreach (KeyValuePair<string, LW_LogCategory> item in dicoToSort)
			{
				_tmpList.Add(item);
			}

			_tmpList.Sort(
				delegate (KeyValuePair<string, LW_LogCategory> pair1,
				KeyValuePair<string, LW_LogCategory> pair2)
				{
					return pair1.Value.mId.CompareTo(pair2.Value.mId);
				}
			);

			Dictionary<string, LW_LogCategory> _newDic = new Dictionary<string, LW_LogCategory>();
			foreach (KeyValuePair<string, LW_LogCategory> item in _tmpList)
			{
				_newDic.Add(item.Key, item.Value);
			}
			dicoToSort = _newDic;
		}

		public static void SortCategoriesAlphabetical(bool ascending, ref Dictionary<string, LW_LogCategory> dicoToSort)
		{
			List<KeyValuePair<string, LW_LogCategory>> _tmpList = new List<KeyValuePair<string, LW_LogCategory>>();
			foreach(KeyValuePair<string, LW_LogCategory> item in dicoToSort)
			{
				_tmpList.Add(item);
			}

			_tmpList.Sort(
				delegate (KeyValuePair<string, LW_LogCategory> pair1,
				KeyValuePair<string, LW_LogCategory> pair2)
				{
					if(ascending)
						return pair1.Key.CompareTo(pair2.Key);
					else
						return pair2.Key.CompareTo(pair1.Key);
				}
			);

			Dictionary<string, LW_LogCategory> _newDic = new Dictionary<string, LW_LogCategory>();
			foreach (KeyValuePair<string, LW_LogCategory> item in _tmpList)
			{
				_newDic.Add(item.Key, item.Value);
			}
			dicoToSort = _newDic;
		}

		public static Dictionary<string, LW_LogCategory> GetCatDico()
		{
			return sCatDico;
		}

		public static void ClearAll()
		{
			foreach (KeyValuePair<string, LW_LogCategory> cat in Logwin_Internal.GetCatDico())
			{
				cat.Value.Clear();
			}
			Logwin_Internal.GetCatDico().Clear();
		}

		public static Logwin_Internal SaveValuesForEditMode()
		{
			Logwin_Internal _save = new Logwin_Internal();
			foreach (KeyValuePair<string, LW_LogCategory> value in sCatDico)
			{
				_save.mCatDicoSaved.Add(value.Key, value.Value);
			}
			return _save;
		}

		public void ClearSavedDico()
		{
			foreach (KeyValuePair<string, LW_LogCategory> cat in mCatDicoSaved)
			{
				cat.Value.Clear();
			}
			mCatDicoSaved.Clear();
		}
	}
}
