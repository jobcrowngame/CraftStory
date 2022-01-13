using JsonConfigData;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemCell : UIBase
{
    Transform Limited { get => FindChiled("Limited"); }
    Text LimitedText { get => FindChiled<Text>("LimitedText"); }
    Transform Mask { get => FindChiled<Transform>("Mask"); }
    Image Icon { get => FindChiled<Image>("Icon"); }
    Text Des { get => FindChiled<Text>("Des"); }
    Button BuyBtn { get => FindChiled<Button>("BuyBtn"); }
    Button PreviewBtn { get => FindChiled<Button>("PreviewBtn"); }
    Text BuyBtnText { get => FindChiled<Text>("Text", BuyBtn.transform); }
    Image BtnCostIcon { get => FindChiled<Image>("Image", BuyBtn.transform); }

    private Shop config;

    public override void Init<T>(T t)
    {
        base.Init(t);

        config = t as Shop;

        BuyBtn.onClick.AddListener(OnClickBuyBtn);
        Limited.gameObject.SetActive(false);
        Mask.gameObject.SetActive(false);
        Icon.sprite = ReadResources<Sprite>(config.IconResources);
        Des.text = config.Des;
        BuyBtnText.text = config.BtnText;
        ShopType shopType = (ShopType)config.Type;

        if (shopType != ShopType.Charge)
        {
            BtnCostIcon.sprite = ReadResources<Sprite>(ConfigMng.E.Item[config.CostItemID].IconResourcesPath);
            BtnCostIcon.gameObject.SetActive(true);
        }

        if (shopType == ShopType.Blueprint || shopType == ShopType.Blueprint2)
        {
            PreviewBtn.gameObject.SetActive(true);
            PreviewBtn.onClick.AddListener(OnClickPreviewBtn);
        }

        UpdateLimited();
    }

    private void UpdateLimited()
    {
        if (config.LimitedCount != -1)
        {
            int limitedCount = ShopResourceLG.E.GetLimitedCount(config.ID);
            int remainCount = config.LimitedCount - limitedCount;
            bool masked = remainCount <= 0;
            LimitedText.text = masked ? "交換可能な上限まで交換済" : string.Format("あと{0}回交換可能", remainCount);
            Mask.gameObject.SetActive(masked);
            Limited.gameObject.SetActive(true);
        }
        else
        {
            LimitedText.text = "";
            Mask.gameObject.SetActive(false);
            Limited.gameObject.SetActive(false);
        }
    }

    private void OnClickBuyBtn()
    {
        ShopType type = (ShopType)config.Type;
        if (type == ShopType.Charge)
        {
            //Logger.Log("OnClickBuyBtn " + config.Name);
            //NWMng.E.IAP.OnPurchaseClicked(config.Name);
        }
        else if (type == ShopType.Point 
            || type == ShopType.Exchange 
            || type == ShopType.Blueprint
            || type == ShopType.CraftResource
            || type == ShopType.EventItem)
        {
            CommonFunction.ShowHintBox(config.IconResources, config.Des2, () =>
            {
                NWMng.E.Buy((rp) =>
                {
                    NWMng.E.GetItems(()=> 
                    {
                        if (ShopChargeLG.E.UI != null) ShopChargeLG.E.UI.RefreshCoins();
                        if (ShopResourceLG.E.UI != null) ShopResourceLG.E.UI.RefreshCoins();
                        HomeLG.E.UI.RefreshCoins();
                        if (config.LimitedCount != -1)
                        {
                            NWMng.E.GetShopLimitedCount((rp4) => {
                                ShopResourceLG.E.SetLimitedCount(config.ID, (int)rp4);
                                UpdateLimited();
                            }, config.ID);
                        }

                        // ロイヤルコインを使うタスク
                        if (config.Type == 8)
                            TaskMng.E.AddMainTaskCount(8);
                    });
                    CommonFunction.ShowHintBar(5);
                }, config.ID);
            },()=> { });
        }
    }

    private void OnClickPreviewBtn()
    {
        var ui = UICtl.E.OpenUI<BlueprintPreviewUI>(UIType.BlueprintPreview, UIOpenType.AllClose, 0);
        ui.SetData(config.Relation, ShopBlueprintLG.E.UI);
    }
}
