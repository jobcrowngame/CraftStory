using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagUI : UIBase
{
    TitleUI Title;
    Transform btnsParent { get => FindChiled("Btns"); }
    MyButton[] btns;
    Transform itemGridRoot { get => FindChiled("Content"); }
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
        Title = FindChiled<TitleUI>("Title");
        Title.SetTitle("もちもの");
        Title.SetOnClose(() => { Close(); GuideLG.E.Next(); });
        Title.EnActiveCoin(1);
        Title.EnActiveCoin(3);

        btns = new MyButton[btnsParent.childCount];
        for (int i = 0; i < btnsParent.childCount; i++)
        {
            btns[i] = btnsParent.GetChild(i).GetComponent<MyButton>();
            btns[i].Index = i;
            btns[i].AddClickListener((index)=> { BagLG.E.OnClickClassificationBtn(index);  });
        }

        var SelectItemBar = FindChiled("SelectItemBar");
        if (SelectItemBar.childCount == 6)
        {
            for (int i = 0; i < 6; i++)
            {
                selectItems[i] = SelectItemBar.GetChild(i).gameObject.AddComponent<BagSelectItem>();
                selectItems[i].Index = i;
            }
        }

        Title.RefreshCoins();
    }

    public override void Open()
    {
        base.Open();

        Logger.Log("Bag Open");

        BagLG.E.Classification = BagLG.BagClassification.All;

        NWMng.E.GetItems(() =>
        {
            RefreshItems();
            RefreshSelectItemBtns();

            GuideLG.E.Next();
        });
        NWMng.E.GetCoins((rp) =>
        {
            DataMng.GetCoins(rp);
            Title.RefreshCoins();
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

        DataMng.E.Items.Sort((a, b) => a.itemId - b.itemId);

        if (BagLG.E.Classification == BagLG.BagClassification.All)
        {
            foreach (var item in DataMng.E.Items)
            {
                AddItem(item);
            }
        }
        else
        {
            foreach (var item in DataMng.E.Items)
            {
                if ((BagLG.BagClassification)item.Config().BagType == BagLG.E.Classification)
                {
                    AddItem(item);
                }
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
            cell.name = cell.ItemData.id.ToString();

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
    public void ChangeSelectBtn(int index)
    {
        foreach (var item in btns)
        {
            item.IsSelected(false);
        }

        btns[index].IsSelected(true);
    }
}