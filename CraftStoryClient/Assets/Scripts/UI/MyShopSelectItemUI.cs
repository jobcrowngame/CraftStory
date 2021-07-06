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
            if (string.IsNullOrEmpty(DataMng.E.UserData.NickName))
            {
                CommonFunction.ShowHintBar(9);
                return;
            }

            if (MyShopSelectItemLG.E.SelectItem.ItemData != null)
            {
                NWMng.E.UploadBlueprintToMyShop((rp) =>
                {
                    DataMng.E.ConsumableItemByGUID((int)rp["itemGuid"]);

                    if (!string.IsNullOrEmpty(rp["myShopItems"].ToString()))
                    {
                        List<MyShopItem> shopItems = JsonMapper.ToObject<List<MyShopItem>>(rp["myShopItems"].ToJson());
                        for (int i = 0; i < shopItems.Count; i++)
                        {
                            DataMng.E.RuntimeData.MyShop.myShopItem[i] = shopItems[i];
                        }
                    }

                    MyShopLG.E.UI.RefreshUI();
                }, MyShopSelectItemLG.E.SelectItem.ItemData.id, MyShopSelectItemLG.E.Index);
            }
            
            Close();
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
            if (item.Config().Type == (int)ItemType.Blueprint)
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
