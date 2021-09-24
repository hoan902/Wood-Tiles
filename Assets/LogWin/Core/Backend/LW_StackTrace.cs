using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Diagnostics;
namespace LogWinInternal
{
	public class LW_StackTrace
	{
		public List<StackItem> mStack;
		public bool mProcessed = false;

		public void Process()
		{
			foreach(StackItem item in mStack)
			{
				item.Process();
			}
			mProcessed = true;
		}

		public class StackItem
		{
			public void Process()
			{
				if(mStackFrame != null)
				{
					try
					{
						mLine = mStackFrame.GetFileLineNumber();
					}catch (Exception) { }
					try
					{
						mPath = mStackFrame.GetFileName().Replace("\\", "/").Replace(Application.dataPath, "Assets");
					}
					catch (Exception) { }
					try
					{
						mFile = mPath.Substring(mPath.LastIndexOf("/") + 1);
					}
					catch (Exception) { }
					try
					{
						mCallerFunc = mStackFrame.GetMethod().DeclaringType.ToString() + ":" + mStackFrame.GetMethod().Name;
					}catch (Exception) { }
				}
			}

			public StackItem(StackFrame stackFrame)
			{
				mStackFrame = stackFrame;
			}

			public void OpenFile()
			{
#if UNITY_EDITOR
				AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(mPath), mLine);
#endif
			}

			public string mCallerFunc;
			public int mLine;
			public string mPath;
			public string mFile;
			StackFrame mStackFrame;
		}

		public LW_StackTrace(int stackFrames = 1)
		{
			mStack = new List<StackItem>(stackFrames);
		}
		
		public void OpenFile(int pos = 0)
		{
			if (mStack.Count > pos && pos >= 0)
				mStack[pos].OpenFile();
		}

		public static LW_StackTrace GenerateStackTrace(int skipLinesCount = 0)
		{
			StackTrace st = new StackTrace(skipLinesCount,true);
			StackFrame sf;
			LW_StackTrace _stack = new LW_StackTrace(st.FrameCount);

			for (int i = 0; i < st.FrameCount; i++)
			{
				sf = st.GetFrame(i);
				_stack.mStack.Add(new StackItem(sf));
			}
			return _stack;
		}

		public void GUIDraw(Rect position, LW_LogElement selectedElem)
		{
#if UNITY_EDITOR
			LW_Tools.SetupStyles();

			bool _style0 = true;
			foreach (StackItem stackItem in mStack)
			{
				Rect _btnRect;

				if (_style0)
					_btnRect = EditorGUILayout.BeginHorizontal(LW_Tools.sElementBoxStyle0);
				else
					_btnRect = EditorGUILayout.BeginHorizontal(LW_Tools.sElementBoxStyle1);



				if (GUI.Button(_btnRect, GUIContent.none, GUIStyle.none))
				{
					if (Event.current.button == 1)      //right click
					{
						GenericMenu _menu = new GenericMenu();
						_menu.AddItem(new GUIContent("Open File"), false, () => { stackItem.OpenFile(); });
						_menu.AddSeparator("");
						_menu.AddItem(new GUIContent("Close"), false, () => { });
						_menu.ShowAsContext();
					}
					else if (Event.current.button == 0) //left click
					{
						stackItem.OpenFile();
					}
				}

				GUILayout.Label(stackItem.mCallerFunc);
				GUILayout.FlexibleSpace();
				GUILayout.Label(stackItem.mFile + "(Line:" + stackItem.mLine + ")");
				GUILayout.EndHorizontal();

				_style0 = !_style0;

			}
#endif
		}
	}
}

