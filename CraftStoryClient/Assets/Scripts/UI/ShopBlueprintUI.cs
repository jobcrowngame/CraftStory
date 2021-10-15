using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopBlueprintUI : UIBase
{
    Title2UI Title { get => FindChiled<Title2UI>("Title2"); }
    MyToggleGroupCtl ToggleBtns { get => FindChiled<MyToggleGroupCtl>("ToggleBtns"); }
    Text Des { get => FindChiled<Text>("Des"); }
    Button BackBtn { get => FindChiled<Button>("BackBtn"); }

    Transform MyshopBlueprint { get => FindChiled("BlueprintList"); }
    Transform MyshopBlueprintParent { get => FindChiled("Cells", MyshopBlueprint.gameObject); }
    Text Page { get => FindChiled<Text>("Page", MyshopBlueprint); }
    InputField InputField { get => FindChiled<InputField>("InputField", MyshopBlueprint); }
    Button LeftBtn { get => FindChiled<Button>("LeftBtn", MyshopBlueprint); }
    Button RightBtn { get => FindChiled<Button>("RightBtn", MyshopBlueprint); }
    Button SearchBtn { get => FindChiled<Button>("SearchBtn", MyshopBlueprint); }
    Dropdown Dropdown { get => FindChiled<Dropdown>("Dropdown", MyshopBlueprint); }

    Transform BlueprintItem { get => FindChiled("BlueprintItem"); }
    Transform BlueprintItemParent { get => FindChiled("Parent", BlueprintItem.gameObject); }

    Transform MyShop { get => FindChiled("MyShop"); }
    ShopBlueprintMyShopCell[] myShopCells;

    private int maxCount;

    public override void Init()
    {
        base.Init();
        ShopBlueprintLG.E.Init(this);

        Title.Init();
        Title.ShowCoin(1);
        Title.ShowCoin(3);

        ToggleBtns.Init();
        ToggleBtns.OnValueChangeAddListener((index) => 
        { 
            ShopBlueprintLG.E.Type = (ShopBlueprintLG.UIType)index;
            GuideLG.E.Next();
        });
        Des.text = ConfigMng.E.MText[4].Text;
        BackBtn.onClick.AddListener(()=> 
        {
            Close();
            GuideLG.E.Next();
        });

        LeftBtn.onClick.AddListener(() => { ShopBlueprintLG.E.OnClickLeftBtn(InputField.text, Dropdown.value); });
        RightBtn.onClick.AddListener(() => { ShopBlueprintLG.E.OnClickRightBtn(InputField.text, Dropdown.value); });
        SearchBtn.onClick.AddListener(() => { ShopBlueprintLG.E.RefreshMyShopBlueprint(InputField.text, Dropdown.value); });
        Dropdown.options.Clear();
        Dropdown.AddOptions(new List<string>
        {
            "�|�C���g��������",
            "�|�C���g��������",
            "�o�^���V������",
            "�o�^���Â���",
            "�w��������",
            "���[�U�[�����ː�������",
        });
        Dropdown.onValueChanged.AddListener((value) =>
        {
            ShopBlueprintLG.E.RefreshMyShopBlueprint(InputField.text, value);
        });

        myShopCells = new ShopBlueprintMyShopCell[MyShop.childCount];
        for (int i = 0; i < MyShop.childCount; i++)
        {
            myShopCells[i] = MyShop.GetChild(i).gameObject.AddComponent<ShopBlueprintMyShopCell>();
        }
    }

    public override void Open()
    {
        base.Open();

        // �J���ꍇ�A�ŐV�̏��Ń\�[�g
        //ShopBlueprintLG.E.Type = ShopBlueprintLG.UIType.Blueprint1;
        //ToggleBtns.SetValue(0);

        Title.RefreshCoins();

        GuideLG.E.Next();

        Dropdown.value = 2;
    }

    public void RefreshItemWindow(ShopBlueprintLG.UIType type)
    {
        MyshopBlueprint.gameObject.SetActive(type == ShopBlueprintLG.UIType.Blueprint1);
        BlueprintItem.gameObject.SetActive(type == ShopBlueprintLG.UIType.Blueprint2);
        MyShop.gameObject.SetActive(type == ShopBlueprintLG.UIType.MyShop);

        if (type == ShopBlueprintLG.UIType.Blueprint1)
        {
            ShopBlueprintLG.E.RefreshMyShopBlueprint(InputField.text, Dropdown.value);
        }
        else if (type == ShopBlueprintLG.UIType.Blueprint2)
        {
            RefreshBlueprint2();
        }
        else
        {
            RefreshMyShopWindow();
        }
    }

    /// <summary>
    /// �y�[�W�����X�V
    /// </summary>
    /// <param name="page">�y�[�W</param>
    public void SetMyShopPageText(int page)
    {
        Page.text = page.ToString();
    }
    /// <summary>
    /// ���[�U�[�̐݌v�}
    /// </summary>
    public void RefreshBlueprint1(List<MyShopItem> items, int maxCount)
    {
        this.maxCount = maxCount;

        // ����7�������̏ꍇ�A�E���{�^����enactive
        RightBtn.enabled = (items != null && 
            (ShopBlueprintLG.E.SelectMyShopPage - 1) * 7 + items.Count < maxCount) ? 
            true : false;

        RightBtn.image.color = (items != null &&
            (ShopBlueprintLG.E.SelectMyShopPage - 1) * 7 + items.Count < maxCount) ?
             Color.white : Color.gray;

        // �y�[�W���P�̏ꍇ�A�����{�^����enactive
        LeftBtn.enabled = ShopBlueprintLG.E.SelectMyShopPage == 1 ? false : true;
        LeftBtn.image.color = ShopBlueprintLG.E.SelectMyShopPage == 1 ? Color.gray : Color.white;

        for (int i = 0; i < MyshopBlueprintParent.childCount; i++)
        {
            var cell = MyshopBlueprintParent.GetChild(i).GetComponent<ShopBlueprintUserCell>();
            if (cell != null)
            {
                if (items == null || i >= items.Count)
                {
                    cell.Set(new MyShopItem());
                }
                else
                {
                    cell.Set(items[i]);
                }
            }
        }
    }
    /// <summary>
    /// �����̐݌v�}
    /// </summary>
    private void RefreshBlueprint2()
    {
        ClearCell(BlueprintItemParent);

        foreach (var item in ConfigMng.E.Shop.Values)
        {
            if (item.Type == 5)
            {
                var cell = AddCell<ShopBlueprintSystemCell>("Prefabs/UI/ShopBlueprintSystemCell", BlueprintItemParent);
                cell.Init(item);
            }
        }
    }
    
    /// <summary>
    /// �}�C�V���b�v�X�V
    /// </summary>
    public void RefreshMyShopWindow()
    {
        for (int i = 0; i < myShopCells.Length; i++)
        {
            myShopCells[i].Init(i + 1);
        }
    }

    /// <summary>
    /// �����ː��X�V
    /// </summary>
    /// <param name="targetAcc">�݌v�}��l</param>
    public void RefreshGoodNum(string targetAcc)
    {
        for (int i = 0; i < MyshopBlueprintParent.childCount; i++)
        {
            var cell = MyshopBlueprintParent.GetChild(i).GetComponent<ShopBlueprintUserCell>();
            if (cell != null && cell.TargetAcc == targetAcc)
            {
                cell.GoodNumberAdd();
            }
        }
    }

    /// <summary>
    /// �R�C�����X�V
    /// </summary>
    public void RefreshCoin()
    {
        Title.RefreshCoins();
    }
}
