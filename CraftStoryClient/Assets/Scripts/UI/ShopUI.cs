using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class ShopUI : UIBase
{
    TitleUI title;
    MyButton[] btns;
    ShopItemCell[] chargeBtns;
    List<ShopMyShopItemCell> myshopItems = new List<ShopMyShopItemCell>();

    Text Title2 { get => FindChiled<Text>("ItemsTitle"); }

    Transform UrlBtns { get => FindChiled("UrlBtns"); }
    Transform ChageWind { get => FindChiled("ChargeWind"); }
    Transform ItemsWind { get => FindChiled("ItemsWind"); }
    Transform itemGridRoot { get => FindChiled("Grid", ItemsWind.gameObject); }
    Transform SubscriptionWind { get => FindChiled("SubscriptionWind"); }
    Transform Gacha { get => FindChiled("Gacha"); }

    Transform Blueprint2Wind { get => FindChiled("Blueprint2Wind"); }
    Transform itemGridRoot2 { get => FindChiled("Grid", Blueprint2Wind.gameObject); }
    Text Page { get => FindChiled<Text>("Page", Blueprint2Wind); }
    InputField InputField { get => FindChiled<InputField>("InputField", Blueprint2Wind); }
    Button LeftBtn { get => FindChiled<Button>("LeftBtn", Blueprint2Wind); }
    Button RightBtn { get => FindChiled<Button>("RightBtn", Blueprint2Wind); }
    Button SearchBtn { get => FindChiled<Button>("SearchBtn", Blueprint2Wind); }
    Dropdown Dropdown { get => FindChiled<Dropdown>("Dropdown", Blueprint2Wind); }

    public ShopType SelectBtnIndex
    {
        set
        {
            foreach (var btn in btns)
            {
                btn.GetComponent<Image>().color = Color.gray;
            }

            btns[(int)value].GetComponent<Image>().color = Color.white;
        }
    }

    public override void Init(object index)
    {
        ShopLG.E.Init(this);

        title = FindChiled<TitleUI>("Title");
        title.SetTitle("ショップ");
        title.SetOnClose(() => { Close(); GuideLG.E.Next(); });
        title.ShowCoin(1);
        title.ShowCoin(2);
        title.ShowCoin(3);

        var urlBtn01 = FindChiled<Button>("UrlBtn1", UrlBtns);
        urlBtn01.onClick.AddListener(() => { Application.OpenURL(PublicPar.UrlBtn1); });
        var urlBtn02 = FindChiled<Button>("UrlBtn2", UrlBtns);
        urlBtn02.onClick.AddListener(() => { Application.OpenURL(PublicPar.UrlBtn2); });

        LeftBtn.onClick.AddListener(()=> { ShopLG.E.OnClickLeftBtn(InputField.text, Dropdown.value); });
        RightBtn.onClick.AddListener(()=> { ShopLG.E.OnClickRightBtn(InputField.text, Dropdown.value); });
        SearchBtn.onClick.AddListener(()=> { ShopLG.E.GetBlueprint2Items(InputField.text, Dropdown.value); });
        Dropdown.options.Clear();
        Dropdown.AddOptions(new List<string>
        {
            "ポイントが高い順",
            "ポイントが安い順",
            "登録が新しい順",
            "登録が古い順"
        });
        Dropdown.value = 0;
        Dropdown.onValueChanged.AddListener((value) => 
        {
            ShopLG.E.GetBlueprint2Items(InputField.text, value);
        });

        var btnsParent = FindChiled("BtnGrid");
        btns = new MyButton[btnsParent.childCount];
        for (int i = 0; i < btnsParent.childCount; i++)
        {
            var child = btnsParent.GetChild(i).GetComponent<MyButton>();
            if (child != null)
            {
                child.Index = i;
                child.AddClickListener((index) => 
                {
                    ShopLG.E.ShopUIType = (ShopType)index; 

                    // ガイド
                    if (ShopLG.E.ShopUIType == ShopType.Blueprint2)
                        GuideLG.E.Next();
                });
                btns[i] = child;
            }
        }

        InitGacha();

         var chargeBtnParent = FindChiled("Grid", ChageWind.gameObject);
        chargeBtns = new ShopItemCell[chargeBtnParent.childCount];
        for (int i = 0; i < chargeBtnParent.childCount; i++)
        {
            chargeBtns[i] = chargeBtnParent.GetChild(i).GetComponent<ShopItemCell>();
        }

        chargeBtns[0].Init(ConfigMng.E.Shop[1]);
        chargeBtns[1].Init(ConfigMng.E.Shop[2]);
        chargeBtns[2].Init(ConfigMng.E.Shop[3]);
        chargeBtns[3].Init(ConfigMng.E.Shop[4]);
        chargeBtns[4].Init(ConfigMng.E.Shop[5]);
    }

    public override void Open(object data)
    {
        base.Open(data);
        ShopLG.E.ShopUIType = (ShopType)data; 
        RefreshCoins();
        InputField.text = "";
        //SelectBtnIndex = 0;
        //ShopLG.E.ShopUIType = ShopUiType.Charge;

        NWMng.E.GetCoins((rp) =>
        {
            DataMng.GetCoins(rp);
            RefreshCoins();
        });

        foreach (var item in myshopItems)
        {
            if (item == null)
                continue;

            item.Open();
        }

        ShopLG.E.GetSubscriptions();
    }

    public void SetTitle2(string msg)
    {
        Title2.text = msg;
    }
    public void SetPageText(string v)
    {
        Page.text = v;
    }

    public void ChangeUIType(ShopType uiType)
    {
        ChageWind.gameObject.SetActive(uiType == ShopType.Charge);
        ItemsWind.gameObject.SetActive(uiType == ShopType.Exchange 
            || uiType == ShopType.Blueprint
            || uiType == ShopType.Point);
        Blueprint2Wind.gameObject.SetActive(uiType == ShopType.Blueprint2);
        SubscriptionWind.gameObject.SetActive(uiType == ShopType.Subscription);
        Gacha.gameObject.SetActive(uiType == ShopType.Gacha);

        UrlBtns.gameObject.SetActive(uiType == ShopType.Charge
           || uiType == ShopType.Subscription);
    }

    public void RefreshCoins()
    {
        title.RefreshCoins();
    }
    public void RefreshItems(ShopType type)
    {
        ClearCell(itemGridRoot);
        ClearCell(itemGridRoot2);

        if (type == ShopType.Blueprint2)
        {
            ShopLG.E.GetBlueprint2Items(InputField.text, Dropdown.value);
        }
        else if (type == ShopType.Subscription)
        {

        }
        else
        {
            foreach (var item in ConfigMng.E.Shop.Values)
            {
                if (item.Type == 1)
                    continue;

                if (item.Type == (int)type)
                {
                    var cell = AddCell<ShopItemCell>("Prefabs/UI/ShopItem", itemGridRoot);
                    cell.Init(item);
                }
            }
        }
    }
    public void RefreshBlueprint2(List<MyShopItem> items)
    {
        ClearCell(itemGridRoot2);

        if (items != null)
        {
            foreach (var item in items)
            {
                var cell = AddCell<ShopMyShopItemCell>("Prefabs/UI/ShopMyShopItem", itemGridRoot2);
                cell.Set(item);
                myshopItems.Add(cell);
            }
        }
    }
    public void RefreshSubscription()
    {
        var parent = FindChiled("Grid", SubscriptionWind.gameObject);
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

    // 課金のメソッド
    public void GrantCredits(string credits)
    {
        IAPMng.E.OnPurchaseClicked(credits);
    }
}
