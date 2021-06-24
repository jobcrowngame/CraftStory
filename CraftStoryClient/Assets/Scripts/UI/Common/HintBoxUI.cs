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

    public void Init(string iconPath, string msg, Action okAction, Action cancelAction = null)
    {
        this.okAction = okAction;
        this.cancelAction = cancelAction;

        Msg1.text = msg;
        Msg1.gameObject.SetActive(true);

        Icon.sprite = ReadResources<Sprite>(iconPath);
        Icon.gameObject.SetActive(true);
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
