using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using EventManager;
using System;
using Random = UnityEngine.Random;
using MEC;
using Lean;
using MEC;

[Serializable]
public struct CellPos
{
    public int posX;
    public int posY;
}

public class GameController : SingletonMonoBehaviour<GameController>
{
    // [SerializeField]
    //Transform itemPrefab;     // prefab of item

    public bool EnableDebug {
        get {
#if UNITY_EDITOR    
            return true;
#else
            return false;
#endif
        }
    }


    public static int ROCKET_ID = 11;

    public bool IsBusy { get; private set; }

    [Header("Special Items")]
    public RocketObject _rocketPrefab;

    public Transform gridParent;
    CellData[,] cellDatas;
    // mapdata
    List<CellItem> cellItems = new List<CellItem>();
    //init data;         // list object item
    Dictionary<int, int> _dictCellType;

    [SerializeField]
    LineConnect line;
    //Draw line connect
    public int width, height;
    //  public float minx, miny;
    GameMaster gameMgr;
    CellItem firstSelected;
    // storage item first selected
    public float cellWidth, cellHeight;
    // cell width and height

    MoveMode moveMode;

    [SerializeField]
    RectTransform rectGrid;

    [SerializeField]
    Sprite[] sprites;

    //Tiles Set sprite array HoanDN
    [SerializeField]
    Sprite[] dogSprites;

    [SerializeField]
    Sprite[] japanSprites;

    [SerializeField]
    Sprite[] usSprites;

    [SerializeField]
    Sprite[] pinkDessertsSprites;

    [SerializeField]
    Sprite[] biscuitsSprites;

    [SerializeField]
    Sprite[] sweetsSprites;

    [SerializeField]
    Sprite[] chocolateSprites;

    [SerializeField]
    Sprite[] halloweenSprites;
    
    [SerializeField]
    Sprite[] valentineSprites;

    [SerializeField]
    Sprite[] medicineSprites;

    [SerializeField]
    Sprite[] teaSprites;
    
    [SerializeField]
    Sprite[] australiaSprites;
    
    [SerializeField]
    Sprite[] cheeseSprites;
    
    [SerializeField]
    Sprite[] hikingSprites;

    //HoanDN Change Tiles Set Theme
    List<Sprite[]> tileSetList;
    int TILES_SET_ID;
    int currentTilesSetID;

    private LevelData _levelData;
    public LevelInfoData _levelInfoData { get; private set; }

    private float timeCurrent;
    float timePlay;
    public ParticleSystem effectWin;

    public GameObject rkSpawn;

    private void Update()
    {
        //DrawMatrix();

        if (SROptions.Current != null && SROptions.Current.GodMod)
        {
            UpdateGodMod(Time.deltaTime);
        }
    }

    /// <summary>
    /// Init game
    /// </summary>
    /// <param name="_width"></param>
    /// <param name="_height"></param>
    /// <param name="_typeCount"></param>
    public float InitMap(GameMaster gameMgr, LevelData levelData, MoveMode _moveMode)
    {
        float _timePlay = 0;
        Reset();
        _levelData = levelData;
        this.width = levelData.Width + 2;
        this.height = levelData.Height + 2;
        TILES_SET_ID = levelData.ThemeId;
        this.gameMgr = gameMgr;
        this.moveMode = _moveMode;
        this._levelInfoData = DesignHelper.GetLevelInfoData(levelData.ID);
        _timePlay = this._levelInfoData._time;

        tileSetList = new List<Sprite[]>();
        tileSetList.Add(dogSprites);
        tileSetList.Add(japanSprites);
        tileSetList.Add(usSprites);
        tileSetList.Add(pinkDessertsSprites);
        tileSetList.Add(biscuitsSprites);
        tileSetList.Add(sweetsSprites);
        tileSetList.Add(chocolateSprites);
        tileSetList.Add(halloweenSprites);
        tileSetList.Add(valentineSprites);
        tileSetList.Add(medicineSprites);
        tileSetList.Add(australiaSprites);
        tileSetList.Add(cheeseSprites);
        tileSetList.Add(hikingSprites);

        if (TILES_SET_ID <= tileSetList.Count - 1 && TILES_SET_ID > 0)
        {
            sprites = tileSetList[TILES_SET_ID];
        }else 
        {
            TILES_SET_ID = Random.Range(0, tileSetList.Count - 1);
            sprites = tileSetList[TILES_SET_ID];
        }
        if (_timePlay <= 0)
        {
            int timeScale = moveMode == MoveMode.Idle ? 2 : 3;
            _timePlay = levelData.Width * levelData.Height * timeScale;
        }
        currentTilesSetID = TILES_SET_ID;
        //Debug.LogError(currentTilesSetID);


        GenMap();
        SortList();
        IsBusy = false;
        return _timePlay;


    }

    public void GoHome()
    {
        Reset();
        UIManager.Instance.ShowPage("StartPage");
    }

    void Reset()
    {
        firstSelected = null;
        line.gameObject.SetActive(false);
        //  dict = null;
        //cellDatas = null;
        // cellItems = null;
        if (cellItems == null)
        {
            cellItems = new List<CellItem>();

        }
        foreach (CellItem item in cellItems)
        {
            if (item.gameObject.activeSelf)
            {
                item.DeSpawnCell();
            }
            //ContentMgr.Instance.Despaw(item.gameObject);
        }

        if (_dictCellType == null)
            _dictCellType = new Dictionary<int, int>();
        else
            _dictCellType.Clear();

        cellItems.Clear();
    }

    public Vector2 GetPos(int posX, int posY)
    {
        var x = cellWidth * (posX - (width - 2) / 2f - .5f) * 1.055f * (1 - 1 / 20f);
        var y = cellHeight * (posY - (height - 2) / 2f - .5f) * 1.05f * (1 - 1 / 20f);
        return new Vector2(x, y);
    }

    int CountEmptyCell()
    {
        int result = 0;
        for (int i = 0; i < this.width - 2; i++)
        {
            for (int j = 0; j < this.height - 2; j++)
            {
                if (this._levelData.MatrixMove[i, j] == 0)
                    ++result;
            }
        }
        return result;
    }

    /// <summary>
    /// Gen map
    /// </summary>
    void GenMap()
    {
        //Minh.HN turn off
        //if (width * height % 2 != 0)     //  the total must be even
        //{
        //    Debug.LogError("Error  total must be even");
        //    return;
        //}

        cellDatas = new CellData[width, height];            //init data
                                                            //cellItems.Clear();
                                                            //        Debug.Log(rectGrid.rect.width + " "+rectGrid.rect.height);
        int tmpCellH = (int)(rectGrid.rect.height / (height - 2));    // screen height is 320

        int tmpCellW = (int)(rectGrid.rect.width / (width - 2));    // screen height is 320

        /*Debug.LogError("tmpCellH = " + tmpCellH);
        Debug.LogError("tmpCellW = " + tmpCellW);
        Debug.LogError("tmpCellH * 0.825f = " + tmpCellH * 0.825f);*/

        if (tmpCellW > tmpCellH * 0.95f)
        {
            cellHeight = tmpCellH;    // screen height is 320
            //cellWidth = (int)(cellHeight * 0.825f); //HoanDN change value to match new tiles cell
            cellWidth = (int)(cellHeight * 0.95f);
        }
        else
        {
            cellWidth = tmpCellW;
            //cellHeight = (int)(cellWidth / 0.825f); //HoanDN change value to match new tiles cell
            cellHeight = (int)(cellWidth / 0.95f);
        }

        cellWidth *= 0.98f;
        cellHeight *= 0.98f;


        int length = (width - 2) * (height - 2);
        //count empty cell
        int emptyCells = CountEmptyCell();

        if ((length - emptyCells) % 2 != 0)
        {
            Debug.LogError($"Total cells must be even!!! {(length - emptyCells)}");
            return;
        }

        int pairs = (int)((length - emptyCells) / 2);
        List<int> pool = new List<int>();
        List<Sprite> _listPoolSprite = new List<Sprite>();
        if (_levelInfoData._totalTile == (length - emptyCells)) //pool from design
        {
            foreach (var tile in _levelInfoData._listTileData)
            {
                for (int i = 0; i < tile.Number; i++)
                {
                    pool.Add((int)(tile.Tile));
                }
            }


        }
        else // random pool old logic
        {
            Debug.LogError($"Level tile design does not match matrix {_levelInfoData.Level} -matrix:{length - emptyCells} -tiledesign:{_levelInfoData._totalTile}");
            if (pairs < sprites.Length)
            {
                int rand = Random.Range(1, 3);
                for (int i = 0; i < pairs - rand; i++)
                {
                    _listPoolSprite.Add(sprites[Random.Range(0, sprites.Length)]);
                }
            }
            else
            {
                _listPoolSprite.AddRange(sprites);
            }


            int maxCount = pairs < _listPoolSprite.Count ? 1 : (pairs / _listPoolSprite.Count) + pairs % _listPoolSprite.Count + 1;

            for (int i = 0; i < (length - emptyCells) / 2; i++)
            {
                int value = 1;
                do
                {
                    value = Random.Range(0, _listPoolSprite.Count);

                } while (pool.FindAll(x => x == value).Count() > (maxCount));
                pool.Add(value);
                pool.Insert(0, value);
            }

        }

        //shufle list to random position item
        for (int i = 0; i < (length - emptyCells); i++)
        {
            int index1 = Random.Range(0, (length - emptyCells));
            int index2 = Random.Range(0, (length - emptyCells));
            int temp = pool[index1];
            pool[index1] = pool[index2];
            pool[index2] = temp;
        }
        //        Debug.Log(minx + "  " + miny + "  " +cellWidth +"   "+ cellHeight);

        // default maps data is -1
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 pos = GetPos(i, j);
                cellDatas[i, j] = new CellData(i, j, -1, pos);
            }
        }

        // create item object and set type object
        int id = 0;
        for (int j = 1; j < height - 1; j++)
        {
            for (int i = width - 2; i > 0; i--)
            {
                if (this._levelData.MatrixMove[i - 1, j - 1] == 0)
                    continue;
                int type = pool[id];
                id++;

                CellItem cellItem = CreateItem(type, i, j);
                cellItems.Add(cellItem);
                cellDatas[i, j].cellType = type;

                int CountType = 0;
                _dictCellType.TryGetValue(cellItem.cellType, out CountType);
                if (CountType != 0)
                {
                    _dictCellType[cellItem.cellType]++;
                }
                else
                {
                    _dictCellType.Add(cellItem.cellType, 1);
                }


            }
        }
        if (!CheckAnswer())
        {
            OnSwap();
        }

        _listPoolSprite.Clear();
        _timerAutoPair = 0f;

    }

    void SortList()
    {
        cellItems = cellItems.OrderByDescending(x => x.posY).ThenBy(x => x.posX).ToList();
        for (int i = 0; i < cellItems.Count; i++)
        {
            cellItems[i].transform.SetSiblingIndex(i);
        }
    }

    /// <summary>
    /// Create Item object
    /// </summary>
    /// <param name="type"></param>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <returns></returns>
    CellItem CreateItem(int type, int i, int j)
    {
        GameObject go = ContentMgr.Instance.GetItem("Cell");
        go.transform.SetParent(gridParent);

        go.GetComponent<RectTransform>().sizeDelta = new Vector2(1 * cellWidth, 1 * cellHeight);
        go.transform.localScale = Vector3.one;

        go.name = (i + "" + j).ToString();
        CellItem item = go.GetComponent<CellItem>();
        Sprite sprite = null;

        if (type >= 0 && type < sprites.Length)
        {
            sprite = sprites[type];
        }
        else
        {
            Debug.LogError($"cant get sprite with {type}");
        }
        item.Init(this, i, j, type, sprite, type >= 28);  //Minh.HN: from 28 is new sprite without border
        return item;
    }

    /// <summary>
    /// Call when select item
    /// </summary>
    /// <param name="item"></param>
    public void OnPressItem(CellItem item)
    {
        if (firstSelected == null)
        {
            firstSelected = item;
            AudioManager.Instance.Shot("button");
            Debug.Log("OnClickFirstSelected");
        }
        else
        {
            if (firstSelected.posX == item.posX && firstSelected.posY == item.posY)    //  select object again
            {
                firstSelected = null;
                return;
            }

            List<CellData> path = CheckPath(firstSelected, item);  // check path 

            if (path != null)
            {
                // destroy end draw line folow path
                OnPair(path, firstSelected, item);
            }
            else
            {
                //Minh.HN: turn off old logic
                //AudioManager.Instance.Shot("false");
                //firstSelected.OnDeselected();
                //item.OnDeselected();
                //firstSelected = null;

                //Minh.HN: new logic
                firstSelected.OnDeselected();
                item.OnDeselected();
                firstSelected = null;

                firstSelected = item;
                item.OnSelected();
            }
        }
        GameMaster.Instance.DoVibratePhone();
    }

    IEnumerator UpdateCellData()
    {
        yield return new WaitForSeconds(0.5f);

        foreach (CellData item in cellDatas)
        {
            CellItem cellItem = cellItems.Find(x => x.posX == item.posX && x.posY == item.posY);
            if (cellItem)
            {
                item.cellType = cellItem.cellType;
            }
            else
            {
                item.cellType = -1;
            }
        }

        if (!CheckAnswer())
        {
            OnSwap();
        }

    }

    IEnumerator MoveItem02(CellItem firstItem, CellItem secondItem, float time, Action<bool> OnComplete)
    {
        int increaseChecker = 0;
        yield return new WaitForSeconds(time);
        float mid = (width - 1) / 2f;
        cellDatas[firstItem.posX, firstItem.posY].cellType = -1;
        cellDatas[secondItem.posX, secondItem.posY].cellType = -1;
        if (_levelData.IsLoop)
        {
            MoveBoardsLoop();
        }
        else if (_levelData.IsMagnet)
        {
            while (increaseChecker <= 4)
            {
                MoveBoardsNonLoopMagnet();
                yield return null;
                increaseChecker++;
            }
        }
        else
        {
            MoveBoardsOneCell();
        }
        StartCoroutine(UpdateCellData());
        SortList();
        yield return new WaitForSeconds(time);
        OnComplete?.Invoke(true);
    }


    private Tuple<CellItem, CellItem> _cachedRocketTarget = null;

    void OnPair(List<CellData> path, CellItem firstItem, CellItem secondItem)
    {
        RocketObject rk01 = null;
        RocketObject rk02 = null;
        CellItem targetRocket01 = null;
        CellItem targetRocket02 = null;
        bool DoRocketLaunch = false;

        AudioManager.Instance.Shot("true");
        //GameMaster.Instance.DoVibratePhone();


        _dictCellType[firstItem.cellType]--;
        _dictCellType[secondItem.cellType]--;

        CellData first = new CellData(firstItem);
        CellData second = new CellData(secondItem);
        cellItems.Remove(firstItem);
        cellItems.Remove(secondItem);

        firstItem.DeSpawnCell(0.2f);
        secondItem.DeSpawnCell(0.2f);

        _cachedRocketTarget = null;
        if (firstItem.cellType == ROCKET_ID)
        {
            var targetCells = FindTargetRocketTile();
            if (targetCells != null && targetCells.Item1 != null && targetCells.Item2 != null)
            {
                _cachedRocketTarget = targetCells;
                targetRocket01 = targetCells.Item1;
                targetRocket01.isTargetOfRocket = true;
                rk01 = LeanPool.Spawn(_rocketPrefab, firstItem.transform.position, Quaternion.identity, gridParent);
                rk01.transform.position = firstItem.transform.position;
                rk01.Initialize(targetCells.Item1.transform);
                GameMaster.Instance.PlayEffect(FX_ENUM.STAR_EXPLODE, rk01.transform.position, rk01.transform.parent);

                targetRocket02 = targetCells.Item2;
                targetRocket02.isTargetOfRocket = true;
                rk02 = LeanPool.Spawn(_rocketPrefab, secondItem.transform.position, Quaternion.identity, gridParent);
                rk02.transform.position = secondItem.transform.position;
                rk02.Initialize(targetCells.Item2.transform);
                GameMaster.Instance.PlayEffect(FX_ENUM.STAR_EXPLODE, rk02.transform.position, rk02.transform.parent);

                DoRocketLaunch = true;
            }

        }
        else
        {
            GameMaster.Instance.PlayEffect(FX_ENUM.STAR_EXPLODE, firstItem.transform.position, firstItem.transform.parent);
            GameMaster.Instance.PlayEffect(FX_ENUM.STAR_EXPLODE, secondItem.transform.position, secondItem.transform.parent);
        }

        IsBusy = true;
        StartCoroutine(MoveItem02(firstItem, secondItem, 0.35f, (success) =>
        {
            if (success)
            {
                if (DoRocketLaunch)
                {
                    OnPairRocketTile(rk01, targetRocket01, rk02, targetRocket02, (complete) =>
                     {
                         //GameMaster.Instance.DoVibratePhone();
                         IsBusy = false;
                     });
                }
                else
                {
                    IsBusy = false;
                }
            }
        }));


        path.Add(second);
        path.Insert(0, first);
        DrawLineConnect(path);

        //reset first selected
        firstSelected = null;

        //Reset celltype data
        cellDatas[firstItem.posX, firstItem.posY].cellType = -1;
        cellDatas[secondItem.posX, secondItem.posY].cellType = -1;

        this.PostEvent(EventID.UpdateScore, (long)(GameDataMgr.CurrentScore + 10));

        CheckWin();
    }


    /// <summary>
    /// Draw line connect
    /// </summary>
    /// <param name="path"></param>
    void DrawLineConnect(List<CellData> path)
    {
        line.DrawLine(path);
    }

    void CheckWin()
    {
        if (cellItems.Count <= 0)
        {
            StartCoroutine(DelayLevelFinish());
        }

        IEnumerator DelayLevelFinish()
        {
            yield return new WaitForSeconds(0.3f);
            this.PostEvent(EventID.ShowWinEffect);
            effectWin.Play();
            yield return new WaitForSeconds(1.8f);
            gameMgr.LeveFinish();
        }
    }

    /// <summary>
    /// check answer 
    /// </summary>
    /// <returns></returns>
    bool CheckAnswer()
    {
        if (cellItems.Count <= 0)
            return true;

        for (int i = 0; i < cellItems.Count - 1; i++)
        {
            for (int j = i + 1; j < cellItems.Count; j++)
            {
                List<CellData> path = CheckPath(cellItems[i], cellItems[j]);
                if (path != null)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void Trick()
    {
        if (cellItems.Count > 0)
        {
            for (int i = 0; i < cellItems.Count - 1; i++)
            {
                for (int j = i + 1; j < cellItems.Count; j++)
                {
                    List<CellData> path = CheckPath(cellItems[i], cellItems[j]);
                    if (path != null)
                    {
                        OnPair(path, cellItems[i], cellItems[j]);
                        return;
                    }
                }
            }
        }

    }

    /// <summary>
    /// swap item if not answer
    /// </summary>
    public void OnSwap()
    {
        gameMgr.UpdateResetCount();
        // swap

        int countPreventOverflow = 0;
        do
        {
            int n = cellItems.Count;
            while (n > 1)
            {
                n--;
                int k = Random.Range(0, n + 1);

                int type = cellItems[n].cellType;
                cellItems[n].cellType = cellItems[k].cellType;
                cellItems[k].cellType = type;
            }
            countPreventOverflow++;
            if (countPreventOverflow >= 100)
            {
                Debug.LogError("Overflow OnSwap");
                break;
            }

        } while (!CheckAnswer());

        foreach (CellItem item in cellItems)
        {
            //if(item!=null)
            item.UpdateSprite(sprites[item.cellType]);
            cellDatas[item.posX, item.posY].cellType = item.cellType;
        }
    }

    //HoanDN Change Tiles Set Theme
    public void OnChangeTheme()
    {
        gameMgr.UpdateResetCountTheme();
        int randomTileSet = Random.Range(0, tileSetList.Count - 1);
        if (randomTileSet != currentTilesSetID)
        {
            sprites = tileSetList[randomTileSet];
            currentTilesSetID = randomTileSet;
            //Debug.LogError("OnChangeTheme: " + currentTilesSetID);
        }
        else
        {
            if(currentTilesSetID >= 0 && currentTilesSetID < tileSetList.Count - 1)
            {
                sprites = tileSetList[randomTileSet + 1];
                currentTilesSetID = randomTileSet;
            }else if (currentTilesSetID == tileSetList.Count - 1)
            {
                sprites = tileSetList[randomTileSet - 1];
                currentTilesSetID = randomTileSet;
            }
            //Debug.LogError("OnChangeTheme get duplicated test: " + currentTilesSetID);
        }
        foreach (CellItem item in cellItems)
        {
            //if(item!=null)
            item.UpdateSprite(sprites[item.cellType]);
            cellDatas[item.posX, item.posY].cellType = item.cellType;
        }
    }


    /// <summary>
    /// Check 2 point are pair
    /// </summary>
    /// <param name="first"></param>
    /// <param name="second"></param>
    List<CellData> CheckPath(CellItem firstItem, CellItem secondItem)
    {
        CellData first = new CellData(firstItem);
        CellData second = new CellData(secondItem);

        List<CellData> result = new List<CellData>();
        if (first.cellType != second.cellType)
        {
            return null;
        }
        if (CheckNearby(first, second))
        {
            result.Add(first);
            result.Add(second);
            return result;
        }
        float distance = width * height;
        bool find = false;
        CellData point1 = null, point2 = null;
        List<CellData> cellsEnableFirst = GetCellsEnableFromCell(first);
        List<CellData> cellsEnableSecond = GetCellsEnableFromCell(second);
        for (int i = 0; i < cellsEnableFirst.Count; i++)
        {
            for (int j = 0; j < cellsEnableSecond.Count; j++)
            {
                if (CheckPathLine(cellsEnableFirst[i], cellsEnableSecond[j]))
                {
                    if (Distance(cellsEnableFirst[i], cellsEnableSecond[j]) < distance)
                    {
                        point1 = cellsEnableFirst[i];
                        point2 = cellsEnableSecond[j];
                        find = true;
                        distance = Distance(cellsEnableFirst[i], cellsEnableSecond[j]);
                    }

                }
            }
        }
        if (find)
        {
            result.Add(point1);
            result.Add(point2);
            return result;
        }
        else
        {
            return null;
        }

    }

    float Distance(CellData first, CellData second)
    {
        return Vector2.Distance(new Vector2(first.posX, first.posY), new Vector2(second.posX, second.posY));
    }

    /// <summary>
    /// check nearby
    /// </summary>
    /// <param name="first"></param>
    /// <param name="second"></param>
    /// <returns></returns>

    bool CheckNearby(CellData first, CellData second)
    {
        if (first.posX == second.posX && Mathf.Abs(first.posY - second.posY) == 1)
        {
            return true;
        }
        else if (first.posY == second.posY && Mathf.Abs(first.posX - second.posX) == 1)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Get cell list enable from cell
    /// </summary>
    /// <param name="from"></param>
    /// <returns></returns>
    List<CellData> GetCellsEnableFromCell(CellData from)
    {
        List<CellData> result = new List<CellData>();
        // for right
        for (int i = from.posX + 1; i < width; i++)
        {
            CellData cell = cellDatas[i, from.posY];
            if (cell.cellType != -1)
                break;
            if (CheckPathLine(from, cell))
                result.Add(cell);
        }
        //for left
        for (int i = from.posX - 1; i >= 0; i--)
        {
            CellData cell = cellDatas[i, from.posY];
            if (cell.cellType != -1)
                break;
            if (CheckPathLine(from, cell))
                result.Add(cell);
        }

        //for up
        for (int i = from.posY + 1; i < height; i++)
        {
            CellData cell = cellDatas[from.posX, i];
            if (cell.cellType != -1)
                break;
            if (CheckPathLine(from, cell))
                result.Add(cell);
        }

        //for down
        for (int i = from.posY - 1; i >= 0; i--)
        {
            CellData cell = cellDatas[from.posX, i];
            if (cell.cellType != -1)
                break;
            if (CheckPathLine(from, cell))
                result.Add(cell);
        }
        return result;
    }

    /// <summary>
    /// Check path line 2 point
    /// </summary>
    /// <param name="to"></param>
    /// <param name="from"></param>
    /// <returns></returns>
    bool CheckPathLine(CellData to, CellData from)
    {
        //if (to.posX == from.posX && to.posY == from.posY)
        //    return true;
        if (to.posX == from.posX)
        {
            int min = Mathf.Min(to.posY, from.posY);
            int max = Mathf.Max(to.posY, from.posY);
            for (int i = min; i <= max; i++)
            {
                if (i == to.posY)
                    continue;
                if (cellDatas[to.posX, i].cellType != -1)
                    return false;
            }
        }
        else if (to.posY == from.posY)
        {
            int min = Mathf.Min(to.posX, from.posX);
            int max = Mathf.Max(to.posX, from.posX);
            for (int i = min; i <= max; i++)
            {
                if (i == to.posX)
                    continue;
                if (cellDatas[i, to.posY].cellType != -1)
                    return false;
            }
        }
        else
            return false;
        return true;
    }

    #region Booster

    public void Hint()
    {
        if (GameDataMgr.BoosterSearch <= 0)
        {
            GetBooster(BOOSTER_ID.SEARCH);
            return;
        }

        GameMaster.Instance.isPlay = true;
        if (cellItems.Count > 0)
        {
            for (int i = 0; i < cellItems.Count - 1; i++)
            {
                for (int j = i + 1; j < cellItems.Count; j++)
                {
                    List<CellData> path = CheckPath(cellItems[i], cellItems[j]);
                    if (path != null)
                    {
                        cellItems[i].Hint();
                        cellItems[j].Hint();
                        --GameDataMgr.BoosterSearch;
                        this.PostEvent(EventID.UpdateBossterSearch, GameDataMgr.BoosterSearch);
                        //   OnPair(path, cellItems[i], cellItems[j]);
                        return;
                    }
                }
            }
        }

    }

    public void GetBooster(BOOSTER_ID boosterID)
    {
        string itemName = boosterID.ToString();
        DialogMessage.Instance.OpenDialog($"Get {itemName}", "Watch a video ad to get more?", "Cancel", "Watch",
            () =>
            {
                AdsManager.instance.ShowAdsReward((success, amount) =>
                {
                    if (success)
                    {
                        int numAdd = 1;
                        DialogMessage.Instance.OpenDialog("Congratulations", $"+{numAdd} {itemName}",
                      () =>
                      {
                          switch (boosterID)
                          {
                              case BOOSTER_ID.NONE:
                                  break;
                              case BOOSTER_ID.SEARCH:
                                  GameDataMgr.BoosterSearch++;
                                  this.PostEvent(EventID.UpdateBossterSearch, GameDataMgr.BoosterSearch);
                                  break;
                              case BOOSTER_ID.SHUFFLE:
                                  GameDataMgr.BoosterShuffle++;
                                  this.PostEvent(EventID.UpdateBoosterShuffle, GameDataMgr.BoosterShuffle);
                                  break;
                              case BOOSTER_ID.ROCKET:
                                  GameDataMgr.BoosterRocket++;
                                  this.PostEvent(EventID.UpdateBoosterRocket, GameDataMgr.BoosterRocket);
                                  break;
                              //HoanDN Change Tiles Set Theme
                              case BOOSTER_ID.THEME:
                                  GameDataMgr.BoosterTheme++;
                                  this.PostEvent(EventID.UpdateBoosterTheme, GameDataMgr.BoosterTheme);
                                  break;
                              default:
                                  break;
                          }
                      });
                    }
                });
            }
        );
    }

    public void Swap()
    {
        //Debug.Log(GameDataMgr.BoosterShuffle);
        if (GameDataMgr.BoosterShuffle <= 0)
        {
            GetBooster(BOOSTER_ID.SHUFFLE);
            return;
        }
        GameMaster.Instance.isPlay = true;
        OnSwap();
    }

    //HoanDN Change Tiles Set Theme
    public void ChangeTheme()
    {
        //Debug.Log(GameDataMgr.BoosterTheme);
        if (GameDataMgr.BoosterTheme <= 0)
        {
            GetBooster(BOOSTER_ID.THEME);
            return;
        }
        GameMaster.Instance.isPlay = true;
        OnChangeTheme();
    }

    public void OnPairRocketTile(RocketObject rk01, CellItem cellToDes01, RocketObject rk02, CellItem cellToDes02, Action<bool> OnComplete)
    {
        cellDatas[cellToDes01.posX, cellToDes01.posY].cellType = -1;
        cellItems.Remove(cellToDes01);
        _dictCellType[cellToDes01.cellType]--;
        //ContentMgr.Instance.Despaw(cellToDes01.gameObject, 0.3f);
        cellToDes01.DeSpawnCell(0.9f, EFFECT_DESPAWN.FADE_SCALE);
        rk01.Launch(0.7f, null);

        cellDatas[cellToDes02.posX, cellToDes02.posY].cellType = -1;
        cellItems.Remove(cellToDes02);
        _dictCellType[cellToDes02.cellType]--;
        //ContentMgr.Instance.Despaw(cellToDes02.gameObject, 0.3f);
        cellToDes02.DeSpawnCell(0.9f, EFFECT_DESPAWN.FADE_SCALE);
        rk02.Launch(0.7f, null);
        StartCoroutine(MoveItem02(cellToDes01, cellToDes02, 1f, OnComplete));
        CheckWin();
    }

    public Tuple<CellItem, CellItem> FindTargetRocketTile()
    {
        Tuple<CellItem, CellItem> result = null;
        if (cellItems.Count > 0)
        {
            int targetType = -1;

            int maxCount = 0;
            foreach (var item in _dictCellType)
            {
                if (item.Value >= 2 && item.Key != 10)
                {
                    targetType = item.Key;
                }
                //else if (item.Value > maxCount)
                //{
                //    maxCount = item.Value;
                //    targetType = item.Key;
                //}
            }

            //check for another firework
            if (targetType == -1)
            {
                int countRocket = 0;
                _dictCellType.TryGetValue(10, out countRocket);
                if (countRocket >= 2)
                {
                    targetType = 10;
                }
            }

            if (targetType != -1)
            {
                if (cellItems.Count > 0)
                {
                    CellItem firstCell = null;
                    CellItem secondCell = null;
                    for (int i = cellItems.Count - 1; i >= 0; i--)
                    {
                        var cell = cellItems[i];
                        if (cell.cellType == targetType)
                        {
                            if (firstCell == null)
                            {
                                firstCell = cell;

                            }
                            else if (secondCell == null)
                            {
                                secondCell = cell;
                                break;
                            }
                        }
                    }
                    result = new Tuple<CellItem, CellItem>(firstCell, secondCell);
                }
            }
        }



        return result;
    }

    public void RandomRocket()
    {
        if (GameDataMgr.BoosterRocket <= 0)
        {
            GetBooster(BOOSTER_ID.ROCKET);
            return;
        }


        int maxPair = 2;
        if (cellItems.Count > 0)
        {
            int targetType = -1;

            int maxCount = 0;
            foreach (var item in _dictCellType)
            {
                if (item.Value >= 2 * maxPair)
                {
                    targetType = item.Key;
                }
                else if (item.Value > maxCount)
                {
                    maxCount = item.Value;
                    targetType = item.Key;
                }
            }

            if (targetType != -1)
            {
                ExplodeAllCellsWithType(targetType, maxPair * 2);
            }


            CheckWin();
        }

        GameDataMgr.BoosterRocket--;
        this.PostEvent(EventID.UpdateBoosterRocket, GameDataMgr.BoosterRocket);

    }

    public void ExplodeAllCellsWithType(int type, int num)
    {
        if (cellItems.Count > 0)
        {
            int countDestroy = 0;
            CellItem firstCell = null;
            CellItem secondCell = null;
            //add rocket object
            RocketObject rk01 = null; 
            for (int i = cellItems.Count - 1; i >= 0; i--)
            {
                var cell = cellItems[i];
                if (cell.cellType == type)
                {
                    cellDatas[cell.posX, cell.posY].cellType = -1;
                    cellItems.RemoveAt(i);
                    _dictCellType[cell.cellType]--;

                    cell.Hint(0.3f);
                    //ContentMgr.Instance.Despaw(cell.gameObject, 1.2f);
                    cell.DeSpawnCell(1.2f, EFFECT_DESPAWN.FADE_SCALE);

                    //launch rocket
                    rk01 = LeanPool.Spawn(_rocketPrefab, rkSpawn.transform.position, Quaternion.identity, gridParent);
                    rk01.transform.position = rkSpawn.transform.position;
                    rk01.Initialize(cell.transform);
                    rk01.Launch(0.7f, null);

                    countDestroy++;

                    if (firstCell == null)
                        firstCell = cell;
                    else if (secondCell == null)
                        secondCell = cell;
                }
                AudioManager.Instance.Shot("true");

                if (countDestroy >= num)
                    break;
            }

            StartCoroutine(MoveItem02(firstCell, secondCell, 1.5f, null));
        }

    }
    #endregion

    #region DEBUG ONLY
    public void CheatWinLevel()
    {
        foreach (var cell in cellItems)
        {
            ContentMgr.Instance.Despaw(cell.gameObject);
        }
        cellItems.Clear();
        CheckWin();
    }

    public void CheatLoseLevel()
    {

    }

    public void DrawMatrix()
    {
        if (EnableDebug)
        {
            if (cellDatas == null || cellDatas.Length <= 0)
                return;

            string maxtrix = "";
            for (int j = height - 1; j >= 0; j--)
            {
                for (int i = 0; i < width; i++)
                {
                    maxtrix += $"{cellDatas[i, j].cellType}\t";
                }
                maxtrix += '\n';
            }

            Logwin.Log("MAXTRIX", maxtrix, "DEBUG");
        }
    }

    public void AutoPair()
    {
        for (int i = 0; i < cellItems.Count - 1; i++)
        {
            for (int j = i + 1; j < cellItems.Count; j++)
            {
                List<CellData> path = CheckPath(cellItems[i], cellItems[j]);
                if (path != null)
                {
                    cellItems[i].Hint();
                    cellItems[j].Hint();
                    OnPair(path, cellItems[i], cellItems[j]);
                    return;
                }
            }
        }

    }

    private float _timerAutoPair = 0;

    public void UpdateGodMod(float _deltaTime)
    {
        _timerAutoPair += _deltaTime;
        if (_timerAutoPair >= 0.8f)
        {
            if (!IsBusy)
                AutoPair();
            _timerAutoPair = 0;
        }
    }


    #endregion

    #region My Move Type
    bool CheckMove(CellPos cellItem, ref CellPos newPos, Move move, bool isLoop = false, bool isFreeway = false)
    {
        bool result = false;
        newPos = cellItem;
        switch (move)
        {
            case Move.Up:
                if (cellItem.posY + 1 > height - 1)
                {
                    if (!isLoop)
                        result = false;
                    else
                    {
                        result = true;
                        newPos.posY = 1;
                    }
                }
                else
                {
                    result = isLoop || isFreeway || (cellItem.posY + 1 < height - 1 && (cellDatas[cellItem.posX, cellItem.posY + 1].cellType == -1));
                    newPos.posY += 1;
                    newPos.posY = Mathf.Clamp(newPos.posY, 1, height - 1);

                }

                break;
            case Move.Dow:
                if (cellItem.posY - 1 < 1)
                {
                    if (!isLoop)
                        result = false;
                    else
                    {
                        result = true;
                        newPos.posY = height - 1;
                    }
                }
                else
                {
                    result = isLoop || isFreeway || (cellItem.posY - 1 > 0 && (cellDatas[cellItem.posX, cellItem.posY - 1].cellType == -1));
                    newPos.posY -= 1;
                    newPos.posY = Mathf.Clamp(newPos.posY, 1, height - 1);
                }

                break;
            case Move.Right:
                if (cellItem.posX + 1 >= width - 1)
                {
                    if (!isLoop)
                        result = false;
                    else
                    {
                        result = true;
                        newPos.posX = 1;
                    }
                }
                else
                {
                    result = isLoop || isFreeway || (cellDatas[cellItem.posX + 1, cellItem.posY].cellType == -1);
                    newPos.posX += 1;

                    newPos.posX = Mathf.Clamp(newPos.posX, 1, width - 1);
                }

                break;
            case Move.Left:
                if (cellItem.posX - 1 < 1)
                {

                    if (!isLoop)
                        result = false;
                    else
                    {
                        result = true;
                        newPos.posX = width - 1;
                    }
                }
                else
                {
                    result = isLoop || isFreeway || (cellDatas[cellItem.posX - 1, cellItem.posY].cellType == -1);
                    newPos.posX -= 1;

                    newPos.posX = Mathf.Clamp(newPos.posX, 1, width - 1);
                }

                break;
            default:
                break;
        }

        if (isLoop)
        {
            newPos = FindTargetCellPosForLoop(cellItem, move);
        }

        return result;
    }

    private void MoveBoardsNonLoopMagnet()
    {
        int countFailMove = 0;
        List<Move> _listMove = new List<Move>();
        int countPreventOverflow = 0;

        while (true)
        {
            for (int i = width - 2; i >= 1; i--)
            {
                for (int j = height - 2; j >= 1; j--)
                {
                    var curPos = new CellPos() { posX = i, posY = j };
                    var nextPos = new CellPos();
                    var curData = cellDatas[curPos.posX, curPos.posY];
                    var move = _levelData.MatrixMove[i - 1, j - 1];
                    ConvertToStepMove(ref _listMove, move);
                    int subMoveFail = 0;
                    foreach (var m in _listMove)
                    {
                        if (CheckMove(curPos, ref nextPos, m, false))
                        {
                            cellDatas[nextPos.posX, nextPos.posY].cellType = curData.cellType;
                            curData.cellType = -1;
                            var cellItem = cellItems.FirstOrDefault(x => x.posX == i && x.posY == j);
                            if (cellItem != null)
                            {
                                cellItem.MoveItem(m);
                            }
                        }
                        else
                        {
                            subMoveFail++;
                        }
                    }
                    if (subMoveFail >= _listMove.Count)
                        countFailMove++;

                }
            }
            countPreventOverflow++;
            if (countPreventOverflow >= 2)
            {
                //Debug.LogError("MoveBoardsNonLoopMagnet overflow!!!");
            }
            if (countPreventOverflow >= 2 || countFailMove >= (width - 2) * (height - 2))
            {
                break;
            }

        }

        countFailMove = 0;
        countPreventOverflow = 0;
        while (true)
        {
            for (int i = 1; i < width - 1; i++)
            {
                for (int j = 1; j < height - 1; j++)
                {
                    var curPos = new CellPos() { posX = i, posY = j };
                    var nextPos = new CellPos();
                    var curData = cellDatas[curPos.posX, curPos.posY];
                    var move = _levelData.MatrixMove[i - 1, j - 1];
                    ConvertToStepMove(ref _listMove, move);

                    int subMoveFail = 0;
                    foreach (var m in _listMove)
                    {
                        if (CheckMove(curPos, ref nextPos, m, false))
                        {
                            cellDatas[nextPos.posX, nextPos.posY].cellType = curData.cellType;
                            curData.cellType = -1;
                            var cellItem = cellItems.FirstOrDefault(x => x.posX == i && x.posY == j);
                            if (cellItem != null)
                            {
                                cellItem.MoveItem(m);
                            }
                        }
                        else
                            subMoveFail++;
                    }

                    if (subMoveFail >= _listMove.Count)
                        countFailMove++;
                }
            }

            countPreventOverflow++;
            if (countPreventOverflow >= 100)
            {
                Debug.LogError("MoveBoardsNonLoopMagnet overflow!!!");
            }
            if (countPreventOverflow >= 100 || countFailMove >= (width - 2) * (height - 2))
            {
                break;
            }
        }

    }

    private void MoveBoardsOneCell()
    {
        List<Move> _listMove = new List<Move>();
        List<CellItem> _listCheckedCells = new List<CellItem>();
        for (int i = 1; i < width - 1; i++)
        {
            for (int j = 1; j < height - 1; j++)
            {
                var curPos = new CellPos() { posX = i, posY = j };
                var nextPos = new CellPos();
                var curData = cellDatas[curPos.posX, curPos.posY];
                var move = _levelData.MatrixMove[i - 1, j - 1];
                ConvertToStepMove(ref _listMove, move);
                int subMoveFail = 0;
                foreach (var m in _listMove)
                {
                    if (m == Move.Right || m == Move.Up)
                        continue;
                    if (CheckMove(curPos, ref nextPos, m, false, true))
                    {
                        var cellItem = cellItems.FirstOrDefault(x => x.posX == curPos.posX && x.posY == curPos.posY);
                        if (cellItem != null && !_listCheckedCells.Contains(cellItem))
                        {
                            cellItem.MoveItem(m);
                            _listCheckedCells.Add(cellItem);
                        }
                        else
                        {
                            //Debug.LogError("find error!!!");
                        }
                    }
                    else
                    {
                        Debug.LogError("Move fail!!!");
                        subMoveFail++;
                    }

                }
            }
        }

        _listMove.Clear();
        //_listCheckedCells.Clear();
        for (int i = width - 2; i >= 1; i--)
        {
            for (int j = height - 2; j >= 1; j--)
            {
                var curPos = new CellPos() { posX = i, posY = j };
                var nextPos = new CellPos();
                var curData = cellDatas[curPos.posX, curPos.posY];
                var move = _levelData.MatrixMove[i - 1, j - 1];
                ConvertToStepMove(ref _listMove, move);
                int subMoveFail = 0;
                foreach (var m in _listMove)
                {
                    if (m == Move.Right || m == Move.Up)
                    {
                        if (CheckMove(curPos, ref nextPos, m, false, true))
                        {
                            var cellItem = cellItems.FirstOrDefault(x => x.posX == curPos.posX && x.posY == curPos.posY);
                            if (cellItem != null && !_listCheckedCells.Contains(cellItem))
                            {
                                cellItem.MoveItem(m);
                                _listCheckedCells.Add(cellItem);
                            }
                            else
                            {
                                // Debug.LogError("find error!!!");
                            }
                        }
                        else
                        {
                            Debug.LogError("Move fail!!!");
                            subMoveFail++;
                        }
                    }


                }
            }
        }
    }

    private void MoveBoardsLoop()
    {
        StartCoroutine(MoveboardsLoopCoroutine());
    }

    IEnumerator MoveboardsLoopCoroutine()
    {
        List<Move> _listMove = new List<Move>();
        List<CellItem> _listMoved = new List<CellItem>();
        //left, up
        for (int i = 1; i < width - 1; i++)
        {
            for (int j = 1; j < height - 1; j++)
            {
                var curPos = new CellPos() { posX = i, posY = j };
                var nextPos = new CellPos() { posX = i, posY = j };
                var move = _levelData.MatrixMove[i - 1, j - 1];
                ConvertToStepMove(ref _listMove, move);
                foreach (var m in _listMove)
                {
                    if (m == Move.Up || m == Move.Dow)
                    {
                        if (CheckMove(curPos, ref nextPos, m, true))
                        {
                            Debug.Log($"x: {i} -y: {j}");
                            var cellItem = cellItems.FirstOrDefault(x => x.posX == i && x.posY == j);
                            if (cellItem != null)
                            {
                                if (nextPos.posX == cellItem.posX && nextPos.posY == cellItem.posY)
                                    Debug.Log("Move the same!!!");

                                if (_listMoved.Contains(cellItem))
                                    continue;
                                cellItem.MoveToPos(nextPos);
                                _listMoved.Add(cellItem);
                            }
                        }
                    }

                }

            }
        }

        yield return new WaitForSeconds(0.02f);
        _listMoved.Clear();
        // right, down
        for (int i = width - 2; i >= 1; i--)
        {
            for (int j = height - 2; j >= 1; j--)
            {
                var curPos = new CellPos() { posX = i, posY = j };
                var nextPos = new CellPos() { posX = i, posY = j };
                var move = _levelData.MatrixMove[i - 1, j - 1];
                ConvertToStepMove(ref _listMove, move);
                foreach (var m in _listMove)
                {
                    if (m == Move.Right || m == Move.Left)
                    {
                        if (CheckMove(curPos, ref nextPos, m, true))
                        {
                            //cellDatas[nextPos.posX, nextPos.posY].cellType = curData.cellType;
                            var cellItem = cellItems.FirstOrDefault(x => x.posX == i && x.posY == j);
                            if (cellItem != null)
                            {
                                if (nextPos.posX == cellItem.posX && nextPos.posY == cellItem.posY)
                                    Debug.Log("Move the same!!!");
                                if (_listMoved.Contains(cellItem))
                                    continue;
                                cellItem.MoveToPos(nextPos);
                                _listMoved.Add(cellItem);
                            }
                        }

                    }
                }

            }
        }
    }

    public void ConvertToStepMove(ref List<Move> listMove, MoveMode mode)
    {
        listMove.Clear();
        switch (mode)
        {
            case MoveMode.Empty:
                break;
            case MoveMode.Idle:
                break;
            case MoveMode.Left:
                listMove.Add(Move.Left);
                break;
            case MoveMode.Right:
                listMove.Add(Move.Right);
                break;
            case MoveMode.Up:
                listMove.Add(Move.Up);
                break;
            case MoveMode.Dow:
                listMove.Add(Move.Dow);
                break;
            case MoveMode.Up_Left:
                listMove.Add(Move.Up);
                listMove.Add(Move.Left);
                break;
            case MoveMode.Up_Right:

                listMove.Add(Move.Up);
                listMove.Add(Move.Right);
                break;
            case MoveMode.Down_Left:
                listMove.Add(Move.Dow);
                listMove.Add(Move.Left);
                break;
            case MoveMode.Down_Right:
                listMove.Add(Move.Dow);
                listMove.Add(Move.Right);
                break;
            default:
                break;
        }
    }

    public CellPos FindTargetCellPosForLoop(CellPos curPos, Move move)
    {
        var result = new CellPos() { posX = curPos.posX, posY = curPos.posY };
        switch (move)
        {
            case Move.Up:
                if (curPos.posY + 1 >= height - 1)
                {
                    for (int h = 0; h < _levelData.Height; h++)
                    {
                        if (_levelData.MatrixMove[curPos.posX - 1, h] == _levelData.MatrixMove[curPos.posX - 1, curPos.posY - 1])
                        {
                            result.posY = h + 1;
                            break;
                        }
                    }
                }
                else
                {
                    result.posY += 1;
                }

                break;
            case Move.Dow:
                if (curPos.posY - 1 < 1)
                {
                    for (int h = _levelData.Height - 1; h >= 0; h--)
                    {
                        if (_levelData.MatrixMove[curPos.posX - 1, h] == _levelData.MatrixMove[curPos.posX - 1, curPos.posY - 1])
                        {
                            result.posY = h + 1;
                            break;
                        }
                    }
                }
                else
                {
                    result.posY -= 1;
                }


                break;
            case Move.Right:
                if (curPos.posX + 1 >= width - 1)
                {
                    for (int w = 0; w < _levelData.Width; w++)
                    {
                        if (_levelData.MatrixMove[w, curPos.posY - 1] == _levelData.MatrixMove[curPos.posX - 1, curPos.posY - 1])
                        {
                            result.posX = w + 1;
                            break;
                        }
                    }
                }
                else
                {
                    result.posX += 1;
                }
                break;
            case Move.Left:
                if (curPos.posX - 1 < 1)
                {
                    for (int w = _levelData.Width - 1; w >= 0; w--)
                    {
                        if (_levelData.MatrixMove[w, curPos.posY - 1] == _levelData.MatrixMove[curPos.posX - 1, curPos.posY - 1])
                        {
                            result.posX = w + 1;
                            break;
                        }
                    }
                }
                else
                {
                    result.posX -= 1;
                }
                break;
            default:
                break;
        }

        return result;
    }
    #endregion
}
