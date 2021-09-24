using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ToggleButton : MonoBehaviour
{
    private Button _button;

    public Image _disableImg;

    public Image _enableImg;

    public Action<bool> _OnToggleChanged;

    private void Awake()
    {
        if (_button == null)
            _button = this.GetComponent<Button>();

        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(ClickButton);
    }

    private bool _isEnable;

    public void Initialize(bool isEnable)
    {
        _isEnable = isEnable;
        _disableImg.gameObject.SetActive(!_isEnable);
        _enableImg.gameObject.SetActive(_isEnable);
        _OnToggleChanged?.Invoke(_isEnable);
    }

    public void ClickButton()
    {
        _isEnable = !_isEnable;
        _disableImg.gameObject.SetActive(!_isEnable);
        _enableImg.gameObject.SetActive(_isEnable);
        _OnToggleChanged?.Invoke(_isEnable);
    }

}
