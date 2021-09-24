using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
namespace LogWinInternal
{
	public class LW_Tools
	{
		static bool sStyleInit = false;

		public static GUIStyle sElementBoxStyle0;
		public static GUIStyle sElementBoxStyle1;
		public static GUIStyle sElementSelectedStyle;
		public static GUIStyle sElementBoxFullGrey;
		public static GUIStyle sElementTooltipFullGrey;


		public static GUIStyle sCenteredText;
		public static GUIContent sIconNew;
		public static GUIContent sIconNotNew;
		public static GUIContent sPrevFrameIcon;
		public static GUIContent sNextFrameIcon;
		public static GUIContent sPrevFrameNotAvailableIcon;
		public static GUIContent sNextFrameNotAvailableIcon;
		public static GUIContent sWarningIcon;
		public static GUIContent sErrorIcon;
		public static GUIContent sSortAlphabeticalIcon;
		public static GUIStyle sSelectedElementLabel;

		public static GUIStyle sSmallBtnStyle;
		public static GUIStyle sSmallBtnPauseStyle;
		public static GUIStyle sSmallBtnPlayStyle;
		public static GUIStyle sActionTextLabelStyle;
		public static GUIStyle sToolbarBtnStyle;
		public static GUIStyle sToolbarBtnDisabledStyle;
		public static GUIStyle sToolbarBtnSelectedStyle;
		public static GUIStyle sToolbarNoPaddingBtnStyle;

		public static GUIStyle sToolbarSortBtnStyle;


		public static GUIStyle sToolbarOptionBtnStyleUnselected;
		public static GUIStyle sToolbarOptionBtnStyleSelected;


		public static GUIStyle sBtnPrevNextNoBgStyle;

		public static GUIStyle sGlobalBoxStyle;
		public static GUIStyle sTitleBG;

		public static GUIStyle sBoxWithTitleStyle;


		public static void SetupStyles(bool force = false)
		{
			if (!force && sStyleInit)
				return;

#if UNITY_EDITOR
			{
				sSortAlphabeticalIcon = EditorGUIUtility.IconContent("AlphabeticalSorting");
				sSortAlphabeticalIcon = new GUIContent(sSortAlphabeticalIcon.image, "Alphabetical Sorting");
			}
			{
				sBoxWithTitleStyle = new GUIStyle("box");
				sBoxWithTitleStyle.normal.textColor = new GUIStyle("label").normal.textColor;
			}
			{
				sSelectedElementLabel = new GUIStyle("label");
				sSelectedElementLabel.normal.textColor = new Color(0,0,0);
			}
			{
				GUIStyleState _normalStyleState = new GUIStyleState();
				Color _col = new Color(0, 0, 0, .05f);
				_normalStyleState.background = LW_Tools.GenerateBgTexture(_col);

				GUIStyleState _hoverStyleState = new GUIStyleState();
				Color _colHover = new Color(1f, 1f, 1f, 0.2f);
				_hoverStyleState.background = LW_Tools.GenerateBgTexture(_colHover);

				sElementBoxStyle0 = new GUIStyle();
				sElementBoxStyle0.normal = _normalStyleState;
				sElementBoxStyle0.hover = _hoverStyleState;
			}
			{
				GUIStyleState _normalStyleState = new GUIStyleState();
				Color _col = new Color(0, 0, 0, 0);
				_normalStyleState.background = LW_Tools.GenerateBgTexture(_col);

				GUIStyleState _hoverStyleState = new GUIStyleState();
				Color _colHover = new Color(1f, 1f, 1f, 0.2f);
				_hoverStyleState.background = LW_Tools.GenerateBgTexture(_colHover);

				sElementBoxStyle1 = new GUIStyle();
				sElementBoxStyle1.normal = _normalStyleState;
				sElementBoxStyle1.hover = _hoverStyleState;
			}
			{
				GUIStyleState _normalStyleState = new GUIStyleState();
				Color _col = new Color(1f, 1f, 1f, 0.5f);
				_normalStyleState.background = LW_Tools.GenerateBgTexture(_col);

				sElementSelectedStyle = new GUIStyle();
				sElementSelectedStyle.normal = _normalStyleState;
				sElementSelectedStyle.border = new RectOffset(2, 2, 2, 2);
			}
			{
				GUIStyleState _normalStyleState = new GUIStyleState();
				Color _col = new Color(.5f, .5f, .5f, 1f);
				_normalStyleState.background = LW_Tools.GenerateBgTexture(_col);

				sElementBoxFullGrey = new GUIStyle("box");
				sElementBoxFullGrey.normal = _normalStyleState;
			}
			{
				GUIStyleState _normalStyleState = new GUIStyleState();
				Color _col = new Color(.5f, .5f, .5f, 1f);
				_normalStyleState.background = LW_Tools.GenerateBgTexture(_col);

				sElementTooltipFullGrey = new GUIStyle("box");
				sElementTooltipFullGrey.normal = _normalStyleState;
				sElementTooltipFullGrey.alignment = TextAnchor.MiddleLeft;

			}
			{
				sCenteredText = new GUIStyle();
				sCenteredText.alignment = TextAnchor.MiddleCenter;
			}
			{
				sIconNew = EditorGUIUtility.IconContent("Collab");
				sIconNew = new GUIContent(sIconNew.image, "Updated this frame");
			}
			{
				sIconNotNew = EditorGUIUtility.IconContent("Profiler.Record");
				sIconNotNew = new GUIContent(sIconNotNew.image, "Jump to entry frame");
			}
			{
				sSmallBtnStyle = new GUIStyle("button");
				sSmallBtnStyle.padding = new RectOffset(2, 2, 2, 2);
				sSmallBtnStyle.contentOffset = new Vector2(1.5f, -1);
				sSmallBtnStyle.clipping = TextClipping.Overflow;
				sSmallBtnStyle.margin.top -= 1;
			}
			{
				sSmallBtnPauseStyle = new GUIStyle("button");
				sSmallBtnPauseStyle.padding = new RectOffset(2, 2, 2, 2);
				sSmallBtnPauseStyle.contentOffset = new Vector2(0, 0);
				sSmallBtnPauseStyle.clipping = TextClipping.Overflow;
				sSmallBtnPauseStyle.margin.top -= 1;
			}
			{
				sSmallBtnPlayStyle = new GUIStyle("button");
				sSmallBtnPlayStyle.padding = new RectOffset(2, 2, 2, 2);
				sSmallBtnPlayStyle.contentOffset = new Vector2(1, 0);
				sSmallBtnPlayStyle.clipping = TextClipping.Overflow;
				sSmallBtnPlayStyle.margin.top -= 1;
			}
			{
				sToolbarBtnStyle = new GUIStyle(EditorStyles.toolbarButton);
			}
			{
				sToolbarBtnSelectedStyle = new GUIStyle(EditorStyles.toolbarButton);
				sToolbarBtnSelectedStyle.normal = sToolbarBtnSelectedStyle.active;
			}
			{
				sToolbarOptionBtnStyleSelected = new GUIStyle(EditorStyles.toolbarButton);
				sToolbarOptionBtnStyleSelected.normal = sToolbarOptionBtnStyleSelected.active;
				sToolbarOptionBtnStyleSelected.padding = new RectOffset(6, 3, 1, 0);
			}
			{
				sToolbarOptionBtnStyleUnselected = new GUIStyle(EditorStyles.toolbarButton);
				sToolbarOptionBtnStyleUnselected.padding = new RectOffset(6, 3, 1, 0);
			}
			{
				sToolbarBtnDisabledStyle = new GUIStyle(EditorStyles.toolbarButton);
				Color _col = sToolbarBtnDisabledStyle.normal.textColor;
				_col.a = .5f;
				sToolbarBtnDisabledStyle.normal.textColor = _col;
				sToolbarBtnDisabledStyle.active = sToolbarBtnDisabledStyle.normal;
			}
			{
				sToolbarSortBtnStyle = new GUIStyle(EditorStyles.toolbarButton);
				sToolbarSortBtnStyle.padding = new RectOffset(2, 2, 2, 2);
			}
			
			{
				sToolbarNoPaddingBtnStyle = new GUIStyle(EditorStyles.toolbarButton);
				sToolbarNoPaddingBtnStyle.padding = new RectOffset(0, 0, 0, 0);
			}
			{
				sActionTextLabelStyle = new GUIStyle("label");
				Color _col = sActionTextLabelStyle.normal.textColor;
				_col.a /= 2f;
				sActionTextLabelStyle.normal.textColor = _col;
			}
			{
				GUIStyleState _normalStyleState = new GUIStyleState();
				Color _col = new Color(0, 0, 0, 0.05f);
				_normalStyleState.background = LW_Tools.GenerateBgTexture(_col);

				sGlobalBoxStyle = new GUIStyle("box");
				sGlobalBoxStyle.normal = _normalStyleState;
				sGlobalBoxStyle.padding = new RectOffset(0, 0, 0, 0);
			}
			{
				GUIStyleState _normalStyleState = new GUIStyleState();
				Color _col = new Color(0, 0, 0, 0.15f);
				_normalStyleState.background = LW_Tools.GenerateBgTexture(_col);

				sTitleBG = new GUIStyle();
				sTitleBG.normal = _normalStyleState;
			}
			{
				sPrevFrameIcon = new GUIContent(EditorGUIUtility.IconContent("Profiler.PrevFrame"));
			}
			{
				sNextFrameIcon = new GUIContent(EditorGUIUtility.IconContent("Profiler.NextFrame"));
			}
			{
				sPrevFrameNotAvailableIcon = new GUIContent(EditorGUIUtility.IconContent("Profiler.PrevFrame"));
				sPrevFrameNotAvailableIcon.image = ChangeTextureColor(sPrevFrameNotAvailableIcon.image as Texture2D, new Color(.5f, .5f, .5f, 1f));
			}
			{
				sNextFrameNotAvailableIcon = new GUIContent(EditorGUIUtility.IconContent("Profiler.NextFrame"));
				sNextFrameNotAvailableIcon.image = ChangeTextureColor(sNextFrameNotAvailableIcon.image as Texture2D, new Color(.5f, .5f, .5f, 1f));
			}
			{
				GUIStyleState _pressedTyleState = new GUIStyleState();
				Color _col = new Color(1f, 1f, 1f, 0.25f);
				_pressedTyleState.background = LW_Tools.GenerateBgTexture(_col);

				sBtnPrevNextNoBgStyle = new GUIStyle();
				sBtnPrevNextNoBgStyle.active = _pressedTyleState;
				sBtnPrevNextNoBgStyle.contentOffset = new Vector2(0, 2);
			}
			{
				sErrorIcon = new GUIContent(EditorGUIUtility.IconContent("console.erroricon.sml"));
			}
			{
				sWarningIcon = new GUIContent(EditorGUIUtility.IconContent("console.warnicon.sml"));
			}
#endif
			sStyleInit = true;
		}

		public static int GetShortcutId(EventModifiers mod)
		{
			switch (mod)
			{
				case EventModifiers.Shift:
					return 0;
				case EventModifiers.Control:
					return 1;
				case EventModifiers.Alt:
					return 2;
				case EventModifiers.Command:
					return 3;
				case EventModifiers.None:
					return 4;
				default:
					return 0;
			}
		}

		public static string GetCurShortcutActionStr(LW_EditorBridge.eMode curMode)
		{
			EventModifiers _curMod = LW_Tools.GetCurrentModifier();

			if (LW_Prefs.openFileShortcut == _curMod)
			{
				return "Open File";
			}
			else if (LW_Prefs.showHistoryShortcut == _curMod)
			{
				if (curMode == LW_EditorBridge.eMode.History || !LW_Prefs.keepHistory)
					return null;
				return "History";
			}
			else if (LW_Prefs.showStacktraceShortcut == _curMod)
			{
				return "StackTrace";
			}
			return null;
		}

		public static object TruncateDecimal(object value, int decimalToKeep)
		{
			if(value is float)
			{
				return TruncateDecimal(Convert.ToSingle( value ), decimalToKeep);
			}
			if(value is double)
			{
				return TruncateDecimal(Convert.ToDouble(value), decimalToKeep);
			}
			return value;
		}

		public static float TruncateDecimal( float value, int decimalToKeep)
		{
			float _multVal = Mathf.Pow(10f, decimalToKeep);
			int aux = (int)(value * _multVal);
			return aux / _multVal;
		}

		public static double TruncateDecimal(double value, int decimalToKeep)
		{
			double _multVal = Math.Pow(10f, decimalToKeep);
			int aux = (int)(value * _multVal);
			return aux / _multVal;
		}

		public static EventModifiers GetCurrentModifier()
		{
			if (Event.current.shift)
				return EventModifiers.Shift;
			if (Event.current.control)
				return EventModifiers.Control;
			if (Event.current.alt)
				return EventModifiers.Alt;
			if (Event.current.command)
				return EventModifiers.Command;
			return EventModifiers.None;
		}

		public static string RemoveFirstLines(string strIn, int linesToRemoveCount)
		{
			if (string.IsNullOrEmpty(strIn) || linesToRemoveCount <= 0)
				return strIn;
			try
			{
				int _ToRemoveId = strIn.IndexOf("\n") + 1;
				for (int i = 1; i < linesToRemoveCount; i++)
				{
					_ToRemoveId = strIn.IndexOf("\n", _ToRemoveId) + 1;
				}
				strIn = strIn.Remove(0, _ToRemoveId);
			}
			catch (Exception) { }

			return strIn;
		}

		public static bool IsNumericType(Type type)
		{
			switch (Type.GetTypeCode(type))
			{
				case TypeCode.Byte:
				case TypeCode.SByte:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.Decimal:
				case TypeCode.Double:
				case TypeCode.Single:
					return true;
				default:
					return false;
			}
		}

		public static Texture2D ChangeTextureColor(Texture2D inTex, Color col)
		{
			Texture2D result = new Texture2D(inTex.width, inTex.height, inTex.format, false);

			Graphics.CopyTexture(inTex, result);

			Color[] _newCol = { col };
			float _a;
			for (int x = 0; x < inTex.width; x++)
			{
				for (int y = 0; y < inTex.height; y++)
				{
					_a = result.GetPixel(x, y).a;
					if (_a > 0f)
					{
						_newCol[0].a = _a;
						result.SetPixels(x, y, 1, 1, _newCol);
					}

				}
			}
			result.Apply();
			return result;
		}


		public static void AddDefine(string define)
		{
			if (string.IsNullOrEmpty(define))
				return;
			List<string> _list = new List<string>();
			_list.Add(define);
			AddDefines(_list);
		}

		public static void AddDefines(List<string> definesToAdd)
		{
#if UNITY_EDITOR
			if (definesToAdd == null || definesToAdd.Count == 0)
				return;

			string _curDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
			List<string> _definesList = new List<string>(_curDefines.Split(';'));

			foreach (string define in definesToAdd)
			{
				if (!_definesList.Contains(define))
					_definesList.Add(define);
			}

			InternalApplyDefine(string.Join(";", _definesList.ToArray()));
#endif
		}

		public static void RemoveDefine(string define)
		{
			if (string.IsNullOrEmpty(define))
				return;
			List<string> _list = new List<string>();
			_list.Add(define);
			RemoveDefines(_list);
		}

		public static void RemoveDefines(List<string> definesToRemove)
		{
#if UNITY_EDITOR
			if (definesToRemove == null || definesToRemove.Count == 0)
				return;

			string _curDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
			List<string> _definesList = new List<string>(_curDefines.Split(';'));

			foreach (var define in definesToRemove)
			{
				_definesList.Remove(define);
			}

			InternalApplyDefine(string.Join(";", _definesList.ToArray()));
#endif
		}

		private static void InternalApplyDefine(string define)
		{
#if UNITY_EDITOR
			PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, define);
#endif
		}
		
		public static void RemovePreprocessorDefinition(string toRemove)
		{
#if UNITY_EDITOR
			string _definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
			string[] _allDefines = _definesString.Split(';');
			string _newDefine = "";
			for (int i = 0; i < _allDefines.Length; i++)
			{
				if (_allDefines[i] == toRemove)
					continue;
				_newDefine += _allDefines[i];
				if (i < _allDefines.Length - 1)
					_newDefine += ";";
			}
			if (_newDefine.EndsWith(";"))
				_newDefine = _newDefine.Substring(0, _newDefine.Length - 1);

			Debug.Log("rem NEW DEFINES : " + _newDefine);
			PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, _newDefine);
#endif
		}

		public static Texture2D GenerateBgTexture(Color col)
		{
			Color[] pix = new Color[1];
			pix[0] = col;

			Texture2D result = new Texture2D(1, 1);
#if UNITY_EDITOR
			result.alphaIsTransparency = true;
#endif
			result.SetPixels(pix);
			result.Apply();
			return result;
		}
	}
}
