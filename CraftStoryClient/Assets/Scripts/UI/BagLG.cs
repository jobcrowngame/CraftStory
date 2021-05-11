using UnityEngine;
using Gs2.Unity.Gs2Inventory.Model;

public class BagLG : UILogicBase<BagLG, BagUI>
{
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
