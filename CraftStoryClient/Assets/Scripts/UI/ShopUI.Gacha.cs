using UnityEngine;
using UnityEngine.UI;

public partial class ShopUI
{
    MyToggleGroupCtl ToggleBtns { get => FindChiled<MyToggleGroupCtl>("ToggleBtns", Gacha); }
    Button RatioBtn { get => FindChiled<Button>("RatioBtn", Gacha); }
    Button StartGachaBtn { get => FindChiled<Button>("StartGachaBtn", Gacha); }
    Text GachaDes { get => FindChiled<Text>("GachaDes", Gacha); }
    Text Cost { get => FindChiled<Text>("Cost", Gacha); }
    Image GachaIcon { get => FindChiled<Image>("GachaIcon"); }

    public void InitGacha()
    {
        ToggleBtns.Init();
        ToggleBtns.OnValueChangeAddListener((index) => 
        {
            SetGachaIndex(index);
        });
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

        for (int i = 0; i < ShopLG.E.GetGachaArr().Length; i++)
        {
            SetToggleImage(ShopLG.E.GetGachaArr()[i], i);
        }
        SetGachaIndex(0);
    }

    /// <summary>
    /// ガチャを指定する
    /// </summary>
    /// <param name="index">インデックス</param>
    public void SetGachaIndex(int index)
    {
        ShopLG.E.SelectedGachaIndex = index;
        ToggleBtns.SetValue(index);
        RefreshGachaUI();
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

    /// <summary>
    /// ボタン画像差し替え
    /// </summary>
    /// <param name="gachaId">ガチャID</param>
    /// <param name="index">インデックス</param>
    private void SetToggleImage(int gachaId, int index)
    {
        var config = ConfigMng.E.Gacha[gachaId];
        ToggleBtns.SetToggleImage(index, config.ToggleImageON, config.ToggleImageOFF);
    }
}
