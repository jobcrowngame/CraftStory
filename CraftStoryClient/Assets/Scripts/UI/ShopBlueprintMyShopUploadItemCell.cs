using System;
using UnityEngine;
using UnityEngine.UI;

public class ShopBlueprintMyShopUploadItemCell : UIBase
{
    Text itemName { get => FindChiled<Text>("Name"); }
    Text itemCount { get => FindChiled<Text>("Count"); }
    Image Icon { get => FindChiled<Image>("Icon"); }
    Button clickBtn { get => transform.GetComponent<Button>(); }
    Transform selected { get => FindChiled("Select"); }
    Transform Lock { get => FindChiled("Lock"); }

    private ItemData itemData;
    public ItemData ItemData { get => itemData; }

    public void Init(ItemData itemData)
    {
        this.itemData = itemData;

        clickBtn.onClick.AddListener(() => 
        {
            ShopBlueprintMyShopSelectItemLG.E.SelectItem = this;

            GuideLG.E.Next();
        });

        Refresh();
    }

    public void Refresh()
    {
        if (itemData == null)
            return;

        itemName.text = string.IsNullOrEmpty(ItemData.newName)
            ? itemData.Config().Name
            : itemData.newName;
        Icon.sprite = ReadResources<Sprite>(itemData.Config().IconResourcesPath);
        itemCount.text = "x" + itemData.count;

        Lock.gameObject.SetActive(itemData.islocked == 1);
    }
    public void IsSelected(bool b)
    {
        selected.gameObject.SetActive(b);
    }
}
