using JsonConfigData;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : UIBase
{
    Button CloseBtn;
    Text Title1;

    Text Coin1;
    Image Coin1Image;
    Text Coin2;
    Image Coin2Image;
    Text Coin3;
    Image Coin3Image;

    Transform itemGridRoot;
    Text Title2;
    Button[] btns;

    Transform ItemsWind;
    Transform ChageWind;

    public override void Init()
    {
        base.Init();

        ShopLG.E.Init(this);
        btns = new Button[2];

        var titleParent = FindChiled("Title");
        CloseBtn = FindChiled<Button>("CloseBtn", titleParent);
        CloseBtn.onClick.AddListener(() => { Close(); });

        Title1 = FindChiled<Text>("TitleText", titleParent);
        Coin1 = FindChiled<Text>("Coin1", titleParent);
        Coin1.text = "0";
        Coin1Image = FindChiled<Image>("Image", Coin1.transform);
        Coin1Image.sprite = ReadResources<Sprite>(ConfigMng.E.Item[9000].IconResourcesPath);
        Coin2 = FindChiled<Text>("Coin2", titleParent);
        Coin2.text = "0";
        Coin2Image = FindChiled<Image>("Image", Coin2.transform);
        Coin2Image.sprite = ReadResources<Sprite>(ConfigMng.E.Item[9001].IconResourcesPath);
        Coin3 = FindChiled<Text>("Coin3", titleParent);
        Coin3.text = "0";
        Coin3Image = FindChiled<Image>("Image", Coin3.transform);
        Coin3Image.sprite = ReadResources<Sprite>(ConfigMng.E.Item[9002].IconResourcesPath);

        var itemsParent = FindChiled("Items");
        itemGridRoot = FindChiled("ItemGrid");
        Title2 = FindChiled<Text>("ItemsTitle", itemsParent);

        var btnsParent = FindChiled("BtnGrid");
        btns[0] = FindChiled<Button>("Button (1)");
        btns[0].onClick.AddListener(() => { ShopLG.E.ShopUIType = ShopUiType.Charge; });
        btns[1] = FindChiled<Button>("Button (2)");
        btns[1].onClick.AddListener(() => { ShopLG.E.ShopUIType = ShopUiType.Exchange; });

        ItemsWind = FindChiled("ItemsWind");
        ChageWind = FindChiled("ChargeWind");

        ShopLG.E.ShopUIType = ShopUiType.Charge;

        Refresh();
    }

    public void Refresh()
    {
        Coin1.text = DataMng.E.UserData.Coin1.ToString();
        Coin2.text = DataMng.E.UserData.Coin2.ToString();
        Coin3.text = DataMng.E.UserData.Coin3.ToString();
    }

    public void SetTitle2(string msg)
    {
        Title2.text = msg;
    }

    public void IsChargeWind(bool b)
    {
        return;

        ChageWind.gameObject.SetActive(b);
        ItemsWind.gameObject.SetActive(!b);
    }

    public void RefreshItems(ShopUiType type)
    {
        ClearCell(itemGridRoot);

        foreach (var item in ConfigMng.E.Shop.Values)
        {
            if (item.Type == (int)type)
            {
                var cell = AddCell<ShopItemCell>("Prefabs/UI/ShopItem", itemGridRoot);
                cell.Init(item);
            }
        }
    }
}
