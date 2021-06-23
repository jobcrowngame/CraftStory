using JsonConfigData;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class ShopUI : UIBase
{
    TitleUI title;

    Transform itemGridRoot;
    Text Title2;
    Button[] btns;

    Transform ItemsWind;
    Transform ChageWind;

    IAPManager IAP;

    ShopItemCell[] chargeBtns;

    public override void Init()
    {
        base.Init();

        ShopLG.E.Init(this);
        btns = new Button[2];

        ItemsWind = FindChiled("ItemsWind");
        ChageWind = FindChiled("ChargeWind");

        title = FindChiled<TitleUI>("Title");
        title.SetTitle("‚à‚¿‚à‚Ì");
        title.SetOnClose(() => { Close(); });

        itemGridRoot = FindChiled("Grid", ItemsWind.gameObject);
        Title2 = FindChiled<Text>("ItemsTitle");

        var btnsParent = FindChiled("BtnGrid");
        btns[0] = FindChiled<Button>("Button (1)");
        btns[0].onClick.AddListener(() => { ShopLG.E.ShopUIType = ShopUiType.Charge; });
        btns[1] = FindChiled<Button>("Button (2)");
        btns[1].onClick.AddListener(() => { ShopLG.E.ShopUIType = ShopUiType.Exchange; });

        ShopLG.E.ShopUIType = ShopUiType.Charge;

        Refresh();

        IAP = new IAPManager();
        IAP.Init();

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
        chargeBtns[5].Init(ConfigMng.E.Shop[6]);
        chargeBtns[6].Init(ConfigMng.E.Shop[7]);
        chargeBtns[7].Init(ConfigMng.E.Shop[8]);
    }

    public void Refresh()
    {
        title.RefreshCoins();
    }

    public void SetTitle2(string msg)
    {
        Title2.text = msg;
    }

    public void IsChargeWind(bool b)
    {
        ChageWind.gameObject.SetActive(b);
        ItemsWind.gameObject.SetActive(!b);
    }

    public void RefreshItems(ShopUiType type)
    {
        ClearCell(itemGridRoot);

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

    public void GrantCredits(string credits)
    {
        IAP.OnPurchaseClicked(credits);
    }
}
