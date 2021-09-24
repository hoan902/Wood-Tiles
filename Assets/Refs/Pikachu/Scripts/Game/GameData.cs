using System;
using System.Collections.Generic;
using UnityEngine;


public class CellData
{
    public int posX;
    public int posY;
    public int cellType;
    public Vector3 posWord;

    public CellData(int _posx, int _posy, int _cellType, Vector3 _posWord)
    {
        this.posX = _posx;
        this.posY = _posy;
        this.cellType = _cellType;
        this.posWord = _posWord;
    }

    public CellData(CellItem item)
    {
        posX = item.posX;
        posY = item.posY;
        cellType = item.cellType;
        posWord = item.transform.localPosition;
    }

}



public enum MoveMode
{
    Empty = 0,
    Idle,
    Left,
    Right,
    Up,
    Dow,
    Up_Left,
    Up_Right,
    Down_Left,
    Down_Right
}

public enum Move
{
    Up,
    Dow,
    Right,
    Left,
}

[Serializable]
public class MissionData
{
    public string missionName;
    public MISSION_ID ID;
    //public bool isLocked;
    //public bool isClear;
    public List<LevelData> levels;
    public MissionData()
    {
        levels = new List<LevelData>();
    }
}

[Serializable]
public class LevelData
{
    public int ID;
    public string LevelName;
    public bool IsCheckPoint = false;

    [Range(4, 20)]
    public int Width;
    [Range(4, 20)]
    public int Height;
    public int ThemeId;
    public MoveMode GlobalMoveMode;

    public bool IsLoop;
    public bool IsMagnet;
    public MoveMode[,] MatrixMove;

    [HideInInspector]
    public MISSION_ID MissionID;

    [HideInInspector]
    public bool IsCustomLevel;

}

public enum MissionState
{
    IsLock = 0,
    IsOpen = 1,
    IsClear = 2,
}

public enum LevelState
{
    IsLock = 0,
    IsOpen = 1,
    IsOneStar = 2,
    IsTwoStar = 3,
    IsThreeStar = 4
}