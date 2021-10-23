using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopResourceUI : UIBase
{
    Title2UI Title { get => FindChiled<Title2UI>("Title2"); }
    Text CoinText4 { get => FindChiled<Text>("CoinText4"); }
    MyToggleGroupCtl ToggleBtns { get => FindChiled<MyToggleGroupCtl>("ToggleBtns"); }
    Text Des { get => FindChiled<Text>("Des"); }
    Button BackBtn { get => FindChiled<Button>("BackBtn"); }
    Transform ItemsWind { get => FindChiled("ItemsWind"); }

    public override void Init()
    {
        base.Init();
        ShopResourceLG.E.Init(this);

        Title.Init();
        Title.ShowCoin(1);
        Title.ShowCoin(2);

        ToggleBtns.Init();
        ToggleBtns.OnValueChangeAddListener((index) => { RefreshItemWindow(index); });

        Des.text = ConfigMng.E.MText[3].Text;

        BackBtn.onClick.AddListener(Close);

        NWMng.E.GetAllShopLimitedCounts((rp) =>
        {
            ShopResourceLG.E.SetAllLimitedCounts(rp); 
            RefreshItemWindow(0);
        });
    }

    public override void Open()
    {
        base.Open();

        RefreshCoins();
    }

    private void RefreshItemWindow(int index)
    {
        int itemType = 
            index == 0 ? 4 : 
            index == 1 ? 7 :
            8;
        var parent = FindChiled("Parent", ItemsWind.gameObject);
        ClearCell(parent);

        foreach (var item in ConfigMng.E.Shop.Values)
        {
            if (item.Type == itemType)
            {
                var cell = AddCell<ShopItemCell>("Prefabs/UI/ShopItem", parent);
                cell.Init(item);
            }
        }
    }

    public void RefreshCoins()
    {
        Title.RefreshCoins();
        RefreshEventCoin();
    }

    public void RefreshEventCoin()
    {
        var coin = (DataMng.E.GetItemByItemId(9004) != null ? DataMng.E.GetItemByItemId(9004).count : 0);
        CoinText4.text = coin.ToString();
    }
}
