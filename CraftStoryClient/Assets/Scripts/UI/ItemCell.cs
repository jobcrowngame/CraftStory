using System.Collections.Generic;
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
        InitUI();

        item = ezitem;

        itemName.text = item.ItemName;
        itemCount.text = "x" + item.Count;
    }

    private void InitUI()
    {
        itemName = FindChiled<Text>("Name");
        itemCount = FindChiled<Text>("Count");

        increaseBtn = FindChiled<Button>("Increase");
        increaseBtn.onClick.AddListener(Increase);

        decreaseBtn = FindChiled<Button>("Decrease");
        decreaseBtn.onClick.AddListener(Decrease);
    }

    public void Increase()
    {
        GS2.E.GetItem(item.ItemName);
    }

    public void Decrease()
    {
        GS2.E.Consume(item.ItemName, 1, item.Name);
    }
    public void Refresh(EzItemSet i)
    {
        if (i.Count <= 0)
        {
            Destroy(this.gameObject);
        }

        itemName.text = i.ItemName;
        itemCount.text = "x" + i.Count;
    }
}