using UnityEngine;
using UnityEngine.UI;

public class ShopSubscriptionCell : UIBase
{
    Transform ItemParent { get => FindChiled("ItemGrid"); }
    Image Icon { get => FindChiled<Image>("Icon"); }
    Text Des { get => FindChiled<Text>("Des"); }
    Button BuyBtn { get => FindChiled<Button>("BuyBtn"); }

    int index = 0;

    public void Set(int index)
    {
        this.index = index;

        var config = ConfigMng.E.Shop[GetShopId()];

        Icon.sprite = ReadResources<Sprite>(config.IconResources);
        Des.text = config.Des;
        AddItemCells(config.Bonus);

        BuyBtn.onClick.AddListener(() =>
        {
            var config = ConfigMng.E.Shop[GetShopId()];
            var ui = UICtl.E.OpenUI<ShopSubscriptionDetailsUI>(UIType.ShopSubscriptionDetails);
            ui.Init(new ShopSubscriptionDetailsLG.SubscriptionData()
            {
                shopId = GetShopId(),
                productId = config.Name
            });
        });
    }

    private void AddItemCells(int bonusId)
    {
        var config = ConfigMng.E.Bonus[bonusId];
        AddItemCell(config.Bonus1, config.BonusCount1);
        AddItemCell(config.Bonus2, config.BonusCount2);
        AddItemCell(config.Bonus3, config.BonusCount3);
    }
    private void AddItemCell(int itemId, int count)
    {
        if (itemId < 0)
            return;

        var cell = AddCell<ShopSubscriptionItemCell>("Prefabs/UI/ShopSubscriptionItemCell", ItemParent);
        if (cell != null)
        {
            cell.Set(itemId, count);
        }
    }
    private int GetShopId()
    {
        return index + 80;
    }
}