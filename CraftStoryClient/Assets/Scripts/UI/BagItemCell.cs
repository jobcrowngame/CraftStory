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
        clickBtn.onClick.AddListener(() => { BagLG.E.SelectItem = this; });

        itemName.text = itemData.Config().Name;
        itemCount.text = "x" + itemData.count;
        Icon.sprite = ReadResources<Sprite>(itemData.Config().IconResourcesPath);
    }

    public void Refresh(ItemData itemData = null)
    {
        if(itemData != null) 
            this.itemData = itemData;

        itemName.text = itemData.Config().Name;
        itemCount.text = "x" + itemData.count;
    }
    public void IsSelected(bool b)
    {
        selected.gameObject.SetActive(b);
    }
}