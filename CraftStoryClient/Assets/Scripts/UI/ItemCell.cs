using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Gs2.Unity.Gs2Inventory.Model;

public class ItemCell : MonoBehaviour
{
    private Text itemName;
    private Text itemCount;
    private Button increaseBtn;
    private Button decreaseBtn;

    private EzItemSet item;

    public void Init(EzItemSet ezitem)
    {
        InitUI();

        this.item = ezitem;

        itemName.text = item.ItemName;
        itemCount.text = "x" + item.Count;
    }

    private bool InitUI()
    {
        var NameObj = CommonFunction.FindChiledByName(gameObject, "Name");
        if (NameObj != null)
            itemName = NameObj.GetComponent<Text>();
        else
            return false;

        var CountObj = CommonFunction.FindChiledByName(gameObject, "Count");
        if (CountObj != null)
            itemCount = CountObj.GetComponent<Text>();
        else
            return false;

        var increaseBtnObj = CommonFunction.FindChiledByName(gameObject, "Increase");
        if (increaseBtnObj != null)
            increaseBtn = increaseBtnObj.GetComponent<Button>();
        else
            return false;

        var decreaseBtnObj = CommonFunction.FindChiledByName(gameObject, "Decrease");
        if (decreaseBtnObj != null)
            decreaseBtn = decreaseBtnObj.GetComponent<Button>();
        else
            return false;

        increaseBtn.onClick.AddListener(() => { ItemLg.E.Increase(item.ItemName); });
        decreaseBtn.onClick.AddListener(() => { ItemLg.E.Decrease(this, item.ItemName, 1); });

        return true;
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