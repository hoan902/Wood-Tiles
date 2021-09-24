using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LogWinInternal
{
	public class LW_LogCategory
	{
		Dictionary<string, LW_LogElementHolder> mLogs = new Dictionary<string, LW_LogElementHolder>();
		public string mName;
#if UNITY_EDITOR
		bool mFoldout = true;
#endif
		public int mId;
		static int sCatCount = 0;

		public LW_LogCategory(string name)
		{
			mName = name;
			mId = sCatCount;
			sCatCount++;
		}

		public Dictionary<string, LW_LogElementHolder> GetHolders()
		{
			return mLogs;
		}

		public void Clear()
		{
			foreach (KeyValuePair<string, LW_LogElementHolder> logHolder in mLogs)
			{
				logHolder.Value.Clear();
			}
			mLogs.Clear();
		}

		public void AddLog(LW_LogElement logElement, string key)
		{
			if (!mLogs.ContainsKey(key))
			{
				mLogs.Add(key, new LW_LogElementHolder(key));
			}
			LW_LogElementHolder _holder = mLogs[key];

			_holder.AddElement(logElement);
		}

		public LW_LogElement GetLog(string key)
		{
			if(mLogs.ContainsKey(key))
				return mLogs[key].mCurrentElement;
			return null;
		}

		public void FreezeAllHisto()
		{
			foreach (KeyValuePair<string, LW_LogElementHolder> logHolder in mLogs)
			{
				logHolder.Value.FreezeHistory();
			}
		}

		public void GUIDraw(LW_LogElement elemSelected, LW_LogElementHolder holderSelected, LW_EditorBridge.eMode mode, bool isAtRealtimeFrame, int frame)
		{
			LW_Tools.SetupStyles();

#if UNITY_EDITOR
			GUILayout.BeginVertical(LW_Tools.sGlobalBoxStyle);


			GUILayout.BeginVertical(LW_Tools.sTitleBG);
			mFoldout = EditorGUILayout.Foldout(mFoldout, mName,true);
			GUILayout.EndVertical();


			bool _style0 = true;
			bool _isElement = false;
			bool _HasDrawElem;

			if (mFoldout)
			{
				EventModifiers _curModifier = LW_Tools.GetCurrentModifier();

				foreach (KeyValuePair<string, LW_LogElementHolder> logHolder in mLogs)
				{
					_isElement = (elemSelected != null && elemSelected.mHolder == logHolder.Value)||(holderSelected != null && holderSelected == logHolder.Value);

					_HasDrawElem = logHolder.Value.GUIDrawLogs(_style0, _isElement, _curModifier, mode, isAtRealtimeFrame, frame);

					if (_HasDrawElem)
					{
						_style0 = !_style0;
					}

					//logHolder.Value.mCurrentElement.GUIDrawSelf(_style0, _isSelectedElement, _curModifier, mode);

					
				}

			}
			GUILayout.EndVertical();
#endif
		}


	}
}