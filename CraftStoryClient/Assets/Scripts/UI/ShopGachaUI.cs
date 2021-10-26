using JsonConfigData;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopGachaUI : UIBase
{
    Title2UI Title { get => FindChiled<Title2UI>("Title2"); }
    public MyToggleGroupCtl ToggleBtns1 { get => FindChiled<MyToggleGroupCtl>("ToggleBtns1"); } // 上部タブボタン
    public MyToggleGroupCtl ToggleBtns2 { get => FindChiled<MyToggleGroupCtl>("ToggleBtns2"); } // 左タブボタン
    Text Des { get => FindChiled<Text>("Des"); } // 左部説明
    Button BackBtn { get => FindChiled<Button>("BackBtn"); } // 戻るボタン
    Image GachaIcon { get => FindChiled<Image>("GachaIcon"); } // 背景画像

    Button RatioBtn { get => FindChiled<Button>("RatioBtn"); } // 提供割合
    Button StartGachaBtn1 { get => FindChiled<Button>("StartGachaBtn1"); } // ガチャボタン(一番右)
    Button StartGachaBtn2 { get => FindChiled<Button>("StartGachaBtn2"); } // ガチャボタン(その左隣)
    Text Cost1 { get => FindChiled<Text>("Cost1"); } // ガチャボタン内のコスト(一番右)
    Text Cost2 { get => FindChiled<Text>("Cost2"); } // ガチャボタン内のコスト(その左隣)
    Image CostImage1 { get => FindChiled<Image>("CostImage1"); } // ガチャボタン内のコストのアイコン(一番右)
    Image CostImage2 { get => FindChiled<Image>("CostImage2"); } // ガチャボタン内のコストのアイコン(その左隣)
    Transform Step1 { get => FindChiled<Transform>("Step1"); } // ガチャボタンのステップ(一番右)
    Text StepText1 { get => FindChiled<Text>("StepText1"); } // ガチャボタンのステップのテキスト(一番右)

    readonly string StepTextTmpl = @"ステップ{0}";

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

        ToggleBtns2.SetValue(0);
        ShopGachaLG.E.GachaType = 0;
        ToggleBtns1.SetValue(0);
        ShopGachaLG.E.Index = 0;
    }

    public override void Open()
    {
        base.Open();

        Title.RefreshCoins();
    }

    /// <summary>
    /// ガチャWindowを更新
    /// </summary>
    public void RefreshGachaUI()
    {
        // 上部タブ画像
        var gachaTabsConfigList = GetGachaTabsConfigList(ShopGachaLG.E.GachaType);
        for (int i = 0; i < 3; i++)
        {
            var gtConf = GetGachaTabsConfig(gachaTabsConfigList, i);
            if (gtConf != null)
            {
                ToggleBtns1.SetActive(i, true);
                ToggleBtns1.SetToggleImage(i, gtConf.ToggleImageON, gtConf.ToggleImageOFF);
            }
            else
            {
                ToggleBtns1.SetActive(i, false);
            }
        }
        // 背景画像
        var gachaTabsConfig = GetGachaTabsConfig(gachaTabsConfigList, ShopGachaLG.E.Index);
        GachaIcon.sprite = ReadResources<Sprite>(gachaTabsConfig.Image);

        // ガチャボタン１(ガチャ実施済回数を取得)
        int gachaGroup1 = int.Parse(gachaTabsConfig.GachaGroup1);
        NWMng.E.GetGacha((rp) =>
        {
            var gacha = int.Parse(rp.ToString());
            var gachaCfg = GetGachaConfig(gachaTabsConfig, gacha, 0);
            int step = GetGachaStep(gachaTabsConfig, gacha, 0);
            // ガチャボタン１背景
            StartGachaBtn1.image.sprite = ReadResources<Sprite>(gachaCfg.BtnImage);
            StartGachaBtn1.onClick.AddListener(() =>
            {
                var ui = UICtl.E.OpenUI<GachaVerificationUI>(UIType.GachaVerification);
                ui.Set(gachaCfg.ID, gachaGroup1);
            });
            // ガチャボタン１コスト
            Cost1.text = gachaCfg.CostCount.ToString();
            CostImage1.sprite = ReadResources<Sprite>(ConfigMng.E.Item[gachaCfg.Cost].IconResourcesPath);
            // 提供割合
            if(ShopGachaLG.E.GachaType == 0)
            {
                RatioBtn.gameObject.SetActive(true);
                RatioBtn.onClick.AddListener(() =>
                {
                    var ui = UICtl.E.OpenUI<GachaRatioUI>(UIType.GachaRatio);
                    ui.Set(gachaCfg.ID);
                });
            }
            else
            {
                RatioBtn.gameObject.SetActive(false);
            }
            // 複数ステップアリの場合のみステップを表示
            if (step != 0)
            {
                StepText1.text = string.Format(StepTextTmpl, step);
                Step1.gameObject.SetActive(true);
            }
            else
            {
                Step1.gameObject.SetActive(false);
            }

        }, gachaGroup1);

        // ガチャボタン２(設定がある場合のみ)
        if (gachaTabsConfig.GachaGroup2 != "N")
        {
            // ガチャボタン２(ガチャ実施済回数を取得)
            int gachaGroup2 = int.Parse(gachaTabsConfig.GachaGroup2);
            NWMng.E.GetGacha((rp) =>
            {
                // ガチャボタン２背景
                var gacha = int.Parse(rp.ToString());
                var gachaCfg = GetGachaConfig(gachaTabsConfig, gacha, 1);
                StartGachaBtn2.image.sprite = ReadResources<Sprite>(gachaCfg.BtnImage);
                StartGachaBtn2.onClick.AddListener(() =>
                {
                    var ui = UICtl.E.OpenUI<GachaVerificationUI>(UIType.GachaVerification);
                    ui.Set(gachaCfg.ID, gachaGroup2);
                });
                StartGachaBtn2.gameObject.SetActive(true);
                // ガチャボタン２コスト
                Cost2.text = gachaCfg.CostCount.ToString();
                CostImage2.sprite = ReadResources<Sprite>(ConfigMng.E.Item[gachaCfg.Cost].IconResourcesPath);
            }, gachaGroup2);
        }
        else
        {
            StartGachaBtn2.gameObject.SetActive(false);
        }
    }

    public void RefreshCoins()
    {
        Title.RefreshCoins();
        HomeLG.E.UI.RefreshCoins();
    }

    // 左ボタン選択状態からGachaTabsの一覧を取得
    private List<GachaTabs> GetGachaTabsConfigList(int index)
    {
        List<GachaTabs> ret = new List<GachaTabs>();
        foreach (GachaTabs gachaTabs in ConfigMng.E.GachaTabs.Values)
        {
            if (index.ToString() == gachaTabs.LeftBtn) ret.Add(gachaTabs);
        }
        return ret;
    }

    // 上部ボタン選択状態からGachaTabsを取得
    private GachaTabs? GetGachaTabsConfig(List<GachaTabs> gachaTabsList, int gachaType)
    {
        foreach (GachaTabs gachaTabs in gachaTabsList)
        {
            if (gachaType.ToString() == gachaTabs.TopBtn) return gachaTabs;
        }
        return null;
    }

    // 左ボタン・上部ボタン選択状態からガチャボタンごとのGachaを取得
    private Gacha? GetGachaConfig(GachaTabs gachaTabs, int gacha, int gachaBtn)
    {
        string gachaIdsCSV =
            gachaBtn == 0 ? gachaTabs.GachaBtn1 :
            gachaBtn == 1 ? gachaTabs.GachaBtn2 :
            null;
        if (string.IsNullOrEmpty(gachaIdsCSV)) return null;

        string[] gachaIds = gachaIdsCSV.Split(',');
        string gachaId = gachaIds[gacha % gachaIds.Length];

        return ConfigMng.E.Gacha[int.Parse(gachaId)];
    }

    // 左ボタン・上部ボタン選択状態からガチャボタンごとのステップを取得(未設定や1ステップの場合0)
    private int GetGachaStep(GachaTabs gachaTabs, int gacha, int gachaBtn)
    {
        string gachaIdsCSV =
            gachaBtn == 0 ? gachaTabs.GachaBtn1 :
            gachaBtn == 1 ? gachaTabs.GachaBtn2 :
            null;
        if (string.IsNullOrEmpty(gachaIdsCSV)) return 0;

        string[] gachaIds = gachaIdsCSV.Split(',');
        if (gachaIds.Length == 1) return 0;

        return gacha % gachaIds.Length + 1;
    }
}
