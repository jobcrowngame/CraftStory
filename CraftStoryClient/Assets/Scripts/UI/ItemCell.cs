using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Gs2.Unity.Gs2Inventory.Model;

public class ItemCell : UIBase
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
        InitUI();

        this.itemData = itemData;

        itemName.text = itemData.Config().Name;
        itemCount.text = "x" + itemData.count;
        Icon.sprite = ReadResources<Sprite>(itemData.Config().IconResourcesPath);
    }

    private void InitUI()
    {
        itemName = FindChiled<Text>("Name");
        itemCount = FindChiled<Text>("Count");
        Icon = FindChiled<Image>("Icon");
        selected = FindChiled("Select");

        clickBtn = transform.GetComponent<Button>();
        clickBtn.onClick.AddListener(() => { BagLG.E.SelectItem = this; });
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