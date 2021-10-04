using UnityEngine;
using UnityEngine.UI;

public class ShopGachaUI : UIBase
{
    Title2UI Title { get => FindChiled<Title2UI>("Title2"); }
    MyToggleGroupCtl ToggleBtns1 { get => FindChiled<MyToggleGroupCtl>("ToggleBtns1"); }
    MyToggleGroupCtl ToggleBtns2 { get => FindChiled<MyToggleGroupCtl>("ToggleBtns2"); }
    Text Des { get => FindChiled<Text>("Des"); }
    Button BackBtn { get => FindChiled<Button>("BackBtn"); }

    Button RatioBtn { get => FindChiled<Button>("RatioBtn"); }
    Button StartGachaBtn { get => FindChiled<Button>("StartGachaBtn"); }
    Text Cost { get => FindChiled<Text>("Cost"); }
    Image GachaIcon { get => FindChiled<Image>("GachaIcon"); }

    public override void Init()
    {
        base.Init();
        ShopGachaLG.E.Init(this);

        Title.Init();
        Title.ShowCoin(1);
        Title.ShowCoin(2);

        ToggleBtns1.Init();
        ToggleBtns1.OnValueChangeAddListener((index) =>
        {
            ShopGachaLG.E.Index = index;
        });

        ToggleBtns2.Init();
        ToggleBtns2.OnValueChangeAddListener((index) =>
        {
            ShopGachaLG.E.GachaType = index;
        });

        Des.text = ConfigMng.E.MText[1].Text;
        BackBtn.onClick.AddListener(Close);

        RatioBtn.onClick.AddListener(() =>
        {
            var ui = UICtl.E.OpenUI<GachaRatioUI>(UIType.GachaRatio);
            ui.Set(ShopGachaLG.E.SelectGachaId);
        });
        StartGachaBtn.onClick.AddListener(() =>
        {
            var ui = UICtl.E.OpenUI<GachaVerificationUI>(UIType.GachaVerification);
            ui.Set(ShopGachaLG.E.SelectGachaId);
        });
    }

    public override void Open()
    {
        base.Open();

        Title.RefreshCoins();
        ToggleBtns1.SetValue(0);
        ShopGachaLG.E.Index = 0;
    }

    /// <summary>
    /// ガチャWindowを更新
    /// </summary>
    public void RefreshGachaUI()
    {
        var config = ConfigMng.E.Gacha[ShopGachaLG.E.SelectGachaId];

        StartGachaBtn.image.sprite = ReadResources<Sprite>(config.BtnImage);
        GachaIcon.sprite = ReadResources<Sprite>(config.Image);
        Cost.text = config.CostCount.ToString();

        // ボタン画像差し替え
        for (int i = 0; i < 3; i++)
        {
            int gachaId = ShopGachaLG.E.GachaIds[ShopGachaLG.E.GachaType, i];
            ToggleBtns1.SetToggleImage(i, ConfigMng.E.Gacha[gachaId].ToggleImageON, ConfigMng.E.Gacha[gachaId].ToggleImageOFF);
        }
    }

    public void RefreshCoins()
    {
        Title.RefreshCoins();
        ShopMarketLG.E.UI.RefreshCoins();
    }
}
