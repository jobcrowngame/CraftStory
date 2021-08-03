using UnityEngine;
using UnityEngine.UI;

public class ShopSubscriptionDetailsUI : UIBase
{
    TitleUI Title { get => FindChiled<TitleUI>("Title"); }
    Image Icon { get => FindChiled<Image>("Icon"); }
    Text Des { get => FindChiled<Text>("Des"); }
    Text Price { get => FindChiled<Text>("Price"); }
    Text Des1 { get => FindChiled<Text>("Des1"); }
    Text Des2 { get => FindChiled<Text>("Des2"); }
    Transform ItemParent { get => FindChiled("Content"); }
    Button OkBtn { get => FindChiled<Button>("OkBtn"); }
    Button CancelBtn { get => FindChiled<Button>("CancelBtn"); }

    ShopSubscriptionDetailsLG.SubscriptionData mData;

    string des1 = "説明1";
    string des2 = "説明2";

    private void Awake()
    {
        OkBtn.onClick.AddListener(() => 
        {
            IAPMng.E.OnPurchaseClicked(mData.productId);
            Close();
        });
        CancelBtn.onClick.AddListener(Close);
    }
    public override void Init(object data)
    {
        base.Init(data);
        ShopSubscriptionDetailsLG.E.Init(this);
        mData = (ShopSubscriptionDetailsLG.SubscriptionData)data;

        Title.SetTitle("サブスクリプション");
        Title.SetOnClose(() => { Close(); });
        Title.EnActiveCoin(1);
        Title.EnActiveCoin(2);
        Title.EnActiveCoin(3);

        var config = ConfigMng.E.Shop[mData.shopId];
        Icon.sprite = ReadResources<Sprite>(config.IconResources);
        Des.text = config.Des;
        Price.text = config.BtnText;
        Des1.text = des1;
        Des2.text = des2;

        AddItemCells(config.Bonus);
    }

    private void AddItemCells(int bonusId)
    {
        ClearCell(ItemParent);

        var config = ConfigMng.E.Bonus[bonusId];
        AddItemCell(config.Bonus1, config.BonusCount1);
        AddItemCell(config.Bonus2, config.BonusCount2);
        AddItemCell(config.Bonus3, config.BonusCount3);
    }
    private void AddItemCell(int itemId, int count)
    {
        if (itemId < 0)
            return;

        var cell = AddCell<ShopSubscriptionDetailsCell>("Prefabs/UI/ShopSubscriptionDetailsCell", ItemParent);
        if (cell != null)
        {
            cell.Set(itemId, count);
        }
    }
}