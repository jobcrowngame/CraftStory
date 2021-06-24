using JsonConfigData;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftUI : UIBase
{
    Transform craftItemParent;
    Text SelectCount;
    Button CloseBtn;
    Button CraftBtn;
    Button RightBtn;
    Button LeftBtn;
    CraftCostCell[] costCells;

    int selectCount = 1;
    Craft selectCraft;

    private ItemType itemType;

    public override void Init()
    {
        base.Init();

        CraftLG.E.Init(this);

        costCells = new CraftCostCell[4];

        craftItemParent = FindChiled("Content");

        var selectParent = FindChiled("SelectParent");
        SelectCount = FindChiled<Text>("SelectCount", selectParent);
        SelectCount.text = selectCount.ToString();

        RightBtn = FindChiled<Button>("RightBtn");
        RightBtn.onClick.AddListener(AddCount);

        LeftBtn = FindChiled<Button>("LeftBtn");
        LeftBtn.onClick.AddListener(RemoveCount);

        CloseBtn = FindChiled<Button>("CloseBtn");
        CloseBtn.onClick.AddListener(() => { Close(); });

        CraftBtn = FindChiled<Button>("CraftBtn");
        CraftBtn.onClick.AddListener(OnCraft);

        var costCellParent = FindChiled("CostList");
        for (int i = 0; i < costCellParent.childCount; i++)
        {
            costCells[i] = costCellParent.GetChild(i).gameObject.AddComponent<CraftCostCell>();
        }
    }

    public void SetType(ItemType type)
    {
        ClearCell(craftItemParent);

        selectCount = 1;
        SelectCount.text = selectCount.ToString();

        selectCraft = null;
        RefreshCost(null);

        itemType = type;
        RefreshCraftItemList();
    }

    private void RefreshCraftItemList()
    {
        foreach (Craft item in ConfigMng.E.Craft.Values)
        {
            if (item.Type == (int)itemType)
            {
                AddCraftItem(item);
            }
        }
    }
    private void AddCraftItem(Craft config)
    {
        var cell = AddCell<CraftItemCell>("Prefabs/UI/IconItem", craftItemParent);
        if (cell != null)
        {
            cell.Init(config);
        }
    }

    private void AddCount()
    {
        if (selectCount < 100)
        {
            selectCount++;
            SelectCount.text = selectCount.ToString();
            RefreshCost(selectCraft);
        }
    }
    private void RemoveCount()
    {
        if (selectCount > 1)
        {
            selectCount--;
            SelectCount.text = selectCount.ToString();
            RefreshCost(selectCraft);
        }
    }
    private void OnCraft()
    {
        if (selectCraft == null)
        { 
            CommonFunction.ShowHintBar(2);
            return;
        }

        if (!CanCreate(selectCraft, selectCount))
        {
            CommonFunction.ShowHintBar(1);
            return;
        }
        else
        {
            NWMng.E.Craft((rp) => 
            {
                NWMng.E.GetItemList((rp2) =>
                {
                    DataMng.GetItems(rp2[0]);
                });
            }, selectCraft, selectCount);
        }
    }

    public void RefreshCost(Craft config)
    {
        selectCraft = config;

        if (config == null)
        {
            for (int i = 0; i < costCells.Length; i++)
            {
                costCells[i].SetInfo(-1, 0, 0);
            }
        }
        else
        {
            costCells[0].SetInfo(config.Cost1, config.Cost1Count, selectCount);
            costCells[1].SetInfo(config.Cost2, config.Cost2Count, selectCount);
            costCells[2].SetInfo(config.Cost3, config.Cost3Count, selectCount);
            costCells[3].SetInfo(config.Cost4, config.Cost4Count, selectCount);
        }
    }

    public bool CanCreate(Craft config, int count)
    {
        if (config == null)
            return false;

        bool ret = true;

        if (ret && config.Cost1 > 0) ret = DataMng.E.GetItemCountByItemID(config.Cost1) >= count;
        if (ret && config.Cost2 > 0) ret = DataMng.E.GetItemCountByItemID(config.Cost2) >= count;
        if (ret && config.Cost3 > 0) ret = DataMng.E.GetItemCountByItemID(config.Cost3) >= count;
        if (ret && config.Cost4 > 0) ret = DataMng.E.GetItemCountByItemID(config.Cost4) >= count;

        return ret;
    }
}
