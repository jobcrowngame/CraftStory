

public class MyShopSelectItemLG : UILogicBase<MyShopSelectItemLG, MyShopSelectItemUI>
{
    public MyShopSelectItemCell SelectItem
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
    private MyShopSelectItemCell selectItem;

    public int Index { get; set; }
}