using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ヒントボックス
/// </summary>
public class HintBoxUI : UIBase
{
    Image Icon { get => FindChiled<Image>("Icon"); }
    Transform Msg1Parent { get => FindChiled("Msg1"); }
    Text Msg1 { get => FindChiled<Text>("Text"); }
    Text Msg2 { get => FindChiled<Text>("Msg2"); }
    Button OkBtn { get => FindChiled<Button>("OKBtn"); }
    Button CancelBtn { get => FindChiled<Button>("CancelBtn"); }

    Action okAction; // Okボタンイベント
    Action cancelAction; // キャンセルボタンイベント

    private void Awake()
    {
        OkBtn.onClick.AddListener(OnClickOkBtn);
        CancelBtn.onClick.AddListener(OnClickCancelBtn);
    }

    public void Init(string iconPath, string msg, Action okAction, Action cancelAction)
    {
        this.okAction = okAction;
        this.cancelAction = cancelAction;

        if (!string.IsNullOrEmpty(iconPath))
        {
            Icon.gameObject.SetActive(true);
            Msg1Parent.gameObject.SetActive(true);
            Msg2.gameObject.SetActive(false);

            Icon.sprite = ReadResources<Sprite>(iconPath);
            Msg1.text = msg;
        }
        else
        {
            Icon.gameObject.SetActive(false);
            Msg1Parent.gameObject.SetActive(false);
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

    public void SetBtnName(string btn1, string btn2)
    {
        OkBtn.GetComponent<Image>().sprite = ReadResources<Sprite>("Textures/" + btn1);
        CancelBtn.GetComponent<Image>().sprite = ReadResources<Sprite>("Textures/" + btn2);
    }
}
