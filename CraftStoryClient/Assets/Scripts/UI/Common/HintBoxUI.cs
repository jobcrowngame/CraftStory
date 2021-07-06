using System;
using UnityEngine;
using UnityEngine.UI;

public class HintBoxUI : UIBase
{
    Image Icon;
    Text Msg1;
    Text Msg2;
    Button OkBtn;
    Button CancelBtn;

    Action okAction;
    Action cancelAction;

    private void Awake()
    {
        Icon = FindChiled<Image>("Icon");
        Msg1 = FindChiled<Text>("Msg1");
        Msg2 = FindChiled<Text>("Msg2");
        OkBtn = FindChiled<Button>("OKBtn");
        OkBtn.onClick.AddListener(OnClickOkBtn);
        CancelBtn = FindChiled<Button>("CancelBtn");
        CancelBtn.onClick.AddListener(OnClickCancelBtn);
    }

    public void Init(string iconPath, string msg, Action okAction, Action cancelAction)
    {
        this.okAction = okAction;
        this.cancelAction = cancelAction;

        if (!string.IsNullOrEmpty(iconPath))
        {
            Icon.gameObject.SetActive(true);
            Msg1.gameObject.SetActive(true);
            Msg2.gameObject.SetActive(false);

            Icon.sprite = ReadResources<Sprite>(iconPath);
            Msg1.text = msg;
        }
        else
        {
            Icon.gameObject.SetActive(false);
            Msg1.gameObject.SetActive(false);
            Msg2.gameObject.SetActive(true);

            Msg2.text = msg;
        }

        CancelBtn.gameObject.SetActive(cancelAction != null);
    }

    private void OnClickOkBtn()
    {
        okAction();
        Destroy();
    }
    private void OnClickCancelBtn()
    {
        if (cancelAction != null)
            cancelAction();

        Destroy();
    }
}
