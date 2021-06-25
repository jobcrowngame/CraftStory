﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagSelectItem : UIBase
{
    Image Icon;
    public int Index { get; set; }

    private ItemData itemData;

    private void Awake()
    {
        Icon = transform.GetComponent<Image>();
        transform.GetComponent<Button>().onClick.AddListener(OnClickSelectItem);
    }

    public void Refresh()
    {
        itemData = DataMng.E.GetItemByEquipedSite(Index + 1);

        Icon.sprite = itemData == null
            ? ReadResources<Sprite>("Textures/icon_noimg")
            : ReadResources<Sprite>(itemData.Config().IconResourcesPath);
    }

    private void OnClickSelectItem()
    {
        if (BagLG.E.SelectItem == null)
            return;

        if (itemData == null)
        {
            NWMng.E.EquitItem((rp) =>
            {
                NWMng.E.GetItemList((rp2) =>
                {
                    DataMng.GetItems(rp2[0]);
                    if (BagLG.E.UI != null) BagLG.E.UI.RefreshSelectItemBtns();

                    PlayerCtl.E.ChangeSelectItem(BagLG.E.SelectItem.ItemData);
                    BagLG.E.SelectItem = null;
                });
            }, BagLG.E.SelectItem.ItemData.id, Index + 1);
        }
        else
        {
            NWMng.E.EquitItem((rp) =>
            {
                NWMng.E.EquitItem((rp2) =>
                {
                    NWMng.E.GetItemList((rp3) => 
                    {
                        DataMng.GetItems(rp3[0]);
                        if (BagLG.E.UI != null) BagLG.E.UI.RefreshSelectItemBtns();

                        PlayerCtl.E.ChangeSelectItem(BagLG.E.SelectItem.ItemData);
                        BagLG.E.SelectItem = null;
                    });
                }, BagLG.E.SelectItem.ItemData.id, Index + 1);
            }, itemData.id, 0);
        }

        PlayerCtl.E.ChangeSelectItem(null);
    }
}
