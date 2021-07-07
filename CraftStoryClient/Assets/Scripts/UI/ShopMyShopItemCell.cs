
using UnityEngine;
using UnityEngine.UI;

public class ShopMyShopItemCell : UIBase
{
    Image Icon { get => FindChiled<Image>("Icon"); }
    Text Des { get => FindChiled<Text>("Des"); }
    Button BuyBtn { get => FindChiled<Button>("BuyBtn"); }
    Button PreviewBtn { get => FindChiled<Button>("PreviewBtn"); }
    Text BuyBtnText { get => FindChiled<Text>("Text", BuyBtn.transform); }
    Image BtnCostIcon { get => FindChiled<Image>("Image", BuyBtn.transform); }

    public void Set(MyShopBlueprintData data)
    {
        Icon.sprite = ReadResources<Sprite>(ConfigMng.E.Item[data.itemId].IconResourcesPath);
        Des.text = data.newName;
        BuyBtnText.text = data.price.ToString();
        BtnCostIcon.sprite = ReadResources<Sprite>(ConfigMng.E.Item[9002].IconResourcesPath);

        PreviewBtn.gameObject.SetActive(true);
        BtnCostIcon.gameObject.SetActive(true);

        BuyBtn.onClick.AddListener(() => 
        {
            string msg = string.Format("{0}ポイントを消耗して交換しますか。", data.price);
            CommonFunction.ShowHintBox(msg,
                () => {
                    if (DataMng.E.RuntimeData.Coin3 < data.price)
                    {
                        CommonFunction.ShowHintBar(15);
                        return;
                    }

                    NWMng.E.BuyMyShopItem((rp) =>
                    {
                        DataMng.E.RuntimeData.Coin3 -= data.price;
                        ShopLG.E.UI.RefreshCoins();
                    }, data.id);
                }, () => { });
        });
        PreviewBtn.onClick.AddListener(() => 
        {
            var ui = UICtl.E.OpenUI<BlueprintPreviewUI>(UIType.BlueprintPreview, UIOpenType.AllClose);
            ui.SetData(data.data, ShopLG.E.UI);
        });
    }
}