

public class ShopBlueprintMyShopSelectItemLG : UILogicBase<ShopBlueprintMyShopSelectItemLG, ShopBlueprintMyShopSelectItemUI>
{
    public ShopBlueprintMyShopUploadItemCell SelectItem
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
    private ShopBlueprintMyShopUploadItemCell selectItem;

    public int Index { get; set; }
}