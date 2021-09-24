using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    Toggle toggle;
    void Awake()
    {
        Button button = GetComponent<Button>();
        toggle = GetComponent<Toggle>();
        if (button != null)
            button.onClick.AddListener(() => { AudioManager.Instance.Shot("button"); });
    }

    void OnEnable()
    {
        if (toggle != null)
            toggle.isOn = AudioManager.Instance.IsMute();
    }
}
