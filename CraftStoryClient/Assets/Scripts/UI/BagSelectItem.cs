﻿using Newtonsoft.Json;
using System.Collections.Generic;
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

    public void SetItem(ItemData itemData)
    {
        Icon.sprite = itemData == null
            ? null
            : ReadResources<Sprite>(itemData.Config().IconResourcesPath);
    }

    public void Refresh()
    {
        itemData = DataMng.E.GetItemByEquipedSite(Index + 1);

        Icon.sprite = itemData == null
            ? null
            : ReadResources<Sprite>(itemData.Config().IconResourcesPath);
    }

    private void OnClickSelectItem()
    {
        if (BagLG.E.SelectItem == null)
            return;
        NWMng.E.EquitItem((rp) => 
        {
            NWMng.E.GetItemList((rp2) =>
            {
                DataMng.E.Items = JsonConvert.DeserializeObject<List<ItemData>>(rp2[0]);

                HomeLG.E.UI.RefreshItemBtns();
                BagLG.E.UI.RefreshSelectItemBtns();
                BagLG.E.SelectItem = null;
            });
        }, BagLG.E.SelectItem.ItemData.id, Index + 1);
    }
}
