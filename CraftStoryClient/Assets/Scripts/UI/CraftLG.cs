﻿
using JsonConfigData;
using static BagLG;

public class CraftLG : UILogicBase<CraftLG, CraftUI>
{
    public int SelectCount
    {
        get => selectCount;
        set
        {
            selectCount = value;

            UI.SetSelectCountText(value.ToString());
        }
    }
    int selectCount = 1;


    public Craft SelectCraft
    {
        get => selectCraft;
        set
        {
            selectCraft = value;

            if (value != null)
            {
                UI.ActiveSlectCountBtns();
                UI.Explanatory.text = ConfigMng.E.Item[selectCraft.ItemID].Explanatory;
            }
            else
            {
                UI.Explanatory.text = UI.ExplanatoryNoSelect;
            }
        }
    }
    Craft selectCraft;

    public CraftItemCell SelectCraftItemCell { get; set; }


    public void OnClickAdd()
    {
        if (selectCount < 99)
        {
            SelectCount++;
            UI.RefreshCost();
        }
    }
    public void OnClickAdd10()
    {
        if (selectCount < 99)
        {
            SelectCount += 10;

            if (SelectCount >= 100)
                SelectCount = 99;

            UI.RefreshCost();
        }
    }
    public void OnClickRemove()
    {
        if (selectCount > 1)
        {
            SelectCount--;
            UI.RefreshCost();
        }
    }
    public void OnClickRemove10()
    {
        if (selectCount > 1)
        {
            SelectCount -= 10;

            if (SelectCount < 1)
                SelectCount = 1;

            UI.RefreshCost();
        }
    }



    /// <summary>
    /// 持ち物の分類
    /// </summary>
    public BagClassification Classification
    {
        get => classification;
        set
        {
            if (classification == value)
                return;

            classification = value;
            UI.SetType();
            UI.ChangeSelectBtn((int)value);
        }
    }
    private BagClassification classification = BagClassification.None;

    /// <summary>
    /// 分類が変更イベント
    /// </summary>
    /// <param name="index"></param>
    public void OnClickClassificationBtn(int index)
    {
        Classification = (BagClassification)index;
    }
}
