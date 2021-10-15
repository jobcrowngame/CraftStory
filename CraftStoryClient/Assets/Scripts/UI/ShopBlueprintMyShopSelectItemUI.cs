using UnityEngine;
using UnityEngine.UI;

public class ShopBlueprintMyShopSelectItemUI : UIBase
{
    Transform itemGridRoot { get => FindChiled("Content"); }
    Button OKBtn { get => FindChiled<Button>("OKBtn"); }
    Button CancelBtn { get => FindChiled<Button>("CancelBtn"); }

    public override void Init()
    {
        base.Init();
        ShopBlueprintMyShopSelectItemLG.E.Init(this);

        OKBtn.onClick.AddListener((UnityEngine.Events.UnityAction)(()=> 
        {
            if (string.IsNullOrEmpty(DataMng.E.RuntimeData.NickName))
            {
                CommonFunction.ShowHintBar(9);
                return;
            }

            if (ShopBlueprintMyShopSelectItemLG.E.SelectItem == null)
            {
                Close();
                return;
            }
            var ui = UICtl.E.OpenUI<ShopBlueprintMyShopUploadUI>((UIType)UIType.ShopBlueprintMyShopUpload);
            ui.SetItemData(ShopBlueprintMyShopSelectItemLG.E.SelectItem.ItemData, ShopBlueprintMyShopSelectItemLG.E.Index);
            Close();

            GuideLG.E.Next();
        }));
        CancelBtn.onClick.AddListener(Close);
    }
    public override void Open()
    {
        base.Open();
        ClearCell(itemGridRoot);

        RefreshItems();
        //MyShopSelectItemLG.E.SelectItem = null;
    }
    public override void Close()
    {
        base.Close();
        ShopBlueprintMyShopSelectItemLG.E.Index = -1;
    }

    public void RefreshItems()
    {
        ClearCell(itemGridRoot);

        if (DataMng.E.Items == null)
            return;

        foreach (var item in DataMng.E.Items)
        {
            if (item.Config().Type == (int)ItemType.Blueprint && !item.IsLocked)
            {
                AddItem(item);
            }
        }
    }
    private void AddItem(ItemData item)
    {
        var cell = AddCell<ShopBlueprintMyShopUploadItemCell>("Prefabs/UI/IconItem", itemGridRoot);
        if (cell != null)
        {
            cell.Init(item);
        }
    }
}
