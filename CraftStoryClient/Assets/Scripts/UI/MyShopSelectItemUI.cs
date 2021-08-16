using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyShopSelectItemUI : UIBase
{
    Transform itemGridRoot { get => FindChiled("Content"); }
    Button OKBtn { get => FindChiled<Button>("OKBtn"); }
    Button CancelBtn { get => FindChiled<Button>("CancelBtn"); }

    public override void Init()
    {
        base.Init();
        MyShopSelectItemLG.E.Init(this);

        OKBtn.onClick.AddListener(()=> 
        {
            if (string.IsNullOrEmpty(DataMng.E.RuntimeData.NickName))
            {
                CommonFunction.ShowHintBar(9);
                return;
            }

            if (MyShopSelectItemLG.E.SelectItem != null && MyShopSelectItemLG.E.SelectItem.ItemData != null)
            {
                var ui = UICtl.E.OpenUI<MyShopUploadUI>(UIType.MyShopUpload);
                ui.SetItemData(MyShopSelectItemLG.E.SelectItem.ItemData, MyShopSelectItemLG.E.Index);
            }
            
            Close();

            GuideLG.E.Next();
        });
        CancelBtn.onClick.AddListener(Close);
    }
    public override void Open()
    {
        base.Open();
        RefreshItems();
    }
    public override void Close()
    {
        base.Close();
        ClearCell(itemGridRoot);
        MyShopSelectItemLG.E.SelectItem = null;
        MyShopSelectItemLG.E.Index = -1;
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
        var cell = AddCell<MyShopSelectItemCell>("Prefabs/UI/IconItem", itemGridRoot);
        if (cell != null)
        {
            cell.Init(item);
        }
    }
}
