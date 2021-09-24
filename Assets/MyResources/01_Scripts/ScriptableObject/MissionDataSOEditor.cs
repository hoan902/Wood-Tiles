using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(MissionDataSO))]
class DecalMeshHelperEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MissionDataSO _target = (MissionDataSO)target;
        base.DrawDefaultInspector();
        //if (GUILayout.Button("Convert From Old Data"))
        //{
        //    _target._listMissions.Clear();

        //    foreach (var m in MissionManager.Instance.Missions)
        //    {
        //        MissionData deepM = new MissionData();
        //        deepM.ID = m.ID;
        //        deepM.missionName = m.ID.ToString();
        //        deepM.levels = new List<LevelData>();
        //        foreach (var lvl in m.levels)
        //        {
        //            deepM.levels.Add(new LevelData()
        //            {
        //                ID = lvl.ID,
        //                width = lvl.width,
        //                height = lvl.height,
        //                isClear = lvl.isClear,
        //                isLocked = lvl.isLocked,
        //                moveMode = lvl.moveMode,
        //                MissionID = lvl.MissionID
        //            });
        //        }

        //        _target._listMissions.Add(deepM);


        //    }

        //}

        if (GUILayout.Button("Save Data"))
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        //if (GUILayout.Button("One Mission Only"))
        //{
        //    for (int i = 1; i < _target._listMissions.Count; i++)
        //    {
        //        _target._listMissions[0].levels.AddRange(_target._listMissions[i].levels);
        //    }
        //}

        if (GUILayout.Button("Reset All Level"))
        {
            for (int i = 0; i < _target._listMissions[0].levels.Count; i++)
            {
                var lvl = _target._listMissions[0].levels[i];
                lvl.LevelName = $"Level {i + 1}";
                lvl.ID = (i + 1);
                lvl.IsCheckPoint = i % 10 == 0;
                lvl.MissionID = _target._listMissions[0].ID;
                lvl.Width = 10;
                lvl.Height = 12;
                lvl.IsLoop = false;
                lvl.IsMagnet = true;
                lvl.GlobalMoveMode = MoveMode.Idle;
            }
        }

    }
}

#endif