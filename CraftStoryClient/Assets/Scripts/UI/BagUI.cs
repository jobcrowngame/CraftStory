using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagUI : UIBase
{
    TitleUI title;
    Transform itemGridRoot;
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
        title = FindChiled<TitleUI>("Title");
        title.SetTitle("もちもの");
        title.SetOnClose(() => { Close(); });
        title.EnActiveCoin(2);
        title.EnActiveCoin(3);

        itemGridRoot = FindChiled("Content");

        var SelectItemBar = FindChiled("SelectItemBar");
        if (SelectItemBar.childCount == 6)
        {
            for (int i = 0; i < 6; i++)
            {
                selectItems[i] = SelectItemBar.GetChild(i).gameObject.AddComponent<BagSelectItem>();
                selectItems[i].Index = i;
            }
        }

        title.RefreshCoins();
    }

    public override void Open()
    {
        base.Open();

        Debug.Log("Bag Open");

        NWMng.E.GetItemList((rp) =>
        {
            DataMng.GetItems(rp[0]);

            RefreshItems();
            RefreshSelectItemBtns();
        });
        NWMng.E.GetCoins((rp) =>
        {
            DataMng.GetCoins(rp[0]);
            title.RefreshCoins();
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