using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : UIBase
{
    TitleUI title;
    Button[] btns;
    ShopItemCell[] chargeBtns;

    Text Title2 { get => FindChiled<Text>("ItemsTitle"); }

    Transform ChageWind { get => FindChiled("ChargeWind"); }
    Transform ItemsWind { get => FindChiled("ItemsWind"); }
    Transform itemGridRoot { get => FindChiled("Grid", ItemsWind.gameObject); }

    Transform Blueprint2Wind { get => FindChiled("Blueprint2Wind"); }
    Transform itemGridRoot2 { get => FindChiled("Grid", Blueprint2Wind.gameObject); }
    Text Page { get => FindChiled<Text>("Page", Blueprint2Wind); }
    InputField InputField { get => FindChiled<InputField>("InputField", Blueprint2Wind); }
    Button LeftBtn { get => FindChiled<Button>("LeftBtn", Blueprint2Wind); }
    Button RightBtn { get => FindChiled<Button>("RightBtn", Blueprint2Wind); }
    Button SearchBtn { get => FindChiled<Button>("SearchBtn", Blueprint2Wind); }
    Dropdown Dropdown { get => FindChiled<Dropdown>("Dropdown", Blueprint2Wind); }

    private int SelectBtnIndex
    {
        get => selectBtnIndex;
        set
        {
            selectBtnIndex = value;

            foreach (var btn in btns)
            {
                btn.GetComponent<Image>().color = Color.gray;
            }

            btns[selectBtnIndex].GetComponent<Image>().color = Color.white;
        }
    }
    private int selectBtnIndex;

    public override void Init()
    {
        base.Init();

        ShopLG.E.Init(this);

        title = FindChiled<TitleUI>("Title");
        title.SetTitle("ショップ");
        title.SetOnClose(() => { Close(); });

        LeftBtn.onClick.AddListener(()=> { ShopLG.E.OnClickLeftBtn(InputField.text, Dropdown.value); });
        RightBtn.onClick.AddListener(()=> { ShopLG.E.OnClickRightBtn(InputField.text, Dropdown.value); });
        SearchBtn.onClick.AddListener(()=> { ShopLG.E.GetBlueprint2Items(InputField.text, Dropdown.value); });
        Dropdown.options.Clear();
        Dropdown.AddOptions(new List<string>
        {
            "ポイントが高い順",
            "ポイントが安い順",
            "登録が新しい順",
            "登録が古い順"
        });
        Dropdown.value = 0;
        Dropdown.onValueChanged.AddListener((value) => 
        {
            ShopLG.E.GetBlueprint2Items(InputField.text, value);
        });

        var btnsParent = FindChiled("BtnGrid");
        btns = new Button[btnsParent.childCount];
        btns[0] = FindChiled<Button>("Button (1)");
        btns[0].onClick.AddListener(() => { ShopLG.E.ShopUIType = ShopUiType.Charge; SelectBtnIndex = 0; });
        btns[1] = FindChiled<Button>("Button (5)");
        btns[1].onClick.AddListener(() => { ShopLG.E.ShopUIType = ShopUiType.Point; SelectBtnIndex = 1; });
        btns[2] = FindChiled<Button>("Button (2)");
        btns[2].onClick.AddListener(() => { ShopLG.E.ShopUIType = ShopUiType.Exchange; SelectBtnIndex = 2; });
        btns[3] = FindChiled<Button>("Button (3)");
        btns[3].onClick.AddListener(() => { ShopLG.E.ShopUIType = ShopUiType.Blueprint; SelectBtnIndex = 3; });
        btns[4] = FindChiled<Button>("Button (4)");
        btns[4].onClick.AddListener(() => { ShopLG.E.ShopUIType = ShopUiType.Blueprint2; SelectBtnIndex = 4; });

        ShopLG.E.ShopUIType = ShopUiType.Charge;

        var chargeBtnParent = FindChiled("Grid", ChageWind.gameObject);
        chargeBtns = new ShopItemCell[chargeBtnParent.childCount];
        for (int i = 0; i < chargeBtnParent.childCount; i++)
        {
            chargeBtns[i] = chargeBtnParent.GetChild(i).GetComponent<ShopItemCell>();
        }

        chargeBtns[0].Init(ConfigMng.E.Shop[1]);
        chargeBtns[1].Init(ConfigMng.E.Shop[2]);
        chargeBtns[2].Init(ConfigMng.E.Shop[3]);
        chargeBtns[3].Init(ConfigMng.E.Shop[4]);
        chargeBtns[4].Init(ConfigMng.E.Shop[5]);
        //chargeBtns[5].Init(ConfigMng.E.Shop[6]);
        //chargeBtns[6].Init(ConfigMng.E.Shop[7]);
        //chargeBtns[7].Init(ConfigMng.E.Shop[8]);

        SelectBtnIndex = 0;
    }

    public override void Open()
    {
        base.Open();
        RefreshCoins();

        ShopLG.E.SelectPage = 1;
        InputField.text = "";
        //SelectBtnIndex = 0;
        //ShopLG.E.ShopUIType = ShopUiType.Charge;

        NWMng.E.GetCoins((rp) =>
        {
            DataMng.GetCoins(rp);
            RefreshCoins();
        });
    }

    public void SetTitle2(string msg)
    {
        Title2.text = msg;
    }
    public void SetPageText(string v)
    {
        Page.text = v;
    }

    public void ChangeUIType(ShopUiType uiType)
    {
        ChageWind.gameObject.SetActive(uiType == ShopUiType.Charge);
        ItemsWind.gameObject.SetActive(uiType == ShopUiType.Exchange 
            || uiType == ShopUiType.Blueprint
            || uiType == ShopUiType.Point);
        Blueprint2Wind.gameObject.SetActive(uiType == ShopUiType.Blueprint2);
    }

    public void RefreshCoins()
    {
        title.RefreshCoins();
    }
    public void RefreshItems(ShopUiType type)
    {
        ClearCell(itemGridRoot);
        ClearCell(itemGridRoot2);

        if (type == ShopUiType.Blueprint2)
        {
            ShopLG.E.GetBlueprint2Items(InputField.text, Dropdown.value);
        }
        else
        {
            foreach (var item in ConfigMng.E.Shop.Values)
            {
                if (item.Type == 1)
                    continue;

                if (item.Type == (int)type)
                {
                    var cell = AddCell<ShopItemCell>("Prefabs/UI/ShopItem", itemGridRoot);
                    cell.Init(item);
                }
            }
        }
    }
    public void RefreshBlueprint2(List<MyShopBlueprintData> items)
    {
        ClearCell(itemGridRoot2);
        if (items != null)
        {
            foreach (var item in items)
            {
                var cell = AddCell<ShopMyShopItemCell>("Prefabs/UI/ShopMyShopItem", itemGridRoot2);
                cell.Set(item);
            }
        }
    }

    public void GrantCredits(string credits)
    {
        IAPMng.E.OnPurchaseClicked(credits);
    }
}
