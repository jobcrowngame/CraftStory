using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagUI : UIBase
{
    Text moneyText;
    Transform itemGridRoot;
    Button closeBtn;
    Dictionary<string, BagItemCell> cellDic;
    BagSelectItem[] selectItems;

    public override void Init()
    {
        base.Init();

        BagLG.E.Init(this);
        cellDic = new Dictionary<string, BagItemCell>();
        selectItems = new BagSelectItem[6];

        InitUI();
    }

    private void InitUI()
    {
        moneyText = FindChiled<Text>("Money");
        itemGridRoot = FindChiled("Content");

        closeBtn = FindChiled<Button>("CloseBtn");
        closeBtn.onClick.AddListener(()=> { Close(); });

        var SelectItemBar = FindChiled("SelectItemBar");
        if (SelectItemBar.childCount == 6)
        {
            for (int i = 0; i < 6; i++)
            {
                selectItems[i] = SelectItemBar.GetChild(i).gameObject.AddComponent<BagSelectItem>();
                selectItems[i].Index = i;
            }
        }
    }

    public override void Open()
    {
        base.Open();

        Debug.Log("Bag Open");

        NWMng.E.GetItemList((rp) =>
        {
            ConfigMng.JsonToItemList(rp[0]);

            RefreshItems();
            RefreshSelectItemBtns();
        });
    }
    public override void Close()
    {
        base.Close();

        ClearCell(itemGridRoot);
    }

    public void RefreshItems()
    {
        ClearCell(itemGridRoot);

        if (DataMng.E.Items == null)
            return;

        foreach (var item in DataMng.E.Items)
        {
            AddItem(item);
        }
    }
    public void RefreshItemByGuid(int guid)
    {
        foreach (var item in cellDic)
        {
            if (item.Value.ItemData.id == guid)
            {
                item.Value.Refresh(item.Value.ItemData);
            }
        }
    }
    private void AddItem(ItemData item)
    {
        var cell = AddCell<BagItemCell>("Prefabs/UI/IconItem", itemGridRoot);
        if (cell != null)
        {
            cell.Init();
            cell.Add(item);

            if (!cellDic.ContainsKey(item.Config().Name))
            {
                cellDic[item.Config().Name] = cell;
            }
        }
    }

    public void RefreshSelectItemBtns()
    {
        foreach (var item in selectItems)
        {
            item.Refresh();
        }
    }
}