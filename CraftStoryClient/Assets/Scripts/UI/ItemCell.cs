using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Gs2.Unity.Gs2Inventory.Model;

public class ItemCell : UIBase
{
    private Text itemName;
    private Text itemCount;
    private Button increaseBtn;
    private Button decreaseBtn;

    private EzItemSet item;

    public void Add(EzItemSet ezitem)
    {
        obj = this.gameObject;

        InitUI();

        this.item = ezitem;

        itemName.text = item.ItemName;
        itemCount.text = "x" + item.Count;
    }

    private void InitUI()
    {
        itemName = FindChiled<Text>("Name");
        itemCount = FindChiled<Text>("Count");

        increaseBtn = FindChiled<Button>("Increase");
        increaseBtn.onClick.AddListener(() => { ItemLg.E.Increase(item.ItemName); });

        decreaseBtn = FindChiled<Button>("Decrease");
        decreaseBtn.onClick.AddListener(() => { ItemLg.E.Decrease(this, item.ItemName, 1); });
    }

    public void IncreaseRespones()
    {

    }

    public void DecreaseRespones(List<EzItemSet> ret)
    {
        foreach (var i in ret)
        {
            if (i.ItemSetId == item.ItemSetId)
            {
                item = i;
                itemName.text = i.ItemName;
                itemCount.text = "x" + i.Count;
                break;
            }
        }
    }
}