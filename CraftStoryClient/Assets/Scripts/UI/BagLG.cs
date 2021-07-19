﻿
public class BagLG : UILogicBase<BagLG, BagUI>
{
    public BagItemCell SelectItem 
    {
        get => selectItem;
        set
        {
            if (selectItem != null)
            {
                selectItem.IsSelected(false);
            }

            selectItem = value;
            if (selectItem != null) selectItem.IsSelected(true);
        }
    }
    private BagItemCell selectItem;

    public BagClassification Classification 
    {
        get => classification;
        set
        {
            if (classification == value)
                return;

            classification = value;
            UI.RefreshItems();
            UI.ChangeSelectBtn((int)value);
        }
    }
    private BagClassification classification = BagClassification.None;

    public void OnClickClassificationBtn(int index)
    {
        Classification = (BagClassification)index;
    }

    public enum BagClassification 
    {
        None = -1,
        All = 0,
        Block = 1,
        Material = 2,
        Blueprint = 3,
        Orther = 4,
    }
}
