using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagItemCell : UIBase
{
    Text itemName;
    Text itemCount;
    Image Icon;
    Button clickBtn;
    Transform selected;
    Transform Lock { get => FindChiled("Lock"); }

    private ItemData itemData;
    public ItemData ItemData { get => itemData; }

    public void Add(ItemData itemData)
    {
        this.itemData = itemData;

        itemName = FindChiled<Text>("Name");
        itemCount = FindChiled<Text>("Count");
        Icon = FindChiled<Image>("Icon");
        selected = FindChiled("Select");

        clickBtn = transform.GetComponent<Button>();
        clickBtn.onClick.AddListener(() => 
        { 
            BagLG.E.SelectItem = this;
            GuideLG.E.Next();
        });

        Lock.gameObject.SetActive(itemData.islocked == 1);

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