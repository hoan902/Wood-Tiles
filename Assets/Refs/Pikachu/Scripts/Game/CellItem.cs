using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using MEC;
using Lean;

public class CellItem : MonoBehaviour
{
    public int posX, posY;  // position in map
    public int cellType;  // type  // null = -1

    private CanvasGroup _canvasGroup;

    public GameObject _selectCursor;

    GameController gameController;
    bool isSelected = false;
    [SerializeField]
    Color selectColor;

    public bool isTargetOfRocket = false;

    // [SerializeField]
    // Sprite normalSprite, selectedSprite;
    [SerializeField]
    Image backgroundImage, itemImage;

    public const float IMG_WIDTH = 150;
    public const float IMG_HEIGHT = 150;

    public void Init(GameController _gameController, int _posx, int _posy, int _cellType, Sprite sprite, bool isNewSpriteWithoutBorder=false)
    {
        //backgroundImage.SetNativeSize();
        backgroundImage.DOKill();
        this.posX = _posx;
        this.posY = _posy;
        this.cellType = _cellType;
        this.gameController = _gameController;
        itemImage.sprite = sprite;
        isSelected = false;
        backgroundImage.color = Color.white;
        Vector2 size = GetComponent<RectTransform>().sizeDelta;
        isTargetOfRocket = false;
        float ratio = size.x / IMG_WIDTH;
        var _targetSize = new Vector2(size.x, IMG_HEIGHT * ratio);
        _targetSize *= 0.9f;

        //if (!isNewSpriteWithoutBorder)
        //{
        //    _targetSize *= 0.9f;
        //}
        //else
        //{
        //    if (sprite.texture.width != sprite.texture.height)
        //    {
        //        float ratioOfSprite = (float)sprite.texture.width / (float)sprite.texture.height;
        //        Debug.LogError("a: " + ratioOfSprite);
        //        _targetSize.y *= 0.8f;
        //        _targetSize.x = _targetSize.y * ratioOfSprite;
        //    }
        //    else
        //    {
        //        _targetSize *= 0.8f;
        //    }
        //}

        itemImage.GetComponent<RectTransform>().sizeDelta = _targetSize;
        //itemImage.transform.localPosition = new Vector2(-5f, 0);
        //HoanDN fix tiles image position to match with the square plain tiles
        itemImage.transform.localPosition = new Vector2(0f, 5f);
        this._canvasGroup.alpha = 1;
        this.transform.localScale = Vector3.one;
        SetPos();
        _selectCursor.gameObject.SetActive(false);
    }

    void SetPos()
    {
        //        Debug.Log(posX +" "+gameController.width);
        Vector2 pos = gameController.GetPos(posX, posY);
        transform.localPosition = pos;
    }

    private void Awake()
    {
        _canvasGroup = this.GetComponent<CanvasGroup>();

    }

    public void OnPressItem()
    {
        //if (!GameMaster.Instance.isPlay || (gameController != null && gameController.IsBusy))
        //    return;

        if (!GameMaster.Instance.isPlay || (gameController == null) || isTargetOfRocket)
        {
            return;
        }
        backgroundImage.DOKill();

        backgroundImage.color = Color.white;
        isSelected = !isSelected;
        if (isSelected)
        {
            OnSelected();
        }
        else
        {
            OnDeselected();
        }
        gameController.OnPressItem(this);
    }

    public void Hint(float TimeFX = 0.5f)
    {
        if (!GameMaster.Instance.isPlay)
            return;

        backgroundImage.DOColor(new Color(1f, 175f / 255f, 175 / 255f), TimeFX).SetLoops(-1, LoopType.Restart);

    }

    private void OnColorUpdated(Color color)
    {
        backgroundImage.color = color;
    }

    public void UpdateSprite(Sprite sprite)
    {
        backgroundImage.color = Color.white;
        //    iTween.Stop(backgroundImage.gameObject);
        itemImage.sprite = sprite;
    }


    /// <summary>
    /// Call when select item
    /// </summary>
    public void OnSelected()
    {
        backgroundImage.color = new Color(198f / 255f, 206f / 255f, 49f / 255f);
        _selectCursor.gameObject.SetActive(true);
    }

    /// <summary>
    /// Call on deslected
    /// </summary>
    public void OnDeselected()
    {
        backgroundImage.color = Color.white;
        isSelected = false;
        _selectCursor.gameObject.SetActive(false);
    }

    public void MoveItem(Move move)
    {
        switch (move)
        {
            case Move.Up:
                ++posY;
                break;
            case Move.Dow:
                --posY;
                break;
            case Move.Right:
                ++posX;
                break;
            case Move.Left:
                --posX;
                break;
            default:
                break;
        }

        MoveAnimation();
    }
    void MoveAnimation()
    {
        Vector2 pos = gameController.GetPos(posX, posY);
        transform.DOLocalMove(pos, 0.3f);
    }

    public void MoveToPos(CellPos newPos)
    {
        this.posX = newPos.posX;
        this.posY = newPos.posY;
        MoveAnimation();
    }

    public void DeSpawnCell(float delay = 0, EFFECT_DESPAWN effect = EFFECT_DESPAWN.NONE)
    {
        if (effect == EFFECT_DESPAWN.NONE)
        {
            Lean.LeanPool.Despawn(this.gameObject, delay);
        }
        else
        {
            switch (effect)
            {
                case EFFECT_DESPAWN.FADE_SCALE:
                    float fxTime = 0.3f;
                    Timing.CallDelayed(delay - fxTime, () =>
                     {
                         GameMaster.Instance.PlayEffect(FX_ENUM.STAR_EXPLODE, transform.position, this.transform.parent);
                         this.transform.DOScale(0.1f, fxTime).SetEase(Ease.Linear);
                         this._canvasGroup.DOFade(0, fxTime * 0.6f).SetEase(Ease.Linear).SetDelay(fxTime * 0.4f).OnComplete(() =>
                         {
                             LeanPool.Despawn(this.gameObject);
                         });
                         AudioManager.Instance.Shot("special");
                     });
                    break;
                default:
                    break;
            }
        }
    }
}
