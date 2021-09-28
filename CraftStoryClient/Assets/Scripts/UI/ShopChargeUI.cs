using UnityEngine;
using UnityEngine.UI;

public class ShopChargeUI : UIBase
{
    Title2UI Title { get => FindChiled<Title2UI>("Title2"); }
    MyToggleGroupCtl ToggleBtns { get => FindChiled<MyToggleGroupCtl>("ToggleBtns"); }
    Text Des { get => FindChiled<Text>("Des"); }
    Button UrlBtn1 { get => FindChiled<Button>("UrlBtn1"); }
    Button UrlBtn2 { get => FindChiled<Button>("UrlBtn2"); }
    Button BackBtn { get => FindChiled<Button>("BackBtn"); }

    Transform ChargeWind { get => FindChiled("ChargeWind"); }
    Transform SubscriptionWind { get => FindChiled("SubscriptionWind"); }
    Transform ItemsWind { get => FindChiled("ItemsWind"); }
    Transform UrlBtns { get => FindChiled("UrlBtns"); }

    ShopItemCell[] chargeBtns;

    public override void Init()
    {
        base.Init();
        ShopChargeLG.E.Init(this);

        Title.Init();
        Title.ShowCoin(1);
        Title.ShowCoin(3);

        ToggleBtns.Init();
        ToggleBtns.OnValueChangeAddListener((index) => 
        {
            ShopChargeLG.E.Type = (ShopChargeLG.UiType)index;
        });

        Des.text = ConfigMng.E.MText[2].Text;

        UrlBtn1.onClick.AddListener(() => { Application.OpenURL(PublicPar.UrlBtn1); });
        UrlBtn2.onClick.AddListener(() => { Application.OpenURL(PublicPar.UrlBtn2); });
        BackBtn.onClick.AddListener(Close);
    }

    public override void Open()
    {
        base.Open();

        ShopChargeLG.E.Type = ShopChargeLG.UiType.Charge;
        Title.RefreshCoins();
    }

    /// <summary>
    /// UIタイプが変更する場合
    /// </summary>
    /// <param name="type"></param>
    public void OnUITypeChange(ShopChargeLG.UiType type)
    {
        ChargeWind.gameObject.SetActive(type == ShopChargeLG.UiType.Charge);
        SubscriptionWind.gameObject.SetActive(type == ShopChargeLG.UiType.Subscription);
        ItemsWind.gameObject.SetActive(type == ShopChargeLG.UiType.ExchangePoint);
        UrlBtns.gameObject.SetActive(type != ShopChargeLG.UiType.ExchangePoint);

        switch (type)
        {
            case ShopChargeLG.UiType.Charge: RefreshChargeWindow(); break;
            case ShopChargeLG.UiType.Subscription: RefreshSubscriptionWindow(); break;
            case ShopChargeLG.UiType.ExchangePoint: RefreshExchangePointWindow(); break;
        }
    }

    /// <summary>
    /// UI 更新
    /// </summary>
    private void RefreshChargeWindow()
    {
        var parent = FindChiled("Parent", ChargeWind.gameObject);
        chargeBtns = new ShopItemCell[parent.childCount];
        for (int i = 0; i < parent.childCount; i++)
        {
            chargeBtns[i] = parent.GetChild(i).GetComponent<ShopItemCell>();
        }

        chargeBtns[0].Init(ConfigMng.E.Shop[1]);
        chargeBtns[1].Init(ConfigMng.E.Shop[2]);
        chargeBtns[2].Init(ConfigMng.E.Shop[3]);
        chargeBtns[3].Init(ConfigMng.E.Shop[4]);
        chargeBtns[4].Init(ConfigMng.E.Shop[5]);
    }
    private void RefreshSubscriptionWindow()
    {
        var parent = FindChiled("Parent", SubscriptionWind.gameObject);
        ClearCell(parent);

        for (int i = 0; i < 3; i++)
        {
            var cell = AddCell<ShopSubscriptionCell>("Prefabs/UI/ShopSubscriptionCell", parent);
            if (cell != null)
            {
                cell.Set(i);
            }
        }
    }
    private void RefreshExchangePointWindow()
    {
        var parent = FindChiled("Parent", ItemsWind.gameObject);
        ClearCell(parent);

        foreach (var item in ConfigMng.E.Shop.Values)
        {
            if (item.Type == 1)
                continue;

            if (item.Type == 3)
            {
                var cell = AddCell<ShopItemCell>("Prefabs/UI/ShopItem", parent);
                cell.Init(item);
            }
        }
    }
}
