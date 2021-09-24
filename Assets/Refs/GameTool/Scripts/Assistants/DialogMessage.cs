using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DialogMessage : SingletonMonoBehaviour<DialogMessage>
{

    [SerializeField]
    Text tileText, messageText;

    [SerializeField]
    GameObject dialogPanel, yesObj, noObj;


    Action okAction, cancelAction;

    public void OpenDialog(string title, string message, Action okAction)
    {
        dialogPanel.SetActive(true);
        tileText.text = title;
        messageText.text = message;
        this.okAction = okAction;
        yesObj.SetActive(true);
        noObj.SetActive(false);

        dialogPanel.transform.SetAsLastSibling();

    }

    public void OpenDialog(string title, string message, string no, string yes, Action okAction, Action cancelAction = null)
    {
        dialogPanel.SetActive(true);
        tileText.text = title;
        messageText.text = message;

        this.okAction = okAction;
        this.cancelAction = cancelAction;
        yesObj.SetActive(true);
        noObj.SetActive(true);
    }

    public void OnPressOkButton()
    {
        dialogPanel.SetActive(false);
        if (okAction != null)
            okAction.Invoke();

    }

    public void OnPressCancelButton()
    {
        dialogPanel.SetActive(false);
        if (cancelAction != null)
            cancelAction.Invoke();

    }
    public void OnPressCloseButton()
    {
        dialogPanel.SetActive(false);

    }
}
