using System;
using UnityEngine;
using UnityEngine.UI;

public class TitleUI : UIBase
{
    Button CloseBtn;
    Text Title;

    Text Coin1;
    Image Coin1Image;
    Text Coin2;
    Image Coin2Image;
    Text Coin3;
    Image Coin3Image;

    Action action;

    private void Awake()
    {
        CloseBtn = FindChiled<Button>("CloseBtn");
        CloseBtn.onClick.AddListener(OnClose);

        Title = FindChiled<Text>("TitleText");
        Coin1 = FindChiled<Text>("Coin1");
        Coin1.text = "0";
        Coin1Image = FindChiled<Image>("Image", Coin1.transform);
        Coin1Image.sprite = ReadResources<Sprite>(ConfigMng.E.Item[9000].IconResourcesPath);
        Coin2 = FindChiled<Text>("Coin2");
        Coin2.text = "0";
        Coin2Image = FindChiled<Image>("Image", Coin2.transform);
        Coin2Image.sprite = ReadResources<Sprite>(ConfigMng.E.Item[9001].IconResourcesPath);
        Coin3 = FindChiled<Text>("Coin3");
        Coin3.text = "0";
        Coin3Image = FindChiled<Image>("Image", Coin3.transform);
        Coin3Image.sprite = ReadResources<Sprite>(ConfigMng.E.Item[9002].IconResourcesPath);
    }

    public void SetTitle(string title)
    {
        Title.text = title;
    }
    public void RefreshCoins()
    {
        Coin1.text = DataMng.E.RuntimeData.Coin1.ToString();
        Coin2.text = DataMng.E.RuntimeData.Coin2.ToString();
        Coin3.text = DataMng.E.RuntimeData.Coin3.ToString();
    }
    public void SetOnClose(Action action)
    {
        this.action = action;
    }
    public void EnActiveCoin(int index)
    {
        if (index == 1) Coin1.gameObject.SetActive(false);
        if (index == 2) Coin2.gameObject.SetActive(false);
        if (index == 3) Coin3.gameObject.SetActive(false);
    }
    public void CloseBtnEnable(bool b)
    {
        CloseBtn.enabled = b;
    }

    private void OnClose()
    {
        if (action != null)
            action();
    }
}
