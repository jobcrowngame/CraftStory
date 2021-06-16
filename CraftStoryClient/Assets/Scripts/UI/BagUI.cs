using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Gs2.Unity.Gs2Inventory.Model;
using Gs2.Unity.Gs2Money.Model;

public class BagUI : UIBase
{
    Text moneyText;
    Transform itemGridRoot;
    Button closeBtn;
    Dictionary<string, ItemCell> cellDic;
    BagSelectItem[] selectItems;

    public override void Init()
    {
        base.Init();

        BagLG.E.Init(this);
        cellDic = new Dictionary<string, ItemCell>();
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
                selectItems[i].index = i;
            }
        }
    }

    public override void Open()
    {
        base.Open();

        RefreshItems();
        RefreshSelectItemBtns();
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
        var cell = AddCell<ItemCell>("Prefabs/UI/IconItem", itemGridRoot);
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
        for (int i = 0; i < selectItems.Length; i++)
        {
            selectItems[i].SetItem(null);
        }

        for (int i = 0; i < DataMng.E.Items.Count; i++)
        {
            if (DataMng.E.Items[i].equipSite > 0)
            {
                selectItems[DataMng.E.Items[i].equipSite - 1].SetItem(DataMng.E.Items[i]);
            }
        }
    }
    public void RefreshSelectItemBtns(int index, ItemData itemData)
    {
        for (int i = 0; i < selectItems.Length; i++)
        {
            selectItems[i].SetItem(null);
        }

        selectItems[index].SetItem(itemData);
    }
}