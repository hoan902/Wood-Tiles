using DG.Tweening;
using MEC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarView : MonoBehaviour
{
    public Image _star;

    private float _initScale;
    private void Awake()
    {
        _initScale = _star.transform.localScale.x;
    }

    public void SetEnable(bool enable, bool withAnim = true, float delay = 0f)
    {
        Timing.CallDelayed(delay, () =>
        {
            _star.gameObject.SetActive(enable);
            _star.transform.localScale = Vector3.one * _initScale;
            if (withAnim)
            {
                _star.transform.localScale = Vector3.one * _initScale * 3f;
                _star.transform.DOScale(Vector3.one * _initScale, 0.3f).SetEase(Ease.OutQuad);
            }
            else
            {
                _star.transform.localScale = Vector3.one * _initScale;
            }
        });

    }
}
