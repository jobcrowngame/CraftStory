using JsonConfigData;
using UnityEngine;
using UnityEngine.UI;

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

        Debug.Log("OnClickBuyBtn " + config.ID);
    }
}
