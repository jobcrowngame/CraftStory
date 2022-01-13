using JsonConfigData;
using System.Collections;
using System.Collections.Generic;
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
    public MyText Explanatory { get => FindChiled<MyText>("Explanatory"); }
    public MyText FoodEffect { get => FindChiled<MyText>("FoodEffect"); }
    CraftCostCell[] costCells;
    ScrollRect ScrollViewRect { get => FindChiled<ScrollRect>("Scroll View"); }
    Transform btnsParent { get => FindChiled("Btns"); }
    MyButton[] btns;

    public readonly string ExplanatoryNoSelect = "アイテムをタップすると説明が表示されます";

    private EntityType entityType;

    public override void Init()
    {
        base.Init();

        CraftLG.E.Init(this);


        RightBtn.onClick.AddListener(CraftLG.E.OnClickAdd);
        RightBtn10.onClick.AddListener(CraftLG.E.OnClickAdd10);
        LeftBtn.onClick.AddListener(CraftLG.E.OnClickRemove);
        LeftBtn10.onClick.AddListener(CraftLG.E.OnClickRemove10);
        CloseBtn.onClick.AddListener(() => { Close(); GuideLG.E.Next(); });
        CraftBtn.onClick.AddListener(OnCraft);

        var costCellParent = FindChiled("CostList");
        costCells = new CraftCostCell[costCellParent.childCount];
        for (int i = 0; i < costCellParent.childCount; i++)
        {
            costCells[i] = costCellParent.GetChild(i).gameObject.AddComponent<CraftCostCell>();
        }

        btns = new MyButton[btnsParent.childCount];
        for (int i = 0; i < btnsParent.childCount; i++)
        {
            btns[i] = btnsParent.GetChild(i).GetComponent<MyButton>();
            btns[i].Index = i;
            btns[i].AddClickListener((index) => { CraftLG.E.OnClickClassificationBtn(index); });
        }
    }
    public override void Open()
    {
        base.Open();

        CraftLG.E.Classification = BagLG.BagClassification.All;

        ActiveSlectCountBtns(false);

        Explanatory.text = ExplanatoryNoSelect;
        ScrollViewRect.enabled = true;
    }

    public void SetType()
    {
        SetType(entityType);
    }

    public void SetType(EntityType type)
    {
        ClearCell(craftItemParent);

        CraftLG.E.SelectCount = 1;
        CraftLG.E.SelectCraft = null;
        entityType = type;

        RefreshCost();
        RefreshCraftCellList();
    }

    public void SetSelectCountText(string text)
    {
        SelectCount.text = text + "　個";
    }

    /// <summary>
    /// クラフトサブのインスタンス
    /// </summary>
    public void RefreshCraftCellList()
    {
        ClearCell(craftItemParent);

        // おすすめリスト
        List<Craft> Recommendation = new List<Craft>();

        // 他のリスト
        List<Craft> Order = new List<Craft>();

        foreach (Craft item in ConfigMng.E.Craft.Values)
        {
            var itemConf = ConfigMng.E.Item[item.ItemID];

            if (CraftLG.E.Classification != BagLG.BagClassification.All &&
                CraftLG.E.Classification != (BagLG.BagClassification)itemConf.BagType)
                continue;

            if (item.Type == (int)entityType)
            {
                // おすすめの場合
                if (item.Recommendation == 1)
                {
                    // 掲示板の場合、始めに作る以外はおすすめしない
                    if (item.ItemID == 3003 && DataMng.E.UserData.FirstCraftMission == 1)
                    {
                        Order.Add(item);
                        continue;
                    }

                    Recommendation.Add(item);
                }
                else
                {
                    Order.Add(item);
                }
            }
        }

        // おすすめをインスタンス
        foreach (var item in Recommendation)
        {
            AddCell(item);
        }

        // ソート
        Order.Sort((a, b) => a.Sort - b.Sort);

        // 他をインスタンス
        foreach (var item in Order)
        {
            AddCell(item);
        }
    }

    /// <summary>
    /// サブを追加
    /// </summary>
    /// <param name="config"></param>
    private void AddCell(Craft config)
    {
        var cell = AddCell<CraftItemCell>("Prefabs/UI/IconItem", craftItemParent);
        if (cell != null)
        {
            cell.gameObject.name = config.ItemID.ToString();
            cell.Init(config);
        }
    }

    /// <summary>
    /// クラフト開始イベント
    /// </summary>
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
            if (DataMng.E.RuntimeData.MapType != MapType.Guide)
            {
                NWMng.E.Craft((rp) =>
                {
                    // 掲示板を作るタスク完了
                    if (CraftLG.E.SelectCraft.ItemID == 3003)
                        TaskMng.E.AddMainTaskCount(3);

                    // かまどを使うタスク
                    TaskMng.E.AddMainTaskCount(4);


                    if (CraftLG.E.SelectCraft.ItemID == 3003 && DataMng.E.UserData.FirstCraftMission == 0)
                    {
                        DataMng.E.UserData.FirstCraftMission = 1;
                        CraftLG.E.SelectCount = 1;
                        CraftLG.E.SelectCraft = null;

                        RefreshCraftCellList();
                    }

                    NWMng.E.GetItems(() =>
                    {
                        CommonFunction.ShowHintBar(6);
                        RefreshCost();
                    });
                }, CraftLG.E.SelectCraft, CraftLG.E.SelectCount);
            }
            else
            {
                DataMng.E.RemoveItemByItemId(CraftLG.E.SelectCraft.Cost1, CraftLG.E.SelectCraft.Cost1Count);
                DataMng.E.RemoveItemByItemId(CraftLG.E.SelectCraft.Cost2, CraftLG.E.SelectCraft.Cost2Count);
                DataMng.E.RemoveItemByItemId(CraftLG.E.SelectCraft.Cost3, CraftLG.E.SelectCraft.Cost3Count);
                DataMng.E.RemoveItemByItemId(CraftLG.E.SelectCraft.Cost4, CraftLG.E.SelectCraft.Cost4Count);
                GuideLG.E.Next();
            }
        }

        UICtl.E.LockUI();
        createCount = CraftLG.E.SelectCount > 10 ? 10 : CraftLG.E.SelectCount;
        maxTimer = step * createCount;
        timer = 0;
        TimeZoneMng.E.AddTimerEvent02(CloneIcon);
    }

    float timer;
    float maxTimer;
    int createCount;
    float step = 10;

    /// <summary>
    /// AnimationIconをコピー
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    private void CloneIcon()
    {
        if (timer >= maxTimer)
        {
            TimeZoneMng.E.RemoveTimerEvent02(CloneIcon);
            UICtl.E.LockUI(false);
            return;
        }

        if (timer++ % step == 0 && CraftLG.E.SelectCraftItemCell != null)
        {
            CraftLG.E.SelectCraftItemCell.CloneIconToBag();
        }

        timer++;
    }

    /// <summary>
    /// コストリストを更新
    /// </summary>
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

    /// <summary>
    /// 作成できるかのチェック
    /// </summary>
    /// <param name="config">設定ファイル</param>
    /// <param name="count">作成数</param>
    /// <returns></returns>
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

    public void ActiveSlectCountBtns(bool b = true)
    {
        ActiveSlectCountBtn(RightBtn, b);
        ActiveSlectCountBtn(RightBtn10, b);
        ActiveSlectCountBtn(LeftBtn, b);
        ActiveSlectCountBtn(LeftBtn10, b);
    }
    private void ActiveSlectCountBtn(Button btn, bool enable)
    {
        btn.enabled = enable;
        btn.GetComponent<Image>().color = enable ? Color.white : Color.grey;
    }

    public void ChangeSelectBtn(int index)
    {
        foreach (var item in btns)
        {
            item.ColorChange(false);
        }

        btns[index].ColorChange(true);
    }
}
