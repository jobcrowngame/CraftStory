using JsonConfigData;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemCell : UIBase
{
    Transform Limited { get => FindChiled("Limited"); }
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
        Limited.gameObject.SetActive(config.LimitedCount > 0);
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
            || type == ShopType.Blueprint)
        {
            CommonFunction.ShowHintBox(config.IconResources, config.Des2, 
                () => {
                    NWMng.E.Buy((rp) =>
                    {
                        NWMng.E.GetItems(null);
                        NWMng.E.GetCoins((rp3) =>
                        {
                            DataMng.GetCoins(rp3);
                            ShopChargeLG.E.UI.RefreshCoins();
                            ShopMarketLG.E.UI.RefreshCoins();
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
