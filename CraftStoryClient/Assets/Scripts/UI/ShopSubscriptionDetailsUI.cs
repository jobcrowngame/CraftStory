using UnityEngine;
using UnityEngine.UI;

public class ShopSubscriptionDetailsUI : UIBase
{
    TitleUI Title { get => FindChiled<TitleUI>("Title"); }
    Text Price { get => FindChiled<Text>("Price"); }
    Transform ItemParent { get => FindChiled("ItemGrid"); }
    Image Icon { get => FindChiled<Image>("Icon"); }
    Text Des { get => FindChiled<Text>("Des"); }
    Button OkBtn { get => FindChiled<Button>("OkBtn"); }
    Button CancelBtn { get => FindChiled<Button>("CancelBtn"); }

    ShopSubscriptionDetailsLG.SubscriptionData mData;

    string des = @"・{0}
・購入した日付から、毎日ログインすることで、最大30回もらえるお得なパスです。
・メッセージボックスから受け取ることができます。

<color=red>※毎日0時にログイン情報が更新されます。
※受取可能期間は、購入時から30日間となります。
※受取可能期間中、同じ「クラパス」を購入することはできません。</color>

";

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

        Title.SetTitle("クラパス");
        Title.SetOnClose(() => { Close(); });

        var config = ConfigMng.E.Shop[mData.shopId];
        Price.text = config.BtnText;
        Icon.sprite = ReadResources<Sprite>(config.IconResources);
        Des.text = string.Format(des, config.Des2);

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

        var cell = AddCell<ShopSubscriptionItemCell>("Prefabs/UI/ShopSubscriptionItemCell", ItemParent);
        if (cell != null)
        {
            cell.Set(itemId, count);
        }
    }
}