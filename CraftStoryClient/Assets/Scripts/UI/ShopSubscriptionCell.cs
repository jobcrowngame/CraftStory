using System;
using UnityEngine;
using UnityEngine.UI;

public class ShopSubscriptionCell : UIBase
{
    Transform ItemParent { get => FindChiled("ItemGrid"); }
    Image Icon { get => FindChiled<Image>("Icon"); }
    Text Des { get => FindChiled<Text>("Des"); }
    Text Time { get => FindChiled<Text>("Time"); }
    Button BuyBtn { get => FindChiled<Button>("BuyBtn"); }

    int index = 0;

    string hintMsg = @"
現在、利用中になっているクラパスがあります。

<color=red>※新しくクラパスを購入する場合、すでに利用中となっているクラパスはリセットされ
　 未購入となります。
※クラパスがリセットされ未購入になると、残りの受取可能期間は無効となります。</color>

クラパスの購入確認画面へ移動しますか？
";

    public void Set(int index)
    {
        this.index = index;

        var config = ConfigMng.E.Shop[GetShopId()];

        Icon.sprite = ReadResources<Sprite>(config.IconResources);
        Des.text = config.Des;
        AddItemCells(config.Bonus);

        FindChiled("Text", BuyBtn.gameObject).GetComponent<Text>().text = config.BtnText;
        BuyBtn.onClick.AddListener(() =>
        {
            if (DataMng.E.RuntimeData.SubscriptionLv > 0)
            {
                CommonFunction.ShowHintBox(hintMsg, () =>{ OpenSubscriptionDetailsUI(); }, () => { });
            }
            else
            {
                OpenSubscriptionDetailsUI();
            }
        });

        CheckActive();
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
    private void CheckActive()
    {
        int shopId = GetShopId();

        if (DataMng.E.RuntimeData.SubscriptionLv == shopId)
        {
            BuyBtn.gameObject.SetActive(false);

            var day = 29 - (DateTime.Now - DataMng.E.RuntimeData.SubscriptionUpdateTime).Days;
            Time.text = string.Format("利用中　残り{0}日", day);
        }
        else
        {
            BuyBtn.gameObject.SetActive(true);
        }
    }
    private void OpenSubscriptionDetailsUI()
    {
        var config = ConfigMng.E.Shop[GetShopId()];
        var ui = UICtl.E.OpenUI<ShopSubscriptionDetailsUI>(UIType.ShopSubscriptionDetails);
        ui.Init(new ShopSubscriptionDetailsLG.SubscriptionData()
        {
            shopId = GetShopId(),
            productId = config.Name
        });
    }
}