using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 共通タイトル
/// </summary>
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
        CloseBtn.onClick.AddListener(()=> 
        {
            if (action != null)
                action();
        });

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

    /// <summary>
    /// タイトル名をセット
    /// </summary>
    /// <param name="title">タイトル</param>
    public void SetTitle(string title)
    {
        Title.text = title;
    }

    /// <summary>
    /// コインを更新
    /// </summary>
    public void RefreshCoins()
    {
        Coin1.text = DataMng.E.RuntimeData.Coin1.ToString();
        Coin2.text = DataMng.E.RuntimeData.Coin2.ToString();
        Coin3.text = DataMng.E.RuntimeData.Coin3.ToString();
    }

    /// <summary>
    /// Closeボタンをクリックする場合のイベント
    /// </summary>
    /// <param name="action">イベント</param>
    public void SetOnClose(Action action)
    {
        this.action = action;
    }

    /// <summary>
    /// コインを隠れる
    /// </summary>
    /// <param name="index"></param>
    public void EnActiveCoin(int index)
    {
        if (index == 1) Coin1.gameObject.SetActive(false);
        if (index == 2) Coin2.gameObject.SetActive(false);
        if (index == 3) Coin3.gameObject.SetActive(false);
    }

    /// <summary>
    /// Closeボタンのenabled設定
    /// </summary>
    /// <param name="b"></param>
    public void CloseBtnEnable(bool b)
    {
        CloseBtn.enabled = b;
    }
}
