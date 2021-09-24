using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleAnimation : ButtonAnimationController
{
    [SerializeField]
    Sprite activeSprite, deactiveSprite;

    [SerializeField]
    Image targetImage;

    void OnEnable()
    {
        UpdateSprite();
    }

    public void OnClick()
    {
        AudioManager.Instance.Mute(AudioManager.Instance.IsMute());
        UpdateSprite();
    }

    void UpdateSprite()
    {
        targetImage.overrideSprite = AudioManager.Instance.IsMute() ? deactiveSprite : activeSprite;
    }

}
