using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketObject : MonoBehaviour
{
    public CanvasGroup _canvasGroup;
    private Transform _target;
    private bool _isLaunched = false;
    public void Initialize(Transform _target)
    {
        this._target = _target;
        Vector3 difference = _target.position - transform.position;
        float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);
        transform.localScale = Vector3.one * 0.3f;
        _canvasGroup.alpha = 0;
        _canvasGroup.DOFade(1f, 0.5f).SetEase(Ease.Linear);
        transform.DOScale(1f, 0.5f).SetEase(Ease.Linear);
        _isLaunched = false;
    }

    public void Launch(float timeLaunch, Action callback)
    {
        transform.DOMove(_target.transform.position, timeLaunch - 0.1f).SetEase(Ease.InOutQuint).OnComplete(() =>
        {
            _canvasGroup.DOFade(0f, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
            {
                Lean.LeanPool.Despawn(this);
            });
            callback?.Invoke();
        }).SetDelay(0.1f);


    }



}
