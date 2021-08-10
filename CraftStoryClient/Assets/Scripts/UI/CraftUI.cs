using JsonConfigData;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CraftUI : UIBase
{
    Transform craftItemParent { get => FindChiled("Content"); }
    Transform selectParent { get => FindChiled("SelectParent"); }
    Text SelectCount { get => FindChiled<Text>("SelectCount", selectParent); }
    Button CloseBtn { get => FindChiled<Button>("CloseBtn"); }
    Button CraftBtn { get => FindChiled<Button>("CraftBtn"); }
    Button RightBtn { get => FindChiled<Button>("RightBtn"); }
    Button RightBtn10 { get => FindChiled<Button>("RightBtn10"); }
    Button LeftBtn { get => FindChiled<Button>("LeftBtn"); }
    Button LeftBtn10 { get => FindChiled<Button>("LeftBtn10"); }
    CraftCostCell[] costCells;


    private EntityType entityType;

    public override void Init()
    {
        base.Init();

        CraftLG.E.Init(this);


        RightBtn.onClick.AddListener(CraftLG.E.OnClickAdd);
        RightBtn10.onClick.AddListener(CraftLG.E.OnClickAdd10);
        LeftBtn.onClick.AddListener(CraftLG.E.OnClickRemove);
        LeftBtn10.onClick.AddListener(CraftLG.E.OnClickRemove10);
        CloseBtn.onClick.AddListener(() => { Close(); });
        CraftBtn.onClick.AddListener(OnCraft);

        var costCellParent = FindChiled("CostList");
        costCells = new CraftCostCell[costCellParent.childCount];
        for (int i = 0; i < costCellParent.childCount; i++)
        {
            costCells[i] = costCellParent.GetChild(i).gameObject.AddComponent<CraftCostCell>();
        }
    }

    public void SetType(EntityType type)
    {
        ClearCell(craftItemParent);

        CraftLG.E.SelectCount = 1;
        CraftLG.E.SelectCraft = null;
        entityType = type;

        RefreshCost();
        RefreshCraftItemList();
    }
    public void SetSelectCountText(string text)
    {
        SelectCount.text = text;
    }

    private void RefreshCraftItemList()
    {
        foreach (Craft item in ConfigMng.E.Craft.Values)
        {
            if (item.Type == (int)entityType)
            {
                var cell = AddCell<CraftItemCell>("Prefabs/UI/IconItem", craftItemParent);
                if (cell != null)
                {
                    cell.Init(item);
                }
            }
        }
    }
    private void OnCraft()
    {
        if (CraftLG.E.SelectCraft == null)
        { 
            CommonFunction.ShowHintBar(2);
            return;
        }

        if (!CanCreate(CraftLG.E.SelectCraft, CraftLG.E.SelectCount))
        {
            CommonFunction.ShowHintBar(1);
            return;
        }
        else
        {
            NWMng.E.Craft((rp) => 
            {
                NWMng.E.GetItems(() =>
                {
                    CommonFunction.ShowHintBar(6);
                    RefreshCost();
                });
            }, CraftLG.E.SelectCraft, CraftLG.E.SelectCount);
        }

        UICtl.E.LockUI();
        StartCoroutine(CloneIcon(CraftLG.E.SelectCount));
    }

    public void RefreshCost()
    {
        if (CraftLG.E.SelectCraft == null)
        {
            for (int i = 0; i < costCells.Length; i++)
            {
                costCells[i].SetInfo(-1, 0, 0);
            }
        }
        else
        {
            costCells[0].SetInfo(CraftLG.E.SelectCraft.Cost1, CraftLG.E.SelectCraft.Cost1Count, CraftLG.E.SelectCount);
            costCells[1].SetInfo(CraftLG.E.SelectCraft.Cost2, CraftLG.E.SelectCraft.Cost2Count, CraftLG.E.SelectCount);
            costCells[2].SetInfo(CraftLG.E.SelectCraft.Cost3, CraftLG.E.SelectCraft.Cost3Count, CraftLG.E.SelectCount);
            costCells[3].SetInfo(CraftLG.E.SelectCraft.Cost4, CraftLG.E.SelectCraft.Cost4Count, CraftLG.E.SelectCount);
        }
    }

    private bool CanCreate(Craft config, int count)
    {
        if (config == null)
            return false;

        bool ret = true;

        if (ret && config.Cost1 > 0) ret = DataMng.E.GetItemCountByItemID(config.Cost1) >= config.Cost1Count * count;
        if (ret && config.Cost2 > 0) ret = DataMng.E.GetItemCountByItemID(config.Cost2) >= config.Cost2Count * count;
        if (ret && config.Cost3 > 0) ret = DataMng.E.GetItemCountByItemID(config.Cost3) >= config.Cost3Count * count;
        if (ret && config.Cost4 > 0) ret = DataMng.E.GetItemCountByItemID(config.Cost4) >= config.Cost4Count * count;

        return ret;
    }

    private IEnumerator CloneIcon(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (CraftLG.E.SelectCraftItemCell != null)
            {
                CraftLG.E.SelectCraftItemCell.CloneIconToBag();
            }
            yield return new WaitForSeconds(0.1f);
        }
        UICtl.E.LockUI(false);
    }
}
