using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class ShopUI
{
    Button RatioBtn { get => FindChiled<Button>("RatioBtn", Gacha); }
    Button StartGachaBtn { get => FindChiled<Button>("StartGachaBtn", Gacha); }
    Text GachaDes { get => FindChiled<Text>("GachaDes", Gacha); }
    Text Cost { get => FindChiled<Text>("Cost", Gacha); }
    Image GachaIcon { get => FindChiled<Image>("GachaIcon"); }
    Button GachaRightBtn { get => FindChiled<Button>("GachaRightBtn", Gacha); }
    Button GachaLeftBtn { get => FindChiled<Button>("GachaLeftBtn", Gacha); }

    public void InitGacha()
    {
        RatioBtn.onClick.AddListener(() =>
        {
            var ui = UICtl.E.OpenUI<GachaRatioUI>(UIType.GachaRatio);
            ui.Set(ShopLG.E.SelectGachaId);
        });
        StartGachaBtn.onClick.AddListener(() =>
        {
            var ui = UICtl.E.OpenUI<GachaVerificationUI>(UIType.GachaVerification);
            ui.Set(ShopLG.E.SelectGachaId);
        });
        GachaRightBtn.onClick.AddListener(OnClickGachaRightBtn);
        GachaLeftBtn.onClick.AddListener(OnClickGachaLeftBtn);
    }

    /// <summary>
    /// ガチャを指定する
    /// </summary>
    /// <param name="index">インデックス</param>
    public void SetGachaIndex(int index)
    {
        ShopLG.E.SelectedGachaIndex = index;
        RefreshGachaUI();
    }
    /// <summary>
    /// 次のガチャ
    /// </summary>
    public void OnClickGachaRightBtn()
    {
        ShopLG.E.SelectedGachaIndex++;
        RefreshGachaUI();
    }
    /// <summary>
    /// 前のガチャ
    /// </summary>
    public void OnClickGachaLeftBtn()
    {
        ShopLG.E.SelectedGachaIndex--;
        RefreshGachaUI();
    }
    /// <summary>
    /// 次のガチャボタンアクティブ
    /// </summary>
    public void ShowGachaRightBtn(bool b = true)
    {
        GachaRightBtn.gameObject.SetActive(b);
    }
    /// <summary>
    /// 前のガチャボタンアクティブ
    /// </summary>
    /// <param name="b"></param>
    public void ShowGachaLeftBtn(bool b = true)
    {
        GachaLeftBtn.gameObject.SetActive(b);
    }
    /// <summary>
    /// ガチャWindowを更新
    /// </summary>
    public void RefreshGachaUI()
    {
        var config = ConfigMng.E.Gacha[ShopLG.E.SelectGachaId];

        StartGachaBtn.image.sprite = ReadResources<Sprite>(config.BtnImage);
        GachaIcon.sprite = ReadResources<Sprite>(config.Image);
        GachaDes.text = config.Title == "N" ? "" : config.Title;
        Cost.text = config.CostCount.ToString();
    }
}
