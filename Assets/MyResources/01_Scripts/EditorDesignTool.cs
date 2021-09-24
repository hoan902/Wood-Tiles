using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using System.IO;
using UnityEditor;

[ExecuteInEditMode]
public class EditorDesignTool : EditorWindow
{
    private MISSION_ID _missionID = MISSION_ID.CLASSIC;
    private int _level = 1;

    private int _width = 4;
    private int _height = 4;
    private int _theme = 0;
    private bool _isLoop = false;
    private bool _isMagnet = true;
    private MoveMode _moveMode = MoveMode.Idle;

    private TileData _tileData;


    [MenuItem("DesignTool/Open")]
    static void Init()
    {
        EditorDesignTool window = (EditorDesignTool)EditorWindow.GetWindow(typeof(EditorDesignTool));
        window.Show();
        window.position = new Rect(20, 80, 550, 500);
    }

    private void OnGUI()
    {
        GUILayout.Label("Create Custom Level Design here!");

        EditorGUILayout.Space();
        _missionID = (MISSION_ID)EditorGUILayout.EnumPopup("World: ", _missionID);
        _level = EditorGUILayout.IntField("Level: ", _level);

        EditorGUILayout.Space();
        _width = EditorGUILayout.IntField("Width: ", _width);
        _height = EditorGUILayout.IntField("Height: ", _height);
        _theme = EditorGUILayout.IntField("Height: ", _theme);
        _isLoop = EditorGUILayout.Toggle("Is Loop:", _isLoop);
        _moveMode = (MoveMode)EditorGUILayout.EnumPopup("Move Mode:", _moveMode);

        EditorGUILayout.Space();
        if (GUILayout.Button("Generate Design File"))
        {
            CreateLevelDesignFile();
        }

        if (Application.isPlaying)
        {
            if (GUILayout.Button("Validate Design File"))
            {
                ValidateDesignFile();
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Please Run Game to Enable VALIDATE DESIGN!", MessageType.Info);
        }


    }

    private void CreateLevelDesignFile()
    {
        var fileName = ResourcesManager.GetLevelFileName(this._missionID, this._level);
        string path = $"Assets/Resources/LevelDesigns/{fileName}.txt";
        if (!File.Exists(path))
        {
            File.WriteAllText(path, GenrateLevelDesignContent());

            AssetDatabase.Refresh();
            // Load object
            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));

            // Select the object in the project folder
            Selection.activeObject = obj;

            // Also flash the folder yellow to highlight it
            EditorGUIUtility.PingObject(obj);

        }
        else
        {
            EditorUtility.DisplayDialog("Fail!", $"Already contains file at path {path} ", "Close");
        }
    }

    private string GenrateLevelDesignContent()
    {
        string result = "";

        result += $"{_width}\n";
        result += $"{_height}\n";
        result += $"Loop:\t{_isLoop}\n";
        result += $"Magnet:\t{_isMagnet}\n";
        result += $"{_theme}\n";

        string row = "";
        for (int i = 0; i < _height; i++)
        {
            row = "";
            for (int j = 0; j < _width; j++)
            {
                row += $"{(int)this._moveMode}";
                if (j < _width - 1)
                    row += '\t';
            }
            result += row;
            result += '\n';
        }

        return result;
    }

    #region Validate Design File
    private void ValidateDesignFile()
    {
        bool isCustomLevel = false;
        bool containLevelTileDesign = false;
        bool validatedLevelTileDesign = false;
        int matrixCells = 0;
        int levelTileCells = 0;

        string Msg = $"Level {_level}\n";

        //check is custom level matrix
        var _levelGeneral = MissionManager.Instance.GetLevelData(this._missionID, this._level);
        if (_levelGeneral != null)
        {
            isCustomLevel = _levelGeneral.IsCustomLevel;
            Msg += $"   - Is Custom Level: {isCustomLevel}\n";
        }

        //check level tile design
        var _levelInfo = DesignHelper.GetLevelInfoData(this._level);
        containLevelTileDesign = _levelInfo != null;
        Msg += $"   - Contains LevelTileDesign: {containLevelTileDesign}\n";
        Msg += "-----------------------------------\n";
        Msg += "Cells Check:\n";
        //check total Matrix cells
        if (_levelGeneral != null && _levelInfo != null)
        {
            matrixCells = _levelGeneral.Width * _levelGeneral.Height - _levelGeneral.CountLevelEmptyCell();
            levelTileCells = _levelInfo._totalTile;

            Msg += $"   - Matrix Cells Count: {matrixCells} ";
            Msg += matrixCells % 2 == 0 ? "<VALID>\n" : "<ERROR>\n";

            Msg += $"   - LevelTileDesign Cells Count: {levelTileCells} ";
            Msg += levelTileCells % 2 == 0 ? "<VALID>\n" : "<ERROR>\n";

            if (matrixCells != levelTileCells)
            {
                Debug.LogError("Level Matrix not match level tile design!");
                Msg += $"   - Cell comparison: <ERROR>\n";
            }
            else
            {
                Msg += $"   - Cell comparison: <VALID>\n";
                validatedLevelTileDesign = true;
            }
            Msg += "-----------------------------------\n";

            //rewards
            Msg += "Rewards:\n";
            foreach (var item in _levelInfo._listRewardData)
            {
                Msg += $"    - {item._extends} - {item._num}";
            }

        }




        EditorUtility.DisplayDialog("Result", Msg, "Close");
    }



    #endregion
}

#endif