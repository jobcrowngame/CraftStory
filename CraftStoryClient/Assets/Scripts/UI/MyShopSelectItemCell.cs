using System;
using UnityEngine;
using UnityEngine.UI;

public class MyShopSelectItemCell : UIBase
{
    Text itemName { get => FindChiled<Text>("Name"); }
    Text itemCount { get => FindChiled<Text>("Count"); }
    Image Icon { get => FindChiled<Image>("Icon"); }
    Button clickBtn { get => transform.GetComponent<Button>(); }
    Transform selected { get => FindChiled("Select"); }

    private ItemData itemData;
    public ItemData ItemData { get => itemData; }

    public void Init(ItemData itemData)
    {
        this.itemData = itemData;

        clickBtn.onClick.AddListener(() => 
        {
            MyShopSelectItemLG.E.SelectItem = this;
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
    }
    public void IsSelected(bool b)
    {
        selected.gameObject.SetActive(b);
    }
}
