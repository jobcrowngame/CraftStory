using UnityEngine;
using UnityEngine.UI;

public class ShopResourceUI : UIBase
{
    Title2UI Title { get => FindChiled<Title2UI>("Title2"); }
    MyToggleGroupCtl ToggleBtns { get => FindChiled<MyToggleGroupCtl>("ToggleBtns"); }
    Text Des { get => FindChiled<Text>("Des"); }
    Button BackBtn { get => FindChiled<Button>("BackBtn"); }
    Transform ItemsWind { get => FindChiled("ItemsWind"); }

    public override void Init()
    {
        base.Init();

        Title.Init();
        Title.ShowCoin(1);
        Title.ShowCoin(2);

        ToggleBtns.Init();
        ToggleBtns.OnValueChangeAddListener((index) => { RefreshItemWindow(index); });

        Des.text = ConfigMng.E.MText[3].Text;

        BackBtn.onClick.AddListener(Close);
    }

    public override void Open()
    {
        base.Open();

        Title.RefreshCoins();
        RefreshItemWindow(0);
    }

    private void RefreshItemWindow(int index)
    {
        int itemType = index == 0 ? 4 : 7;
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
}
