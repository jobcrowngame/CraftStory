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

    public override void Init()
    {
        base.Init();

        BagLG.E.Init(this);
        cellDic = new Dictionary<string, ItemCell>();

        InitUI();
        RefreshMoney();
    }

    private void InitUI()
    {
        moneyText = FindChiled<Text>("Money");
        itemGridRoot = FindChiled("Content");

        closeBtn = FindChiled<Button>("CloseBtn");
        closeBtn.onClick.AddListener(()=> { Close(); });
    }

    public override void Open()
    {
        base.Open();

        BagLG.E.GetItemList();
        RefreshMoney();
    }
    public override void Close()
    {
        base.Close();

        ClearCell(itemGridRoot);
    }

    public void AddItems(List<EzItemSet> itemList)
    {
        foreach (var item in itemList)
        {
            AddItem(item);
        }
    }
    private void AddItem(EzItemSet item)
    {
        var cell = AddCell<ItemCell>("Prefabs/UI/Item", itemGridRoot);
        if (cell != null)
        {
            cell.Init();
            cell.Add(item);

            if (!cellDic.ContainsKey(item.Name))
            {
                cellDic[item.Name] = cell;
            }
        }
    }

    private void RefreshMoney()
    {
        BagLG.E.GetMoney(0);
    }
    public void RefreshMoneyResponse(EzWallet item)
    {
        moneyText.text = (item.Free + item.Paid).ToString();
    }

    public void DecreaseRespones(List<EzItemSet> r)
    {
        foreach (var i in r)
        {
            if (cellDic.ContainsKey(i.Name))
            {
                cellDic[i.Name].Refresh(i);
            }
        }
    }
}