using JsonConfigData;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;

public class ShopItemCell : UIBase
{
    Transform Limited;
    Image Icon;
    Text Name;
    Text Des;
    Button BuyBtn;
    Text BuyBtnText;

    private Shop config;

    private void Awake()
    {
        Limited = FindChiled("Limited");
        Icon = FindChiled<Image>("Icon");
        Name = FindChiled<Text>("Name");
        Des = FindChiled<Text>("Des");

        BuyBtn = FindChiled<Button>("BuyBtn");
        BuyBtnText = FindChiled<Text>("Text", BuyBtn.transform);
        BuyBtn.onClick.AddListener(OnClickBuyBtn);
    }

    public override void Init<T>(T t)
    {
        base.Init(t);

        config = t as Shop;

        Limited.gameObject.SetActive(config.LimitedCount > 0);
        Icon.sprite = ReadResources<Sprite>(config.IconResources);
        Name.text = config.Name;
        Des.text = config.Des;
        BuyBtnText.text = config.BtnText;
    }

    private void OnClickBuyBtn()
    {
        if (config.Type == 1)
        {
            Debug.Log("OnClickBuyBtn " + config.Name);

            //NWMng.E.IAP.OnPurchaseClicked(config.Name);
        }
        else if (config.Type == 2)
        {
            NWMng.E.Buy((rp) =>
            {
                NWMng.E.GetItemList((rp2) =>
                {
                    DataMng.GetItems(rp2[0]);
                });
                NWMng.E.GetCoins((rp3) =>
                {
                    DataMng.GetCoins(rp3[0]);
                    ShopLG.E.UI.Refresh();
                });
            }, config.ID);
        }
    }
}
