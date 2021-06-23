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

    public override void Init()
    {
        base.Init();

        ShopLG.E.Init(this);
        btns = new Button[2];

        title = FindChiled<TitleUI>("Title");
        title.SetTitle("��������");
        title.SetOnClose(() => { Close(); });

        var itemsParent = FindChiled("Items");
        itemGridRoot = FindChiled("ItemGrid");
        Title2 = FindChiled<Text>("ItemsTitle", itemsParent);

        var btnsParent = FindChiled("BtnGrid");
        btns[0] = FindChiled<Button>("Button (1)");
        btns[0].onClick.AddListener(() => { ShopLG.E.ShopUIType = ShopUiType.Charge; });
        btns[1] = FindChiled<Button>("Button (2)");
        btns[1].onClick.AddListener(() => { ShopLG.E.ShopUIType = ShopUiType.Exchange; });

        ItemsWind = FindChiled("ItemsWind");
        ChageWind = FindChiled("ChargeWind");

        ShopLG.E.ShopUIType = ShopUiType.Charge;

        Refresh();

        IAP = new IAPManager();
        IAP.Init();
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
