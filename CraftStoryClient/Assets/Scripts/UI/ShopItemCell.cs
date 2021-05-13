using Gs2.Unity.Gs2Showcase.Model;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemCell : UIBase
{
    Text Name;
    Button BuyBtn;

    private EzDisplayItem item;

    private void InitUI()
    {
        Name = FindChiled<Text>("Name");

        BuyBtn = FindChiled<Button>("BuyBtn");
        BuyBtn.onClick.AddListener(() => { Buy(); });
    }

    public void Add(EzDisplayItem item)
    {
        this.item = item;

        InitUI();

        Name.text = item.SalesItem.Name;
    }

    public void Buy()
    {
        GS2.E.Buy((r) =>
        {
            var giftUI = UICtl.E.OpenUI<GiftBoxUI>(UIType.GiftBox) as GiftBoxUI;
            giftUI.AddItem(r);
        }, ShopLG.E.ShowcaseName, item);
    }
}
