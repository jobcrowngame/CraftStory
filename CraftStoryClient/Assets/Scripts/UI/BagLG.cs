using UnityEngine;
using Gs2.Unity.Gs2Inventory.Model;
using System.Collections;
using System;

public class BagLG : UILogicBase<BagLG, BagUI>
{
    public void GetMoney(int slot)
    {
        GS2.E.GetMoney((item) =>
        {
            ui.RefreshMoneyResponse(item);
        }, slot);
    }

    public void GetInventory()
    {
        GS2.E.GetInventory();
    }

    public void GetItemList()
    {
        GS2.E.ListItems((items)=> 
        {
            ui.AddItems(items);
        },"1", 5);
    }
}
