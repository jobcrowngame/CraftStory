using JsonConfigData;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;

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

        if (config.Type != 1)
        {
            BtnCostIcon.sprite = ReadResources<Sprite>(ConfigMng.E.Item[config.CostItemID].IconResourcesPath);
            BtnCostIcon.gameObject.SetActive(true);
        }

        if (config.Type == 3)
        {
            PreviewBtn.gameObject.SetActive(true);
            PreviewBtn.onClick.AddListener(OnClickPreviewBtn);
        }
    }

    private void OnClickBuyBtn()
    {
        if (config.Type == 1)
        {
            //Logger.Log("OnClickBuyBtn " + config.Name);
            //NWMng.E.IAP.OnPurchaseClicked(config.Name);
        }
        else if (config.Type == 2 || config.Type == 3)
        {
            CommonFunction.ShowHintBox(config.IconResources, config.Des2, 
                () => {
                    NWMng.E.Buy((rp) =>
                    {
                        NWMng.E.GetItemList((rp2) =>
                        {
                            DataMng.GetItems(rp2);
                        });
                        NWMng.E.GetCoins((rp3) =>
                        {
                            DataMng.GetCoins(rp3);
                            ShopLG.E.UI.RefreshCoins();
                        });

                        CommonFunction.ShowHintBar(5);
                    }, config.ID);
                },()=> { });
        }
    }

    private void OnClickPreviewBtn()
    {
        var ui = UICtl.E.OpenUI<BlueprintPreviewUI>(UIType.BlueprintPreview, UIOpenType.AllClose);
        ui.SetData(config.Relation);
    }
}
